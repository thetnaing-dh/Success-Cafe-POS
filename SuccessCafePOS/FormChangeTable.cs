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
    public partial class FormChangeTable: Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        Button btnTableGroup, btnTable;
        string selectTableGroup = "0", selectTable = "0";
        public string invid {  get; set; }  
        public string tableName { get; set; }

        public FormChangeTable()
        {
            InitializeComponent();
        }

        private void FormChangeTable_Load(object sender, EventArgs e)
        {
            loadTableGroup();
            loadTable();
        }

        void loadTableGroup()
        {
            try
            {
                flowLayoutTableGroup.Controls.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("SELECT * FROM TableGroup", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    btnTableGroup = new Button();
                    btnTableGroup.Width = 102;
                    btnTableGroup.Height = 70;
                    btnTableGroup.Text = dr["name"].ToString();
                    btnTableGroup.BackColor = Color.FromArgb(255, 207, 78);
                    //btnTableGroup.FlatStyle = FlatStyle.Flat;
                    //btnTableGroup.FlatAppearance.BorderSize = 0;
                    btnTableGroup.FlatAppearance.MouseOverBackColor = Color.Orange;
                    btnTableGroup.Cursor = Cursors.Hand;
                    btnTableGroup.Tag = dr["id"];
                    btnTableGroup.Click += BtnTableGroup_Click;

                    flowLayoutTableGroup.Controls.Add(btnTableGroup);
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

        private void BtnTableGroup_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectTableGroup = clickedbutton.Tag.ToString();
            loadTable();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            tableName = null;
            this.Close();
        }

        private void btnCategorySave_Click(object sender, EventArgs e)
        {
            try {
                db.cn.Open();
                cmd = new SQLiteCommand("update tables set tblstatus = '' where id like " +
                    "(select tableid from salehead where id like @id); " +
                    "update salehead set tableid = @tableid where id like @id; " +
                    "update tables set tblstatus = 'Pending' where id like @tableid", db.cn);
                cmd.Parameters.AddWithValue("@tableid", selectTable);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                this.Close();
            }
            catch {              
                db.cn.Close();
            }
        }

        void loadTable()
        {
            try
            {
                flowLayoutTable.Controls.Clear();

                db.cn.Open();
                if (selectTableGroup == "0")
                {
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE (SELECT MIN(groupid) FROM Tables) and inactive = 0", db.cn);

                }
                else
                {
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE @id and inactive = 0", db.cn);
                    cmd.Parameters.AddWithValue("@id", selectTableGroup);
                }

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["tblstatus"].ToString() != "Pending")
                    {
                        btnTable = new Button();
                        btnTable.Width = 102;
                        btnTable.Height = 70;
                        btnTable.Text = dr["name"].ToString();
                        if (dr["tblstatus"].ToString() == "Pending")
                        {
                            btnTable.BackColor = Color.FromArgb(250, 70, 70);
                            btnTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 20, 30);
                        }
                        else
                        {
                            btnTable.BackColor = Color.FromArgb(82, 244, 138);
                            btnTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 220, 60);
                        }
                        //btnTable.FlatStyle = FlatStyle.Flat;
                        //btnTable.FlatAppearance.BorderSize = 0;
                        btnTable.Cursor = Cursors.Hand;
                        btnTable.Tag = dr["id"];
                        btnTable.Click += BtnTable_Click;

                        flowLayoutTable.Controls.Add(btnTable);
                    }                 
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

        private void BtnTable_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectTable = clickedbutton.Tag.ToString();
            tableName = clickedbutton.Text.ToString();
            clickedbutton.BackColor = Color.Red;
        }
    }
}
