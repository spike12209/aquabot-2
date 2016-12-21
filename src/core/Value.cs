using System;
using System.Windows.Forms;

class Value {
	public readonly Control Ctrl;
	public readonly Value Next;
	public string Val;

	public Value(Value next, Control ctrl, string val) {
		Next = next;
		Ctrl = ctrl;
		Val  = val;
	}

}
