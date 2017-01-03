using System.IO;
using static Parca;
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
	public ChangeNode RecordChange(object val) {
		DieIf(FirstSideEffect != null, 
				"Can't have change and side effects at the move level.");

		DieIf(Change != null, 
				"Can't update the change. (Once set, is readonly).");

		return (Change = new ChangeNode(InputName, val));
	}

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public SideEffectNode RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	/// Records a Side Effect **RELATIVE TO THE MOVE**.
	public SideEffectNode RecordSide(SideEffectNode se) {
		DieIf(se == null, "Side effect can't be null.");
		DieIf(Change != null, "Can't have change and SE at the MOVE level.");

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

	// Este seria el formato de un script.
	// Cuando hay un cambio.
	// focus:  precio
	// change: 123
	// move:   #<= A donde va lo decide el tab order.
	//
	// Cuando hay side effects.
	// move:
	// assert: total, 333
	//
	// Cuando no tenemos ni cambios ni se, solo emitimos un move.
	// move:
	//

	public void CreateScript(StringWriter buffer) {
		if (Change != null) {
			buffer.Write($"focus:  {InputName}\n");
			buffer.Write($"change: {Change.Value}\n");
			buffer.Write("move:\n");
		}
		else if (FirstSideEffect != null) {
			buffer.Write("move:\n");
			var se = FirstSideEffect;
			while (se != null) {
				buffer.Write($"assert: {se.InputName}, se.Value\n");
				se = se.Next;
			}
		}
		else {
			buffer.Write("move:\n");
		}
	}

	public SideEffectNode GetLastSideEffect() =>
		SideAt(SideCount - 1);
}
