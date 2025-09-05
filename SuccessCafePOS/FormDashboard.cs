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
    public partial class FormDashboard : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormDashboard()
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            loadRecord();
            checkUserToDelete();
            checkUserToEdit();
        }

        
        void loadRecord()
        {
            try
            {
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                db.cn.Open();
                cmd = new SQLiteCommand("select count(id) from salehead where status like 'Order'", db.cn);
                lbTotalOrder.Text = cmd.ExecuteScalar().ToString()==""?"0":cmd.ExecuteScalar().ToString();
                db.cn.Close();

                db.cn.Open();
                cmd = new SQLiteCommand("select sum(balance) from accounts where cashinout=0 and date between @fdate and @tdate", db.cn);
                cmd.Parameters.AddWithValue("@fdate", dt1.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@tdate", dt2.ToString("yyyy-MM-dd"));
                lbTotalExpenses.Text = cmd.ExecuteScalar().ToString()==""?"0": cmd.ExecuteScalar().ToString();
                db.cn.Close();

                db.cn.Open();
                cmd = new SQLiteCommand("select sum(balance) from accounts where cashinout=1 and date between @fdate and @tdate", db.cn);
                cmd.Parameters.AddWithValue("@fdate", dt1.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@tdate", dt2.ToString("yyyy-MM-dd"));
                lbTotalRevenues.Text = cmd.ExecuteScalar().ToString() == "" ? "0" : cmd.ExecuteScalar().ToString();
                db.cn.Close();

                dataGridView1.Rows.Clear();
                db.cn.Open();
                if (txtFromDate.Text == txtToDate.Text)
                {
                    cmd = new SQLiteCommand("select sh.id,date,invno,t.name tname,c.name cname,totalQty,totalAmount,svrchr,taxchr,dischr,foc,grandtotal,advance from salehead sh left join tables t on sh.tableid=t.id left join customer c on sh.custid=c.id where status like 'Complete' and date like @date", db.cn);
                    cmd.Parameters.AddWithValue("@date", dt2.ToString("yyyy-MM-dd"));
                }
                else {
                   cmd = new SQLiteCommand("select sh.id,date,invno,t.name tname,c.name cname,totalQty,totalAmount,svrchr,taxchr,dischr,foc,grandtotal,advance from salehead sh left join tables t on sh.tableid=t.id left join customer c on sh.custid=c.id where status like 'Complete' and date between @fdate and @tdate", db.cn);
                    cmd.Parameters.AddWithValue("@fdate", dt1.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@tdate", dt2.ToString("yyyy-MM-dd"));
                }

                dr = cmd.ExecuteReader();
                int i = 0, qty = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], i, DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), dr["invno"], dr["tname"], dr["cname"].ToString() == "" ? "Customer" : dr["cname"], dr["totalQty"], dr["totalAmount"], dr["dischr"],dr["foc"], dr["svrchr"], dr["taxchr"],dr["advance"], dr["grandtotal"]);
                    qty += Convert.ToInt32(dr["totalQty"]);
               
                }
                dr.Close();
                db.cn.Close();
                lbTotalTables.Text = i.ToString();
                lbTotalQty.Text = qty.ToString();
           
            }
            catch   
            {
           
                db.cn.Close();
            }
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

        private void calendarTo_DateChanged(object sender, DateRangeEventArgs e)
        {
            txtToDate.Text = calendarTo.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtToDate.SelectionStart = 0;
            txtToDate.SelectionLength = 0;
            calendarTo.Hide();
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
                            cmd = new SQLiteCommand("update salehead set status='Deleted' where id like @id; " +
                                "delete from accounts where transid like @id; ", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();

                            loadRecord();
                        }
                    }
                    else if (colName == "colEdit")
                    {
                        FormEditInvoice frm = new FormEditInvoice();
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

        private void txtFromDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void txtToDate_TextChanged(object sender, EventArgs e)
        {
            loadRecord();
        }

        private void panel10_Click(object sender, EventArgs e)
        {
            FormOrderList form = new FormOrderList();
            form.ShowDialog();
            loadRecord();
        }

        private void calendarTo_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtToDate.Text = calendarTo.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtToDate.SelectionStart = 0;
            txtToDate.SelectionLength = 0;
            calendarTo.Hide();
        }

        private void panel12_Click(object sender, EventArgs e)
        {
            FormExpenses frm = new FormExpenses();
            frm.ShowDialog();
        }
    }
}
