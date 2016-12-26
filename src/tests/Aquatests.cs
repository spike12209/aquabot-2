#pragma warning disable 414

using System;
using static Atropos;
using static System.Console;

using _ = System.Action<Contest.Core.Runner>;

class Aquatests {
	/// Base class for Moves, Changes and Side Effects.
	class Node {

		public Node() {}

		public Node(string inputName, object val) {
			InputName = inputName;
			Value = val;
		}

		public object Value;
		public string InputName, Notes;
	}

	/// This nodes are used to represent side effects.
	/// move 1
	///	   \
	///	  move 2 ---- change 2.1 ----- side 2.1.1
	///	     \			                 \
	///	      \		                     side 2.1.2
	///	       \
	///	      move 3 ----------------- side 3.0.1
	///	         \
	///	          *
	class SideNode : Node {
		public SideNode(string lbl = null, object val = null) 
			: base(lbl, val) {}

		public SideNode Next;
	}

	/// This nodes are used to represent changes.
	/// move 1
	///	   \
	///	  move 2 ----- change 2.1
	///	     \
	///	    move 3 --- change 3.1
	///	       \
	///	        *
	class ChangeNode : Node {
		public ChangeNode(string lbl = null, object val = null) 
			: base(lbl, val) {}

		public SideNode   Side;

		/// Records a Side Effect **RELATIVE TO THE CHANGE**.
		public void RecordSide(SideNode side) {
			//TODO: Record Side **To Change**.
		}
	}

	/// This nodes are used to represent a move in the "moves Lane".
	/// move 1
	///	   \
	///	  move 2
	///	     \
	///	    move 3
	///	       \
	///	        *
	class MoveNode : Node {

		public MoveNode(string inputName = null, object val = null) 
			: base(inputName, val) {}

		public MoveNode 
			/// Points to the next move.
			Next, 
			/// Points to the last move.
			LastMove;

	// +----------------------------------------------------+
		// | IMPORTANT: Change and Side are mutualy exclusive.  |
		// | If a change has side effects, those side effects   |
		// | start at the change node, *NOT* at the move.       |
		// | Sometimes moves cause side effects, in those cases |
		// | a move has a side effect but doesn't point to a    |
		// | change.                                            |
		// +----------------------------------------------------+
		/// Points to a change produced by the move.
		public ChangeNode Change; 

		/// Points to a side effect produced by the move.
		public SideNode Side;
		// ------------------------------------------------------

		public int MovesCount;

		/// Records a move.
		public void RecordMove(MoveNode mv) {
			MovesCount++;
			LastMove = mv;

			if (Next == null) { // First move.
				Next = mv;
				return;
			}

			MoveNode tail = Next;
			while (tail.Next != null)
				tail = tail.Next;

			tail.Next = mv;
		}

		/// Records a Change **RELATIVE TO THE MOVE**.
		public void RecordChange(ChangeNode change) {
			//TODO: Record Change **To Move**.
		}

		/// Records a Side Effect **RELATIVE TO THE MOVE**.
		public void RecordSide(ChangeNode change) {
			//TODO: Record Side **To Move**.
		}
	}


	static MoveNode Lane = null;

	static void MakeChange(ref MoveNode m, string lbl, object val) =>
		m.Change = new ChangeNode(lbl, val);

	_ before_each = assert => { Lane = new MoveNode(); };

	_ after_each  = assert => {  /*TODO: Cleanup (if needed). */};
	
	_ add_move = assert => {
		var m1 = new MoveNode();
		var m2 = new MoveNode();

		Lane.RecordMove(m1);
		Lane.RecordMove(m2);
		assert.Equal(2,  Lane.MovesCount);
		assert.Equal(m2, Lane.LastMove);
	};

	_ add_change = assert => {
		var m1   = new MoveNode();
		MakeChange(ref m1, "prc", 123);
		Lane.RecordMove(m1);
		assert.Equal(1,     Lane.MovesCount);
		assert.Equal("prc", Lane.LastMove.Change.InputName);
		assert.Equal(123,   Lane.LastMove.Change.Value);
	};

	_ add_side_effect = assert => {
			
	};

	// _ record_moves_changes_and_side_effects =  assert => {
	// 	// -----------------
	// 	// Initial values
	// 	// -----------------
	// 	// tot = 0.00
	// 	// pri = 0.00
	// 	// qty = 0.00
	// 	// rte = 0.21
	// 	// -----------------
	// 	// Formulas
	// 	// -----------------
	// 	// tot = prc * qty;
	// 	// tax = tot * rte;
	// 	// -----------------
	// 	var Lane = new MoveNode();
	// 	var m1   = new MoveNode();
	// 	Lane.Move(m1);
	// 	// (tot = 14*0) == 0 => No SE.
	// 	Lane.LastMove.RecordChange(new ChangeNode("prc", 14));
    //
	// 	var m2   = new MoveNode();
	// 	// (tot = 14 * 5) == 70 => SE on tot => (0 => 70).
	// 	Lane.LastMove.RecordChange(new ChangeNode("qty", 5));
    //     // (tax = 70 * .21) == 14.7 => SE on tax (0 => 14.7).
	// 	Lane.LastMove.Change.RecordSide(new SideNode("tot", 70));
	// 	Lane.LastMove.Change.RecordSide(new SideNode("tax", 14.7));
	// 	// No deps on tax. No SE.
	// 	Lane.Move(m2);
    //
	// 	// TODO: Walk graph and run assertions.
	// };
}
