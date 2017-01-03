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
		const int LEFT = 20, LBLWIDTH = 45;

		Control last, 
				txtPrice, txtQty, txtNet, txtTax, txtTot, 
				lblPrice, lblQty, lblNet, lblTax, lblTot;

		int left = LEFT + LBLWIDTH + 5;

		// Create inputs.
		txtPrice      = new TextBox();
		txtPrice.Name = "prc";
		txtPrice.Top  = 20;
		txtPrice.Left = left;
		last          = txtPrice;

		txtQty      = new TextBox();
		txtQty.Name = "qty";
		txtQty.Top  = GetTop(last);
		txtQty.Left = left;
		last        = txtQty;

		txtTot      = new TextBox();
		txtTot.Name = "tot";
		txtTot.Top  = GetTop(last);
		txtTot.Left = left;
		last        = txtTot;

		// Create labels.
		lblPrice       = new Label();
		lblPrice.Text  = "Price:";
		lblPrice.Left  = LEFT;
		lblPrice.Top   = txtPrice.Top;
		lblPrice.Width = LBLWIDTH;

		lblQty         = new Label();
		lblQty.Text    = "Quantity:";
		lblQty.Left    = LEFT;
		lblQty.Top     = txtQty.Top;
		lblQty.Width   = LBLWIDTH;

		lblTot         = new Label();
		lblTot.Text    = "Total:";
		lblTot.Left    = LEFT;
		lblTot.Top     = txtTot.Top;
		lblTot.Width   = LBLWIDTH;

		// Add controls to the form.
		f.Controls.Add(lblPrice);
		f.Controls.Add(lblQty);
		f.Controls.Add(lblTot);

		f.Controls.Add(txtPrice);
		f.Controls.Add(txtQty);
		f.Controls.Add(txtTot);

		// Handlers
		Action<Control, Control> updateTot = (price, qty) => {
			try {
				int p = IsNullOrEmpty(price.Text) ? 0 : ToInt32(txtPrice.Text);
				int q = IsNullOrEmpty(qty.Text)   ? 0 : ToInt32(qty.Text);
				txtTot.Text = (p * q).ToString();
				// To test errors....
				// txtTot.Text = "123";
			}
			catch {
				txtTot.Text = "#ERR";
			}
		};

		// Hook handlers
		txtPrice.LostFocus += (s, e) => updateTot(txtPrice, txtQty);
		txtQty.LostFocus   += (s, e) => updateTot(txtPrice, txtQty);

		f.Shown += (s, e) => {
			txtPrice.Text = "123";	
			txtQty.Text = "2";	
			updateTot(txtPrice, txtQty);
		};
	}

	static void Main(params string [] args) {
		var f = new Form();
		CreateCtrls(f);
		Aquaforms.Watch(f);
		f.ShowDialog();
	}

}
