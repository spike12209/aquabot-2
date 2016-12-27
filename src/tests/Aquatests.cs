#pragma warning disable 414

using System;
using static Atropos;
using static System.Console;

using _ = System.Action<Contest.Core.Runner>;

class Aquatests {

	static MoveNode Lane = null;

	static void MakeChange(ref MoveNode m, string lbl, object val) =>
		m.Change = new ChangeNode(lbl, val);

	_ before_each = assert => { Lane = new MoveNode(); };

	_ after_each  = assert => {  /*TODO: Cleanup (if needed). */};
	
	_ record_move = assert => {
		var m1 = new MoveNode();
		var m2 = new MoveNode();

		Lane.RecordMove(m1);
		Lane.RecordMove(m2);
		assert.Equal(2,  Lane.MovesCount);
		assert.Equal(m2, Lane.LastMove);
	};

	_ record_change = assert => {
		var m1   = new MoveNode();
		MakeChange(ref m1, "prc", 123);
		Lane.RecordMove(m1);
		assert.Equal(1,     Lane.MovesCount);
		assert.Equal("prc", Lane.LastMove.Change.InputName);
		assert.Equal(123,   Lane.LastMove.Change.Value);
	};

	_ record_multiple_moves = assert => {
		var m1   = new MoveNode();
		var m2   = new MoveNode();
		var m3   = new MoveNode();
		Lane.RecordMove(m1);
		Lane.RecordMove(m2);
		Lane.RecordMove(m3);
		assert.Equal(3,     Lane.MovesCount);

		assert.Equal(m1, Lane.MoveAt(0));
		assert.Equal(m2, Lane.MoveAt(1));
		assert.Equal(m3, Lane.MoveAt(2));
	};

	_ record_side_effect_RELATIVE_TO_MOVE = assert => {
		var mv1   = new MoveNode();
		mv1.RecordSide("tot", 321);
		Lane.RecordMove(mv1);
		var lmv = Lane.LastMove;
		assert.Equal(1, lmv.SideCount);
		assert.Equal("tot", lmv.LastSide.InputName);
		assert.Equal(321,   lmv.LastSide.Value);
	};


	_ record_side_effect_RELATIVE_TO_CHANGE = assert => {
		var mv1   = new MoveNode();
		mv1.RecordChange("prc", 123);
		mv1.Change.RecordSide("tot", 321);
		Lane.RecordMove(mv1);

		var lchng = Lane.LastMove.Change;
		assert.Equal("tot", lchng.LastSide.InputName);
		assert.Equal(321,   lchng.LastSide.Value);
	};

	_ record_multple_side_effects_RELATIVES_TO_CHANGE = assert => {
		var mv1   = new MoveNode();
		mv1.RecordChange("prc", 123);
		// There are two side effects when price changes.
		mv1.Change.RecordSide("tot", 321);
		mv1.Change.RecordSide("tax", 62.3);

		assert.Equal("tot", mv1.Change.SideAt(0).InputName);
		assert.Equal(321,   mv1.Change.SideAt(0).Value);

		assert.Equal("tax", mv1.Change.SideAt(1).InputName);
		assert.Equal(62.3,  mv1.Change.SideAt(1).Value);
	};

	_ record_multiple_side_effects_RELATIVE_TO_MOVE = assert => {
		var mv1   = new MoveNode();
		mv1.RecordSide("tot", 321);
		mv1.RecordSide("tax", 62.3);

		assert.Equal("tot", mv1.SideAt(0).InputName);
		assert.Equal(321,   mv1.SideAt(0).Value);

		assert.Equal("tax", mv1.SideAt(1).InputName);
		assert.Equal(62.3,  mv1.SideAt(1).Value);
	};
}
