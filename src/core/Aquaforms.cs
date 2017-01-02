using System;
using System.Drawing;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;
using static ConditionalHelpers;

/// This class tracks controls changes on a given form.
public class Aquaforms {

	/// Inits the capture of a new frame.
	static void CaptureFrame(Form f, Control sender, ValueStore values, Lane lane) {
		BeginFrame(values, sender, lane);

		Unless(sender == null, () => {
			DieUnless(UpdateValueStore(sender, values), "Fail to update vals.");
			PrintChange(sender);
		});

		// Register dependencies changes.
		CaptureSideEffectsRec(f, values, lane);

		EndFrame();
	}

	/// This happens when a value gets updated because a control 
	/// looses its focus but that control didn't change.
	static void CaptureFrameOther(Form f, ValueStore values, Lane lane) => 
		CaptureFrame(f, null, values, lane);

	/// Captures changes that were not produced by a user input. 
	static void CaptureOther(Form f, ValueStore values, Lane lane) {
		foreach(Control c in f.Controls)
			if (HasChanged(c, values)) {
				CaptureFrameOther(f, values, lane);
			}
	}

	static void CaptureInput(Form f, Control c, ValueStore values, Lane lane) =>
		CaptureFrame(f, c, values, lane);

	/// Hook handlers to track changes.
	static void HookCtrls(Form f, Control ctrl, ValueStore values, Lane lane) {
		var t = ctrl.GetType();
		if (t != typeof(Form)) {
			ctrl.LostFocus += (s, e) => {
				var c = (Control) s;
				if (HasChanged(c, values)) {
					CaptureInput(f, c, values, lane);
				}
				else {
					CaptureOther(f, values, lane);
				}
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(f, c, values, lane);
		}
	}

	/// Hook handlers to track changes.
	static void HookCtrls(Form f, ValueStore values, Lane lane) =>
		HookCtrls(f, f, values, lane);

	/// Initializes Aquaforms. Basically, it hooks controls 
	/// and creates the value storage database to track controls changes.
	static ValueStore Init(Form f, Lane lane) {
		ValueStore values = new ValueStore();
		HookCtrls(f, values, lane);
		return values;
	}

	/// Aqua's entry point. 
	/// This method starts looking for changes, attach commands, and so on...
	public static void Watch(Form f) {	

		Lane lane = new Lane();
		AquaCmds.AttachTo(f, lane);

		f.Shown += (s, e) => {
			ValueStore values = Init(f, lane);

			PrintSolidLine(); //<= Begin spanshot.
			WriteLine("| First Snapshot");
			PrintSolidLine();
			FirstSnapshot(f, values);
			WriteLine("|");
			PrintSolidLine(); //<= End snapshot.
		};
	}


	/// Draws a solid line to the console.
	static void PrintSolidLine() => 
		WriteLine("+".PadRight(60, '-'));

	/// Takes the first snapshot, which is a picture of the whole form.
	static void FirstSnapshot(Control ctrl, ValueStore values) {

		var t = ctrl.GetType();
		if (t != typeof(Form)) {
			WriteLine($"| ({ctrl.Name})"  + $" ({t}) "  + ctrl.Text);
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
		WriteLine($"| Cambio ({ctrl.Name}) ({ctrl.GetType()}) {ctrl.Text}");

	/// Compares the current value of a given control agaist the previous
	/// registered value for that particular control. Returns true if the 
	/// value is different, otherwise, false.
	static bool HasChanged(Control c, ValueStore values) {
		var pval = values.Get(c);
		return String.Compare(pval, c.Text) != 0;
	}

	/// Capture SEs produced by changes on a control.
	static void CaptureSideEffectsRec(Control c, ValueStore values, Lane lane) {
		foreach(Control ctrl in c.Controls) {
			if (HasChanged(ctrl, values)) {
				DieUnless(UpdateValueStore(ctrl, values), "Fail to update values.");
				PrintChange(ctrl);
			}
			CaptureSideEffectsRec(ctrl, values, lane);
		}
	}

	/// Marks the begining of a frame.
	static void BeginFrame(ValueStore values, Control input, Lane lane) {
		var frameMsg = $"| Frame No: {values.FramesCount += 1}";
		Unless(input == null, ()=> { 
			lane.MoveTo(input.Name);
			lane.GetLastMove().RecordChange(input.Text);
			frameMsg += $" -- input: {input.Name}";
		});

		WriteLine(frameMsg);
		PrintSolidLine();
	}

	/// Marks the end of a frame.
	static void EndFrame() {
		WriteLine("|");
		PrintSolidLine();
	}


}
