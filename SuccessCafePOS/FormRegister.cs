using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SuccessCafePOS
{
    public partial class FormRegister : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormRegister()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void loadReg()
        {
            if (Status.valid >= 1)
            {
                lbDate.Visible = true;
                lbDate.Text = string.Format("{0:dd/MMM/yyyy}", DateTime.Now.AddDays(Status.valid)) + "  for [" + string.Format("{0}", Status.valid + "] days");
            }
            else if (Status.valid <= 0)
            {
                Status.valid = 0;
            }
        }

        private void FormRegister_Load(object sender, EventArgs e)
        {
            txtKey.Text = ComputerInfo.GetProductKey();
            checkReg();
            loadReg();         
        }

        void checkReg()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select strftime(\"%Y-%m-%d\", syscode, 'unixepoch') expired from register where id = (select max(id) from register) and productkey like @key", db.cn);
                cmd.Parameters.AddWithValue("@key",txtKey.Text);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Status.expire = Convert.ToDateTime(dr["expired"]);
                    Status.valid = (Status.expire - DateTime.Now).Days + 1;
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
                Status.valid = 0;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtSr.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Fill Serial No. to register!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSr.Focus();
                    return;
                }
                string str = txtSr.Text.Replace("-", "");
                if(str.Length != 30)
                {
                    MessageBox.Show("Invalid Register!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSr.Clear();
                    return;
                }
                string fsr = str.Substring(0, 13);
                string lsr = str.Substring(17, 13);
                int v = 0;
                db.cn.Open();
                cmd = new SQLiteCommand("select pcid from register where fsr like @fsr or lsr like @lsr", db.cn);
                cmd.Parameters.AddWithValue("@fsr", fsr);
                cmd.Parameters.AddWithValue("@lsr", lsr);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    v = 0;
                }
                else
                {
                    v = 1;
                }
                dr.Close();
                db.cn.Close();

                if (v == 0)
                {
                    MessageBox.Show("Invalid Register!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSr.Clear();
                    return;
                }

                else
                {
                    KeyManager km = new KeyManager(txtKey.Text);

                    if (km.ValidKey(ref str))
                    {
                        int day = 0;
                        DateTime dt = DateTime.Now;
                        KeyValuesClass kv = new KeyValuesClass();
                        //Decrypt license key
                        if (km.DisassembleKey(str, ref kv))
                        {
                            if(kv.Version == 233 && kv.ProductCode == 18)
                            {
                                day = (kv.Expiration - DateTime.Now.Date).Days + Status.valid;
                                dt = DateTime.Now.Date.AddDays(day);
                            }
                            else
                            {
                                MessageBox.Show("Invalid Register!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtSr.Clear();
                                return;
                            }
                        }                       

                        db.cn.Open();
                        cmd = new SQLiteCommand("insert into register(pcid,productkey,fsr,lsr,days,expired,syscode) values(@pcid,@key,@fsr,@lsr,@day,@date,unixepoch(@date))", db.cn);
                        cmd.Parameters.AddWithValue("@pcid", str);
                        cmd.Parameters.AddWithValue("@key", txtKey.Text);
                        cmd.Parameters.AddWithValue("@fsr", fsr);
                        cmd.Parameters.AddWithValue("@lsr", lsr);
                        cmd.Parameters.AddWithValue("@day", day);
                        cmd.Parameters.AddWithValue("@date", dt );
                        cmd.Parameters.AddWithValue("@syscode", 1);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();

                        MessageBox.Show("Registration complete successfully!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        checkReg();
                        loadReg();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Register!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    txtSr.Clear();
                }
            }
            catch  
            {
                db.cn.Close();
             }
        }

        private void btnTrial_Click(object sender, EventArgs e)
        {
            if (Status.valid == 0)
            {
                MessageBox.Show("Your trial has ended!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.Close();
        }
    }
}
