using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormOrderList : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormOrderList()
        {
            InitializeComponent();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void loadRecord()
        {
            try
            {

                dataGridView1.Rows.Clear();
                db.cn.Open();
                if (txtFromDate.Text == txtToDate.Text)
                {
                    DateTime dt = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cmd = new SQLiteCommand("select * from orderview where (date like @date or status like 'Order') and status !='Deleted' and totalQty > 0", db.cn);
                    cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                }
                else
                {
                    DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cmd = new SQLiteCommand("select * from orderview where (status like 'Order' or date between @fdate and @tdate) and status !='Deleted' and totalQty > 0", db.cn);
                    cmd.Parameters.AddWithValue("@fdate", dt1.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@tdate", dt2.ToString("yyyy-MM-dd"));
                }

                dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], i, DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), dr["invno"], dr["tblname"], dr["custname"].ToString() == "" ? "Customer" : dr["custname"], DateTime.ParseExact(dr["orderdate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), dr["takeoutdate"].ToString()=="" ? "-" : DateTime.ParseExact(dr["takeoutdate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), DateTime.ParseExact(dr["time"].ToString(), "hh:mm:ss:tt", CultureInfo.InvariantCulture).ToString("hh:mm:tt"), dr["totalQty"], dr["grandtotal"], dr["remark"], dr["status"]);
                }
                dr.Close();
                db.cn.Close();
               
            }
            catch  
            {
            
                db.cn.Close();
            }
        }
        private void FormOrderList_Load(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            loadRecord();
            checkUserToDelete();
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

        private void calendarTo_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtToDate.Text = calendarTo.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtToDate.SelectionStart = 0;
            txtToDate.SelectionLength = 0;
            calendarTo.Hide();
        }

        private void calendarFrom_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtFromDate.Text = calendarFrom.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtFromDate.SelectionStart = 0;
            txtFromDate.SelectionLength = 0;
            calendarFrom.Hide();
        }

        private void txtFromDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void txtToDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView1.Columns[e.ColumnIndex].Name;

                DataGridViewRow row = dataGridView1.CurrentRow;
                string invid = row.Cells["colID"].Value.ToString();              

                if (row != null)
                {
                    if (colName == "colDelete")
                    {
                        if (MessageBox.Show("Are you sure to Delete this Invoice?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("update salehead set status='Deleted' where id like @id", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();

                            loadRecord();
                        }
                    }
                    else if (colName == "colTakeOut")
                    {
                        FormOrder frm = new FormOrder();
                        frm.invid = invid;
                        this.Hide();
                        frm.ShowDialog();
                        this.Show();
                        loadRecord();
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string takeout = row.Cells["colStatus"].Value.ToString();
               
                if (takeout != "Order")
                {
                   row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 100);                   
                }
                else
                {
                    row.Cells["colTakeOutDate"].Value = "-";
                }
            }
        }
    }
}
