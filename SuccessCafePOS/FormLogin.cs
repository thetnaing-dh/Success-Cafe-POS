using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormLogin : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        string productkey = ComputerInfo.GetProductKey();
        string serialno = "";
        Backup bk = new Backup();

        public FormLogin()
        {
            InitializeComponent();
        }

        string CheckProductKey()
        {
            string key = "";
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select productkey from register where id like (select max(id) from register)",db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    key = dr.GetString(0);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return key;
        }

        void checkReg()
        {
            try
            {
                string storeProductKey = CheckProductKey();
                if (storeProductKey == productkey || storeProductKey == "Success Professional Institute")
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("select expired from register where expired like '2018-03-23' and id = (select max(id) from register)", db.cn);
                    dr = cmd.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        SQLiteCommand cmd1 = new SQLiteCommand("update register set expired = date('now','30 days'),syscode = unixepoch(date('now','30 days')),productkey ='" + productkey + "' where expired like '2018-03-23'", db.cn);
                        cmd1.ExecuteNonQuery();
                    }
                    else
                    {
                        dr.Close();
                        cmd = new SQLiteCommand("select strftime(\"%Y-%m-%d\", syscode, 'unixepoch') expired from register where id = (select max(id) from register)", db.cn);
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            Status.expire = Convert.ToDateTime(dr["expired"].ToString());
                            Status.valid = (Status.expire - DateTime.Now).Days + 1;
                        }
                        dr.Close();
                    }

                    db.cn.Close();
                }
                else
                {
                    Status.valid = 0;
                }
            }
            catch
            {
                db.cn.Close();
                Status.valid = 0;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            bk.backup();
            Application.Exit();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                login(txtUserName.Text, txtPassword.Text);
            }
        }

        void login(string user, string pass)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from users where username like @name and password like @pass and inactive = 0", db.cn);
                cmd.Parameters.AddWithValue("@name", user);
                cmd.Parameters.AddWithValue("@pass", pass);
                txtPassword.Clear();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string userid = dr["id"].ToString();
                    string username = dr["username"].ToString();
                    dr.Close();
                    db.cn.Close();
                    if (Status.valid <= 0)
                    {
                        MessageBox.Show("Please Register to Login!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormRegister form = new FormRegister();
                        form.ShowDialog();
                        checkReg();
                        return;
                    }
                    else if (Status.valid < 5)
                    {
                        MessageBox.Show("Your system is expired soon! Please Register.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    if (Screen.FromControl(this).Bounds.Width > 1536)
                    {
                        FormMain1536 frm = new FormMain1536();
                        frm.Width = 1536;
                        frm.Height = 816;
                        frm.StartPosition = FormStartPosition.CenterScreen;
                        Status.userid = userid;
                        Status.username = username;
                        this.Hide();
                        frm.ShowDialog();
                    }

                    else if (Screen.FromControl(this).Bounds.Width > 1366)
                    {
                        FormMain1536 frm = new FormMain1536();
                        frm.WindowState = FormWindowState.Maximized;
                        Status.userid = userid;
                        Status.username = username;
                        this.Hide();
                        frm.ShowDialog();
                    }

                    else if (Screen.FromControl(this).Bounds.Width > 1280)
                    {
                        FormMain frm = new FormMain();
                        Status.userid = userid;
                        Status.username = username;
                        this.Hide();
                        frm.ShowDialog();
                    }
                    else
                    {
                        FormMain1280 frm = new FormMain1280();
                        Status.userid = userid;
                        Status.username = username;
                        this.Hide();
                        frm.ShowDialog();
                    }    
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    db.cn.Close();
                }
            }
            catch
            {

                db.cn.Close();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            login(txtUserName.Text, txtPassword.Text);
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                bool isUserActive = false;
                db.cn.Open();      
                cmd = new SQLiteCommand("Select username from users where inactive = 0 order by id limit 1", db.cn);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    isUserActive = true;                   
                }
                dr.Close();
                db.cn.Close();
                if (isUserActive)
                {                    
                    db.cn.Open();
                    cmd = new SQLiteCommand("Select username from users where inactive = 0 order by id limit 1", db.cn);
                    txtUserName.Text = cmd.ExecuteScalar().ToString();
                    db.cn.Close();
                }
                else
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update users set inactive = 0, settings = 1 where id like (select min(id) from users)", db.cn);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();

                    db.cn.Open();
                    cmd = new SQLiteCommand("Select username from users where inactive = 0 order by id limit 1", db.cn);
                    txtUserName.Text = cmd.ExecuteScalar().ToString();
                    db.cn.Close();
                }               
            }
            catch
            {
                db.cn.Open();
            }
            autoCompleteUserName();
            checkReg();
        }

        void autoCompleteUserName()
        {
            try
            {
                AutoCompleteStringCollection myCollection = new AutoCompleteStringCollection();
                txtUserName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                db.cn.Open();
                cmd = new SQLiteCommand("select username from users where inactive = 0", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    myCollection.Add(dr["username"].ToString());

                }
                txtUserName.AutoCompleteCustomSource = myCollection;

                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void pbManual_Click(object sender, EventArgs e)
        {
            string exePath =
    System.IO.Path.GetDirectoryName(
       System.Reflection.Assembly.GetEntryAssembly().Location);

            string file = exePath + @"\User Manual.pdf";
            Process.Start(file);
        }
    }
}
