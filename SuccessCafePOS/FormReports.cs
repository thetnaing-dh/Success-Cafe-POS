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
using static System.Windows.Forms.AxHost;

namespace SuccessCafePOS
{
    public partial class FormReports : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataAdapter da;
        DataSet ds;
        MyanmartoEnglish mte = new MyanmartoEnglish();

        FormReportViewer frm = new FormReportViewer();
        public FormReports()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbItemList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                if ((cboCategory.Text == "" && cboItem.Text != "") || (cboCategory.Text != "" && cboItem.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.itemsview;" +
                   "CREATE VIEW itemsview AS select * from items where id in('" + cboItem.SelectedValue + "');", db.cn);
                }
                else if (cboCategory.Text != "" && cboItem.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.itemsview;" +
                "CREATE VIEW itemsview AS select * from items where categoryid in('" + cboCategory.SelectedValue + "');", db.cn);
                }
                else if (cboCategory.Text == "" && cboItem.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.itemsview;" +
                    "CREATE VIEW itemsview AS select * from items;", db.cn);
                }            
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "ItemList";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch
            {
               db.cn.Close();
            }
        }

        private void lbCustomerList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                if ((cboCustGroup.Text == "" && cboCustomer.Text != "") || (cboCustGroup.Text != "" && cboCustomer.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.custview;" +
                "CREATE VIEW custview AS select * from customer where id in('" + cboCustomer.SelectedValue + "');", db.cn);
                }
                else if (cboCustGroup.Text != "" && cboCustomer.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.custview;" +
                "CREATE VIEW custview AS select * from customer where groupid in('" + cboCustGroup.SelectedValue + "') ;", db.cn);
                }
                else if (cboCustGroup.Text == "" && cboCustomer.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.custview;" +
                  "CREATE VIEW custview AS select * from customer;", db.cn);
                }

                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "CustList";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }   
      

        private void lbTableList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
              
                if ((cboTableGroup.Text == "" && cboTable.Text != "") || (cboTableGroup.Text != "" && cboTable.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.tablesview;" +
    "CREATE VIEW tablesview AS select * from tables where id in('" + cboTable.SelectedValue + "') ; ", db.cn);
                }
                else if (cboTableGroup.Text != "" && cboTable.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.tablesview;" +
      "CREATE VIEW tablesview AS select * from tables where groupid in('" + cboTableGroup.SelectedValue + "'); ", db.cn);
                }
                else if (cboTableGroup.Text == "" && cboTable.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.tablesview;" +
      "CREATE VIEW tablesview AS select * from tables; ", db.cn);
                }
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "TableList";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }         
        }

        private void FormReports_Load(object sender, EventArgs e)
        {
           txtFromDate.Text = txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

        private void lbIncExp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.accountsview;" +
                    "CREATE VIEW accountsview AS select date,concat(status,'-',descriptions)description,iif(cashinout=1,balance,0)debit,iif(cashinout=0,balance,0)credit from accounts where balance !=0 and date between '" + fdate + "' and '"+tdate+ "' order by date;", db.cn);
                cmd.ExecuteNonQuery();               
                db.cn.Close();
                frm.reportname = "IncomesExpenses";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {                
                db.cn.Close();
            }        
        }

        private void lbDailySale_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.dailysaleview;" +
                    "CREATE VIEW dailysaleview AS select sh.id,date,invno,t.name tblname,ifnull(c.name,'Customer') custname,totalQty qty,totalAmount amount,dischr discount,svrchr charges,taxchr tax,advance,grandtotal balance from salehead sh left join tables t on sh.tableid=t.id left join customer c on sh.custid=c.id where sh.status in ('Complete','Order') and totalqty!=0 and date between '" + fdate + "' and '" + tdate + "';", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "DailySale";
                frm.fdate =  txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbSaleByMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                if((cboCategory.Text=="" && cboItem.Text != "") || (cboCategory.Text != "" && cboItem.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebymenuview;" +
                 "CREATE VIEW salebymenuview AS select code itemid,name itemname,sum(sd.qty) qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from saledet sd left join salehead sh on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and i.id in('" + cboItem.SelectedValue + "') and sh.date between '" + fdate + "' and '" + tdate + "' group by code,i.name,sd.price,sd.remark,sd.takeaway ;", db.cn);
                }
                else if (cboCategory.Text!="" && cboItem.Text =="")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebymenuview;" +
                "CREATE VIEW salebymenuview AS select code itemid,name itemname,sum(sd.qty) qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from saledet sd left join salehead sh on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and i.categoryid in('" + cboCategory.SelectedValue + "') and sh.date between '" + fdate + "' and '" + tdate + "' group by code,i.name,sd.price,sd.remark,sd.takeaway ;", db.cn);
                }
                else if (cboCategory.Text=="" &&  cboItem.Text =="")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebymenuview;" +
                    "CREATE VIEW salebymenuview AS select code itemid,name itemname,sum(sd.qty) qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from saledet sd left join salehead sh on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and sh.date between '" + fdate + "' and '" + tdate + "' group by code,i.name,sd.price,sd.remark,sd.takeaway ;", db.cn);
                }
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "SaleByMenu";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbSaleByCustomer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                if ((cboCustGroup.Text == "" && cboCustomer.Text != "") || (cboCustGroup.Text != "" && cboCustomer.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebycustview;" +
                "CREATE VIEW salebycustview AS select ifnull(c.name,'Customer') custname,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join Saledet sd on sh.id=sd.id left join customer c on sh.custid=c.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and c.id in('" + cboCustomer.SelectedValue+"') and sh.date between '" + fdate + "' and '" + tdate + "' group by c.name,i.code,i.name,sd.price,sd.remark,takeaway;", db.cn);
                }
                else if (cboCustGroup.Text != "" && cboCustomer.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebycustview;" +
                "CREATE VIEW salebycustview AS select ifnull(c.name,'Customer') custname,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join Saledet sd on sh.id=sd.id left join customer c on sh.custid=c.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and c.groupid in('" + cboCustGroup.SelectedValue+"') and sh.date between '" + fdate + "' and '" + tdate + "' group by c.name,i.code,i.name,sd.price,sd.remark,takeaway;", db.cn);
                }
                else if (cboCustGroup.Text == "" && cboCustomer.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebycustview;" +
                  "CREATE VIEW salebycustview AS select ifnull(c.name,'Customer') custname,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join Saledet sd on sh.id=sd.id left join customer c on sh.custid=c.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and sh.date between '" + fdate + "' and '" + tdate + "' group by c.name,i.code,i.name,sd.price,sd.remark,takeaway;", db.cn);
                }

                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "SaleByCustomer";
                frm.fdate=txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbSaleByTable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");


                if ((cboTableGroup.Text == "" && cboTable.Text != "") || (cboTableGroup.Text != "" && cboTable.Text != ""))
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebytableview;" +
      "CREATE VIEW salebytableview AS select t.name tablename,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join tables t on t.id=sh.tableid left join Saledet sd on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and t.id in('" + cboTable.SelectedValue+"') and sh.date between '" + fdate + "' and '" + tdate + "' group by i.code,i.name,sd.price,sd.remark,sd.takeaway", db.cn);

                }
                else if (cboTableGroup.Text != "" && cboTable.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebytableview;" +
      "CREATE VIEW salebytableview AS select t.name tablename,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join tables t on t.id=sh.tableid left join Saledet sd on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and t.groupid in('" + cboTableGroup.SelectedValue+"') and sh.date between '" + fdate + "' and '" + tdate + "' group by i.code,i.name,sd.price,sd.remark,sd.takeaway", db.cn);

                }
                else if (cboTableGroup.Text == "" && cboTable.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebytableview;" +
      "CREATE VIEW salebytableview AS select t.name tablename,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]','')takeaway  from salehead sh left join tables t on t.id=sh.tableid left join Saledet sd on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and sh.date between '" + fdate + "' and '" + tdate + "' group by i.code,i.name,sd.price,sd.remark,sd.takeaway", db.cn);

                }
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "SaleByTable";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbExpense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.expensesview; " +
                    "CREATE VIEW expensesview AS select date,concat(status,'-',descriptions)desc,iif(cashinout=0,balance,0)cost from accounts where balance !=0 and cashinout=0 and date between '" + fdate + "' and '"+tdate+"' order by date; ", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "Expenses";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbTopSalesMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebymenuview;" +
                    "CREATE VIEW salebymenuview AS select code itemid,name itemname,sum(sd.qty) qty,sd.price,sd.remark,sd.takeaway from saledet sd left join salehead sh on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and date between '" + fdate + "' and '" + tdate + "' group by code,i.name;", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "TopSaleMenu";
                string top = mte.convert_text(txtTop.Text);
                if (top == "")
                {
                    frm.topcount = 10;
                }
                else
                {
                    frm.topcount = Convert.ToInt32(top);
                }
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbSaleByInvoice_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.salebyinvoiceview;" +
                    "CREATE VIEW salebyinvoiceview AS select sh.invno invoice,code itemid,i.name itemname,sd.qty,sd.price,sd.remark,iif(sd.takeaway=1,'[Takeaway]',NULL)takeaway,sh.date,ifnull(c.name,'Customer') custname,sh.taxchr,sh.svrchr,sh.dischr,sh.grandtotal,iif(sd.foc=1,'[FOC]',NULL)foc,sh.advance from salehead sh left join customer c on sh.custid=c.id left join tables t on t.id=sh.tableid left join Saledet sd on sh.id=sd.id left join items i on sd.itemid=i.id where sh.status in ('Complete','Order') and sh.date between '" + fdate + "' and '" + tdate + "'", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "SaleByInvoice";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbSaleOrderList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                if (cboOrderStatus.Text == "")
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.saleorderlistview;" +
                "CREATE VIEW saleorderlistview AS select sh.date,invno invoice,ifnull(c.name,'Customer') custname,orderdate,takeoutdate,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=0,NULL,'[Takeaway]')takeaway,sh.taxchr,sh.svrchr,sh.dischr,sh.grandtotal,iif(sd.foc=1,'[FOC]',NULL)foc,sh.advance from tblOrder ol left join salehead sh on ol.id=sh.id left join customer c on sh.custid=c.id left join Saledet sd on sh.id=sd.id left join items i on i.id=sd.itemid where sh.status in ('Complete','Order') and sh.date between '" + fdate + "' and '" + tdate + "' group by sh.date,sh.invno,c.name,orderdate,takeoutdate,i.code,i.name,sd.price,sd.remark,sd.takeaway", db.cn);

                }
                else
                {
                    cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.saleorderlistview;" +
                        "CREATE VIEW saleorderlistview AS select sh.date,invno invoice,ifnull(c.name,'Customer') custname,orderdate,takeoutdate,code itemid,i.name itemname,sum(sd.qty)qty,sd.price,sd.remark,iif(sd.takeaway=0,NULL,'[Takeaway]')takeaway,sh.taxchr,sh.svrchr,sh.dischr,sh.grandtotal,iif(sd.foc=1,'[FOC]',NULL)foc,sh.advance from tblOrder ol left join salehead sh on ol.id=sh.id left join customer c on sh.custid=c.id left join Saledet sd on sh.id=sd.id left join items i on i.id=sd.itemid where sh.status ='" + cboOrderStatus.Text+"' and sh.date between '" + fdate + "' and '" + tdate + "' group by sh.date,sh.invno,c.name,orderdate,takeoutdate,i.code,i.name,sd.price,sd.remark,sd.takeaway", db.cn);
                }
                    cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "SaleOrderList";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void lbMonthlySale_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fromdate = new DateTime(dt1.Year, dt1.Month, 1);
                string fdate = fromdate.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.dailysaleview;" +
                    "CREATE VIEW dailysaleview AS select sh.id,strftime('%Y-%m',date)date,invno,t.name tblname,c.name custname,sum(totalQty) qty,sum(totalAmount) amount,sum(dischr) discount,sum(svrchr) charges,sum(taxchr) tax,sum(advance) advance,sum(grandtotal) balance from salehead sh left join tables t on sh.tableid=t.id left join customer c on sh.custid=c.id where sh.status in ('Complete','Order') group by strftime('%Y-%m',date) Having date between '" + fdate + "' and '" + tdate + "';", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "MonthlySale";
                frm.fdate= fromdate.ToString("dd/MM/yyyy");
                frm.tdate= txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }

        }

        private void cboCategory_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from Category ORDER BY name", db.cn);
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboCategory.DataSource = ds.Tables[0];
                cboCategory.DisplayMember = "name";
                cboCategory.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboTableGroup_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from TableGroup ORDER BY name", db.cn);
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboTableGroup.DataSource = ds.Tables[0];
                cboTableGroup.DisplayMember = "name";
                cboTableGroup.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboCustGroup_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from CustGroup", db.cn);
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboCustGroup.DataSource = ds.Tables[0];
                cboCustGroup.DisplayMember = "name";
                cboCustGroup.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Ignore the key
            }
        }

        private void cboItem_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                if(cboCategory.Text =="")
                {
                    cmd = new SQLiteCommand("select id,name from items ORDER BY name", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id,name from items where categoryid like @id ORDER BY name", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboCategory.SelectedValue);
                }
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboItem.DataSource = ds.Tables[0];
                cboItem.DisplayMember = "name";
                cboItem.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboTable_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                if (cboTableGroup.Text == "")
                {
                    cmd = new SQLiteCommand("select id,name from Tables ORDER BY name", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id,name from Tables where groupid like @id ORDER BY name", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboTableGroup.SelectedValue);
                }            
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboTable.DataSource = ds.Tables[0];
                cboTable.DisplayMember = "name";
                cboTable.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboCustomer_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                if (cboCustGroup.Text == "")
                {
                    cmd = new SQLiteCommand("select id,name from customer ORDER BY name", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id,name from customer where groupid like @id ORDER BY name", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboCustGroup.SelectedValue);
                }

                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboCustomer.DataSource = ds.Tables[0];
                cboCustomer.DisplayMember = "name";
                cboCustomer.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboCategory_DropDownClosed(object sender, EventArgs e)
        {
            cboItem.Text = "";
        }

        private void cboTableGroup_DropDownClosed(object sender, EventArgs e)
        {
            cboTable.Text = "";
        }

        private void cboCustGroup_DropDownClosed(object sender, EventArgs e)
        {
            cboCustomer.Text = "";
        }

        private void lbDailyIncomes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                db.cn.Open();
                DateTime dt1 = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = dt1.ToString("yyyy-MM-dd");
                string tdate = dt2.ToString("yyyy-MM-dd");

                cmd = new SQLiteCommand("DROP VIEW IF EXISTS main.accountsview;" +
                    "CREATE VIEW accountsview AS select date,concat(status,'-',descriptions)description,iif(cashinout=1,balance,0)debit,iif(cashinout=0,balance,0)credit from accounts where balance !=0 and cashinout=1 and date between '" + fdate + "' and '" + tdate + "' order by date;", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                frm.reportname = "Incomes";
                frm.fdate = txtFromDate.Text;
                frm.tdate = txtToDate.Text;
                frm.ShowDialog();
            }
            catch   
            {
                db.cn.Close();
            }
        }

        private void txtTop_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
