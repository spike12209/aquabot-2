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
	class SideEffectNode : Node {
		public SideEffectNode(string inputName = null, object val = null) 
			: base(inputName, val) {}

		public SideEffectNode Next;
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

		public int SideCount;
		public SideEffectNode SideEffects;

		/// Records a Side Effect **RELATIVE TO THE CHANGE**.
		public void RecordSide(SideEffectNode side) {
			SideCount ++;

			if (SideEffects == null) { // First side;
				SideEffects = new SideEffectNode();
				return;
			}

			SideEffectNode tail = SideEffects.Next;
			while (tail != null) {
				tail = tail.Next;
			}
			tail.Next = side;

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

		public SideEffectNode 
			/// Points to the first side effect produced by the move.
			Side, 
			/// Points to the last side effect.
			LastSide;
		// ------------------------------------------------------

		public int MovesCount, SideCount;

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
		public void RecordSide(SideEffectNode se) {
			SideCount ++;
			LastSide = se;

			if (Side == null) { // First side effect;
				Side = se;
				return;
			}

			SideEffectNode tail = Side.Next;
			while (tail != null) {
				tail = tail.Next;
			}
			tail.Next = se;
		}
	}


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

	_ record_side_effect_RELATIVE_TO_MOVE = assert => {
		var mv1   = new MoveNode();
		mv1.RecordSide(new SideEffectNode("tot", 321));
		Lane.RecordMove(mv1);
		var lmv = Lane.LastMove;
		assert.Equal(1, lmv.SideCount);
		assert.Equal("tot", lmv.LastSide.InputName);
		assert.Equal(321,   lmv.LastSide.Value);
	};

	_ record_side_effect_RELATIVE_TO_CHANGE = assert => {
		//TODO:
	};
}
