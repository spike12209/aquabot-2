using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using static System.String;
using static Parca;
using static System.StringSplitOptions;
using static System.Console;


/// Eval replay scripts and performas actions on target forms.
public class Interpreter {
	const string TAB = "{TAB}";

	static void Pass(Control ctrl) {
		ctrl.BackColor = Color.Olive;
	}

	static void Fail(Control ctrl, object expected) {
		//TODO: Tooltip showing the error when the user hovers over the control.
		var msg = $"[ctrl.Name] - Expected {expected} was {ctrl.Text}.";
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
	/// Runs previous assertions (if any).
	public Action<Form, Stack<Action>> Move = (target, asserts) => {
		Write($"move:   (from {target.ActiveControl.Name}).\n");
		SendKeys.Send(TAB);

		Thread.Sleep(500);
		// Are there any pending assertion?
		while (asserts.Count > 0)
			asserts.Pop()();
	};
	
	/// Moves the cursor to the specified control.
	public Action<Form, string> Focus = (target, name) => {
		Write($"focus:  (on {name}).\n");
		Control ctrl = FindCtrlOrDie(target, name);
		// Select is better than focus beacuse it works even before
		// the form is shown. (Could be useful for unattended tests).
		ctrl.Select();
	};

	/// Changes the value of the control that currently has focus.
	public Action<Form, object> Change = (target, newValue) => {
		Write($"change: {target.ActiveControl.Name} to {newValue}.\n");
		// TODO: Make this work for any control (cbo, nums, etc...).
		target.ActiveControl.Text = newValue?.ToString();
	};

	/// Asserts that the value of the control matches the specified value.
	public Action<Form, string, object, Stack<Action>> Assert = 
		(target, name, value, asserts) => {
			// Will be called on the next move.
			asserts.Push(()=> {
					Control ctrl = FindCtrlOrDie(target, name);
					Write($"assert: {name} equals {value}.\n");
					if (ctrl.Text == value?.ToString())
						Pass(ctrl);
					else
						Fail(ctrl, value);
				});
	};

	/// Invoques the specified command with the given args.
	/// It dies if the command doesn't exists.
	void DispatchCmd(Form target, Stack<Action> asserts, string cmd, 
			params string[] args) {

		DieIf(target == null,     "Target form can't be null.");
		DieIf(IsNullOrEmpty(cmd), "Cmd is required.");
		
		if (cmd.StartsWith(";")) //<= Comment
			return;

		cmd = cmd.ToLower();
		switch (cmd) {
			case "move:": 
				Move(target, asserts); 
				break;
			case "focus:": 
				DieIf(args.Length == 0, "[Focus] Name is required.");
				Focus(target, args[0]); 
				break;
			case "change:": 
				Change(target, args.Length > 0 ? args[0] : null);
				break;
			case "assert:": 
				DieIf(args.Length == 0, "[Assert] name is required.");
				DieIf(args.Length == 1, "[Assert] value is required.");
				DieIf(asserts == null,  "[Assert] asserts is required.");
				Assert(target, args[0], args[1], asserts); 
				break;
			default:
				Die($"Unknown cmd => {cmd}");
				break;
		}
	}

	/// Executes a line of the script.
	void Dispatch(string line, Form target, Stack<Action> asserts) {
		var delim = new [] { ' ' };
		var words = line.Split(delim, RemoveEmptyEntries);
		if (words.Length == 0)
			return; //<= Empty line.

		if (words.Length == 1)
			DispatchCmd(target, asserts, words[0]);
		else {
			var args = new string[words.Length - 1];
			Array.Copy(words, 1, args, 0, args.Length);
			DispatchCmd(target, asserts, words[0], args);
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
		var asserts = new Stack<Action>();
		for (int i = 0; i < iseq.Length; ++i) 
			Dispatch(iseq[i], target, asserts);
	}
}
