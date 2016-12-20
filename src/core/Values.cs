using System;
using System.Windows.Forms;

class Values {
	Value Head;
	public bool Update(Control ctrl, object val) {
		var node = Head;
		while(node != null) {
			if (node.Ctrl == ctrl) {
				node.Val = val;
				return true;
			}
			node = node.Next; 
		}
		return false;
	}

	public void Set(Control ctrl, object val) {
		Head = new Value(Head, ctrl, val);
	}

	public object Get(Control ctrl) {
		var node = Head;
		while(node != null) {
			if (node.Ctrl == ctrl)
				return node.Val;
			node = node.Next; 
		}
		return null;
	}
}
