using System;
using System.IO;
using System.Text;
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
	public static void AttachTo(Form host, Lane lane) {
		var cmds = new AquaCmds(host, lane);
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

	static void Replay (string script, Form host) {
		try {
			var inter = new Interpreter();
			inter.Eval(script, host);
		}
		catch(Exception ex) {
			MessageBox.Show(ex.Message);
		}
	}


	static string OpenScript() {
		using (var ofd = new OpenFileDialog()) {
			ofd.Filter = "Quality assurance test files (*.qat)|*.qat";
			if (ofd.ShowDialog() == DialogResult.OK) {
				var script = new StringBuilder();
				using (var sr = new StreamReader(ofd.OpenFile())) {
					while(sr.Peek() > 0)
						script.AppendLine(sr.ReadLine());
				}
				return script.ToString();
			}
		}
		return null;
	}

	/// host is the form under test.
	/// lane is some sort of a timeline where changes and SE are reflected.
	public AquaCmds(Form host, Lane lane) {
		// Attach
		Owner = host;
		// Commands
		Button btnRec = null, btnRep = null, btnStop, btnNotes, btnOpen, btnSave;
		Action record, replay, stop, save, takeNote;

		takeNote = ()=> Write("Adding notes...\n");

		replay = ()=> { 
			var script = OpenScript();
			btnRec.Enabled = false;
			Replay(script, Owner);
		};

		record = ()=> { 
			Write("TODO: Start recording...\n");
			btnRep.Enabled = false;
		};

		stop = () => {
			btnRec.Enabled = true;
			btnRep.Enabled = true;
		};

		save = () => {
			var src = lane.CreateScript();
			using (var sfd = new SaveFileDialog()) {
				sfd.Filter = "Quality assurance test files (*.qat)|*.qat";
				if (sfd.ShowDialog() != DialogResult.OK) 
					return;

				Stream strm = null;
				if ((strm = sfd.OpenFile()) != null) {
					using (var sw = new StreamWriter(strm)) {
						sw.Write(src);
						sw.Close();
					}
					strm.Close();
				}
			}
		};

		btnRec   = CreateBtn("Record", record, null);
		btnRep   = CreateBtn("Replay", replay, btnRec);
		btnStop  = CreateBtn("Stop",   stop, btnRep);
		btnNotes = CreateBtn("Notes",  takeNote, btnStop);
		// btnOpen  = CreateBtn("Open",   open, btnNotes);
		btnSave  = CreateBtn("Save",   save, btnNotes);

		Controls.Add(btnRec);
		Controls.Add(btnRep);
		Controls.Add(btnStop);
		Controls.Add(btnNotes);
		// Controls.Add(btnOpen);
		Controls.Add(btnSave);

		// Position
		FormBorderStyle = FormBorderStyle.None;
		SetPositionOnScreen(host);
	}

}
