using System;
using System.Drawing;
using System.Windows.Forms;

using static System.Console;
using static System.Convert;
using static Atropos;

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

	static void OpenScript() {
		using (var ofd = new OpenFileDialog()) {
			ofd.Filter = "Quality assurance test files (*.qat)|*.qat";
			if (ofd.ShowDialog() == DialogResult.OK) {
			}
		
		}
	}

	public AquaCmds(Form host) {
		// Attach
		Owner = host;
		// Commands
		Button btnRec = null, btnRep = null, btnStop, btnNotes, btnOpen, btnSave;
		Action record, replay, stop;

		record = ()=> { 
			Write("TODO: Start recording...\n");
			btnRep.Enabled = false;
		};

		replay = ()=> { 
			Write("TODO: Start replaying...\n");
			btnRec.Enabled = false;
		};

		stop = () => {
			btnRec.Enabled = true;
			btnRep.Enabled = true;
		};

		btnRec   = CreateBtn("Record", record, null);
		btnRep   = CreateBtn("Replay", replay, btnRec);
		btnStop  = CreateBtn("Stop",   stop, btnRep);
		btnNotes = CreateBtn("Notes",  ()=> Write("Adding notes...\n"), btnStop);
		btnOpen  = CreateBtn("Open",   OpenScript, btnNotes);
		btnSave  = CreateBtn("Save",   ()=> Write("Saving...\n"), btnOpen);

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
