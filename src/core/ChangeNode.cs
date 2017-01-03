using System.IO;

using static Parca;

/// This nodes are used to represent changes.
/// (One move can have at most one change).
/// move 1
///	   \
///	  move 2 ----- change 2.1
///	     \
///	    move 3 --- change 3.1
///	       \
///	        *
public class ChangeNode : Node {
	public ChangeNode(string lbl = null, object val = null) 
		: base(lbl, val) {}

	public int SideCount;

	/// Points to the first side effect produced by the change.
	public SideEffectNode FirstSideEffect;

	public SideEffectNode GetLastSideEffect() =>
		SideAt(SideCount -1);

	public SideEffectNode SideAt(int idx) {
		DieIf(SideCount == 0, "There is no side effects for this change.");
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

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
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

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
	public SideEffectNode RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	public void CreateScript(StringWriter buffer) {
		buffer.Write($"focus:  {InputName}\n");
		buffer.Write($"change: {Value}\n");
		buffer.Write("move:\n");

		SideEffectNode se = null;
		for (int i = 0; i < SideCount; ++i) {
			se = SideAt(i);
			buffer.Write($"assert: {se.InputName} {se.Value}\n");
		}
		buffer.Write("move:\n");
	}
}
