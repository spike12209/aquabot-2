using System;
using System.Collections.Generic;

using static Parca;
using static System.String;

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
		$"[{InputName}] expected '{Expected}' was '{Actual}'";
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
		if (Head == null)
			Head = new PreCondNode(pce);
		else {
			var tail = Head.Next;
			while(tail != null)
				tail = tail.Next;

			tail = new PreCondNode(pce);
		}
		Count++;
	}

	public PreCondError At(int idx) {
		DieIf(idx >= Count, $"idx must less than {Count}.");

		if (Head == null)
			Die("There are no errors.");

		if (idx == 0)
			return Head.Err;

		var tail = Head.Next;
		for (int i = 0; tail != null; ++i) {
			if (i == idx)
				break;
			tail = tail.Next;
		}

		DieIf(tail == null, $"Internal error. Fail to get item at idx {idx}.");
		return tail.Err;
	}

	public void Destroy() {
		// TODO: Dispose each itemb before 'nulling' Head.
		Head = null;
	}
}
