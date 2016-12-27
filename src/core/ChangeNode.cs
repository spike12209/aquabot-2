using static Atropos;

/// This nodes are used to represent changes.
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

	public SideEffectNode 
		/// Points to the first side effect produced by the change.
		FirstSideEffect,
		/// Points to the last side effect produced by the change.
		LastSide
		;

	public SideEffectNode SideAt(int idx) {
		DieIf(idx >= SideCount, 
			$"Idx {idx} is out of range. Must be {SideCount - 1} or less.");

		var node = FirstSideEffect;
		int i = 0;
		while (i < SideCount) {
			DieIf(node == null, $"Internal Err. Node can't be null (at {i}).");
			if (i == idx) 
				break;
			++i;
			node = node.Next;
		}
		return node;
	}

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
	public void RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
	public void RecordSide(SideEffectNode se) {
		DieIf(se == null, "Side effect can't be null.");

		SideCount ++;
		LastSide = se;

		if (FirstSideEffect == null) { // First side effect;
			FirstSideEffect = se;
		}
		else {
			var node = FirstSideEffect;
			while (node.Next != null)
				node = node.Next;
			node.Next = se;
		}
	}
}
