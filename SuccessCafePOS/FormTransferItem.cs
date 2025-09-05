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
    public partial class FormTransferItem : Form
    {
        public string ftable {  get; set; }
        public string invid { get; set; }
        public string toinvid { get; set; }

        public string totable { get; set; }


        DBConnection db = new DBConnection();
        SQLiteCommand cmd,cmd2;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        DataSet ds;
        public FormTransferItem()
        {
            InitializeComponent();
        }

        void loadFromGrid()
        {
            try
            {
                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand( "select sd.id,srno,itemid,name,qty,price,takeaway,printed,cancel,sd.remark,sd.foc,sd.billno from Saledet_tmp sd left join items it on sd.itemid=it.id where sd.id like @invid and qty > 0 and not(printed= 1 and cancel =1) order by srno; ", db.cn);
                cmd.Parameters.AddWithValue("@invid", invid);
                dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], dr["srno"], i, dr["itemid"], dr["name"], dr["qty"], dr["price"], dr["printed"], dr["cancel"], dr["takeaway"],dr["foc"], dr["remark"], dr["billno"]);
                }
                dr.Close();
                db.cn.Close();

            }
            catch
            {
                db.cn.Close();
            }
        }
        void loadToGrid()
        {
            try
            {      
                dataGridView2.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select sd.id,srno,itemid,name,qty,price,takeaway,printed,cancel,sd.remark,sd.foc from Saledet_tmp sd left join items it on sd.itemid=it.id where sd.id like @invid and not(printed= 1 and cancel =1) order by srno; ", db.cn);
                cmd.Parameters.AddWithValue("@invid", toinvid);  
                dr = cmd.ExecuteReader();
                int j = 0;
                while (dr.Read())
                {
                    j++;
                    dataGridView2.Rows.Add(dr["id"], dr["srno"], j, dr["itemid"] ,dr["name"], dr["qty"], dr["printed"], dr["takeaway"], dr["foc"], dr["remark"]);
                    toinvid = dr["id"].ToString();
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

        private void FormTransferItem_Load(object sender, EventArgs e)
        {
            txtTable.Text = ftable;
            toinvid = invid;
            db.cn.Open();
            cmd = new SQLiteCommand("delete from saledet_tmp; " +
                "insert into saledet_tmp select * from saledet where id like @invid; ", db.cn);
            cmd.Parameters.AddWithValue("@invid", invid);
            cmd.ExecuteNonQuery();
            db.cn.Close();
            loadFromGrid();
        }

        private void cboTable_DropDown(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from tables where name != @name and tblstatus like 'Pending'", db.cn);
                cmd.Parameters.AddWithValue("@name", ftable);
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
                db.cn.Open();
                cmd = new SQLiteCommand("select max(id) from salehead where tableid like @tableid and status like 'Pending';", db.cn);
                cmd.Parameters.AddWithValue("@tableid", cboTable.SelectedValue);
                toinvid = cmd.ExecuteScalar().ToString();
                db.cn.Close();

                db.cn.Open();

                cmd = new SQLiteCommand("select * from saledet_tmp where id like @invid; ", db.cn);
                cmd.Parameters.AddWithValue("@invid", toinvid);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    dr.Close();
                    db.cn.Close();
                    loadToGrid();
                }
                else
                {
                    dr.Close(); 
                    cmd2 = new SQLiteCommand("insert into saledet_tmp select * from saledet where id like @invid; ", db.cn);
                    cmd2.Parameters.AddWithValue("@invid", toinvid);
                    cmd2.ExecuteNonQuery();

                    db.cn.Close();

                    loadToGrid();
                }  
            }
            catch   
            {
               
                db.cn.Close();
            }
        }

        string checkitemhave()
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            string tosr = "";
            string sr = row.Cells["colSr"].Value.ToString();
            string item = row.Cells["colItemid"].Value.ToString();
            string price = row.Cells["colPrice"].Value.ToString();
            string print = row.Cells["colPrinted"].Value.ToString();
            string cancel = row.Cells["colCancel"].Value.ToString();
            string ta = row.Cells["colTakeAway"].Value.ToString();
            string foc = row.Cells["colFOC"].Value.ToString();
            string bill = row.Cells["colBill"].Value.ToString();
            string remark = row.Cells["colRemark"].Value==null?"": row.Cells["colRemark"].Value.ToString();
            int qty = Convert.ToInt32(row.Cells["colQty"].Value.ToString());

            db.cn.Open();
            cmd = new SQLiteCommand("select max(srno)srno from saledet_tmp where id = @id and " +
                "itemid  = @item and price = @price and printed = @print and cancel  = @cancel and" +
                " takeaway  = @ta and foc =@foc and billno=@bill and ifnull(remark,'') like @remark ", db.cn);
            cmd.Parameters.AddWithValue("@id", toinvid);
            cmd.Parameters.AddWithValue("@item", item);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@print", print);
            cmd.Parameters.AddWithValue("@cancel", cancel);
            cmd.Parameters.AddWithValue("@ta", ta);
            cmd.Parameters.AddWithValue("@foc", foc);
            cmd.Parameters.AddWithValue("@bill", bill);
            cmd.Parameters.AddWithValue("@remark", remark);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                tosr = dr["srno"].ToString();
            }

            dr.Close();
            db.cn.Close();

            return tosr;
        }

        private void btnMoveItem_Click(object sender, EventArgs e)
        {
            if (cboTable.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Table!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboTable.Focus();
            }
            else
            {
                try
                {
                    string tosr = checkitemhave();

                    DataGridViewRow row = dataGridView1.CurrentRow;
                    int qty = Convert.ToInt32(row.Cells["colQty"].Value.ToString());
                    string sr = row.Cells["colSr"].Value.ToString();
                    string bill = row.Cells["colBill"].Value.ToString();

                    db.cn.Open();
                                    
                        if (tosr != "")
                        {
                            cmd = new SQLiteCommand("update saledet_tmp set qty = qty - 1 where id like @id and srno like @sr and billno like @bill ; " +
                                "update saledet_tmp set qty = qty + 1 where id like @toid and srno like @tosr and billno like @bill; ", db.cn);
                        }
                        else
                        {                           
                            cmd = new SQLiteCommand("insert into saledet_tmp select @toid,(select ifnull(max(srno)+1,1) from saledet_tmp where id like @toid),itemid,1,price,takeaway,printed,remark,cancel,billno,foc,cost from saledet_tmp where id like @id and srno like @sr and billno like @bill; " +
                                "update saledet_tmp set qty = qty - 1 where id like @id and srno like @sr and billno like @bill", db.cn);
                        }
                        cmd.Parameters.AddWithValue("@id", invid);
                        cmd.Parameters.AddWithValue("@sr", sr);
                        cmd.Parameters.AddWithValue("@tableid", cboTable.SelectedValue);
                        cmd.Parameters.AddWithValue("@toid", toinvid);
                        cmd.Parameters.AddWithValue("@tosr", tosr);
                    cmd.Parameters.AddWithValue("@bill", bill);
                    cmd.ExecuteNonQuery();
                        
                    db.cn.Close();                 
                }
                catch(Exception we)
                {
                    MessageBox.Show(we.Message);
                    db.cn.Close();
                }
                loadFromGrid();
                loadToGrid();
            }
        }

        private void btnMoveAll_Click(object sender, EventArgs e)
        {
            if (cboTable.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Table!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboTable.Focus();
            }
            else
            {
                try
                {
                    string tosr = checkitemhave();

                    DataGridViewRow row = dataGridView1.CurrentRow;
                    string sr = row.Cells["colSr"].Value.ToString();
                    int qty = Convert.ToInt32(row.Cells["colQty"].Value.ToString());
                    string bill = row.Cells["colBill"].Value.ToString();


                    db.cn.Open();

                    if (tosr != "")
                    {
                       cmd = new SQLiteCommand("update saledet_tmp set qty = qty - @qty where id like @id and srno like @sr and billno like @bill; " +
                            "update saledet_tmp set qty = qty + @qty where id like @toid and srno like @tosr and billno like @bill", db.cn);
                    }
                    else
                    {
                       cmd = new SQLiteCommand("update saledet_tmp set id=@toid,srno=(select ifnull(max(srno)+1,1) from saledet_tmp where id like @toid) where id like @id and srno like @sr and billno like @bill;", db.cn);
                    }
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@sr", sr);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@tableid", cboTable.SelectedValue);
                    cmd.Parameters.AddWithValue("@toid", toinvid);
                    cmd.Parameters.AddWithValue("@tosr", tosr);
                    cmd.Parameters.AddWithValue("@bill", bill);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    dataGridView1.Rows.Remove(row);
                }
                catch   
                {
                   
                    db.cn.Close();
                }
                loadFromGrid();
                loadToGrid();
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
                cmd = new SQLiteCommand("delete from saledet where id like @id; select id from saledet_tmp;", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    cmd2 = new SQLiteCommand("delete from saledet where id like @toid", db.cn);
                  
                    cmd2.Parameters.AddWithValue("@toid", dr["id"]);
                    cmd2.ExecuteNonQuery();
                }
                dr.Close();

                cmd = new SQLiteCommand("delete from saledet_tmp where qty <= 0; " +
                    "insert into saledet select * from saledet_tmp;", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();

                this.Close();
            }
            catch(Exception ew)
            {
                MessageBox.Show(ew.Message);
                db.cn.Close();
            }
        }
    }
}
