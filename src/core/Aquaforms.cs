using System;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;

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
				WriteLine($"P {pval}");
				if (pval != c.Text)
					WriteLine( $"S ({c.Name})"  + $" ({t}) "  + c.Text);
				else
					//TODO: Die if update fails.
					_values.Update(ctrl, ctrl.Text);
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(c);
		}
	}
}
