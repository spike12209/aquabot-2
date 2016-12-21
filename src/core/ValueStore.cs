using System;
using System.Windows.Forms;

using static System.String;

/// This class acts as an in memory database to track controls changes.
class ValueStore {
	Value _head;

	// Since from the winforms perspective there is no way to tell the
	// difference between null and empty, we use and with that avoid
	// a whole bunch of null checking.
	static string Str(object val) =>
		val == null ? Empty : Intern(val.ToString());

	public bool Update(Control ctrl, object val) {
		var node = _head;
		while (node != null) {
			if (node.Ctrl == ctrl) {
				node.Val = Str(val);
				return true;
			}
			node = node.Next; 
		}
		return false;
	}

	public void Set(Control ctrl, object val) {
		_head = new Value(_head, ctrl, Str(val));
	}

	public string Get(Control ctrl) {
		var node = _head;
		while (node != null) {
			if (node.Ctrl == ctrl)
				return node.Val;
			node = node.Next; 
		}
		return null;
	}
}
