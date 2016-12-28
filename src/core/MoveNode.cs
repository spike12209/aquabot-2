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

	public MoveNode NextMove;

	// ==========================================================
	// IMPORTANT:
	// ==========================================================
	// +----------------------------------------------------+
	// | Change and FirstSideEffect are mutualy exclusive.  |
	// | If a change has side effects, those side effects   |
	// | start at the change node, *NOT* at the move.       |
	// | Sometimes moves cause side effects, in those cases |
	// | a move has a side effect but doesn't point to a    |
	// | change.                                            |
	// +----------------------------------------------------+
	public ChangeNode Change; // One move == One Change

	/// Points to the first side effect produced by the move.
	/// One move may produce more than one side effect.
	public SideEffectNode FirstSideEffect;
	// ==========================================================

	public int SideCount;

	/// Records a Change **RELATIVE TO THE MOVE**.
	public ChangeNode RecordChange(object val) =>
		(Change = new ChangeNode(InputName, val));

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public SideEffectNode RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public SideEffectNode RecordSide(SideEffectNode se) {
		DieIf(se == null, "Side effect can't be null.");
		SideCount ++;

		if (FirstSideEffect == null) { // First side effect;
			FirstSideEffect = se;
		}
		else {
			var node = FirstSideEffect;
			while (node.Next != null)
				node = node.Next;
			node.Next = se;
		}
		return se;
	}

	/// Refers to side effects relatives TO THE MOVE.
	public SideEffectNode SideAt(int idx) {
		DieIf(SideCount == 0, 
			"There are no side effect relative to this move.");

		DieIf(idx >= SideCount, 
			$"Idx {idx} is out of range. Must be {SideCount - 1} or less.");

		var node = FirstSideEffect;
		int i = 0;
		while (i < SideCount) {
			if (i == idx) 
				break;
			++i;
			node = node.Next;
		}
		return node;
	}

	public SideEffectNode GetLastSideEffect() =>
		SideAt(SideCount - 1);
}
