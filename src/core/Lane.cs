using System.IO;
using static Parca;
using static System.Console;

/// Represents a set of moves between inputs on a form.
/// (Those moves are also used to represent changes ans side effects).
public class Lane {
	public MoveNode 
		FirstMove,
		NextMove;

	public int MovesCount;

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

	public string CreateScript() {
		var mv   = FirstMove;
		var strw = new StringWriter();
		while (mv != null) {
			mv.CreateScript(strw);
			mv = mv.NextMove;
		}

		// This move reflects the last change (when replaying).
		strw.Write("move:\n");
		return strw.ToString();
	}
}
