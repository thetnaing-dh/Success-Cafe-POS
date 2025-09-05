using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormAdvance : Form
    {
        public string adv {  get; set; }
        MyanmartoEnglish mte = new MyanmartoEnglish();
        public int bal { get; set; }

        public FormAdvance()
        {
            InitializeComponent();
        }

        private void FormAdvance_Load(object sender, EventArgs e)
        {

        }

        private void txtAdvance_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = txtAdvance.Text;

            int selectionStart = txtAdvance.SelectionStart;
            int selectionLength = txtAdvance.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, e.KeyChar.ToString());

            if (int.TryParse(newText, out int result))
            {
                if (result > bal)
                {
                    e.Handled = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtAdvance.Text.Trim().Length == 0) {
                adv = "0";
            }
            else
            {
                adv = mte.convert_text(txtAdvance.Text);
            }
                this.Close();
        }

        private void txtAdvance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (txtAdvance.Text.Trim().Length == 0)
                {
                    adv = "0";
                }
                else
                {
                    adv = mte.convert_text(txtAdvance.Text);
                }
                this.Close();
            }
        }
    }
}
