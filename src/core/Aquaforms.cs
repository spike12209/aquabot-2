using System;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;

/// This class tracks controls changes on a given form.
public class Aquaforms {

	/// Aqua's entry point. 
	/// This method starts looking for changes on the given form.
	public static void Watch(Form f) {	
		f.Shown += (s, e) => {
			ValueStore values = Init(f);
			PrintSolidLine(); //<= Begin spanshot.
			WriteLine("First Snapshot");
			FirstSnapshot(f, values);
			PrintSolidLine(); //<= End snapshot.
		};
	}

	/// Initializes Aquaforms. Basically, it hooks controls 
	/// and creates the value storage database to track controls changes.
	static ValueStore Init(Form f) {
		ValueStore values = new ValueStore();
		HookCtrls(f, values);
		return values;
	}

	/// Draws a solid line to the console.
	static void PrintSolidLine() => 
		WriteLine("".PadRight(60, '-'));

	/// Takes the first snapshot, which is a picture of the whole form.
	static void FirstSnapshot(Control ctrl, ValueStore values) {

		var t = ctrl.GetType();
		if (t != typeof(Form)) {
			WriteLine($"({ctrl.Name})"  + $" ({t}) "  + ctrl.Text);
			values.Set(ctrl, ctrl.Text);
		}

		foreach(Control c in ctrl.Controls)
			FirstSnapshot(c, values);

	}

	/// Update control changes to the value store.
	static bool UpdateValueStore(Control ctrl, ValueStore values) => 
		values.Update(ctrl, ctrl.Text);

	/// Prints a control change.
	static void PrintChange(Control ctrl) =>
		WriteLine($"Cambio ({ctrl.Name}) ({ctrl.GetType()}) {ctrl.Text}");

	/// Compares the current value of a given control agaist the previous
	/// registered value for that particular control. Returns true if the 
	/// value is different, otherwise, false.
	static bool HasChanged(Control c, ValueStore values) {
		var pval = values.Get(c);
		return String.Compare(pval, c.Text) != 0;
	}

	/// Capture the changes (if any) for a given control and its child
	/// controls.
	static void CaptureChangesRec(Control c, ValueStore values) {
		foreach(Control ctrl in c.Controls) {
			if (HasChanged(ctrl, values)) {
				DieUnless(UpdateValueStore(ctrl, values), "Fail to update values.");
				PrintChange(ctrl);
			}
			CaptureChangesRec(ctrl, values);
		}
	}

	/// Marks the begining of a frame.
	static void BeginFrame() {
		PrintSolidLine();
		WriteLine("Frame");
		PrintSolidLine();
	}

	/// Marks the end of a frame.
	static void EndFrame() =>
		PrintSolidLine();

	/// Inits the capture of a new frame.
	static void CaptureFrame(Form f, Control sender, ValueStore values) {
		BeginFrame();

		// Register sender change.
		DieUnless(UpdateValueStore(sender, values), "Fail to update values.");
		PrintChange(sender);
		// Register dependencies changes.
		CaptureChangesRec(f, values);

		EndFrame();
	}

	/// Hook handlers to track changes.
	static void HookCtrls(Form f, ValueStore values) =>
		HookCtrls(f, f, values);

	/// Hook handlers to track changes.
	static void HookCtrls(Form f, Control ctrl, ValueStore values) {
		var t = ctrl.GetType();
		if (t != typeof(Form)) {
			ctrl.LostFocus += (s, e) => {
				var c = (Control) s;
				if (HasChanged(c, values))
					CaptureFrame(f, c, values);
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(f, c, values);
		}
	}
}
