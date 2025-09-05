using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuccessCafePOS.Properties;

namespace SuccessCafePOS
{
    public partial class FormSetting : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        Backup bk = new Backup();

        public FormSetting()
        {
            InitializeComponent();
        }

     

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void loadSystemInfo()
        {
            try
            {
                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select * from settings_tmp", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    txtName.Text = dr["name"].ToString();
                    txtTitle.Text = dr["title"].ToString();
                    dataGridView1.Rows.Add(dr["header1"], dr["header2"], dr["header3"], dr["header4"], dr["footer1"], dr["footer2"]);
                }
                dr.Close();

                dataGridView2.Rows.Clear();
                cmd = new SQLiteCommand("select * from settings_tmp", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataGridView2.Rows.Add(dr["password"],dr["invStartText"],dr["InvNoCount"], dr["govtax"], dr["svrtax"], dr["billcount"], dr["kitchencount"], dr["directprint"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch  
            {             
                db.cn.Close();
            }
        }

        void loadUser()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from users_tmp", db.cn);
                dr = cmd.ExecuteReader();
                dataGridView3.Rows.Clear();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView3.Rows.Add(dr["id"], i, dr["username"], dr["password"], dr["dashboard"], dr["menuitem"], dr["tables"], dr["customer"], dr["orderlist"], dr["expenses"], dr["reports"], dr["settings"], dr["pricechange"], dr["allowedit"], dr["allowdelete"], dr["inactive"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch(Exception e) 
            {
             //   MessageBox.Show(e.Message);
                db.cn.Close();
            }
        }

        void insertIntoTmp()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from settings_tmp; " +
                    "delete from users_tmp; " +
                    "insert into Settings_tmp select * from settings; " +
                    "insert into users_tmp select * from users; ", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView3.Rows[0];
            row.Height = 30;
            insertIntoTmp();
            loadSystemInfo();
            loadUser();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bk.backup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bk.restore();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            FormRegister frm = new FormRegister();
            frm.ShowDialog();
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.Value != null)
            {
                e.Value = new String('*', e.Value.ToString().Length);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from settings; " +
                    "delete from users; " +
                    "insert into Settings select * from settings_tmp; " +
                    "insert into users select * from users_tmp; " +
                    "update Settings set name=@name,title=@title; ", db.cn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text==""?" ":txtTitle.Text);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                this.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                if (row != null)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update settings_tmp set header1=@h1,header2=@h2," +
                        "header3=@h3,header4=@h4,footer1=@f1,footer2=@f2", db.cn);
                    cmd.Parameters.AddWithValue("@h1", row.Cells[0].Value);
                    cmd.Parameters.AddWithValue("@h2", row.Cells[1].Value);
                    cmd.Parameters.AddWithValue("@h3", row.Cells[2].Value);
                    cmd.Parameters.AddWithValue("@h4", row.Cells[3].Value);
                    cmd.Parameters.AddWithValue("@f1", row.Cells[4].Value);
                    cmd.Parameters.AddWithValue("@f2", row.Cells[5].Value);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch   
            {
               
                db.cn.Close();
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView2.CurrentRow;
                if (row != null)
                {
                    if (Convert.ToDecimal(row.Cells[3].Value) > 100)
                    {
                       MessageBox.Show("Please check Goverment Tax!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       row.Cells[3].Value = 0;
                        return;
                    }
                    if (Convert.ToDecimal(row.Cells[4].Value) > 100)
                    {                      
                        MessageBox.Show("Please check Services Tax!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        row.Cells[4].Value = 0;
                        return;
                    }

                    int invcount = row.Cells[2].Value == null ? 0 : Convert.ToInt32(row.Cells[2].Value.ToString());
                    decimal govtax = row.Cells[3].Value == null ? 0 : Convert.ToDecimal(row.Cells[3].Value.ToString());
                    decimal svrtax = row.Cells[4].Value == null ? 0 : Convert.ToDecimal(row.Cells[4].Value.ToString());
                    int billcount = row.Cells[5].Value == null ? 0 : Convert.ToInt32(row.Cells[5].Value.ToString());
                    int kitcount = row.Cells[6].Value == null ? 0 : Convert.ToInt32(row.Cells[6].Value.ToString());

                    db.cn.Open();
                    cmd = new SQLiteCommand("update settings_tmp set password=@pw,invstarttext=@invstart," +
                        "invnocount=@invcount," +
                        "govtax=@govtax,svrtax=@svrtax," +
                        "billcount=@billcount,kitchencount=@kitcount,directprint=@directprint;", db.cn);
                    cmd.Parameters.AddWithValue("@pw", row.Cells[0].Value);
                    cmd.Parameters.AddWithValue("@invstart", row.Cells[1].Value);
                    cmd.Parameters.AddWithValue("@invcount", invcount);
                    cmd.Parameters.AddWithValue("@govtax",govtax);
                    cmd.Parameters.AddWithValue("@svrtax", svrtax);
                    cmd.Parameters.AddWithValue("@billcount", billcount);
                    cmd.Parameters.AddWithValue("@kitcount", kitcount);
                    cmd.Parameters.AddWithValue("@directprint", row.Cells["colDirectPrint"].Value);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch   
            {
            

                db.cn.Close();
            }
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView3.CurrentRow;
                if (row != null)
                {   var id = row.Cells[0].Value;
                    var username = row.Cells[2].Value;
                    var password = row.Cells[3].Value;
                    var dashboard = row.Cells[4].Value;
                    var mi = row.Cells[5].Value;
                    var tbl = row.Cells[6].Value;
                    var cust = row.Cells[7].Value;
                    var ol = row.Cells[8].Value;
                    var exp = row.Cells[9].Value;
                    var rpt = row.Cells[10].Value;
                    var set = row.Cells[11].Value;
                    var pc = row.Cells[12].Value;
                    var edt = row.Cells[13].Value;
                    var del = row.Cells[14].Value;                  
                    var act = row.Cells[15].Value;
                                      
                    db.cn.Open();
                    if (id == null)
                    {
                        cmd = new SQLiteCommand("insert into users_tmp(id,username) values ((select ifnull(max(id)+1,1) from users_tmp),@username)", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("update users_tmp set username=@username, password=@password," +
                       "dashboard=@db, menuitem=@mi, tables=@tbl, customer=@cust, expenses=@exp," +
                       "reports=@rpt, settings=@set, orderlist=@ol,pricechange=@pc,allowedit=@edt,allowdelete=@del,inactive=@in where id like @id; ", db.cn);
                    }
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@db", dashboard);
                    cmd.Parameters.AddWithValue("@mi", mi);
                    cmd.Parameters.AddWithValue("@tbl", tbl);
                    cmd.Parameters.AddWithValue("@cust", cust);
                    cmd.Parameters.AddWithValue("@exp", exp);
                    cmd.Parameters.AddWithValue("@rpt", rpt);
                    cmd.Parameters.AddWithValue("@set", set);
                    cmd.Parameters.AddWithValue("@ol", ol);
                    cmd.Parameters.AddWithValue("@pc", pc);
                    cmd.Parameters.AddWithValue("@edt", edt);
                    cmd.Parameters.AddWithValue("@del", del);                 
                    cmd.Parameters.AddWithValue("@in", act);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    
                    loadUser();
                }           
            }
            catch     
            {
                 
               
                db.cn.Close();
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView3.Columns[e.ColumnIndex].Name;
                DataGridViewRow row = dataGridView3.CurrentRow;
                if (row.Cells[0].Value == null)
                {
                    if (colName == "colName")
                    {
                        row.ReadOnly = false;                    
                    }
                    else
                    {
                        row.ReadOnly = true;                      
                    }
                }
                 if (row.Cells[0].Value != null && colName == "colDelete")
                {
                    if (dataGridView3.Rows.Count > 2)
                    {
                        if (MessageBox.Show("Are you sure to Delete this User?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("delete from users_tmp where id like @id; ", db.cn);
                            cmd.Parameters.AddWithValue("@id", row.Cells[0].Value);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                            dataGridView3.Rows.Remove(row);
                        }
                    }
                    else
                    {
                        MessageBox.Show("System must have a least one user!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }   
        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell dgc = dataGridView2.CurrentCell;
           // MessageBox.Show(dgc.ColumnIndex.ToString());
            if (dgc.ColumnIndex == 2 || dgc.ColumnIndex==5 || dgc.ColumnIndex==6) //Desired Column
            {
                e.Control.KeyPress -= colTax_KeyPress;
                e.Control.KeyPress -= colSvr_KeyPress;
                e.Control.KeyPress += colInvCount_KeyPress;
            }
            else if (dgc.ColumnIndex == 3 || dgc.ColumnIndex == 4) //Desired Column
            {
                TextBox txt = e.Control as TextBox;
                if (txt != null)
                {
                    e.Control.KeyPress -= colInvCount_KeyPress;
                    e.Control.KeyPress -= colTax_KeyPress;
                    e.Control.KeyPress -= colSvr_KeyPress;
                    txt.KeyPress += colTax_KeyPress;
                }
            }
            else
            {
                e.Control.KeyPress -= colInvCount_KeyPress;
                e.Control.KeyPress -= colTax_KeyPress;
                e.Control.KeyPress -= colSvr_KeyPress;
            }

        }

        private void colInvCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //allow number, backspace 
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void colTax_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Allow control keys like Backspace
            if (char.IsControl(e.KeyChar))
                return;

            // Allow digits
            if (char.IsDigit(e.KeyChar))
                return;

            // Allow only one decimal point
            if (e.KeyChar == '.' && !txt.Text.Contains("."))
                return;

            // Otherwise, block the input
            e.Handled = true;

        }
        private void colSvr_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridViewRow row = dataGridView2.CurrentRow;

            if (row.Cells[4].Value != null)
            {
                if (e.KeyChar == '.' && row.Cells[4].Value.ToString().Contains("."))
                {
                    // Stop more than one dot Char
                    e.Handled = true;
                }
                else if (e.KeyChar == '.' && row.Cells[4].Value.ToString().Length == 0)
                {
                    // Stop first char as a dot input
                    e.Handled = true;
                }
                else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    // Stop allow other than digit and control
                    e.Handled = true;
                }               
            }           
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView2.Columns[e.ColumnIndex].Name;
            if(colName == "colAddPrinter")
            {
                FormAddPrinter frm = new FormAddPrinter();
                frm.ShowDialog();
            }          
            else if (colName == "colClear")
            {
                FormClearData frm = new FormClearData();
                frm.ShowDialog();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            object O = Resources.ResourceManager.GetObject("delete");
            Image image = (Image)O;

            if (e.ColumnIndex == 3 && e.Value != null)
            {
                e.Value = new String('*', e.Value.ToString().Length);
            }

            if (e.ColumnIndex == 16 && e.Value != null)
            {
                e.Value = image;
            }
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            string exePath =
   System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetEntryAssembly().Location);

            string file = exePath + @"\User Manual.pdf";
            Process.Start(file);
        }
    }
}
