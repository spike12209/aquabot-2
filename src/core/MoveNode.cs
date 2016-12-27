using static Atropos;
using static System.Console;

/// This nodes are used to represent a move in the "moves Lane".
/// move 1
///	   \
///	  move 2
///	     \
///	    move 3
///	       \
///	        *
public class MoveNode : Node {

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
	public ChangeNode Change; // One move == One Change


	public SideEffectNode 
		/// Points to the first side effect produced by the move.
		/// One move may produce more than one side effect.
		Side, 
		/// Points to the last side effect. Produced by MOVE.
		LastSide;
	// ------------------------------------------------------

	public int MovesCount, SideCount;

	// TODO: Review err messages. ie. idx == 0.
	public MoveNode MoveAt(int idx) {
		DieIf(idx >= MovesCount, 
				"Idx must be less than {MovesCount}.");

		var last = Next;
		if (idx == 0)
			return last;

		DieIf(last == null, "Internal error. Last can't be null.");

		int i = 0;
		while(last.Next != null) {
			last = last.Next;
			i++;
			if (i == idx)
				break;
		}

		return last;

	}

	/// Records a move.
	public void RecordMove(MoveNode mv) {
		MovesCount++;
		LastMove = mv;

		if (Next == null) { // First move.
			Next = mv;
			return;
		}

		var last = Next;
		while (last.Next != null)
			last = last.Next;

		last.Next = mv;
	}

	/// Records a Change **RELATIVE TO THE MOVE**.
	public void RecordChange(string inputName, object val) =>
		RecordChange(new ChangeNode(inputName, val));

	/// Records a Change **RELATIVE TO THE MOVE**.
	public void RecordChange(ChangeNode change) {
		Change = change;
	}

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public void RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public void RecordSide(SideEffectNode se) {
		DieIf(se == null, "Side effect can't be null.");

		SideCount ++;
		LastSide = se;

		if (Side == null) { // First side effect;
			Side = se;
		}
		else {
			var last = Side;
			while (last.Next != null)
				last = last.Next;
			last.Next = se;
		}
	}

	public SideEffectNode SideAt(int idx) {
		DieIf(idx >= SideCount, 
				$"Idx {idx} is out of range. Must be {SideCount - 1} or less.");

		// Seguir desde aca.
		// Los nodos se estan agregando correctamente... por que falla?
		SideEffectNode node = Side;
		int i = 0;
		while (i < SideCount) {
			DieIf(node == null, $"Internal Err. Node at {i}");
			Write($"node at({i}) => {node}\n");
			if (i == idx) 
				break;
			++i;
			node = node.Next;
		}
		return node;
	}
}
