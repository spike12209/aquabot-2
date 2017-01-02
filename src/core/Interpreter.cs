using System;
using System.Drawing;
using System.Windows.Forms;

using static System.String;
using static Atropos;
using static System.StringSplitOptions;
using static System.Console;


/// Eval replay scripts and performas actions on target forms.
public class Interpreter {
	const string TAB = "{TAB}";

	static void Pass(Control ctrl) {
		ctrl.BackColor = Color.Olive;
	}

	static void Fail(Control ctrl, object expected) {
		//TODO: Tooltip or MsgBox showing the error msg;
		// var msg = $"Expected {expected} was {ctrl.Text}.");
		ctrl.BackColor = Color.Coral;
	}

	/// Finds a "child" control into the target's controls collection.
	/// (Returns null if can't find the control).
	static Control FindCtrl(Control target, string name) {
		Control res = null;
		foreach(Control ctrl in target.Controls) {
			if (ctrl?.Name == name) {
				res = ctrl;
				break;
			}
			else {
				if ((res = FindCtrl(ctrl, name)) != null)
					break;
			}
		}
		return res;
	}

	/// Finds a control into the form or it's controls collection.
	/// (Dies if can't find the control).
	static Control FindCtrlOrDie(Form target, string name) {
		Control res = null;
		foreach(Control ctrl in target.Controls) {
			if (ctrl?.Name == name) {
				res = ctrl;
				break;
			}
			// Find res in ctrl.Controls
			if ((res = FindCtrl(ctrl, name)) != null)
				break;
		}
		DieIf(res == null, $"Failed to find {name}.");
		return res;
	}

	/// Moves the cursor to the next control (based on tab order).
	public Action<Form> Move = target => {
		SendKeys.Send(TAB);
	};
	
	/// Moves the cursor to the specified control.
	public Action<Form, string> Focus = (target, name) => {
		Control ctrl = FindCtrlOrDie(target, name);
		// Select is better than focus beacuse it works even before
		// the form is shown. (Could be useful for unattended tests).
		ctrl.Select();
	};

	/// Changes the value of the control that currently has focus.
	public Action<Form, object> Change = (target, newValue) => {
		// TODO: Make this work for any control (cbo, nums, etc...).
		target.ActiveControl.Text = newValue?.ToString();
	};

	/// Asserts that the value of the control matches the 
	/// specified value.
	public Action<Form, string, object> Assert = (target, name, value) => {
		Control ctrl = FindCtrlOrDie(target, name);
		if (ctrl.Text == value?.ToString())
			Pass(ctrl);
		else
			Fail(ctrl, value);
	};

	/// Invoques the specified command with the given args.
	/// It dies if the command doesn't exists.
	void DispatchCmd(Form target, string cmd, params string[] args) {

		DieIf(target == null, "Target form can't be null.");
		DieIf(IsNullOrEmpty(cmd), "Cmd is required.");

		cmd = cmd.ToLower();
		Write($"Cmd {cmd}\n");
		switch (cmd) {
			case ";": break ; // <= Comment.
			case "move:": 
				Move(target); 
				break;
			case "focus:": 
				DieIf(args.Length == 0, "[Focus] Name is required.");
				Focus(target, args[0]); 
				break;
			case "change:": 
				DieIf(args.Length == 0, "[Change] Name is required.");
				Change(target, args[0]);
				break;
			case "assert:": 
				DieIf(args.Length == 0, "[Assert] Name is required.");
				DieIf(args.Length == 1, "[Assert] Value is required.");
				Assert(target, args[0], args[1]); 
				break;
			default:
				Die($"Unknown cmd => {cmd}");
				break;
		}
	
	}

	/// Executes a line of the script.
	void Dispatch(string line, Form target) {
		var delim = new [] { ' ' };
		var words = line.Split(delim, RemoveEmptyEntries);
		if (words.Length == 0)
			return; //<= Empty line.

		if (words.Length == 1)
			DispatchCmd(target, words[0]);
		else {
			var args = new string[words.Length - 1];
			Array.Copy(words, 1, args, 0, args.Length);
			DispatchCmd(target, words[0], args);
		}
	}

	/// Cleans garbage tokens.
	static string CleanScript(string script) =>
		script.Replace("\t", "");

	/// Parses and executes a replay script.
	/// It dies on syntax errors.
	public void Eval(string script, Form target) {
		DieIf(IsNullOrEmpty(script), "Script can't be null or empty.");

		script = CleanScript(script);
		var delim = new [] { '\n'};
		var iseq  = script.Split(delim, RemoveEmptyEntries);
		for (int i = 0; i < iseq.Length; ++i) 
			Dispatch(iseq[i], target);
	}
}
