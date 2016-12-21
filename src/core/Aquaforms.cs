using System;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;

public class Aquaforms {
	static readonly Values _values = new Values();

	static void PrintSolidLine() => 
		WriteLine("".PadRight(60, '-'));

	public static void Watch(Form f) {	
		f.Shown += (s, e) => {
			HookCtrls(f);
			WriteLine("First Snapshot");
			PrintSolidLine();
			FirstSnapshot(f);
			PrintSolidLine();
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

	static bool UpdateValues(Control ctrl) => 
		_values.Update(ctrl, ctrl.Text);

	static void PrintChange(Control ctrl) =>
		WriteLine($"Cambio ({ctrl.Name}) ({ctrl.GetType()}) {ctrl.Text}");

	/// Compares the current value of a given control agaist the previous
	/// registered value for that particular control. Returns true if the 
	/// value is different, otherwise, false.
	static bool HasChanged(Control c) {
		var pval = _values.Get(c);
		return String.Compare(pval, c.Text) != 0;
	}

	/// Capture the changes (if any) for a given control and its child
	/// controls.
	static void CaptureChangesRec(Control c) {
		foreach(Control ctrl in c.Controls) {
			if (HasChanged(ctrl)) {
				DieUnless(UpdateValues(ctrl), "Fail to update values.");
				PrintChange(ctrl);
			}
			CaptureChangesRec(ctrl);
		}
	}

	static void BeginFrame() {
		PrintSolidLine();
		WriteLine("Frame");
		PrintSolidLine();
	}

	static void EndFrame() =>
		PrintSolidLine();

	/// Inits the capture of a new frame.
	static void CaptureFrame(Form f, Control sender) {
		BeginFrame();

		// Register sender change.
		DieUnless(UpdateValues(sender), "Fail to update values.");
		PrintChange(sender);
		// Register dependencies changes.
		CaptureChangesRec(f);

		EndFrame();
	}

	static void HookCtrls(Form f) =>
		HookCtrls(f, f);

	static void HookCtrls(Form f, Control ctrl) {
		var t = ctrl.GetType();
		if (t != typeof(Form)) {
			ctrl.LostFocus += (s, e) => {
				var c = (Control) s;
				if (HasChanged(c))
					CaptureFrame(f, c);
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(f, c);
		}
	}
}
