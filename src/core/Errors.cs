using System;
using System.Collections.Generic;

using static Parca;
using static System.String;
using static System.Console;

/// Represents a pre-condition error.
public class PreCondError {
	public readonly string InputName;
	public readonly object Expected, Actual;

	public PreCondError(string inputName, object expected, object actual) {
		InputName = inputName;
		Expected  = expected;
		Actual    = actual;
	}

	public override string ToString() =>
		$"Pre-condition failed for [{InputName}]. " +
	   	$"Expected [{Expected}]. Was [{Actual}].";
} 

/// Precondition errors.
public class PreCondErrors {
	class PreCondNode {
		public PreCondNode Next;
		public readonly PreCondError Err;

		public PreCondNode(PreCondError err) {
			Err  = err;
		}
	}

	public int Count;
	PreCondNode Head;

	public void Add(string name, object expected, object actual) {
		DieIf(IsNullOrEmpty(name), "[Add] name is required.");

		var pce = new PreCondError(name, expected, actual);
		if (Head == null) {
			Head = new PreCondNode(pce);
			WriteLine($"Set head {Count}");
		}
		else {
			var tail = Head;
			while(tail.Next != null)
				tail = tail.Next;

			tail.Next = new PreCondNode(pce);
			WriteLine($"Set next {Count}");
		}
		Count++;
	}

	public PreCondError At(int idx) {
		DieIf(idx >= Count, $"idx must less than {Count}.");
		DieIf(Head == null, "There are no errors.");

		var node = Head;
		if (idx == 0)
			return node.Err;

		int i = 0;
		while (node.Next != null) {
			node = node.Next;
			i++;
			if (i == idx)
				break;
		}

		DieIf(node == null, $"Internal error. Fail to get item at idx {idx}.");
		return node.Err;
	}

	public void Clear() {
		// TODO: Dispose each itemb before 'nulling' Head.
		Head  = null;
		Count = 0;
	}
}
