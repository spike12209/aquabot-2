using System;
using System.Windows.Forms;

class Value {
	public readonly Control Ctrl;
	public readonly Value Next;
	public object Val;

	public Value(Value next, Control ctrl, object val) {
		Next = next;
		Ctrl = ctrl;
		Val  = val;
	}

}
