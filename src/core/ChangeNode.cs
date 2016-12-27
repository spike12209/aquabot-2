
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

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
	public void RecordSide(string inputName, object val) =>
		RecordSide(new SideEffectNode(inputName, val));

	/// Records a Side Effect **RELATIVE TO THE CHANGE**.
	public void RecordSide(SideEffectNode se) {
		SideCount ++;
		LastSide = se;

		if (FirstSideEffect == null) {
			FirstSideEffect = se;
			return;
		} 
		else {
			//TODO: Find last and set se.
			
		}
	}
}
