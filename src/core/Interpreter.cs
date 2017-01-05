using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using static System.String;
using static Parca;
using static System.StringSplitOptions;
using static System.Console;
using static System.Windows.Forms.MessageBoxButtons;


/// Eval replay scripts and performas actions on target forms.
public class Interpreter {
	const string 
		APPNAME = "AquaForms",
		TAB = "{TAB}";

	static void Pass(Control ctrl) {
		ctrl.BackColor = Color.Olive;
	}

	static void Fail(Control c, object expected) {
		//TODO: Tooltip showing the error when the user hovers over the control.
		var msg = $"[{c.Name}] failed. Expected: [{expected}]. Was: [{c.Text}].";
		WriteLine(msg);
		c.BackColor = Color.Coral;
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
		DieIf(res == null, $"Failed to find [{name}]");
		return res;
	}

	/// Moves the cursor to the next control (based on tab order).
	/// Runs previous assertions (if any).
	public Action<Form, Stack<Action>> Move = (target, asserts) => {
		Write($"move: (from [{target.ActiveControl.Name}])\n");
		SendKeys.Send(TAB);

		Thread.Sleep(500);
		// Are there any pending assertion?
		while (asserts.Count > 0)
			asserts.Pop()();
	};
	
	/// Moves the cursor to the specified control.
	public Action<Form, string> Focus = (target, name) => {
		Write($"focus: [{name}]\n");
		Control ctrl = FindCtrlOrDie(target, name);
		ctrl.Focus();
	};

	/// Changes the value of the control that currently has focus.
	public Action<Form, object> Change = (target, newValue) => {
		Write($"change: [{target.ActiveControl.Name}] to [{newValue}]\n");
		// TODO: Make this work for any control (cbo, nums, etc...).
		target.ActiveControl.Text = newValue?.ToString();
	};

	public bool Confirm(string msg) {
		return MessageBox.Show(msg, APPNAME, YesNo) == DialogResult.Yes;
	}

	bool AutoFix(PreCondError err) {
		DieIf(err == null, "Internal Error. Err can't be null.");
		var msg = $"{err.ToString()}\nDo you want autofix it?";
		return Confirm(msg);
	}

	public Action End = () => { };

	static void Fix(Form f, string name, string text) =>
		FindCtrlOrDie(f, name).Text = text;

	public bool Start (Form f, PreCondErrors errors) {
		WriteLine($"Start errors count: {errors.Count}");

		if (errors.Count  == 0)
			return true;

		int fixes = 0;
		for (int i = 0; i < errors.Count; ++i) {
			var err = errors.At(i);
			if (AutoFix(err)) {
				Fix(f, err.InputName, err.Expected?.ToString());
				++fixes;
			}
		}

		if (fixes == errors.Count) {
			errors.Clear();
			return Start(f, errors);
		}
		return false;
	}

	public Action<Form, string, object, PreCondErrors> Ensure = 
		(f, name, value, errors) => {
			Control ctrl = FindCtrlOrDie(f, name);
			Write($"ensure: [{name}] equals [{value}]\n");
			if (ctrl.Text != value?.ToString())
				errors.Add(name, value, ctrl.Text);
	};

	/// Asserts that the value of the control matches the specified value.
	public Action<Form, string, object, Stack<Action>> Assert = 
		(target, name, value, asserts) => {
			// Will be called on the next move.
			asserts.Push(()=> {
				Control ctrl = FindCtrlOrDie(target, name);
				Write($"assert: [{name}] equals [value]\n");
				if (ctrl.Text == value?.ToString())
					Pass(ctrl);
				else
					Fail(ctrl, value);
			});
	};

	/// Invoques the specified command with the given args.
	/// (It dies if the command doesn't exists).
	/// Returns true if the command success, false otherwise.
	bool DispatchCmd(Form f, Stack<Action> asserts, PreCondErrors errors,
		   	string cmd, params string[] args) {

		DieIf(f == null, "Target form can't be null.");
		DieIf(IsNullOrEmpty(cmd), "Cmd is required.");
		
		if (cmd.StartsWith(";")) //<= Comment
			return true;

		cmd = cmd.ToLower();
		switch (cmd) {
			case "move:":
				Move(f, asserts); 
				break;
			case "focus:": 
				DieIf(args.Length == 0, "[Focus] Name is required.");
				Focus(f, args[0]); 
				break;
			case "change:": 
				Change(f, args.Length > 0 ? args[0] : null);
				break;
			case "end:": 
				End();
				break;
			case "start:": 
				if (!Start(f, errors)) // Can't start due to preconds errors.
					return false;
				else
					return Start(f, errors);
			case "ensure:": 
				DieIf(args.Length == 0, "[Ensure] name is required.");
				DieIf(args.Length == 1, "[Ensure] value is required.");
				Ensure(f, args[0], args[1], errors); 
				break;
			case "assert:": 
				DieIf(args.Length == 0, "[Assert] name is required.");
				DieIf(args.Length == 1, "[Assert] value is required.");
				DieIf(asserts == null,  "[Assert] asserts is required.");
				Assert(f, args[0], args[1], asserts); 
				break;
			default:
				Die($"Unknown cmd => {cmd}");
				break;
		}
		return true;
	}

	/// Executes a line of the script.
	bool Dispatch(string line, Form target, Stack<Action> asserts, 
			PreCondErrors errors) {

		var delim = new [] { ' ' };
		var words = line.Split(delim, RemoveEmptyEntries);
		if (words.Length == 0)
			return true; //<= Empty line.

		if (words.Length == 1)
			return DispatchCmd(target, asserts, errors, words[0]);
		else {
			var args = new string[words.Length - 1];
			Array.Copy(words, 1, args, 0, args.Length);
			return DispatchCmd(target, asserts, errors, words[0], args);
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
		var reader  = new StringReader(script);
		var asserts = new Stack<Action>();
		var errors  = new PreCondErrors();
		string line = null;

		while((line = reader.ReadLine()) != null)
			if (!Dispatch(line, target, asserts, errors))
				break;
	}
}
