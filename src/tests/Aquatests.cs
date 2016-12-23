#pragma warning disable 414

using System;
using _ = System.Action<Contest.Core.Runner>;

class Aquatests {
	/// Base class for Moves, Changes and Side Effects.
	class Node {
		public object Value;
		public string Label, Notes;
	}

	/// This nodes are used to represent a move in the "moves lane".
	class MoveNode : Node {

		/// Points to the next move.
		public MoveNode Next;

		// +----------------------------------------------------+
		// | IMPORTANT: Change and Side are mutualy exclusive.  |
		// | If a change has side effects, those side effects   |
		// | start at the change node, *NOT* at the move.       |
		// | Sometimes moves cause side effects, in those cases |
		// | a move has a side effect but doesn't point to a    |
		// | change.                                            |
		// +----------------------------------------------------+
		/// Points to a change produced by the move.
		public ChandeNode Change;
		/// Points to a side effect produced by the move.
		public SideNode Side;
		// ------------------------------------------------------
	}

	/// This nodes are used to represent changes.
	class ChangeNode : Node {
		public ChandeNode Next;
		public SideNode   Side;
	}

	/// This nodes are used to represent side effects.
	class SideNode : Node {
		public SideNode Next;
	}

	/// DAG representing moves on the form.
	class MovesLane {
	
	}

	_ add_nodes_to_the_move_lane =  assert => {

	};
}
