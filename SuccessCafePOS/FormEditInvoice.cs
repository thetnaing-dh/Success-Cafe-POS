using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Diagnostics;

namespace SuccessCafePOS
{
    public partial class FormEditInvoice : Form
    {
        public string invid { get; set; }
        string billNo = "1";
        int afterLoad = 0;
        DBConnection db = new DBConnection();
        SQLiteCommand cmd,cmd1,cmd2;
        SQLiteDataReader dr;
        SQLiteDataAdapter da,da1,da2;
        DataSet ds;
        Panel pnItem;
        Label lbItem;
        DataTable dt,dt1,dt2;
        string selectCategory = "0",selectItem ="0";
        Button btnItem,btnCategory;
        decimal taxper = 0;
        decimal svrper = 0;

        MyanmartoEnglish mte = new MyanmartoEnglish();

        public FormEditInvoice()
        {
            InitializeComponent();
        }

     
        private void FormEditInvoice_Load(object sender, EventArgs e)
        {
            insertToTmp();
            loadCategory();
            loadItem();
            loadRecord();
            loadSaleDetail();
            calculateGrand();
            afterLoad = 1;
        }

        void insertToTmp()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from salehead_tmp; " +
                    "delete from saledet_tmp; " +
                    "delete from splitbill_tmp; " +
                    "insert into salehead_tmp select * from salehead where id like @id; " +
                    "insert into saledet_tmp select * from saledet where id like @id; " +
                    "insert into splitbill_tmp select * from splitbill where id like @id; ", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {             
                db.cn.Close();
            }
        }

        void loadRecord()
        {
            try
            {
                afterLoad = 0;
                db.cn.Open();
                cmd = new SQLiteCommand("select sh.id,date,invno,tableid,t.name tbl,c.name cust,sh.memcard,concat(sh.remark,b.remark) remark,b.svrchr,b.taxper,b.disper,b.dischr,b.taxchr,b.advance,b.grandtotal,b.billno from salehead_tmp sh left join splitbill_tmp b on sh.id=b.id left join customer c on sh.custid = c.id left join tables t on t.id = sh.tableid where sh.id like @id and b.billno like @billno", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@billno", cboBillNo.SelectedValue == null ? 1 : cboBillNo.SelectedValue);               
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtDate.Text = DateTime.ParseExact(dr["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    cboBillNo.Text = dr["billno"] == null ? "1" : dr["billno"].ToString();
                    invid = dr["id"].ToString();
                    cboTable.Text = dr["tbl"].ToString();
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
                }
                dr.Close();
                db.cn.Close();
                afterLoad = 1;              
            }
            catch  
            {               
                db.cn.Close();
            }
        }
        decimal getTax()
        {
            decimal tax = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select taxper from splitbill_tmp where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                tax = Convert.ToDecimal(cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return tax;
        }

        decimal getSvrChr()
        {
            decimal tax = 0;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select svrper from splitbill_tmp where id like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                tax = Convert.ToDecimal(cmd.ExecuteScalar());
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return tax;
        }

        void loadSaleDetail()
        {
            try
            {                
                int totalQty = 0, totalAmt = 0;
                dataGridView1.Rows.Clear();                
                db.cn.Open();
                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,(qty*price) amount,takeaway,foc,printed,cancel,sd.remark,it.id itid from Saledet_tmp sd left join items it on sd.itemid=it.id where sd.id like @invid and sd.billno = @billno and cancel != 1 ;", db.cn);
                cmd.Parameters.AddWithValue("@invid", invid);
                cmd.Parameters.AddWithValue("@billno", Convert.ToInt32(billNo));
                dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], dr["srno"], i, dr["name"], dr["qty"], dr["price"], dr["amount"], dr["printed"], dr["cancel"], dr["takeaway"],dr["foc"], dr["remark"], dr["itid"]);
                    totalQty += Convert.ToInt32(dr["qty"]);
                    totalAmt += Convert.ToInt32(dr["amount"]);
                }
                dr.Close();
                db.cn.Close();
                lbTotalQty.Text = totalQty.ToString();
                lbTotalAmount.Text = totalAmt.ToString();            
            }
            catch
            {                    
                db.cn.Close();
            }
        }

        void calculateGrand()
        {
            afterLoad = 0;
            taxper = getTax();
            svrper = getSvrChr();
            decimal disper = Convert.ToDecimal(txtDisPer.Text);

            decimal total = Convert.ToDecimal(lbTotalAmount.Text);
            if (svrper != 0)
            {
                txtSvrChr.Text = (total * svrper / 100).ToString("0") == string.Empty ? "0" : (total * svrper / 100).ToString("0");
            }
            if (disper != 0)
            {
                txtDisChr.Text = (total * disper / 100).ToString("0") == string.Empty ? "0" : (total * Convert.ToDecimal(txtDisPer.Text) / 100).ToString("0");
            }
            decimal chr = Convert.ToDecimal(txtSvrChr.Text);
            decimal dis = Convert.ToDecimal(txtDisChr.Text);
            decimal adv = Convert.ToDecimal(txtAdvance.Text);
            if (taxper != 0)
            {
                txtTax.Text = (total * taxper / 100).ToString("0") == string.Empty ? "0" : (total * taxper / 100).ToString("0");
            }
            decimal tax = Convert.ToDecimal(txtTax.Text);
            afterLoad = 1;

            lbGrandTotal.Text = (total - dis + tax + chr - adv).ToString("0");

        }

        void loadItem()
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

                    pnItem.Width = 120;
                    pnItem.Height = 135;
                    pnItem.Cursor = Cursors.Hand;
                                       
                    btnItem.Height = 120;
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

        private void BtnItem_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectItem = clickedbutton.Tag.ToString();
            int price = getItemPrice(selectItem);
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("insert into saledet_tmp(id,srno,itemid,price,billno,cost) values(@id,ifnull((select max(srno)+1 from saledet_tmp where id like @id),1),@itemid,@price,@billno,ifnull((select cost from items where id like @itemid),0))", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@itemid", selectItem);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@billno", cboBillNo.SelectedValue == null ? 1 : cboBillNo.SelectedValue);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {                         
                db.cn.Close();
            }
            loadSaleDetail();
            calculateGrand();          
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void loadCategory()
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
                    btnCategory.Width = 120;
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

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (afterLoad == 1)
                {
                    db.cn.Open();
                    DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cmd = new SQLiteCommand("update salehead_tmp set date=@date where id like @id", db.cn);
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

        private void txtInvoice_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (afterLoad == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead_tmp set invno=@invno where id like @id", db.cn);
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

        private void txtRemark_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (afterLoad == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead_tmp set remark=@remark where id like @id", db.cn);
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
                if (afterLoad == 1)
                {
                    string cid = "", cname = "";

                    db.cn.Open();
                    cmd = new SQLiteCommand("select id,name from customer where memcard like @memcard", db.cn);
                    cmd.Parameters.AddWithValue("@memcard", txtMemCard.Text);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cid = dr["id"].ToString();
                        cname = dr["name"].ToString();
                    }
                    dr.Close();
                    db.cn.Close();

                    if (cid != "")
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("update salehead_tmp set custid=@custid,memcard=@memcard where id like @id", db.cn);
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.Parameters.AddWithValue("@custid", cid);
                        cmd.Parameters.AddWithValue("@memcard", txtMemCard.Text);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        cboCustomer.Text = cname;
                        cboCustomer.SelectedItem = cname;
                    }
                }
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
                if (afterLoad == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead_tmp set custid=@custid where id like @id", db.cn);
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

        private void cboCustomer_DropDown(object sender, EventArgs e)
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
                            loadSaleDetail();
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

        void cancelItem()
        {
            try
            {
                string sr = "0", qty = "0";
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
                            cmd = new SQLiteCommand("delete from saledet_tmp where id like @id and srno like @sr", db.cn);
                            cmd.Parameters.AddWithValue("@id", invid);
                            cmd.Parameters.AddWithValue("@sr", sr);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                    }
                    loadSaleDetail();
                    calculateGrand();
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        void updatenet()
        {
            try
            {
                if (afterLoad == 1)
                {                   
                    db.cn.Open();
                    cmd = new SQLiteCommand("update splitbill_tmp set totalQty=@qty,totalAmount=@amt,svrchr=@chr,taxper =@taxper,taxchr=@taxchr,disper=@disper,dischr=@dischr,grandtotal=@grand,advance=@advance,balance=@balance,svrper=@svrper where id like @id and billno like @billno", db.cn);
                    cmd.Parameters.AddWithValue("@qty", lbTotalQty.Text);
                    cmd.Parameters.AddWithValue("@amt", lbTotalAmount.Text);
                    cmd.Parameters.AddWithValue("@chr", txtSvrChr.Text);
                    cmd.Parameters.AddWithValue("@taxper", taxper);
                    cmd.Parameters.AddWithValue("@svrper", svrper);
                    cmd.Parameters.AddWithValue("@taxchr", txtTax.Text);
                    cmd.Parameters.AddWithValue("@disper", txtDisPer.Text);
                    cmd.Parameters.AddWithValue("@dischr", txtDisChr.Text);
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

        private void lbGrandTotal_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lbGrandTotal.Text) < 0)
            {
                afterLoad = 0;
                txtAdvance.Text = "0";             
                afterLoad = 1;
                calculateGrand();
            }
            updatenet();
        }

        private void txtDisPer_TextChanged(object sender, EventArgs e)
        {
            if (afterLoad == 1)
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
            var regex = new Regex(@"[^0-9.၀-၉\b]");
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

        private void txtDisChr_TextChanged(object sender, EventArgs e)
        {
			if (afterLoad == 1)
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
                if (result > Convert.ToInt32(lbTotalAmount.Text) )
                {
                    e.Handled = true;
                }
            }
        }

        private void txtDisChr_Leave(object sender, EventArgs e)
        {
            if (txtDisChr.Text.Trim().Length == 0)
            {
                txtDisChr.Text = "0";
            }
            else
            {
                txtDisChr.Text = mte.convert_text(txtDisChr.Text);
            }
        }

        private void txtSvrChr_TextChanged(object sender, EventArgs e)
        {
			if (afterLoad == 1)
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

        private void txtTax_TextChanged(object sender, EventArgs e)
        {
			if (afterLoad == 1)
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

        private void txtSvrChr_Leave(object sender, EventArgs e)
        {
            if (txtSvrChr.Text.Trim().Length == 0)
            {
                txtSvrChr.Text = "0";
            }
            else
            {
                txtSvrChr.Text = mte.convert_text(txtSvrChr.Text);
            }
        }

        private void txtTax_Leave(object sender, EventArgs e)
        {
            if (txtTax.Text.Trim().Length == 0)
            {
                txtTax.Text = "0";
            }
            else
            {
                txtTax.Text = mte.convert_text(txtTax.Text);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell dgc = dataGridView1.CurrentCell;
            if (dgc.ColumnIndex == 4 || dgc.ColumnIndex == 5) //Desired Column
            {
                e.Control.KeyPress += new KeyPressEventHandler(colQty_KeyPress);
            }
        }

        private void txtAdvance_TextChanged(object sender, EventArgs e)
        {
			if (afterLoad == 1)
			{
				decimal tax = Convert.ToDecimal(txtTax.Text);
				decimal chr = Convert.ToDecimal(txtSvrChr.Text);
				decimal total = Convert.ToDecimal(lbTotalAmount.Text);
				decimal dis = Convert.ToDecimal(txtDisChr.Text);
				decimal adv = 0;
				if (txtAdvance.Text != String.Empty)
				{
					adv = Convert.ToDecimal(mte.convert_text(txtAdvance.Text));
				}

				lbGrandTotal.Text = (total - dis + tax + chr - adv).ToString("0");
			}
		}

        private void txtAdvance_Leave(object sender, EventArgs e)
        {
            if (txtAdvance.Text.Trim().Length == 0)
            {
                txtAdvance.Text = "0";
            }
            else
            {
                txtAdvance.Text = mte.convert_text(txtAdvance.Text);
            }
        }

        private void txtAdvance_KeyPress(object sender, KeyPressEventArgs e)
        {
            int bal = Convert.ToInt32(lbTotalAmount.Text) + Convert.ToInt32(txtSvrChr.Text) + Convert.ToInt32(txtTax.Text) - Convert.ToInt32(txtDisChr.Text);

            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            string newText = mte.convert_text( txtAdvance.Text);

            int selectionStart = txtAdvance.SelectionStart;
            int selectionLength = txtAdvance.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));

            if (int.TryParse(newText, out int result))
            {
                if (result > bal)
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

            string newText =mte.convert_text( txtTax.Text);

            int selectionStart = txtTax.SelectionStart;
            int selectionLength = txtTax.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, mte.convert_text(e.KeyChar.ToString()));
                       
       
            if (int.TryParse(newText, out int result))
            {
                if (result >= Convert.ToInt32(lbTotalAmount.Text))
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

        private void cboBillNo_DropDown(object sender, EventArgs e)
        {
            try
            {
                if (invid != "0")
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("select distinct(billno) from saledet_tmp where id like @id order by billno", db.cn);
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
            
        }

        private void cboBillNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            billNo = cboBillNo.SelectedValue.ToString();
            loadRecord();
            loadSaleDetail();
            updatenet();
        }
        int getItemPrice(string id)
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
                        cmd = new SQLiteCommand("update saledet_tmp set foc=@foc,price=@price where id like @id and srno like @sr", db.cn);
                        cmd.Parameters.AddWithValue("@foc", foc);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@amt", price * Convert.ToInt32(qty));
                        cmd.ExecuteNonQuery();
                        db.cn.Close();

                        loadSaleDetail();
                        calculateGrand();
                    }
                }
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void colQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            //allow number, backspace 
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void cboBillNo_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboBillNo.SelectedValue != null)
            {
                billNo = cboBillNo.SelectedValue.ToString();
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cancelItem();
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
                    if (colName == "colTakeAway")
                    {
                        var id = row.Cells["colID"].Value;
                        var sr = row.Cells["colSr"].Value;
                        var ta = row.Cells["colTakeAway"].Value;
                        db.cn.Open();
                        cmd = new SQLiteCommand("update saledet_tmp set takeaway=@ta where id like @id and srno like @sr", db.cn);
                        cmd.Parameters.AddWithValue("@ta", ta);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                    }
                    else if (colName == "colQty")
                    {
                        var cellvalue = dataGridView1[4, e.RowIndex].Value;
                        if (cellvalue == null || cellvalue.ToString() == "0")
                        {
                            cancelItem();
                        }
                        else
                        {
                            db.cn.Open();
                            cmd = new SQLiteCommand("update saledet_tmp set qty=@qty where id like @id and srno like @sr and billno like @billno", db.cn);
                            cmd.Parameters.AddWithValue("@qty", cellvalue.ToString());
                            cmd.Parameters.AddWithValue("@id", dataGridView1[0, e.RowIndex].Value.ToString());
                            cmd.Parameters.AddWithValue("@sr", dataGridView1[1, e.RowIndex].Value.ToString());
                            cmd.Parameters.AddWithValue("@billno", billNo);
                            cmd.ExecuteNonQuery();
                            db.cn.Close();
                        }
                        loadSaleDetail();
                        calculateGrand();
                    }
                    else if (colName == "colPrice")
                    {
                        var cellvalue = dataGridView1[5, e.RowIndex].Value;

                        db.cn.Open();
                        cmd = new SQLiteCommand("update saledet_tmp set price=@price where id like @id and srno like @sr and billno like @billno", db.cn);
                        if (cellvalue == null)
                        {
                            cmd.Parameters.AddWithValue("@price", 0);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@price", cellvalue.ToString());
                        }
                        cmd.Parameters.AddWithValue("@id", dataGridView1[0, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@sr", dataGridView1[1, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@billno", billNo);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        loadSaleDetail();
                    }
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
                cmd = new SQLiteCommand("select * from tables", db.cn);
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

        private void cboTable_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (afterLoad == 1)
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("update salehead_tmp set tableid=@tableid where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@tableid", cboTable.SelectedValue);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Decimal focCost = 0;
                bool HasFoc = false;

                db.cn.Open();
                cmd = new SQLiteCommand("select foc,qty,cost from saledet_tmp where id like @id;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["foc"].ToString() == "1")
                    {
                        focCost += Convert.ToInt32(dr["qty"].ToString()) * Convert.ToDecimal(dr["cost"].ToString());
                        HasFoc = true;
                    }
                }
                dr.Close();
                db.cn.Close();
                string foccmd = "";
                if (HasFoc)
                {
                    foccmd = " insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                   "values (@date,@invno,@id,'FOC',0,@foc);";
                }
                db.cn.Open();
                cmd = new SQLiteCommand("update salehead_tmp set " +
                    "totalQty  = (select sum(totalqty) from splitbill_tmp where id like @id), " +
                    "totalAmount  = (select sum(totalamount) from splitbill_tmp where id like @id), " +
                    "svrchr=(select sum(svrchr) from splitbill_tmp where id like @id), " +
                    "taxper=(select taxper from splitbill_tmp where id like @id limit 1), " +
                    "taxchr=(select sum(taxchr) from splitbill_tmp where id like @id), " +
                    "disper=(select disper from splitbill_tmp where id like @id), " +
                    "dischr=(select sum(dischr) from splitbill_tmp where id like @id), " +
                    "grandtotal=(select sum(grandtotal) from splitbill_tmp where id like @id), " +
                    "advance=(select sum(advance) from splitbill_tmp where id like @id), " +
                    "balance=(select IIF(sum(advance)=0,sum(grandtotal),sum(advance)) from splitbill_tmp where id like @id)," +
                    "foc=@foc,svrper= (select svrper from splitbill_tmp where id like @id limit 1)" +
                    "where id like @id; " +
                 "update accounts set balance=0 where transid like @id and status in('FOC','Sale','Sale Edited','Advance','OrderTakeOut'); "+
                 "insert into accounts(date,descriptions,transid,status,cashinout,balance)" +
                    " values (@date,@invno,@id,'Sale Edited',1," +
                    "(select grandtotal + advance from salehead_tmp where id like @id)); " + foccmd
               , db.cn);
                DateTime dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@invno", txtInvoice.Text);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@foc", focCost);
                cmd.ExecuteNonQuery();

                cmd = new SQLiteCommand("delete from salehead where id like @id; " +
                    "delete from saledet where id like @id; " +
                    "delete from splitbill where id like @id; " +
                    "insert into salehead select * from salehead_tmp; " +
                    "insert into saledet select * from saledet_tmp; " +
                    "insert into splitbill select * from splitbill_tmp;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                MessageBox.Show("Saved successfully!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
              
                db.cn.Close();
            }
            this.Close();
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
                printer.PrintToPrinter(localReport);                
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
        private void BtnCategory_Click(object sender, EventArgs e)
        {
            Button clickedbutton = (Button)sender;
            selectCategory = clickedbutton.Tag.ToString();
            loadItem();
        }
    }
}
