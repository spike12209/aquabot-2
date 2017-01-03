#pragma warning disable 414

using System;
using System.Windows.Forms;

using static System.Console;
using static Parca;
using static MoveNode;

using _ = System.Action<Contest.Core.Runner>;

class Aquatests {

	static Lane Lane;

	static void MakeChange(ref MoveNode m, string lbl, object val) =>
		m.Change = new ChangeNode(lbl, val);

	_ before_each = test =>
		Lane = new Lane();
	
	_ record_move = assert => {
		var m1 = Lane.MoveTo("foo");
		var m2 = Lane.MoveTo("bar");

		assert.IsNotNull(m1);
		assert.Equal(2,  Lane.MovesCount);
		assert.Equal(m2, Lane.GetLastMove());
	};

	_ record_change = assert => {
		var m1  = Lane.MoveTo("prc");
		var ch1 = m1.RecordChange(123);

		assert.Equal(1,   Lane.MovesCount);
		assert.Equal(ch1, Lane.GetLastMove().Change);
	};

	_ record_multiple_moves = assert => {
		var m1 = Lane.MoveTo("foo");
		var m2 = Lane.MoveTo("bar");
		var m3 = Lane.MoveTo("baz");

		assert.Equal(3,  Lane.MovesCount);
		assert.Equal(m1, Lane.MoveAt(0));
		assert.Equal(m2, Lane.MoveAt(1));
		assert.Equal(m3, Lane.MoveAt(2));
	};

	_ record_side_effect_RELATIVE_TO_MOVE = assert => {
		var mv1 = Lane.MoveTo("prc");
		var se1 = mv1.RecordSide("tot", 321);

		assert.Equal(1,   Lane.GetLastMove().SideCount);
		assert.Equal(se1, Lane.GetLastMove().GetLastSideEffect());
	};


	_ record_side_effect_RELATIVE_TO_CHANGE = assert => {
		var mv1 = Lane.MoveTo("prc");
		var ch1 = mv1.RecordChange(123);
		var se1 = ch1.RecordSide("tot", 321);

		var lastch = Lane.GetLastMove().Change;
		assert.Equal(se1, lastch.GetLastSideEffect());
	};

	_ record_multple_side_effects_RELATIVES_TO_CHANGE = assert => {
		var mv1 = Lane.MoveTo("prc");
		mv1.RecordChange(100);
		// There are two side effects when price changes.
		var se1 = mv1.Change.RecordSide("tot", 363);
		var se2 = mv1.Change.RecordSide("tax", 63);

		assert.Equal(se1, mv1.Change.SideAt(0));
		assert.Equal(se2, mv1.Change.SideAt(1));
	};

	_ record_multiple_side_effects_RELATIVE_TO_MOVE = assert => {
		var mv1 = Lane.MoveTo("prc");
		var se1 = mv1.RecordSide("tot", 363);
		var se2 = mv1.RecordSide("tax",  63);

		assert.Equal(se1, mv1.SideAt(0));
		assert.Equal(se2, mv1.SideAt(1));

	};

	// Seguir desde aca
	// Create Replay Script
	//
	// Instruction set
	// set:      name, value //<= set input value on name
	// moveto:   name        //<= move to name
	// assert:   name, value //<= assert input value       
	// shownote: name, note  //<= shows note attached to name.
	// stop:                 //<= Pauses the execution. (Dbg step by step).


	// When replaying on debug mode, each instruction is followed by stop call.
	// This allows users to see notes on each step
	//
	//
	// When an assert pass, we set the control's font to bold green. When it 
	// fails, to red bold. This is a neat way to signal successes and errors.
	_ create_replay_script = assert => {
		
		var lane = new Lane();

		// moveto: prc
		var mv1   = lane.MoveTo("prc");
		// set: prc, 123
		var ch1  = mv1.RecordChange(123);
		// assert: net, 0
		ch1.RecordSide("net", 0); // <= prc * qty
		// assert: tax, 0
		ch1.RecordSide("tax", 0); // <= prc * qty * .21
		// assert: tot, 0
		ch1.RecordSide("tot", 0); // <= net + tax


		var mv2   = lane.MoveTo("qty");
		mv2.RecordChange(3);
		mv2.Change.RecordSide("net", 300);
		mv2.Change.RecordSide("tax", 63);
		mv2.Change.RecordSide("tot", 363);

		var script = lane.CreateScript();
		Write($"{script}\n");
	};


	_ eval_script = assert => {
		var inter = new Interpreter();
		const string src = @"
			; This is a comment
			focus:  prc
			change: 123
			move:
			focus:  qty ; This is an inline comment.
			change: 3
			move:
			assert: tot, 369
			";

		int countch = 0, countmv = 0, counta = 0, countf = 0;

		inter.Move   = (target, asserts) => countmv++;
		inter.Focus  = (target, name) => countf++;
		inter.Change = (target, val)  => countch++;
		inter.Assert = (target, asserts, name, val) => counta++;

		var f = new Form();
		inter.Eval(src, f);

		assert.Equal(2, countf); 
		assert.Equal(2, countmv);
		assert.Equal(2, countch);
		assert.Equal(1, counta);
	
	};



}
