using System;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;

public class Aquaforms {
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
				if (String.Compare(pval?.ToString(), c.Text) != 0) {
					WriteLine($"Cambio ({c.Name}) ({t}) {c.Text}");
					DieUnless(_values.Update(ctrl, ctrl.Text), 
							"Fail to update values.");
				}
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(c);
		}
	}
}
