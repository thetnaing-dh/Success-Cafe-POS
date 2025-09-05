using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormSettingPassCheck : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormSettingPassCheck()
        {
            InitializeComponent();
        }

        void checkPass()
        {
            try
            {
                db.cn.Open();
                bool passValid = false;
                cmd = new SQLiteCommand("select * from settings where password like @password", db.cn);
                cmd.Parameters.AddWithValue("@password", txtPass.Text);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    passValid = true;
                }
                dr.Close();
                db.cn.Close();
                if (passValid)
                {
                    FormSetting frm = new FormSetting();
                    frm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Password!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void FormSettingPassCheck_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            checkPass();
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                checkPass();
            }
        }
    }
}
