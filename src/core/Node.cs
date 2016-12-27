
/// Base class for Moves, Changes and Side Effects.
public class Node {

	public Node() {}

	public Node(string inputName, object val) {
		InputName = inputName;
		Value     = val;
	}

	public object Value;
	public string InputName, Notes;
}

