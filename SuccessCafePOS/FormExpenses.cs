using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormExpenses : Form
    {
        MyanmartoEnglish mte = new MyanmartoEnglish();
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        string invid = "0";

        public FormExpenses()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            if (calendar.Visible == false)
            {
                calendar.Visible = true;
            }
            else { calendar.Visible = false; }
        }

        void loadRecord()
        {
            try
            {
                DateTime fdate = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime tdate = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                db.cn.Open();
                if (txtSearch.Text.Trim().Length > 0)
                {
                    cmd = new SQLiteCommand("select * from expenses where date between @fdate and @tdate and description like @desc order by date", db.cn);
                    cmd.Parameters.AddWithValue("@desc", txtSearch.Text + "%");
                }
                else
                {
                    cmd = new SQLiteCommand("select * from expenses where date between @fdate and @tdate order by date", db.cn);
                }
                cmd.Parameters.AddWithValue("@fdate", fdate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@tdate", tdate.ToString("yyyy-MM-dd"));
                dr = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], i, DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), dr["description"], dr["cost"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDate.Text = calendar.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtDate.SelectionStart = 0;
            txtDate.SelectionLength = 0;
            calendar.Hide();
        }

        private void btnFromDate_Click(object sender, EventArgs e)
        {
            if (calendarFrom.Visible == false)
            {
                calendarFrom.Visible = true;
            }
            else { calendarFrom.Visible = false; }
        }

        private void btnToDate_Click(object sender, EventArgs e)
        {
            if (calendarTo.Visible == false)
            {
                calendarTo.Visible = true;
            }
            else { calendarTo.Visible = false; }
        }

        private void calendarFrom_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtFromDate.Text = calendarFrom.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtFromDate.SelectionStart = 0;
            txtFromDate.SelectionLength = 0;
            calendarFrom.Hide();
        }

        private void calendarTo_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtToDate.Text = calendarTo.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtToDate.SelectionStart = 0;
            txtToDate.SelectionLength = 0;
            calendarTo.Hide();
        }

        void autoCompleteDesc()
        {
            try
            {
                AutoCompleteStringCollection myCollection = new AutoCompleteStringCollection();
                txtName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                db.cn.Open();
                cmd = new SQLiteCommand("select description desc from expenses", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    myCollection.Add(dr["desc"].ToString());

                }
                txtName.AutoCompleteCustomSource = myCollection;

                dr.Close();
                db.cn.Close();
            }
            catch
            {
                dr.Close();
                db.cn.Close();
            }
        }

        void checkUserToEdit()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select allowedit from users where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", Status.userid);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr["allowedit"].ToString() == "0")
                {
                    dataGridView1.Columns["colEdit"].Visible = false;
                }
                else
                {
                    dataGridView1.Columns["colEdit"].Visible = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        void checkUserToDelete()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select allowdelete from users where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", Status.userid);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr["allowdelete"].ToString() == "0")
                {
                    dataGridView1.Columns["colDelete"].Visible = false;
                }
                else
                {
                    dataGridView1.Columns["colDelete"].Visible = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void FormExpenses_Load(object sender, EventArgs e)
        {
            autoCompleteDesc();
            txtDate.Text = txtFromDate.Text = txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            loadRecord();
            checkUserToDelete();
            checkUserToEdit();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
               
                string cost = mte.convert_text(txtCost.Text);
                

                if (txtName.Text.Trim().Length > 0)
                {
                    db.cn.Open();
                    if (invid == "0")
                    {
                        cmd = new SQLiteCommand("insert into expenses(date,description,cost) values (@date,@description,@cost); " +
                           "insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                           " values (@date,@description,(select max(id) from expenses),'Expenses',0,@cost); ", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("update expenses set date=@date,description=@description,cost=@cost where id like @id; " +
                            "update accounts set date=@date,descriptions=@description,balance=@cost where transid like @id and status like 'Expenses'; ", db.cn);
                    }
                    cmd.Parameters.AddWithValue("@id", invid);
                    DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@description", txtName.Text);
                    cmd.Parameters.AddWithValue("@cost", cost==""?"0":cost);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    loadRecord();
                    clear();
                    autoCompleteDesc();
                }
                else
                {
                    txtName.Focus();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }
        void clear()
        {
            invid = "0";
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtName.Clear();
            txtCost.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dataGridView1.CurrentRow;
          
          
            if (row != null)
            {                
                if (colName == "colEdit")
                {
                    invid = row.Cells["colID"].Value.ToString();
                    txtDate.Text = row.Cells["colDate"].Value.ToString();
                    txtName.Text = row.Cells["colDesc"].Value.ToString();
                    txtCost.Text = row.Cells["colCost"].Value.ToString();
                }
                else if (colName == "colDelete")
                {
                    try
                    {
                        if (MessageBox.Show("Are you sure to Delete this Record?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("delete from expenses where id like @id; " +
                                "delete from accounts where transid like @id and status like 'Expenses'; ", db.cn);
                            cmd.Parameters.AddWithValue("@id", row.Cells["colID"].Value.ToString());
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                            clear();
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                    catch
                    {
                        db.cn.Close();
                    }
                }
            }
        }

        private void txtFromDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void txtToDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void txtCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
