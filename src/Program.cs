using System;
using System.Windows.Forms;

using static System.Convert;
using static System.Console;
using static System.String;

class Program {

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

	static int GetTop(Control last) {
		return last.Top + last.Height + 10;
	}

	static void CreateCtrls(Form f) {
		const int LEFT = 20;

		Control last, txtPrice, txtQty, txtTot;
		txtPrice      = new TextBox();
		txtPrice.Name = "Price";
		txtPrice.Top  = 20;
		last          = txtPrice;
		last.Left     = LEFT;

		txtQty      = new TextBox();
		txtQty.Name = "Qty";
		txtQty.Top  = GetTop(last);
		last        = txtQty;
		last.Left   = LEFT;

		txtTot      = new TextBox();
		txtTot.Name = "Tot";
		txtTot.Top  = GetTop(last);
		txtTot.Left = LEFT;

		f.Controls.Add(txtPrice);
		f.Controls.Add(txtQty);
		f.Controls.Add(txtTot);

		// Handlers
		Action<Control, Control> updateTot = (price, qty) => {
			int p = IsNullOrEmpty(price.Text) ? 0 : ToInt32(txtPrice.Text);
			int q = IsNullOrEmpty(qty.Text)   ? 0 : ToInt32(qty.Text);
			txtTot.Text = (p * q).ToString();
		};

		//Hook handlers
		txtPrice.LostFocus += (s, e) => updateTot(txtPrice, txtQty);
		txtQty.LostFocus   += (s, e) => updateTot(txtPrice, txtQty);

		f.Shown += (s, e) => {
			txtPrice.Text = "123";	
			txtQty.Text = "2";	
			updateTot(txtPrice, txtQty);
			WriteLine("Su shown");
		};
	}

	static void Main(params string [] args) {
		var f = new Form();
		CreateCtrls(f);
		Aqua.Watch(f);
		f.ShowDialog();
	}

	class Aqua {
		static readonly Values _values = new Values();

		public static void Watch(Form f) {	
			f.Shown += (s, e) => {
				WriteLine("Mi shown");
				HookCtrls(f);
				WriteLine("".PadRight(60, '-'));
				FirstSnapshot(f);
				WriteLine("".PadRight(60, '-'));
			};
		}

		static void FirstSnapshot(Control ctrl) {

			var t = ctrl.GetType();
			if (t != typeof(Form)) {
				WriteLine($"({ctrl.Name})"  + $" ({t}) "  + ctrl.Text);
				_values.Set(ctrl, ctrl.Text);
			}


			foreach(Control c in ctrl.Controls)
				FirstSnapshot(c);
		}


		static void HookCtrls(Control ctrl) {
			var t = ctrl.GetType();
			if (t != typeof(Form)) {

				ctrl.LostFocus += (s, e) => {
					var c = (Control)s;
					var pval = _values.Get(c).ToString();
					WriteLine($"P {pval}");
					if (pval != c.Text)
						WriteLine( $"S ({c.Name})"  + $" ({t}) "  + c.Text);
					else
						_values.Update(ctrl, ctrl.Text);
				};
			}

			foreach(Control c in ctrl.Controls) {
				HookCtrls(c);
			}
		}
	}
}
