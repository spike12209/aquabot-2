using System.IO;
using System.Collections.Generic;
using static Parca;
using static System.Console;

/// Represents a set of moves between inputs on a form.
/// (Those moves are also used to represent changes ans side effects).
public class Lane {
	public MoveNode 
		FirstMove,
		NextMove;

	public int MovesCount;
	public bool IsRecording;

	/// Form's state before the action begins.
	public readonly Dictionary<string, string> StartingState = 
		new Dictionary<string, string> ();

	public MoveNode MoveTo(string inputName) {
		var mv = new MoveNode(inputName, null);
		MovesCount++;

		if (FirstMove == null)
			return (FirstMove = mv);

		var node = FirstMove;
		while (node.NextMove != null)
			node = node.NextMove;

		return (node.NextMove = mv);
	}

	public MoveNode MoveAt(int idx) {
		DieIf(MovesCount == 0,   "There are no moves at this lane.");
		DieIf(idx >= MovesCount, "Idx must be less than {MovesCount}.");

		var node = FirstMove;
		if (idx == 0)
			return node;

		int i = 0;
		while (node.NextMove != null) {
			node = node.NextMove;
			i++;
			if (i == idx)
				break;
		}

		return node;
	}

	public MoveNode GetLastMove() =>
		MoveAt(MovesCount - 1);

	public void Clear() {
		// TODO: Dispose Moves
		FirstMove  = null;
		NextMove   = null;
		MovesCount = 0;
		StartingState.Clear();
	}

	public string CreateScript() {
		var mv     = FirstMove;
		var strw   = new StringWriter();
		int fcount = 0;

		// This part of the script ensures that the initial
		// state of the form under tests matches the initial state
		// of the same form when we were recording the script.
		strw.WriteLine($";---------------------------------------------");
		strw.WriteLine($"; Check preconditions");
		strw.WriteLine($";---------------------------------------------");
		foreach(var k in StartingState.Keys)
			strw.WriteLine($"ensure: {k} {StartingState[k]}");

		strw.WriteLine($";---------------------------------------------");

		strw.WriteLine("start:");

		while (mv != null) {
			strw.WriteLine($";---------------------------------------------");
			strw.WriteLine($"; Frame No: {++fcount}");
			strw.WriteLine($";---------------------------------------------");
			mv.CreateScript(strw);
			mv = mv.NextMove;
		}

		// This move reflects the last change (when replaying).
		strw.WriteLine("end:");
		return strw.ToString();
	}
}
