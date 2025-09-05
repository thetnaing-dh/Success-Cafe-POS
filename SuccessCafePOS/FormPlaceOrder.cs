using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormPlaceOrder : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        string isRead = "";
        public string invid {  get; set; }

        public string status { get; set; }

        public FormPlaceOrder()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPlaceOrder_Load(object sender, EventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            cboHH.Text = DateTime.Now.ToString("hh");
            cboMM.Text = DateTime.Now.ToString("mm");
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from tblorder where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    isRead = dr["orderdate"].ToString();
                    DateTime time = DateTime.ParseExact(dr["time"].ToString(), "hh:mm:ss:tt", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    txtDate.Text = DateTime.ParseExact(dr["orderdate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    txtRemark.Text = dr["Remark"].ToString();
                    cboHH.Text = time.ToString("hh");
                    cboMM.Text = time.ToString("mm");
                    cboTime.Text = time.ToString("tt");
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {               
                db.cn.Close();
            }
        }      

        private void btnDate_Click(object sender, EventArgs e)
        {
            if (calendar.Visible == false)
            {
                calendar.Visible = true;
            }
            else { calendar.Visible = false; }
        }

        private void calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDate.Text = calendar.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtDate.SelectionStart = 0;
            txtDate.SelectionLength = 0;
            calendar.Hide();
        }

        private void cboMM_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = cboMM.Text;

            int selectionStart = cboMM.SelectionStart;
            int selectionLength = cboMM.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, e.KeyChar.ToString());

            if (int.TryParse(newText, out int result))
            {
                if (result > 60)
                {
                    e.Handled = true;
                }
            }
        }

        private void cboHH_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = cboHH.Text;

            int selectionStart = cboHH.SelectionStart;
            int selectionLength = cboHH.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, e.KeyChar.ToString());

            if (int.TryParse(newText, out int result))
            {
                if (result > 12)
                {
                    e.Handled = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string str = cboHH.Text + ":" + cboMM.Text + ":" + cboTime.Text;              
                DateTime time = DateTime.ParseExact(str, "hh:mm:tt", CultureInfo.InvariantCulture, DateTimeStyles.None);
                db.cn.Open();
                if (isRead == "")
                {
                    cmd = new SQLiteCommand("insert into tblOrder(id,orderdate,time,remark) values(@id,@orderdate,@time,@remark);" +
                        "update salehead set status = 'Order' where id like @id; ", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("update tblOrder set orderdate=@orderdate,time=@time,remark=@remark where id like @id", db.cn);
                }
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@orderdate", dt.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@time", time.ToString("hh:mm:ss:tt"));
                cmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                cmd.ExecuteNonQuery();

                status = "Order";
              
                db.cn.Close();
                this.Close();
            }
            catch  
            {
                
                db.cn.Close();
            }
        }
    }
}
