using System;
using System.Linq;
using System.Windows.Forms;

using static System.Convert;
using static System.Console;
using static System.String;

class Program {

	static int GetTop(Control last) {
		return last.Top + last.Height + 10;
	}

	static void CreateCtrls(Form f) {
		const int LEFT = 20, LBLWIDTH = 50;

		Control last, 
				txtPrice, txtQty,  txtTot, txtNet, txtTax,
				lblPrice, lblQty,  lblTot, lblNet, lblTax;

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

		txtNet      = new TextBox();
		txtNet.Name = "net";
		txtNet.Top  = GetTop(last);
		txtNet.Left = left;
		last        = txtNet;

		txtTax      = new TextBox();
		txtTax.Name = "tax";
		txtTax.Top  = GetTop(last);
		txtTax.Left = left;
		last        = txtTax;

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

		lblNet         = new Label();
		lblNet.Text    = "Net:";
		lblNet.Left    = LEFT;
		lblNet.Top     = txtNet.Top;
		lblNet.Width   = LBLWIDTH;

		lblTax         = new Label();
		lblTax.Text    = "Tax:";
		lblTax.Left    = LEFT;
		lblTax.Top     = txtTax.Top;
		lblTax.Width   = LBLWIDTH;

		lblTot         = new Label();
		lblTot.Text    = "Total:";
		lblTot.Left    = LEFT;
		lblTot.Top     = txtTot.Top;
		lblTot.Width   = LBLWIDTH;

		// Add controls to the form.
		f.Controls.Add(lblPrice);
		f.Controls.Add(lblQty);
		f.Controls.Add(lblNet);
		f.Controls.Add(lblTax);
		f.Controls.Add(lblTot);

		f.Controls.Add(txtPrice);
		f.Controls.Add(txtQty);
		f.Controls.Add(txtNet);
		f.Controls.Add(txtTax);
		f.Controls.Add(txtTot);

		// Handlers
		Action<Control, Control> updateTot = (price, qty) => {
			try {
				double p    = IsNullOrEmpty(price.Text) ? 0d : ToDouble(txtPrice.Text);
				double q    = IsNullOrEmpty(qty.Text)   ? 0d : ToDouble(qty.Text);
				double net  = p * q;
				double tax  = net * 0.21;
				txtNet.Text = net.ToString();
				txtTax.Text = tax.ToString();
				txtTot.Text = (net + tax).ToString();
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
			txtPrice.Text = "100";	
			txtQty.Text = "2";	
			updateTot(txtPrice, txtQty);
		};

		var txts = (from c in f.Controls.OfType<TextBox>() select c).ToArray();
		foreach(var txt in txts)
			txt.TextAlign = HorizontalAlignment.Right;
	}

	[STAThread]
	static void Main(params string [] args) {
		var f = new Form();
		f.Text = "Test Form";
		CreateCtrls(f);
		bool quiet = args.Length >= 1 ? args[0] == "-q" : false;

		Aquaforms.Watch(f, quiet);
		Application.Run(f);
	}

}
