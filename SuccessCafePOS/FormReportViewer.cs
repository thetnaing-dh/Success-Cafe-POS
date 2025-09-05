using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace SuccessCafePOS
{
    public partial class FormReportViewer : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd, cmd1, cmd2;
        SQLiteDataAdapter da, da1, da2;
        DataTable dt, dt1, dt2;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        SQLiteDataReader dr1;

        public string billNo {  get; set; }
        public string fdate { get; set; }
        public string tdate { get; set; }
        public string reportname { get; set; }
        public string invid { get; set; }
        public int topcount {  get; set; }

        public FormReportViewer()
        {
            InitializeComponent();
        }

        void checkDataToPrint()
        {
            dr1 = cmd.ExecuteReader();
            dr1.Read();
            if (dr1.HasRows == false)
            {
                dr1.Close();
                MessageBox.Show("No Data to Report!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                db.cn.Close();
                this.Close();
            }
            dr1.Close();
        }
     
        void AddReportPara()
        {
            ReportParameter[] parameters = new ReportParameter[]
               {
                    new ReportParameter("fDate", fdate),
                    new ReportParameter("tDate", tdate)
               };
            reportViewer1.LocalReport.SetParameters(parameters);
        }

        private void FormReportViewer_Load(object sender, EventArgs e)
        {
            reportViewer1.Refresh();
            if (reportname == "Bill")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from settings", db.cn);
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                cmd1 = new SQLiteCommand("select srno sr,name item,sum(qty) qty,s.price,(sum(qty)*price)amount,remark,takeaway,foc from saledet s " +
                    "left join items i on s.itemid = i.id where s.id like @id and cancel like '0' and billno like @billno group by name,takeaway,s.price,remark,foc", db.cn);
                cmd1.Parameters.AddWithValue("@id", invid);
                cmd1.Parameters.AddWithValue("@billno", billNo);
                da1 = new SQLiteDataAdapter(cmd1);
                dt1 = new DataTable();
                da1.Fill(dt1);

                cmd2 = new SQLiteCommand("select s.date,invno,t.name tablename,c.name custname,s.memcard,concat(s.remark,sb.remark,o.remark) remark, sb.totalQty qty," +
                    "sb.taxchr tax,sb.svrchr svrcharge,sb.dischr dis,sb.totalamount amount,sb.grandtotal grand,sb.advance,sb.balance,orderdate from splitbill sb " +
                    "left join salehead s on sb.id = s.id left join tblorder o on o.id = s.id " +
                    "left join tables t on s.tableid = t.id left join customer c on s.custid=c.id where s.id like @id and billno like @billno", db.cn);
                cmd2.Parameters.AddWithValue("@id", invid);
                cmd2.Parameters.AddWithValue("@billno", billNo);
                da2 = new SQLiteDataAdapter(cmd2);
                dt2 = new DataTable();
                da2.Fill(dt2);


                ReportDataSource source = new ReportDataSource("DataSet1", dt);
                ReportDataSource source1 = new ReportDataSource("DataSet2", dt1);
                ReportDataSource source2 = new ReportDataSource("DataSet3", dt2);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.DataSources.Add(source1);
                reportViewer1.LocalReport.DataSources.Add(source2);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptBillSlip.rdlc";
                ReportParameter[] parameters = new ReportParameter[]
                {
                    new ReportParameter("username", Status.username)
                };
                reportViewer1.LocalReport.SetParameters(parameters);
                db.cn.Close();
            }

            else if (reportname == "OrderTakeOut")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from settings", db.cn);
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                cmd1 = new SQLiteCommand("select srno sr,name item,sum(qty) qty,s.price,(sum(qty)*price)amount,remark,takeaway,foc from saledet s " +
                    "left join items i on s.itemid = i.id where s.id like @id and cancel like '0' group by name,takeaway,s.price,remark,foc", db.cn);
                cmd1.Parameters.AddWithValue("@id", invid);
                da1 = new SQLiteDataAdapter(cmd1);
                dt1 = new DataTable();
                da1.Fill(dt1);

                cmd2 = new SQLiteCommand("select s.date,invno,t.name tablename,c.name custname,s.memcard,concat(s.remark,sb.remark,o.remark) remark, sb.totalQty qty," +
                    "sb.taxchr tax,sb.svrchr svrcharge,sb.dischr dis,sb.totalamount amount,sb.grandtotal grand,sb.advance,sb.balance,orderdate,takeoutdate from splitbill sb " +
                    "left join salehead s on sb.id = s.id left join tblorder o on o.id = s.id " +
                    "left join tables t on s.tableid = t.id left join customer c on s.custid=c.id where s.id like @id", db.cn);
                cmd2.Parameters.AddWithValue("@id", invid);
                da2 = new SQLiteDataAdapter(cmd2);
                dt2 = new DataTable();
                da2.Fill(dt2);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);
                ReportDataSource source1 = new ReportDataSource("DataSet2", dt1);
                ReportDataSource source2 = new ReportDataSource("DataSet3", dt2);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.DataSources.Add(source1);
                reportViewer1.LocalReport.DataSources.Add(source2);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptOrderTakeOutSlip.rdlc";
                ReportParameter[] parameters = new ReportParameter[]
                {
                    new ReportParameter("username", Status.username)
                };
                reportViewer1.LocalReport.SetParameters(parameters);
                db.cn.Close();
            }

            else if (reportname == "Kitchen")
            {
                LocalReport localReport = new LocalReport();
                Boolean itemtoPrint = false, canceltoPrint = false;

                db.cn.Open();
                cmd = new SQLiteCommand("select id from saledet where id like @id and printed like '0' and cancel like '0'", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr1 = cmd.ExecuteReader();
                if (dr1.Read())
                {
                    itemtoPrint = true;
                }
                dr1.Close();
                db.cn.Close();

                db.cn.Open();
                cmd = new SQLiteCommand("select id from saledet where id like @id and printed like '0' and cancel like '1'", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr1 = cmd.ExecuteReader();
                if (dr1.Read())
                {
                    canceltoPrint = true;
                }
                dr1.Close();
                db.cn.Close();

                if (itemtoPrint == true || canceltoPrint == true)
                {
                    if (canceltoPrint == true)
                    {
                        db.cn.Open();
                        cmd1 = new SQLiteCommand("select t.name,i.name item,qty,sd.remark,sd.takeaway from Saledet sd left join Salehead sh on sd.id = sh.id left join Items i on sd.itemid=i.id left join tables t on sh.tableid = t.id where sh.id like @id and printed like '0' and cancel like '1'", db.cn);
                        cmd1.Parameters.AddWithValue("@id", invid);
                        da1 = new SQLiteDataAdapter(cmd1);
                        dt1 = new DataTable();
                        da1.Fill(dt1);

                        ReportDataSource source2 = new ReportDataSource("DataSet1", dt1);
                      
                        localReport.DataSources.Clear();
                        localReport.DataSources.Add(source2);
                        localReport.ReportPath = @".\Reports\rptCancelSlip.rdlc";
                       
                        db.cn.Close();
                    }
                    if (itemtoPrint == true)
                    {
                        db.cn.Open();
                        cmd1 = new SQLiteCommand("select t.name,i.name item,qty,sd.remark,sd.takeaway from Saledet sd left join Salehead sh on sd.id = sh.id left join Items i on sd.itemid=i.id left join tables t on sh.tableid = t.id where sh.id like @id and printed like '0' and cancel like '0'", db.cn);
                        cmd1.Parameters.AddWithValue("@id", invid);
                        da1 = new SQLiteDataAdapter(cmd1);
                        dt1 = new DataTable();
                        da1.Fill(dt1);

                        ReportDataSource source2 = new ReportDataSource("DataSet1", dt1);
                       
                        localReport.DataSources.Clear();
                        localReport.DataSources.Add(source2);
                        localReport.ReportPath = @".\Reports\rptKitchenSlip.rdlc";
                      
                        db.cn.Close();
                    }
                }
                else
                {
                    MessageBox.Show("No Item to Print!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }

            else if (reportname == "ItemList")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select code itemid,i.name itemname, c.name category, saleprice price, cost from itemsview i left join Category c on i.categoryid=c.id where inactive=0", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptItemList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "CustList")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select srno custid,c.name custname,phone,email,address,g.name groupname,memcard, discount memdis from custview c left join CustGroup g on c.groupid = g.id where inactive=0", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptCustomerList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "TableList")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select t.srno tableid,t.name tablename,seats,charges,g.name groupname from tablesview t left join tablegroup g on t.groupid = g.id where inactive = 0", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptTableList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "IncomesExpenses")

            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from accountsview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptIncomeExpense.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "Incomes")

            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from accountsview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptIncomesList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "DailySale")

            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from dailysaleview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptDailySales.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "MonthlySale")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from dailysaleview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptMonthlySales.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "SaleByMenu")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from salebymenuview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptSaleByMenu.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "SaleByCustomer")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from salebycustview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptSaleByCustomer.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "SaleByTable")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from salebytableview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptSaleByTable.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "Expenses")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from expensesview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptExpenseList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "TopSaleMenu")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from salebymenuview order by qty desc limit " + topcount , db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptTopSaleMenu.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "SaleByInvoice")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from salebyinvoiceview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptSaleByInvoice.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            else if (reportname == "SaleOrderList")
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from saleorderlistview", db.cn);
                checkDataToPrint();
                da = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                ReportDataSource source = new ReportDataSource("DataSet1", dt);

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(source);
                reportViewer1.LocalReport.ReportPath = @".\Reports\rptSaleOrderList.rdlc";
                AddReportPara();
                db.cn.Close();
            }

            this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.ZoomMode = ZoomMode.Percent;
            this.reportViewer1.ZoomPercent = 100;
            this.reportViewer1.RefreshReport();
        }
    }
}
