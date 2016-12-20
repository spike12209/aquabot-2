using System;
using System.Windows.Forms;

using static System.Convert;
using static System.Console;
using static System.String;

class Program {

	static int GetTop(Control last) {
		return last.Top + last.Height + 10;
	}

	static void CreateCtrls(Form f) {
		const int LEFT = 20;

		Control last, txtPrice, txtQty, txtTot;
		txtPrice      = new TextBox();
		txtPrice.Name = "Price";
		txtPrice.Top  = 20;
		last          = txtPrice;
		last.Left     = LEFT;

		txtQty      = new TextBox();
		txtQty.Name = "Qty";
		txtQty.Top  = GetTop(last);
		last        = txtQty;
		last.Left   = LEFT;

		txtTot      = new TextBox();
		txtTot.Name = "Tot";
		txtTot.Top  = GetTop(last);
		txtTot.Left = LEFT;

		f.Controls.Add(txtPrice);
		f.Controls.Add(txtQty);
		f.Controls.Add(txtTot);

		// Handlers
		Action<Control, Control> updateTot = (price, qty) => {
			int p = IsNullOrEmpty(price.Text) ? 0 : ToInt32(txtPrice.Text);
			int q = IsNullOrEmpty(qty.Text)   ? 0 : ToInt32(qty.Text);
			txtTot.Text = (p * q).ToString();
		};

		// Hook handlers
		txtPrice.LostFocus += (s, e) => updateTot(txtPrice, txtQty);
		txtQty.LostFocus   += (s, e) => updateTot(txtPrice, txtQty);

		f.Shown += (s, e) => {
			txtPrice.Text = "123";	
			txtQty.Text = "2";	
			updateTot(txtPrice, txtQty);
			WriteLine("Su shown");
		};
	}

	static void Main(params string [] args) {
		var f = new Form();
		CreateCtrls(f);
		Aquaforms.Watch(f);
		f.ShowDialog();
	}

}
