using Microsoft.Reporting.WinForms;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace SuccessCafePOS
{
    public partial class FormMain : Form
    {
        Backup bk = new Backup();
        DBConnection db = new DBConnection();
        SQLiteCommand cmd, cmd1, cmd2;
        SQLiteDataReader dr, dr1;
        SQLiteDataAdapter da, da1, da2;
        DataTable dt, dt1, dt2;
        DataSet ds;
        Panel pnItem;
        Button btnTableGroup, btnTable, btnCategory, btnItem;
        Label lbItem;
        string selectTable = "0", selectTableGroup = "0", selectCategory = "0", selectItem = "0", tableid = "0", invid = "0", billNo = "1";
        int loading = 0;
        int takeaway = 0;
        string cust_id = null, cname = "";
        string userid = "";
        string invstatus = "Complete";
        decimal taxper = 0;
        decimal svrper = 0;
        MyanmartoEnglish mte = new MyanmartoEnglish();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            userid = Status.userid;
            lbUsername.Text = Status.username;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                  
            if (this.Width < 1366)
            {               
                panelMain.Width = 1280;
                panel1.Width = 290;
                panel2.Width = 290;
                panelMain.Location = new Point(0, 0);
            }

            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            loadTableGroup();

            loadTable();

            loadCategory();

            loadItem();

            loadUserRight();

            checkAllowPriceChange();

        }
        private void loadUserRight()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from users where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", userid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    btnDashboard.Visible = Convert.ToBoolean(dr["dashboard"]);
                    btnItems.Visible = Convert.ToBoolean(dr["menuitem"]);
                    btnTables.Visible = Convert.ToBoolean(dr["tables"]);
                    btnCustomer.Visible = Convert.ToBoolean(dr["customer"]);
                    btnOrderList.Visible = Convert.ToBoolean(dr["orderlist"]);
                    btnExpenses.Visible = Convert.ToBoolean(dr["expenses"]);
                    btnReports.Visible = Convert.ToBoolean(dr["reports"]);
                    btnSettings.Visible = Convert.ToBoolean(dr["settings"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void checkAllowPriceChange()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select pricechange from users where id like @id; ", db.cn);
                cmd.Parameters.AddWithValue("@id", userid);
                string pc = cmd.ExecuteScalar().ToString();
                if (pc == "0")
                {
                    dataGridView1.Columns["colPrice"].ReadOnly = true;
                }
                else
                {
                    dataGridView1.Columns["colPrice"].ReadOnly = false;
                }
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void loadTableGroup()
        {
            try
            {
                flowLayoutTableGroup.Controls.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("SELECT * FROM TableGroup order by name", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    btnTableGroup = new Button();
                    btnTableGroup.Width = 100;
                    btnTableGroup.Height = 70;
                    btnTableGroup.Cursor = Cursors.Hand;
                    btnTableGroup.BackColor = Color.FromArgb(255, 207, 78);
                    btnTableGroup.Tag = dr["id"];
                    btnTableGroup.Text = dr["name"].ToString();
                    btnTableGroup.Click += BtnTableGroup_Click;
                    flowLayoutTableGroup.Controls.Add(btnTableGroup);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void BtnTableGroup_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectTableGroup = clickedbutton.Tag.ToString();
            loadTable();
        }
        private void loadTable()
        {
            try
            {
                flowLayoutTable.Controls.Clear();
                db.cn.Open();
                if (selectTableGroup == "0")
                {
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE (SELECT MIN(groupid) FROM Tables) and inactive = 0 order by srno,name", db.cn);

                }
                else
                {
                    cmd = new SQLiteCommand("SELECT * FROM Tables t WHERE groupid LIKE @id and inactive = 0 order by srno,name", db.cn);
                    cmd.Parameters.AddWithValue("@id", selectTableGroup);
                }
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    btnTable = new Button();
                    btnTable.Width = 100;
                    btnTable.Height = 70;
                    btnTable.Cursor = Cursors.Hand;
                    if (dr["tblstatus"].ToString() == "Pending")
                    {
                        btnTable.BackColor = Color.FromArgb(250, 70, 70);
                    }
                    else
                    {
                        btnTable.BackColor = Color.FromArgb(82, 244, 138);
                    }
                    btnTable.Tag = dr["id"];
                    btnTable.Text = dr["name"].ToString();
                    btnTable.Click += BtnTable_Click;
                    flowLayoutTable.Controls.Add(btnTable);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void loadCategory()
        {
            try
            {
                flowLayoutCategory.Controls.Clear();

                db.cn.Open();
                cmd = new SQLiteCommand("SELECT * FROM Category order by name", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    btnCategory = new Button();
                    btnCategory.Width = 110;
                    btnCategory.Height = 70;
                    btnCategory.Cursor = Cursors.Hand;
                    btnCategory.BackColor = Color.FromArgb(255, 207, 78);
                    btnCategory.Tag = dr["id"];
                    btnCategory.Text = dr["name"].ToString();
                    btnCategory.Click += BtnCategory_Click;
                    flowLayoutCategory.Controls.Add(btnCategory);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private void BtnCategory_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectCategory = clickedbutton.Tag.ToString();
            loadItem();
        }
        private void loadItem()
        {
            try
            {
                flowLayoutItem.Controls.Clear();

                db.cn.Open();
                if (selectCategory == "0")
                {
                    cmd = new SQLiteCommand("SELECT * FROM Items where inactive like '0' and categoryid like (select min(id) from category)", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("SELECT * FROM Items where inactive like '0' and categoryid like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", selectCategory);
                }

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    pnItem = new Panel();
                    btnItem = new Button();
                    lbItem = new Label();

                    pnItem.Width = 110;
                    pnItem.Height = 125;
                    pnItem.Cursor = Cursors.Hand;

                    btnItem.Height = 110;
                    btnItem.Dock = DockStyle.Top;
                    btnItem.BackColor = Color.FromArgb(82, 244, 138);
                    btnItem.BackgroundImageLayout = ImageLayout.Stretch;
                    btnItem.Tag = dr["id"];
                    byte[] img = dr["image"] as byte[];
                    if (img != null)
                    {
                        MemoryStream ms = new MemoryStream(img);
                        btnItem.BackgroundImage = Image.FromStream(ms);
                        lbItem.Text = dr["name"].ToString();
                        lbItem.Dock = DockStyle.Top;
                        lbItem.TextAlign = ContentAlignment.TopCenter;
                        lbItem.Font = new Font(lbItem.Font.FontFamily, lbItem.Font.Size - 1);
                        pnItem.Controls.Add(lbItem);
                    }
                    else
                    {
                        btnItem.Text = dr["name"].ToString();
                    }
                    btnItem.Click += BtnItem_Click;

                    pnItem.Controls.Add(btnItem);

                    flowLayoutItem.Controls.Add(pnItem);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }
        private string getInvoiceNo()
        {
            string inv_no = "";
            try
            {
                string inv_start = "0";
                int inv_count = 0;

                db.cn.Open();
                cmd = new SQLiteCommand("select InvStartText,InvNoCount from Settings;", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    inv_start = dr["InvStartText"].ToString();
                    inv_count = Convert.ToInt32(dr["InvNoCount"].ToString());
                }
                dr.Close();

                cmd = new SQLiteCommand("select max(invno) from salehead where invno like @str and length(invno)=@len; ", db.cn);
                cmd.Parameters.AddWithValue("@str", inv_start + "%");
                cmd.Parameters.AddWithValue("@len", inv_start.Length + inv_count);
                string maxid = cmd.ExecuteScalar() as string;

                if (maxid != null)
                {
                    int intval = int.Parse(maxid.Substring(inv_start.Length, inv_count));
                    intval++;
                    inv_no = inv_start.PadRight(inv_start.Length + inv_count - intval.ToString().Length, '0') + intval;
                }
                else
                {
                    inv_no = inv_start.PadRight(inv_start.Length + inv_count - 1, '0') + "1";
                }
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return inv_no;
        }
        private void takenewinvoice()
        {
            try
            {
                string invno = getInvoiceNo();

                db.cn.Open();
                cmd = new SQLiteCommand("INSERT INTO salehead(date,tableid,invno) values(@date,@id,@inv); " +
                    "insert into splitbill(id,billno) values((select max(id) from salehead),1); " +
                    "update tables set tblstatus = 'Pending' where id like @id;", db.cn);
                DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@id", selectTable);
                cmd.Parameters.AddWithValue("@inv", invno);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void loadinvoice(string id)
        {
            try
            {
                loading = 0;
                db.cn.Open();
                if (id == "")
                {
                    cmd = new SQLiteCommand("select t.takeaway,sh.id,date,invno,tableid,t.name tbl,c.name cust,sh.memcard," +
                        "b.remark,b.svrchr,b.taxper,b.disper,b.dischr,b.taxchr,b.advance,b.grandtotal,billno,sh.status from salehead sh " +
                        "left join splitbill b on sh.id=b.id left join customer c on sh.custid = c.id left join tables t on t.id = sh.tableid " +
                        "where sh.status in ('Pending') and sh.id like (select max(id)from Salehead) and b.billno like '1'", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select t.takeaway,sh.id,date,invno,tableid,t.name tbl,c.name cust,sh.memcard,b.remark," +
                        "b.svrchr,b.taxper,b.disper,b.dischr,b.taxchr,b.advance,b.grandtotal,billno,sh.status from salehead sh " +
                        "left join splitbill b on sh.id=b.id left join customer c on sh.custid = c.id left join tables t on t.id = sh.tableid " +
                        "where sh.status in('Pending','Order') and sh.id like @id and b.billno like @billno", db.cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@billno", cboBillNo.SelectedValue == null ? 1 : cboBillNo.SelectedValue);
                }
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtDate.Text = DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    invid = dr["id"].ToString();
                    tableid = dr["tableid"].ToString();
                    cboTable.Text = dr["tbl"].ToString();
                    txtInvoice.Text = dr["invno"].ToString();
                    cboCustomer.SelectedItem = dr["cust"].ToString();
                    cboCustomer.Text = dr["cust"].ToString();
                    txtMemCard.Text = dr["memcard"].ToString();
                    txtRemark.Text = dr["remark"] == null ? "" : dr["remark"].ToString();
                    txtSvrChr.Text = dr["svrchr"].ToString();
                    txtTax.Text = dr["taxchr"].ToString();
                    txtDisChr.Text = dr["dischr"].ToString();
                    txtDisPer.Text = dr["disper"].ToString();
                    txtAdvance.Text = dr["advance"].ToString();
                    lbGrandTotal.Text = dr["grandtotal"].ToString();
                    billNo = cboBillNo.Text = dr["billno"].ToString();
                    invstatus = dr["status"].ToString();
                    takeaway = Convert.ToInt32(dr["takeaway"].ToString());
                }
                dr.Close();
                db.cn.Close();

                loadSaleDetail(invid);

                loading = 1;
            }
            catch
            {
                db.cn.Close();
            }
        }

        private bool check_table_empty(string id)
        {
            bool table_empty = true;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from tables where id like @id and tblstatus='Pending'", db.cn);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    table_empty = false;
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return table_empty;
        }

        private void get_inv_id(string id)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select sh.id from tables t left join salehead sh on t.id = sh.tableid " +
                    "where tableid like @id and tblstatus like 'Pending' order by sh.id desc", db.cn);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                dr.Read();
                invid = dr["id"].ToString();
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void BtnTable_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectTable = clickedbutton.Tag.ToString();
            loading = 0;
            try
            {
                if (check_table_empty(selectTable))
                {
                    if (MessageBox.Show("Take a New Table?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        takenewinvoice();
                        cboTable.Text = clickedbutton.Text;
                        loadinvoice("");
                    }
                }
                else
                {
                    cboTable.Text = clickedbutton.Text;
                    get_inv_id(selectTable);
                    cboBillNo.SelectedValue = cboBillNo.Text = "1";
                    loadinvoice(invid);
                }
                loadTable();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private int getItemPrice(string id)
        {
            int price = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select saleprice from items where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", id);
                price = Convert.ToInt32(cmd.ExecuteScalar() == null ? 0 : cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return price;
        }

        private void BtnItem_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectItem = clickedbutton.Tag.ToString();
            int price = getItemPrice(selectItem);
            try
            {
                if (invid != "0")
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("insert into saledet(id,srno,itemid,price,takeaway,billno,cost) values(@id,ifnull((select max(srno)+1 from saledet where id like @id),1),@itemid,@price,@ta,@billno,ifnull((select cost from items where id like @itemid),0))", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@itemid", selectItem);
                    cmd.Parameters.AddWithValue("@ta", takeaway);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@billno", cboBillNo.SelectedValue == null ? 1 : cboBillNo.SelectedValue);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch
            {
                db.cn.Close();
            }
            loadSaleDetail(invid);
        }

        private void loadSaleDetail(string id)
        {
            try
            {
                decimal taxper = getTax();
                decimal svrper = getSvrChr();
                int totalQty = 0, totalAmt = 0;

                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,(qty*price) amount,takeaway,foc,printed," +
                    "cancel,sd.remark,it.id itid,sd.cost " +
                    "from Saledet sd left join items it on sd.itemid=it.id " +
                    "where sd.id like @invid and sd.billno = @billno and not(printed= 1 and cancel =1)", db.cn);
                cmd.Parameters.AddWithValue("@invid", id);
                cmd.Parameters.AddWithValue("@billno", Convert.ToInt32(billNo));
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataGridView1.Rows.Add(dr["id"], dr["srno"], "", dr["name"], dr["qty"], dr["price"], dr["amount"], dr["printed"], dr["cancel"], dr["takeaway"], dr["foc"], dr["remark"], dr["itid"], dr["cost"]);
                }
                dr.Close();

                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,(qty*price) amount,takeaway,foc,printed," +
                  "cancel,sd.remark,it.id itid,sd.cost " +
                  "from Saledet sd left join items it on sd.itemid=it.id " +
                  "where sd.id like @invid and sd.billno = @billno and  cancel != 1", db.cn);
                cmd.Parameters.AddWithValue("@invid", id);
                cmd.Parameters.AddWithValue("@billno", Convert.ToInt32(billNo));
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    totalQty += Convert.ToInt32(dr["qty"]);
                    totalAmt += Convert.ToInt32(dr["amount"]);
                }
                dr.Close();
                db.cn.Close();
                lbTotalQty.Text = totalQty.ToString();
                lbTotalAmount.Text = totalAmt.ToString();

                calculateGrand();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private decimal getTax()
        {
            decimal tax = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select govtax from settings", db.cn);
                tax = Convert.ToDecimal(cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return tax;
        }

        private decimal getSvrChr()
        {
            decimal tax = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select svrtax from settings", db.cn);
                tax = Convert.ToDecimal(cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return tax;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell dgc = dataGridView1.CurrentCell;
            if (dgc.ColumnIndex == 4 || dgc.ColumnIndex == 5)
            {
                e.Control.KeyPress += new KeyPressEventHandler(colQty_KeyPress);
            }
        }

        private void colQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cancelItem();
            }
        }

        private void calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDate.Text = calendar.SelectionRange.Start.ToString("dd/MM/yyyy");
            txtDate.SelectionStart = 0;
            txtDate.SelectionLength = 0;
            calendar.Hide();
        }
        private void btnCustomer_Click(object sender, EventArgs e)
        {
            FormCustomer frm = new FormCustomer();
            frm.ShowDialog();
        }

        private void cboCustomer_DropDown(object sender, EventArgs e)
        {
            loadCustomer();
        }

        private void loadCustomer()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from Customer where inactive = 0", db.cn);
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

        private void btnKitchen_Click(object sender, EventArgs e)
        {
            LocalReport localReport = new LocalReport();
            bool itemtoPrint = false, canceltoPrint = false;
            int printcount = getKitchenPrintCount();

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
                for (int i = 1; i <= printcount; i++)
                {
                    if (canceltoPrint == true)
                    {
                        db.cn.Open();

                        cmd = new SQLiteCommand("select DISTINCT(p.printer)printer,p.id id from Saledet sd left join items i on sd.itemid=i.id left join printer p on i.printer=p.id where sd.id like @id", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            cmd1 = new SQLiteCommand("select t.name,i.name item,qty,sd.remark,sd.takeaway from Saledet sd left join Salehead sh on sd.id = sh.id left join Items i on sd.itemid=i.id left join tables t on sh.tableid = t.id where sh.id like @id and printed like '0' and cancel like '1' and i.printer like @printer", db.cn);
                            cmd1.Parameters.AddWithValue("@printer", dr["id"].ToString());
                            cmd1.Parameters.AddWithValue("@id", invid);
                            da1 = new SQLiteDataAdapter(cmd1);
                            dt1 = new DataTable();
                            da1.Fill(dt1);

                            ReportDataSource source2 = new ReportDataSource("DataSet1", dt1);

                            localReport.DataSources.Clear();
                            localReport.DataSources.Add(source2);
                            localReport.ReportPath = @".\Reports\rptCancelSlip.rdlc";
                            DirectPrint printer = new DirectPrint();
                            if (dr["printer"].ToString() == "")
                            {
                                printer.PrintToPrinter(localReport);
                            }
                            else
                            {
                                printer.PrintToPrinter(localReport, dr["printer"].ToString());
                            }
                        }
                        dr.Close();
                        db.cn.Close();
                    }
                    if (itemtoPrint == true)
                    {
                        db.cn.Open();

                        cmd = new SQLiteCommand("select DISTINCT(p.printer)printer,p.id id from Saledet sd left join items i on sd.itemid=i.id left join printer p on i.printer=p.id where sd.id like @id", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            cmd1 = new SQLiteCommand("select t.name,i.name item,qty,sd.remark,sd.takeaway from Saledet sd left join Salehead sh on sd.id = sh.id left join Items i on sd.itemid=i.id left join tables t on sh.tableid = t.id where sh.id like @id and printed like '0' and cancel like '0' and i.printer like @printer", db.cn);
                            cmd1.Parameters.AddWithValue("@printer", dr["id"].ToString());
                            cmd1.Parameters.AddWithValue("@id", invid);
                            da1 = new SQLiteDataAdapter(cmd1);
                            dt1 = new DataTable();
                            da1.Fill(dt1);

                            ReportDataSource source2 = new ReportDataSource("DataSet1", dt1);

                            localReport.DataSources.Clear();
                            localReport.DataSources.Add(source2);
                            localReport.ReportPath = @".\Reports\rptKitchenSlip.rdlc";
                            DirectPrint printer = new DirectPrint();
                            if (dr["printer"].ToString() == "")
                            {
                                printer.PrintToPrinter(localReport);
                            }
                            else
                            {
                                printer.PrintToPrinter(localReport, dr["printer"].ToString());
                            }
                        }
                        dr.Close();
                        db.cn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("No Item to Print!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("update saledet set printed=1 where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }

            loadSaleDetail(invid);
        }

        private bool checkPrintDirect()
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
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Item to Print!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LocalReport localReport = new LocalReport();

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

            localReport.DataSources.Clear();
            localReport.DataSources.Add(source);
            localReport.DataSources.Add(source1);
            localReport.DataSources.Add(source2);
            localReport.ReportPath = @".\Reports\rptBillSlip.rdlc";
            ReportParameter[] parameters = new ReportParameter[]
            {
                    new ReportParameter("username", Status.username)
            };
            localReport.SetParameters(parameters);

            DirectPrint printer = new DirectPrint();
            db.cn.Close();

            if (checkPrintDirect() == true)
            {
                for (int i = 1; i <= getPrintCount(); i++)
                {
                    printer.PrintToPrinter(localReport);
                }
            }
            else
            {
                FormReportViewer frm = new FormReportViewer();
                frm.reportname = "Bill";
                frm.invid = invid;
                frm.billNo = billNo;
                frm.ShowDialog();
            }
        }

        private int getPrintCount()
        {
            int count = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select billcount from settings", db.cn);
                count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return count;
        }

        private int getKitchenPrintCount()
        {
            int count = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select kitchencount from settings", db.cn);
                count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return count;
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    db.cn.Open();
                    DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cmd = new SQLiteCommand("update salehead set date=@date where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
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
                cmd = new SQLiteCommand("select * from tables where tblstatus like 'Pending'", db.cn);
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

        private void cboCustomer_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead set custid=@custid where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@custid", cboCustomer.SelectedValue);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    txtMemCard.Clear();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void txtRemark_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead set remark=@remark where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void txtMemCard_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    if (txtMemCard.Text.Trim().Length > 0)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("select id,name,discount from customer where memcard like @memcard", db.cn);
                        cmd.Parameters.AddWithValue("@memcard", txtMemCard.Text);
                        dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            cust_id = dr["id"].ToString();
                            cname = dr["name"].ToString();
                            loading = 0;
                            txtDisPer.Text = dr["discount"].ToString();
                            loading = 1;
                        }
                        else
                        {
                            cust_id = "";
                        }
                        dr.Close();
                        db.cn.Close();

                        if (cust_id != "")
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("update salehead set custid=@custid,memcard=@memcard where id like @id", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.Parameters.AddWithValue("@custid", cust_id);
                            cmd.Parameters.AddWithValue("@memcard", txtMemCard.Text);
                            cmd.ExecuteNonQuery();
                            cboCustomer.Text = cname;
                            cboCustomer.SelectedItem = cname;
                            loading = 0;
                            decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                            decimal grand = Convert.ToDecimal(lbGrandTotal.Text);
                            txtDisChr.Text = (total * Convert.ToDecimal(txtDisPer.Text) / 100).ToString("0") == string.Empty ? "0" : (total * Convert.ToDecimal(txtDisPer.Text) / 100).ToString("0");
                            decimal dis = Convert.ToDecimal(txtDisChr.Text);
                            lbGrandTotal.Text = (grand - dis).ToString("0");
                            loading = 1;
                            db.cn.Close();
                        }
                        else
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("update salehead set memcard=@memcard where id like @id", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.Parameters.AddWithValue("@memcard", "");
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                            loading = 0;
                            decimal total = Convert.ToDecimal(lbGrandTotal.Text);
                            decimal dis = Convert.ToDecimal(txtDisChr.Text);
                            lbGrandTotal.Text = (total + dis).ToString("0");
                            txtDisPer.Text = "0";
                            txtDisChr.Text = "0";
                            loading = 1;
                        }
                        updatenet();
                    }
                    else
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("update salehead set memcard=@memcard where id like @id", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.Parameters.AddWithValue("@memcard", "");
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        loading = 0;
                        decimal total = Convert.ToDecimal(lbGrandTotal.Text);
                        decimal dis = Convert.ToDecimal(txtDisChr.Text);
                        lbGrandTotal.Text = (total + dis).ToString("0");
                        txtDisPer.Text = "0";
                        txtDisChr.Text = "0";
                        loading = 1;
                        updatenet();
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
            if (dataGridView1.Columns[e.ColumnIndex].Name == "sr")
            {
                e.Value = e.RowIndex + 1;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string printed = row.Cells["colPrinted"].Value.ToString();
                string cancel = row.Cells["colCancel"].Value.ToString();

                if (printed == "1" && cancel == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCyan;
                    row.Cells["colQty"].ReadOnly = true;
                    row.Cells["colPrice"].ReadOnly = true;
                    row.Cells["colTakeAway"].ReadOnly = true;
                    row.Cells["colRemark"].ReadOnly = true;
                }
                else if (printed == "0" && cancel == "1")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 100);
                    row.Cells["colQty"].ReadOnly = true;
                    row.Cells["colPrice"].ReadOnly = true;
                    row.Cells["colTakeAway"].ReadOnly = true;
                    row.Cells["colRemark"].ReadOnly = true;
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView1.Columns[e.ColumnIndex].Name;

                DataGridViewRow row = dataGridView1.CurrentRow;
                if (row != null)
                {
                    if (colName == "colQty")
                    {
                        var cellvalue = dataGridView1[4, e.RowIndex].Value;
                        if (cellvalue == null || cellvalue.ToString() == "0")
                        {
                            cancelItem();
                        }
                        else
                        {
                            string qty = mte.convert_text(cellvalue.ToString());
                            db.cn.Open();
                            cmd = new SQLiteCommand("update saledet set qty=@qty where id like @id and srno like @sr and billno like @billno", db.cn);
                            cmd.Parameters.AddWithValue("@qty", qty);
                            cmd.Parameters.AddWithValue("@id", dataGridView1[0, e.RowIndex].Value.ToString());
                            cmd.Parameters.AddWithValue("@sr", dataGridView1[1, e.RowIndex].Value.ToString());
                            cmd.Parameters.AddWithValue("@billno", billNo);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                        }
                        loadSaleDetail(invid);
                    }
                    else if (colName == "colPrice")
                    {
                        var cellvalue = dataGridView1[5, e.RowIndex].Value;

                        db.cn.Open();
                        cmd = new SQLiteCommand("update saledet set price=@price where id like @id and srno like @sr and billno like @billno", db.cn);
                        if (cellvalue == null)
                        {
                            cmd.Parameters.AddWithValue("@price", 0);
                        }
                        else
                        {
                            string price = mte.convert_text(cellvalue.ToString());
                            cmd.Parameters.AddWithValue("@price", price);
                        }
                        cmd.Parameters.AddWithValue("@id", dataGridView1[0, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@sr", dataGridView1[1, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@billno", billNo);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        loadSaleDetail(invid);
                    }
                    else if (colName == "colTakeAway")
                    {
                        var id = row.Cells["colID"].Value;
                        var sr = row.Cells["colSr"].Value;
                        var ta = row.Cells["colTakeAway"].Value;
                        db.cn.Open();
                        cmd = new SQLiteCommand("update saledet set takeaway=@ta where id like @id and srno like @sr", db.cn);
                        cmd.Parameters.AddWithValue("@ta", ta);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView1.Columns[e.ColumnIndex].Name;

                DataGridViewRow row = dataGridView1.CurrentRow;

                string printed = row.Cells["colPrinted"].Value.ToString();
                string sr = row.Cells["colSr"].Value.ToString();

                if (row != null)
                {
                    if (colName == "colRemark")
                    {
                        if (printed.ToString() != "1")
                        {
                            FormRemarks frm = new FormRemarks();
                            frm.id = invid;
                            frm.sr = sr;
                            frm.ShowDialog();
                            loadSaleDetail(invid);
                        }
                    }
                    else if (colName == "colDelete")
                    {
                        cancelItem();
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (invid != "0")
                {
                    if (MessageBox.Show("Are you sure to Cancel this Table?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("update tables set tblstatus = '' where name like @name; " +
                       "delete from saledet where id like @id; " +
                       "delete from salehead where id like @id; " +
                       "delete from splitbill where id like @id; ", db.cn);
                        cmd.Parameters.AddWithValue("@name", cboTable.Text);
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        clear();
                    }
                }
                else
                {
                    MessageBox.Show("Please select Table to Cancel!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void clear()
        {
            invid = "0";
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtInvoice.Clear();
            cboBillNo.Text = "";
            cboCustomer.Text = "";
            cboTable.Text = "";
            txtMemCard.Clear();
            txtRemark.Clear();
            dataGridView1.Rows.Clear();
            txtSvrChr.Text = "0";
            txtTax.Text = "0";
            lbTotalQty.Text = "0";
            lbTotalAmount.Text = "0";
            txtDisChr.Text = "0";
            txtDisPer.Text = "0";
            txtAdvance.Text = "0";
            lbGrandTotal.Text = "0";
            loadTable();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            bk.backup();
            Application.Exit();
        }

        private void cboTable_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                loading = 0;
                db.cn.Open();
                cmd = new SQLiteCommand("select sh.id,date,invno,c.name cust,sh.memcard,remark,t.takeaway from salehead sh left join customer c on sh.custid = c.id left join tables t on t.id = sh.tableid where tableid like @tableid and tblstatus like 'Pending' and sh.status in ('Pending','Order') order by sh.id desc", db.cn);
                cmd.Parameters.AddWithValue("@tableid", cboTable.SelectedValue);
                dr = cmd.ExecuteReader();
                dr.Read();
                txtDate.Text = DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                invid = dr["id"].ToString();
                tableid = cboTable.SelectedValue.ToString();
                txtInvoice.Text = dr["invno"].ToString();
                cboCustomer.SelectedItem = dr["cust"].ToString();
                cboCustomer.Text = dr["cust"].ToString();
                txtMemCard.Text = dr["memcard"].ToString();
                txtRemark.Text = dr["remark"].ToString();
                takeaway = Convert.ToInt32(dr["takeaway"].ToString());
                cboBillNo.Text = "1";
                billNo = "1";
                dr.Close();
                db.cn.Close();
                loading = 1;
                loadSaleDetail(invid);
            }
            catch
            {

                db.cn.Close();
            }
        }

        private void tblTableChange_Click(object sender, EventArgs e)
        {
            if (invid != "0")
            {
                FormChangeTable frm = new FormChangeTable();
                frm.invid = invid;
                frm.ShowDialog();
                if (frm.tableName != null)
                {
                    cboTable.Text = frm.tableName;
                    cboTable.SelectedItem = frm.tableName;
                    invid = frm.invid;
                    loadinvoice(invid);
                    loadTable();
                }
            }
            else
            {
                MessageBox.Show("No Table to Change!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTableMerge_Click(object sender, EventArgs e)
        {
            if (invid != "0")
            {
                FormMergeTable frm = new FormMergeTable();
                frm.invid = invid;
                frm.tableid = tableid;
                frm.ShowDialog();
                if (frm.tableName != null)
                {
                    cboTable.Text = frm.tableName;
                    cboTable.SelectedItem = frm.tableName;
                    loadTable();
                    invid = frm.newInvId;
                    loadinvoice(invid);
                    loadSaleDetail(invid);
                }
            }
            else
            {
                MessageBox.Show("No Table to Merge!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSplitBill_Click(object sender, EventArgs e)
        {
            if (invid == "0" || dataGridView1.Rows.Count <= 1)
            {
                MessageBox.Show("No Data to Split Bill!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row != null)
            {
                int qty = Convert.ToInt32(row.Cells["colQty"].Value.ToString());
                bool isSplit = false;
                db.cn.Open();
                cmd = new SQLiteCommand("select id from saledet where billno = @billno", db.cn);
                cmd.Parameters.AddWithValue("@billno", Convert.ToInt32(billNo) + 1);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    isSplit = true;
                }
                dr.Close();
                db.cn.Close();
                if (dataGridView1.Rows.Count > 1 || qty > 1 || isSplit == true)
                {
                    FormSplitBill frm = new FormSplitBill();
                    frm.invid = invid;
                    frm.frombillno = billNo;
                    frm.ShowDialog();
                    loadSaleDetail(invid);
                    decimal tax = getTax();
                    decimal svr = getSvrChr();
                    decimal dis = Convert.ToDecimal(txtDisPer.Text);
                    db.cn.Open();
                    cmd = new SQLiteCommand("update splitbill set taxper=@tax,svrper=@svr,disper=@dis where id like @id; " +
                        "update splitbill set " +
                        "taxchr=(totalamount*taxper/100)," +
                        "svrchr=(totalamount*svrper/100)," +
                        "dischr=(totalamount*disper/100)," +
                        "grandtotal=totalamount+(totalamount*taxper/100)+(totalamount*svrper/100)-(totalamount*disper/100)," +
                        "balance=totalamount+(totalamount*taxper/100)+(totalamount*svrper/100)-(totalamount*disper/100) where id like @id and billno > @billno;", db.cn);
                    cmd.Parameters.AddWithValue("@tax", tax);
                    cmd.Parameters.AddWithValue("@svr", svr);
                    cmd.Parameters.AddWithValue("@dis", dis);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@billno", billNo);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
        }

        private void cboBillNo_DropDown(object sender, EventArgs e)
        {
            try
            {
                if (invid != "0")
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("select distinct(billno) from saledet where id like @id order by billno", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    da = new SQLiteDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                    cboBillNo.DataSource = ds.Tables[0];
                    cboBillNo.DisplayMember = "billno";
                    cboBillNo.ValueMember = "billno";
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboBillNo_DropDownClosed(object sender, EventArgs e)
        {
            if (cboBillNo.SelectedValue != null)
            {
                billNo = cboBillNo.SelectedValue.ToString();
                loadSaleDetail(invid);
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            FormDashboard frm = new FormDashboard();
            frm.ShowDialog();
        }

        private void txtDisChr_TextChanged(object sender, EventArgs e)
        {
            if (loading == 1)
            {
                decimal tax = Convert.ToDecimal(txtTax.Text);
                decimal chr = Convert.ToDecimal(txtSvrChr.Text);
                decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                decimal dis = 0;
                decimal adv = Convert.ToDecimal(txtAdvance.Text);
                if (txtDisChr.Text != String.Empty)
                {
                    dis = Convert.ToDecimal(mte.convert_text(txtDisChr.Text));
                }
                lbGrandTotal.Text = (total + tax + chr - adv - dis).ToString("0");
            }
        }

        private void txtDisPer_TextChanged(object sender, EventArgs e)
        {
            if (loading == 1)
            {
                if (txtDisPer.Text != String.Empty)
                {
                    txtDisChr.Text = (Convert.ToDecimal(lbTotalAmount.Text)
                     * Convert.ToDecimal(mte.convert_text(txtDisPer.Text)) / 100).ToString("0");
                }
                else
                {
                    txtDisChr.Text = "0";
                }
            }
        }

        private void txtDisPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (lbTotalAmount.Text == "0")
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && txtDisPer.Text.Contains("."))
            {
                // Stop more than one dot Char
                e.Handled = true;
            }
            else if (e.KeyChar == '.' && txtDisPer.Text.Length == 0)
            {
                // Stop first char as a dot input
                e.Handled = true;
            }
            //else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //{
            //    // Stop allow other than digit and control
            //    e.Handled = true;
            //}
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = mte.convert_text(txtDisPer.Text);

            int selectionStart = txtDisPer.SelectionStart;
            int selectionLength = txtDisPer.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));

            if (decimal.TryParse(newText, out decimal result))
            {
                if (result > 100)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtDisPer_Leave(object sender, EventArgs e)
        {
            if (txtDisPer.Text.Trim().Length == 0)
            {
                txtDisPer.Text = "0";
            }
            else
            {
                txtDisPer.Text = mte.convert_text(txtDisPer.Text);
            }
        }

        private void txtInvoice_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead set invno=@invno where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@invno", txtInvoice.Text);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void txtSvrChr_TextChanged(object sender, EventArgs e)
        {
            if (loading == 1)
            {
                decimal chr = 0;
                decimal tax = Convert.ToDecimal(txtTax.Text);
                decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                decimal dis = Convert.ToDecimal(txtDisChr.Text);
                decimal adv = Convert.ToDecimal(txtAdvance.Text);
                if (txtSvrChr.Text != String.Empty)
                {
                    chr = Convert.ToDecimal(mte.convert_text(txtSvrChr.Text));
                }

                lbGrandTotal.Text = (total + tax + chr - dis - adv).ToString("0");
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select password from settings;", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Status.password = dr["password"].ToString();
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }

            if (Status.password != string.Empty)
            {
                FormSettingPassCheck frm = new FormSettingPassCheck();
                frm.ShowDialog();
            }
            else
            {
                FormSetting frm = new FormSetting();
                frm.ShowDialog();
            }
            loadUserRight();
            loadTable();
            checkAllowPriceChange();
            clear();
        }

        private void btnExpenses_Click_1(object sender, EventArgs e)
        {
            FormExpenses frm = new FormExpenses();
            frm.ShowDialog();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            FormReports frm = new FormReports();
            frm.ShowDialog();
        }

        private void txtAdvance_TextChanged(object sender, EventArgs e)
        {
            if (loading == 1)
            {
                decimal tax = Convert.ToDecimal(txtTax.Text);
                decimal chr = Convert.ToDecimal(txtSvrChr.Text);
                decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                decimal dis = Convert.ToDecimal(txtDisChr.Text);
                decimal adv = 0;
                if (txtAdvance.Text != String.Empty)
                {
                    adv = Convert.ToDecimal(txtAdvance.Text);
                }

                lbGrandTotal.Text = (total + tax + chr - dis - adv).ToString("0");
            }
        }

        private void txtAdvance_Leave(object sender, EventArgs e)
        {
            if (txtAdvance.Text.Trim().Length == 0)
            {
                txtAdvance.Text = "0";
            }
        }

        private void txtAdvance_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = txtAdvance.Text;

            int selectionStart = txtAdvance.SelectionStart;
            int selectionLength = txtAdvance.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, e.KeyChar.ToString());

            if (int.TryParse(newText, out int result))
            {
                if (result > Convert.ToInt32(lbTotalAmount.Text))
                {
                    e.Handled = true;
                }
            }
        }

        private void txtTax_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = mte.convert_text(txtTax.Text);

            int selectionStart = txtTax.SelectionStart;
            int selectionLength = txtTax.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));

            if (int.TryParse(newText, out int result))
            {
                if (result > Convert.ToInt32(lbTotalAmount.Text))
                {
                    e.Handled = true;
                }
            }
        }

        private void txtSvrChr_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = mte.convert_text(txtSvrChr.Text);

            int selectionStart = txtSvrChr.SelectionStart;
            int selectionLength = txtSvrChr.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));

            if (int.TryParse(newText, out int result))
            {
                if (result > Convert.ToInt32(lbTotalAmount.Text))
                {
                    e.Handled = true;
                }
            }
        }

        private void btnOrderList_Click(object sender, EventArgs e)
        {
            FormOrderList frm = new FormOrderList();
            frm.ShowDialog();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            if (invid != "0" && row != null)
            {
                FormPlaceOrder frm = new FormPlaceOrder();
                frm.invid = invid;
                frm.ShowDialog();
                invstatus = frm.status;
            }
            else
            {
                MessageBox.Show("No Item to Order!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lbGrandTotal.Text) > 0)
            {
                if (invstatus == "Order")
                {
                    FormAdvance frm = new FormAdvance();
                    frm.bal = Convert.ToInt32(lbGrandTotal.Text) + Convert.ToInt32(txtAdvance.Text);
                    frm.ShowDialog();
                    txtAdvance.Text = frm.adv;
                }
                else
                {
                    MessageBox.Show("Make an Order!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataGridViewRow row = dataGridView1.CurrentRow;

                    if (invid != "0" && row != null)
                    {
                        FormPlaceOrder frm = new FormPlaceOrder();
                        frm.invid = invid;
                        frm.ShowDialog();
                        invstatus = frm.status;

                        FormAdvance form = new FormAdvance();
                        form.bal = Convert.ToInt32(lbGrandTotal.Text) + Convert.ToInt32(txtAdvance.Text);
                        form.ShowDialog();
                        txtAdvance.Text = form.adv;
                    }
                    else
                    {
                        MessageBox.Show("No Item to Order!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("No Data to Pay Advance!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTransferItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            if (invid != "0" && row != null)
            {
                FormTransferItem frm = new FormTransferItem();
                frm.ftable = cboTable.Text;
                frm.invid = invid;
                frm.ShowDialog();
                invid = frm.toinvid;
                loadinvoice(invid);
                loadSaleDetail(invid);
            }
            else
            {
                MessageBox.Show("No Item to Transfer!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void txtTax_TextChanged(object sender, EventArgs e)
        {
            if (loading == 1)
            {
                decimal tax = 0;
                decimal chr = Convert.ToDecimal(txtSvrChr.Text);
                decimal total = Convert.ToDecimal(lbTotalAmount.Text);
                decimal dis = Convert.ToDecimal(txtDisChr.Text);
                decimal adv = Convert.ToDecimal(txtAdvance.Text);
                if (txtTax.Text != String.Empty)
                {
                    tax = Convert.ToDecimal(mte.convert_text(txtTax.Text));
                }

                lbGrandTotal.Text = (total - dis + tax + chr - adv).ToString("0");
            }
        }

        private void cboTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView1.Columns[e.ColumnIndex].Name;

                DataGridViewRow row = dataGridView1.CurrentRow;

                if (row != null)
                {
                    if (colName == "colFOC")
                    {
                        var id = row.Cells["colID"].Value;
                        var sr = row.Cells["colSr"].Value;
                        var foc = row.Cells["colFOC"].Value;
                        var qty = row.Cells["colQty"].Value;
                        var itemid = row.Cells["colItemID"].Value;
                        int price = 0;
                        if (foc.ToString() == "0")
                        {
                            foc = "1";
                            price = 0;
                        }
                        else
                        {
                            foc = "0";
                            price = getItemPrice(itemid.ToString());
                        }

                        db.cn.Open();
                        cmd = new SQLiteCommand("update saledet set foc=@foc,price=@price where id like @id and srno like @sr", db.cn);
                        cmd.Parameters.AddWithValue("@foc", foc);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@amt", price * Convert.ToInt32(qty));
                        cmd.ExecuteNonQuery();
                        db.cn.Close();

                        loadSaleDetail(invid);
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            FormLogin frm = new FormLogin();
            frm.Show();
            this.Close();
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;
                cust_id = null;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cboCustomer_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (loading == 1)
                {
                    if (cboCustomer.Text == "")
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("update salehead set custid=@custid where id like @id", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.Parameters.AddWithValue("@custid", null);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        txtMemCard.Clear();
                        cust_id = null;
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboCustomer_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedValue != null)
            {
                cust_id = cboCustomer.SelectedValue.ToString();
            }
        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cust_id = null;
            }
        }

        private void cboBillNo_Leave(object sender, EventArgs e)
        {
            if (cboBillNo.Text == String.Empty)
            {
                cboBillNo.Text = "1";
            }
        }

        private void cboBillNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadinvoice(invid);
            updatenet();
        }

        private void txtTax_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtTax.Text))
            {
                txtTax.Text = "0";
            }
            else
            {
                txtTax.Text = mte.convert_text(txtTax.Text);
            }
        }

        private void txtSvrChr_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtSvrChr.Text))
            {
                txtSvrChr.Text = "0";
            }
            else
            {
                txtSvrChr.Text = mte.convert_text(txtSvrChr.Text);
            }
        }

        private void txtDisChr_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtDisChr.Text))
            {
                txtDisChr.Text = "0";
            }
            else
            {
                txtDisChr.Text = mte.convert_text(txtDisChr.Text);
            }
        }

        private void txtDisChr_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = mte.convert_text(txtDisChr.Text);

            int selectionStart = txtDisChr.SelectionStart;
            int selectionLength = txtDisChr.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));

            if (int.TryParse(newText, out int result))
            {
                if (result > Convert.ToInt32(lbTotalAmount.Text))
                {
                    e.Handled = true;
                }
            }
        }

        void updatenet()
        {
            try
            {
                if (loading == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update splitbill set totalQty=@qty,totalAmount=@amt,svrchr=@chr,taxper =@taxper," +
                        "taxchr=@taxchr,disper=@disper,dischr=@dischr,grandtotal=@grand,advance=@advance,balance=@balance,svrper=@svrper " +
                        "where id like @id and billno like @billno", db.cn);
                    cmd.Parameters.AddWithValue("@qty", lbTotalQty.Text);
                    cmd.Parameters.AddWithValue("@amt", lbTotalAmount.Text);
                    cmd.Parameters.AddWithValue("@chr", mte.convert_text(txtSvrChr.Text));
                    cmd.Parameters.AddWithValue("@taxper", taxper);
                    cmd.Parameters.AddWithValue("@svrper", svrper);
                    cmd.Parameters.AddWithValue("@taxchr", mte.convert_text(txtTax.Text));
                    cmd.Parameters.AddWithValue("@disper", mte.convert_text(txtDisPer.Text));
                    cmd.Parameters.AddWithValue("@dischr", mte.convert_text(txtDisChr.Text));
                    cmd.Parameters.AddWithValue("@grand", lbGrandTotal.Text);
                    cmd.Parameters.AddWithValue("@advance", txtAdvance.Text);
                    cmd.Parameters.AddWithValue("@balance", (Convert.ToDecimal(lbGrandTotal.Text) - Convert.ToDecimal(txtAdvance.Text)));
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@billno", cboBillNo.SelectedValue == null ? 1 : cboBillNo.SelectedValue);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }
        void calculateGrand()
        {
            loading = 0;
            taxper = getTax();
            svrper = getSvrChr();

            decimal chr = Convert.ToDecimal(txtSvrChr.Text);
            decimal tax = Convert.ToDecimal(txtTax.Text);
            decimal adv = Convert.ToDecimal(txtAdvance.Text);
            decimal disper = Convert.ToDecimal(txtDisPer.Text);
            decimal dis = Convert.ToDecimal(txtDisChr.Text);
            decimal total = Convert.ToDecimal(lbTotalAmount.Text);

            if (svrper != 0)
            {
                txtSvrChr.Text = (total * svrper / 100).ToString("0") == string.Empty ? "0" : (total * svrper / 100).ToString("0");
            }
            if (disper != 0)
            {
                txtDisChr.Text = (total * disper / 100).ToString("0") == string.Empty ? "0" : (total * Convert.ToDecimal(txtDisPer.Text) / 100).ToString("0");
            }
            if (taxper != 0)
            {
                txtTax.Text = (total * taxper / 100).ToString("0") == string.Empty ? "0" : (total * taxper / 100).ToString("0");
            }
            loading = 1;

            lbGrandTotal.Text = (total - dis + tax + chr - adv).ToString("0");
        }

        private void lbGrandTotal_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lbGrandTotal.Text) < 0)
            {
                loading = 0;
                txtAdvance.Text = "0";
                loading = 1;
                calculateGrand();
            }
            updatenet();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool foc_exist = false;
                decimal foc_cost = 0;
                string foc_cmd = "", sql_cmd = "";

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No Data to Save!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (invstatus == "Order" && cboCustomer.Text == "")
                {
                    MessageBox.Show("Please select Customer to Order!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadCustomer();
                    cboCustomer.Focus();
                    return;
                }

                db.cn.Open();
                cmd = new SQLiteCommand("select foc,qty,cost from saledet where id like @id;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["foc"].ToString() == "1")
                    {
                        foc_cost += Convert.ToInt32(dr["qty"].ToString()) * Convert.ToDecimal(dr["cost"].ToString());
                        foc_exist = true;
                    }
                }
                dr.Close();
                db.cn.Close();

                if (foc_exist)
                {
                    foc_cmd = " insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                   "values (@date,@invno,@id,'FOC',0,@foc);";
                }

                if (invstatus == "Pending")
                {
                    sql_cmd = " insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                    " values (@date,@invno," +
                    "@id,'Sale',1," +
                    "(select grandtotal from salehead where id like @id)); ";
                    invstatus = "Complete";
                }

                if (invstatus == "Order")
                {
                    sql_cmd = " insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                   " values (@date,@invno," +
                   "@id,(select IIF(advance=0,'Credit','Advance') from salehead where id like @id),1," +
                   "(select advance from salehead where id like @id)); ";
                }

                db.cn.Open();
                cmd = new SQLiteCommand("update salehead set status = @status, custid=@custid," +
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
                    "svrper= (select svrper from splitbill where id like @id limit 1)," +
                    "foc=@foc " +
                    "where id like @id; " +
                    "update tables set tblstatus ='' where name like @name; " + sql_cmd + foc_cmd, db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@invno", txtInvoice.Text);
                cmd.Parameters.AddWithValue("@status", invstatus);
                cmd.Parameters.AddWithValue("@name", cboTable.Text);
                cmd.Parameters.AddWithValue("@custid", cust_id);
                DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@foc", foc_cost);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                MessageBox.Show("Saved successfully!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clear();
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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnTables_Click(object sender, EventArgs e)
        {
            FormTable frm = new FormTable();
            frm.ShowDialog();
            loadTableGroup();
            loadTable();
        }

        private void btnItems_Click(object sender, EventArgs e)
        {
            FormMenu frm = new FormMenu();
            frm.ShowDialog();
            loadCategory();
            loadItem();
        }

        private void cancelItem()
        {
            try
            {
                string sr = "0", printed = "0", cancel = "0", qty = "0";
                DataGridViewRow row = dataGridView1.CurrentRow;

                if (row != null)
                {
                    sr = row.Cells["colSr"].Value.ToString();
                    if (row.Cells["colQty"].Value != null)
                    {
                        qty = row.Cells["colQty"].Value.ToString();
                    }
                }

                if (MessageBox.Show("Cancel this Item?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (row != null)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("select printed,cancel from saledet where id like @id and srno like @sr", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            printed = dr["printed"].ToString();
                            cancel = dr["cancel"].ToString();
                        }
                        dr.Close();
                        db.cn.Close();

                        if (printed == "0" && cancel == "0")
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("delete from saledet where id like @id and srno like @sr", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.Parameters.AddWithValue("@sr", sr);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                            dataGridView1.Rows.Remove(row);
                        }
                        else if (printed == "1" && cancel == "0")
                        {
                            if (qty == "0" || qty == "1")
                            {
                                db.cn.Open();
                                cmd = new SQLiteCommand("update saledet set printed = 0,cancel = 1 where id like @id and srno like @sr", db.cn);
                                cmd.Parameters.AddWithValue("@id", invid);
                                cmd.Parameters.AddWithValue("@sr", sr);
                                cmd.ExecuteNonQuery();
                                db.cn.Close();
                            }
                            else
                            {
                                FormCancelQty frm = new FormCancelQty();
                                frm.invid = invid;
                                frm.srno = sr;
                                frm.qty = qty;
                                frm.ShowDialog();
                            }
                        }
                        loadSaleDetail(invid);
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }
    }
}

