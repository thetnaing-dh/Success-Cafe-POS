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
    public partial class FormMergeTable: Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd,cmd2;
        SQLiteDataReader dr;
        Button btnTableGroup, btnTable;
        String selectTableGroup = "0", selectTable = "0";
        public string invid { get; set; }
        public string tableName { get; set; }
        public string tableid { get; set; }
        public string newInvId { get; set; } = "0";


        public FormMergeTable()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            tableName = null;
            this.Close();         
        }

        private void FormMergeTable_Load(object sender, EventArgs e)
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

        private void btnCategorySave_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();

                cmd = new SQLiteCommand("select srno from saledet where id like @id; ", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    if (dr.HasRows)
                    {
                        cmd2 = new SQLiteCommand("update saledet set id = @newid, srno = (select ifnull(max(srno)+1,1) from saledet where id like @newid) where id like @id and srno like @sr; ", db.cn);
                        cmd2.Parameters.AddWithValue("@id", invid);
                        cmd2.Parameters.AddWithValue("@newid", newInvId);
                        cmd2.Parameters.AddWithValue("@sr", dr["srno"].ToString());
                        cmd2.ExecuteNonQuery();
                    }
                }
                dr.Close();

                cmd = new SQLiteCommand("update salehead set tableid = @tableid where id like @id; " +
                    "update tables set tblstatus ='' where id like @tid; " +                    
                    "delete from salehead where id like @id;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@newid", newInvId);
                cmd.Parameters.AddWithValue("@tid", tableid);
                cmd.Parameters.AddWithValue("@tableid", selectTable);              
                cmd.ExecuteNonQuery(); 
                
                db.cn.Close();
                this.Close();
            }
            catch   
            {           
              
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
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE (SELECT MIN(groupid) FROM Tables) and tblstatus like 'Pending' and t.id !=@tid", db.cn);

                }
                else
                {
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE @id and tblstatus like 'Pending' and t.id != @tid", db.cn);
                    cmd.Parameters.AddWithValue("@id", selectTableGroup);
                }
                cmd.Parameters.AddWithValue("@tid", tableid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
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
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select distinct(id) from salehead where tableid like @id and status like 'Pending'", db.cn);
                cmd.Parameters.AddWithValue("@id", selectTable);
                newInvId = cmd.ExecuteScalar().ToString();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            tableName = clickedbutton.Text.ToString();
            clickedbutton.BackColor = Color.Red;
        }
    }
}
