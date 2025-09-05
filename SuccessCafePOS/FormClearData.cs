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
    public partial class FormClearData : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormClearData()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void loadRecord()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from filename", db.cn);
                dr = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (dr.Read())
                {
                    dataGridView1.Rows.Add(dr["name"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch    
            {
            
                db.cn.Close();
            }
        }

        private void FormClearData_Load(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear Data?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sqlcmd = "";
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["colName"].Value.ToString() == "Sales" && Convert.ToInt32(row.Cells["colClear"].Value)== 1)
                    {
                        sqlcmd += " delete from salehead; delete from saledet; delete from splitbill; delete from tblorder;  " +
                            "delete from accounts where status != 'Expenses'; delete from salehead_tmp; " +
                            "delete from saledet_tmp; delete from splitbill_tmp; " +
                          "update sqlite_sequence set seq=0 where name like 'salehead'; " +
                          "update sqlite_sequence set seq=0 where name like 'salehead_tmp'; " +
                          "update tables set tblstatus = ''; ";
                    }
                    else if (row.Cells["colName"].Value.ToString() == "Expenses" && Convert.ToInt32(row.Cells["colClear"].Value) == 1)
                    {
                        sqlcmd += " delete from expenses; delete from accounts where status like 'Expenses'; " +
                           "update sqlite_sequence set seq=0 where name like 'expenses'; ";
                    }
                    else if (row.Cells["colName"].Value.ToString() == "Menu Items" && Convert.ToInt32(row.Cells["colClear"].Value) == 1)
                    {
                        sqlcmd += " delete from items; delete from category; update sqlite_sequence set seq=0 where name like 'items'; " +
                            "update sqlite_sequence set seq=0 where name like 'Category'; ";
                    }
                    else if (row.Cells["colName"].Value.ToString() == "Tables" && Convert.ToInt32(row.Cells["colClear"].Value) == 1)
                    {
                        sqlcmd += " delete from tables; delete from tablegroup; update sqlite_sequence set seq=0 where name like 'tables'; " +
                          "update sqlite_sequence set seq=0 where name like 'tableGroup'; ";
                    }
                    else if (row.Cells["colName"].Value.ToString() == "Customers" && Convert.ToInt32(row.Cells["colClear"].Value) == 1)
                    {
                        sqlcmd += " delete from customer; delete from custGroup; update sqlite_sequence set seq=0 where name like 'customer'; " +
                           "update sqlite_sequence set seq=0 where name like 'custGroup'; ";
                    }
                    else if (row.Cells["colName"].Value.ToString() == "Item Remarks" && Convert.ToInt32(row.Cells["colClear"].Value) == 1)
                    {
                        sqlcmd += " delete from remarks; " +
                           "update sqlite_sequence set seq=0 where name like 'remarks'; ";
                    }
                    try
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand(sqlcmd, db.cn);
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
    }
}
