#pragma warning disable 414

using System;
using _ = System.Action<Contest.Core.Runner>;

class Aquatests {
	/// Base class for Moves, Changes and Side Effects.
	class Node {

		public Node() {}

		public Node(string lbl, object val) {
			Label = lbl;
			Value = val;
		}

		public object Value;
		public string Label, Notes;
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

		/// Registers a Side Effect **RELATIVE TO THE CHANGE**.
		public void RegisterSide(SideNode side) {
			//TODO: Register Side **To Change**.
		}
	}

	/// This nodes are used to represent a move in the "moves lane".
	/// move 1
	///	   \
	///	  move 2
	///	     \
	///	    move 3
	///	       \
	///	        *
	class MoveNode : Node {

		public MoveNode(string lbl = null, object val = null) 
			: base(lbl, val) {}

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

		/// Records a move.
		public void Move(MoveNode mv) {
			LastMove = mv;

			if (Next == null) { // First move.
				Next = mv;
				return;
			}

			MoveNode tail;
			while ((tail = Next) != null)
				;
			tail.Next = mv;
		}

		/// Registers a Change **RELATIVE TO THE MOVE**.
		public void RegisterChange(ChangeNode change) {
			//TODO: Register Change **To Move**.
		}

		/// Registers a Side Effect **RELATIVE TO THE MOVE**.
		public void RegisterSide(ChangeNode change) {
			//TODO: Register Side **To Move**.
		}
	}


	_ record_moves_changes_and_side_effects =  assert => {
		// -----------------
		// Initial values
		// -----------------
		// tot = 0.00
		// pri = 0.00
		// qty = 0.00
		// rte = 0.21
		// -----------------
		// Formulas
		// -----------------
		// tot = prc * qty;
		// tax = tot * rte;
		// -----------------
		var lane = new MoveNode();
		var m1   = new MoveNode();
		lane.Move(m1);
		// (tot = 14*0) == 0 => No SE.
		lane.LastMove.RegisterChange(new ChangeNode("prc", 14));

		var m2   = new MoveNode();
		// (tot = 14 * 5) == 70 => SE on tot => (0 => 70).
		lane.LastMove.RegisterChange(new ChangeNode("qty", 5));
        // (tax = 70 * .21) == 14.7 => SE on tax (0 => 14.7).
		lane.LastMove.Change.RegisterSide(new SideNode("tot", 70));
		lane.LastMove.Change.RegisterSide(new SideNode("tax", 14.7));
		// No deps on tax. No SE.
		lane.Move(m2);

		// TODO: Walk graph and run assertions.
	};
}
