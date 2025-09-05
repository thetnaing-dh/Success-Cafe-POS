using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace SuccessCafePOS
{
    public partial class FormOrder: Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd, cmd1, cmd2, cmd3;
        SQLiteDataReader dr, dr1;
        SQLiteDataAdapter da, da1, da2, da3;
        DataTable dt, dt1, dt2, dt3;
     
        public string invid { get; set; }
        int afterLoad;
        public FormOrder()
        {
            InitializeComponent();
        }      

        void loadRecord()
        {
            try
            {
                afterLoad = 0;
                db.cn.Open();
                cmd = new SQLiteCommand("select sh.id,date,invno,t.name tablename,c.name cust,sh.memcard,concat(sh.remark,' ',o.remark)remark,svrchr,taxper,disper,dischr,taxchr,advance,grandtotal,takeoutdate from salehead sh left join tblorder o on sh.id=o.id left join customer c on sh.custid = c.id left join tables t on t.id = sh.tableid where sh.id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                dr.Read();
                txtDate.Text = dr["takeoutdate"].ToString() == "" ? DateTime.Now.ToString("dd/MM/yyyy") : DateTime.ParseExact(dr["takeoutdate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                invid = dr["id"].ToString();
                cboTable.Text = dr["tablename"].ToString();
                txtInvoice.Text = dr["invno"].ToString();
                cboCustomer.SelectedItem = dr["cust"].ToString();
                cboCustomer.Text = dr["cust"].ToString();
                txtMemCard.Text = dr["memcard"].ToString();
                txtRemark.Text = dr["remark"].ToString();
                txtSvrChr.Text = dr["svrchr"].ToString();
                txtTax.Text = dr["taxchr"].ToString();
                txtDisChr.Text = dr["dischr"].ToString();
                txtDisPer.Text = dr["disper"].ToString();
                txtAdvance.Text = dr["advance"].ToString();
                lbGrandTotal.Text = dr["grandtotal"].ToString();               
                dr.Close();
                db.cn.Close();
                afterLoad = 1;
            }
            catch  
            {
              
                db.cn.Close();
            }
        }

        void loadSaleDetail()
        {
            try
            {
                int totalQty = 0, totalAmt = 0;
                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,(qty*price) amount,takeaway,foc,printed,cancel,sd.remark,it.id itid,billno from Saledet sd left join items it on sd.itemid=it.id where sd.id like @invid and not(printed= 1 and cancel =1)", db.cn);
                cmd.Parameters.AddWithValue("@invid", invid);
                dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], dr["srno"], i, dr["name"], dr["qty"], dr["price"], dr["amount"], dr["printed"], dr["cancel"], dr["takeaway"],dr["foc"], dr["remark"], dr["itid"], dr["billno"]);
                    totalQty += Convert.ToInt32(dr["qty"]);
                    totalAmt += Convert.ToInt32(dr["amount"]);
                }
                dr.Close();
                db.cn.Close();
                lbTotalQty.Text = totalQty.ToString();
                lbTotalAmount.Text = totalAmt.ToString();
                afterLoad = 0;
                decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                decimal chr = Convert.ToDecimal(txtSvrChr.Text);
                decimal dis = Convert.ToDecimal(txtDisChr.Text);
                decimal adv = Convert.ToDecimal(txtAdvance.Text);
                decimal tax = Convert.ToDecimal(txtTax.Text);
                lbGrandTotal.Text = (total - dis + tax + chr - adv).ToString();
                afterLoad = 1;
            }
            catch
            {
                db.cn.Close();
            }

        }

        void updateDate()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("update tblorder set takeoutdate=@date where id like @id;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            loadRecord();
            loadSaleDetail();
            updateDate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            if (afterLoad == 1)
            {
                updateDate();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from accounts where transid like @id and status like 'OrderTakeOut';", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();

                db.cn.Open();                
                cmd = new SQLiteCommand("update salehead set status = @status," +
                 "totalQty  = (select sum(totalqty) from splitbill where id like @id), " +
                    "totalAmount  = (select sum(totalamount) from splitbill where id like @id), " +
                    "svrchr=(select sum(svrchr) from splitbill where id like @id), " +
                    "taxper=(select taxper from splitbill where id like @id limit 1), " +
                    "taxchr=(select sum(taxchr) from splitbill where id like @id), " +
                    "disper=(select disper from splitbill where id like @id), " +
                    "dischr=(select sum(dischr) from splitbill where id like @id), " +
                    "grandtotal=(select sum(grandtotal) from splitbill where id like @id), " +
                    "advance=(select sum(advance) from splitbill where id like @id), " +
                    "balance=(select IIF(sum(advance)=0,sum(grandtotal),sum(advance)) from splitbill where id like @id)," +
                     "svrper= (select svrper from splitbill where id like @id limit 1)" +
                    " where id like @id; " +
                    "update tblorder set takeoutdate=@date,balance=@bal where id like @id; " +
                    "insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                    " values (@date,@invno,@id,'OrderTakeOut',1,@bal); ",db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@invno", txtInvoice.Text);
                DateTime dt = DateTime.ParseExact(txtDate.Text,"dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@bal", Convert.ToDecimal(lbGrandTotal.Text));
                cmd.Parameters.AddWithValue("@status", "Complete");
                cmd.ExecuteNonQuery();
                db.cn.Close();
                MessageBox.Show("Saved successfully!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
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

        private void cboTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        bool checkPrintDirect()
        {
            bool directprint = false;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select directprint from settings;", db.cn);
                directprint = Convert.ToBoolean(cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return directprint;
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {          

            LocalReport localReport = new LocalReport();

            db.cn.Open();
            cmd = new SQLiteCommand("select * from settings", db.cn);
            da = new SQLiteDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            //  cmd1 = new SQLiteCommand("select date,invno,tableid,custid,memcard,remark from salehead", db.cn);

            cmd1 = new SQLiteCommand("select srno sr,name item,sum(qty) qty,s.price,(sum(qty)*price)amount,remark,takeaway,foc from saledet s " +
                "left join items i on s.itemid = i.id where s.id like @id and cancel like '0' group by name,takeaway,s.price,remark,foc", db.cn);
            cmd1.Parameters.AddWithValue("@id", invid);
           // cmd1.Parameters.AddWithValue("@billno", billNo);
            da1 = new SQLiteDataAdapter(cmd1);
            dt1 = new DataTable();
            da1.Fill(dt1);

            cmd2 = new SQLiteCommand("select s.date,invno,t.name tablename,c.name custname,s.memcard,concat(s.remark,sb.remark,o.remark) remark, sb.totalQty qty," +
                "sb.taxchr tax,sb.svrchr svrcharge,sb.dischr dis,sb.totalamount amount,sb.grandtotal grand,sb.advance,sb.balance,orderdate,o.takeoutdate from splitbill sb " +
                "left join salehead s on sb.id = s.id left join tblorder o on o.id = s.id " +
                "left join tables t on s.tableid = t.id left join customer c on s.custid=c.id where s.id like @id", db.cn);
            cmd2.Parameters.AddWithValue("@id", invid); 
            da2 = new SQLiteDataAdapter(cmd2);
            dt2 = new DataTable();
            da2.Fill(dt2);           

            ReportDataSource source = new ReportDataSource("DataSet1", dt);
            ReportDataSource source1 = new ReportDataSource("DataSet2", dt1);
            ReportDataSource source2 = new ReportDataSource("DataSet3", dt2);
         
            localReport.ReportPath = @".\Reports\rptOrderTakeOutSlip.rdlc";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(source);
            localReport.DataSources.Add(source1);
            localReport.DataSources.Add(source2);
             ReportParameter[] parameters = new ReportParameter[]
             {
                    new ReportParameter("username", Status.username)
             };
            localReport.SetParameters(parameters);
         
            db.cn.Close();

            DirectPrint printer = new DirectPrint();

            if (checkPrintDirect() == true)
            {               
                printer.PrintToPrinter(localReport);               
            }
            else
            {
                FormReportViewer frm = new FormReportViewer();
                frm.reportname = "OrderTakeOut";
                frm.invid = invid;
                frm.billNo = "0";
                frm.ShowDialog();
            }        
        }
    }
}
