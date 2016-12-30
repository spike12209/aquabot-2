using System;
using System.Windows.Forms;

using static System.String;
using static Atropos;
using static System.StringSplitOptions;
using static System.Console;


/// Eval replay scripts and performas actions on target forms.
public class Interpreter {

	/// Moves the cursor to the next control (based on tab order).
	public Action<Form> Move = target => { /*TODO: move*/ };
	
	/// Moves the cursor to the specified control.
	public Action<Form, string> Focus = (target, name) => {};

	/// Changes the value of the control that currently has focus.
	public Action<Form, object> Change = (target, newValue) => {};

	/// Asserts that the value of the control matches the 
	/// specified value.
	public Action<Form, string, object> Assert = (target, name, value) => {};

	/// Invoques the specified command with the given args.
	/// It dies if the command doesn't exists.
	void DispatchCmd(Form target, string cmd, params string[] args) {

		DieIf(target == null, "Target form can't be null.");
		DieIf(IsNullOrEmpty(cmd), "Cmd is required.");

		cmd = cmd.ToLower();
		Write($"Cmd {cmd}\n");
		switch (cmd) {
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
		var delim = new [] { '\n', ';' };
		var iseq  = script.Split(delim, RemoveEmptyEntries);
		for (int i = 0; i < iseq.Length; ++i) 
			Dispatch(iseq[i], target);
	}
}
