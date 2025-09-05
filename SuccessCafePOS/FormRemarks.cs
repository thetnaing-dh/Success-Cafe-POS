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
    public partial class FormRemarks: Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        Button btnRemark;
        public string id {  get; set; }
        public string sr { get; set; }

        public FormRemarks()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormRemarks_Load(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select remark from saledet where id like @id and srno like @sr", db.cn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@sr", sr);
                txtRemark.Text = cmd.ExecuteScalar().ToString();                
                db.cn.Close();

                flowLayoutPanel1.Controls.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("SELECT * FROM Remarks", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    btnRemark = new Button();
                    btnRemark.Width = 120;
                    btnRemark.Height = 70;
                    btnRemark.Text = dr["remark"].ToString();
                    btnRemark.BackColor = Color.FromArgb(255, 207, 78);
                    btnRemark.FlatStyle = FlatStyle.Flat;
                    btnRemark.FlatAppearance.BorderSize = 0;
                    btnRemark.FlatAppearance.MouseOverBackColor = Color.Orange;
                    btnRemark.Cursor = Cursors.Hand;
                    btnRemark.Tag = dr["id"];
                    btnRemark.Click += BtnRemark_Click;

                    flowLayoutPanel1.Controls.Add(btnRemark);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                dr.Close();
                db.cn.Close();
            }
        }

        private void BtnRemark_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            if (txtRemark.Text.Length == 0) {
                txtRemark.Text = clickedbutton.Text;
            }
            else
            {
                txtRemark.Text += ", " + clickedbutton.Text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtRemark.Clear();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("update saledet set remark=@remark where id like @id and srno like @sr", db.cn);
                cmd.Parameters.AddWithValue("@remark",txtRemark.Text);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@sr", sr);
                cmd.ExecuteNonQuery();
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
