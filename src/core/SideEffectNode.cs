
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
public class SideEffectNode : Node {
	public SideEffectNode(string inputName = null, object val = null) 
		: base(inputName, val) {}

	public SideEffectNode Next;

	public override string  ToString() {
		return $"{InputName}: {Value}";
	}
}
