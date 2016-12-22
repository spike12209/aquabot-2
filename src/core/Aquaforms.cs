using System;
using System.Drawing;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;

/// This class tracks controls changes on a given form.
public class Aquaforms {

	/// This is a custom toolbar we add to the form under test.
	class AquaCmds : Form {
		const int 
			CTRLOFF = 10, //<= Space between controls.
			CTRLH   = 30,
			CTRLTOP = 10;

		/// Attaches the aqua commands tool bar to the form under test.
		public static void AttachTo(Form host) {
			var cmds = new AquaCmds(host);
			cmds.Show();
		}

		/// Helper method to create command buttons.
		static Button CreateBtn(string text, Action onClick, Control previous) {
			var btn = new Button();
			if (previous == null) {
				btn.Left = CTRLOFF;
				btn.Top  = CTRLTOP;
			}
			else {
				btn.Left = previous.Left + previous.Width + CTRLOFF;
				btn.Top = previous.Top;
			}

			btn.Text = text;
			btn.Click += (s, e) => onClick();
			return btn;
		}

		static int SumCtrlsWidth(Form f, int offset) {
			int res = 0;
			foreach(Control ctrl in f.Controls)
				res += ctrl.Width + offset;

			return res;
		}

		void SetPositionOnScreen(Form host) {
			var screen = Screen.FromControl(host);
			Height = (CTRLTOP * 2) + CTRLH;
			Width = SumCtrlsWidth(this, CTRLOFF) + CTRLOFF;

			StartPosition = FormStartPosition.Manual;
			int x = screen.Bounds.Width / 2 - Width / 2;
			int y = 0;
			Location = new Point(x, y);
		}
		
		public AquaCmds(Form host) {
			// Attach
			Owner = host;

			// Commands
			Button btnRec, btnRep, btnStop, btnNotes, btnOpen, btnSave;
			btnRec   = CreateBtn("Record", ()=> WriteLine("Recording ...."), null);
			btnRep   = CreateBtn("Replay", ()=> WriteLine("Replaying ...."), btnRec);
			btnStop  = CreateBtn("Stop",   ()=> WriteLine("Stop ...."), btnRep);
			btnNotes = CreateBtn("Notes",  ()=> WriteLine("Adding notes ...."), btnStop);
			btnOpen  = CreateBtn("Open",  ()=> WriteLine("Opening..."), btnNotes);
			btnSave  = CreateBtn("Save",  ()=> WriteLine("Saving..."), btnOpen);

			Controls.Add(btnRec);
			Controls.Add(btnRep);
			Controls.Add(btnStop);
			Controls.Add(btnNotes);
			Controls.Add(btnOpen);
			Controls.Add(btnSave);

			// Position
			FormBorderStyle = FormBorderStyle.None;
			SetPositionOnScreen(host);
		}

	}

	/// Aqua's entry point. 
	/// This method starts looking for changes, attach commands, and so on...
	public static void Watch(Form f) {	

		AquaCmds.AttachTo(f);

		f.Shown += (s, e) => {
			ValueStore values = Init(f);

			PrintSolidLine(); //<= Begin spanshot.
			WriteLine("| First Snapshot");
			PrintSolidLine();
			FirstSnapshot(f, values);
			WriteLine("|");
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
	static void BeginFrame(ValueStore values) {
		WriteLine($"| Frame No: {values.FramesCount += 1}");
		PrintSolidLine();
	}

	/// Marks the end of a frame.
	static void EndFrame() {
		WriteLine("|");
		PrintSolidLine();
	}

	/// Inits the capture of a new frame.
	static void CaptureFrame(Form f, Control sender, ValueStore values) {
		BeginFrame(values);

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
				if (HasChanged(c, values)) {
					//TODO: This is a usr action. Must be recorded accodringly.
					//      (i.e. Mark the action on the script or something).
					//      jhh
					//      One user input can have zero or more side effects.
					//      Size effects are always caused (directly or 
					//      indirectly) by a usr input.
					//
					//      chage txtFoo -> 
					//			side effects on txtBar
					CaptureFrame(f, c, values);
				}
				else {
					// At this branch changes are *NO* user inputs but
					// side effects of those inputs.
					//FIXME: Look for dependencies changes.
				}
			};
		}

		foreach(Control c in ctrl.Controls) {
			HookCtrls(f, c, values);
		}
	}
}
