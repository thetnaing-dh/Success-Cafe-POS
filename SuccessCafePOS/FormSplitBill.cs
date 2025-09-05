using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormSplitBill : Form
    {
        public string invid { get; set; }
        public string frombillno { get; set; }

        DBConnection db = new DBConnection();
        SQLiteCommand cmd,cmd2;
        SQLiteDataReader dr;

        public FormSplitBill()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void loadFromGrid()
        {
            try
            {
                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,takeaway,printed,cancel,sd.remark,sd.foc from Saledet_tmp sd left join items it on sd.itemid=it.id where sd.id like @invid and qty > 0 and billno like @billno and not(printed= 1 and cancel =1) order by srno", db.cn);
                cmd.Parameters.AddWithValue("@invid", invid);
                cmd.Parameters.AddWithValue("billno", frombillno);
                dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(dr["id"], dr["srno"], i, dr["name"], dr["qty"], dr["printed"], dr["takeaway"],dr["foc"], dr["remark"]);
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
                cmd = new SQLiteCommand("select sd.id,srno,name,qty,price,takeaway,printed,cancel,sd.foc,sd.remark from Saledet_tmp sd left join items it on sd.itemid=it.id where sd.id like @invid and qty > 0 and billno like @billno and not(printed= 1 and cancel =1) ", db.cn);
                cmd.Parameters.AddWithValue("@invid", invid);
                cmd.Parameters.AddWithValue("billno", lbToBillNo.Text);
                dr = cmd.ExecuteReader();
                int j = 0;
                while (dr.Read())
                {
                    j++;
                    dataGridView2.Rows.Add(dr["id"], dr["srno"], j, dr["name"], dr["qty"], dr["printed"], dr["takeaway"], dr["foc"], dr["remark"]);
                }
                dr.Close();
               db.cn.Close();

            }
            catch
            {
                dr.Close();
                db.cn.Close();
            }
        }

        private void FormSplitBill_Load(object sender, EventArgs e)
        {
            lbFromBillNo.Text = frombillno;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from saledet_tmp; " +
                    "insert into saledet_tmp select * from saledet where id like @id; ", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch  
            {
              
                db.cn.Close();
            }
            lbToBillNo.Text = (Convert.ToInt32(frombillno) + 1).ToString();
            loadFromGrid();
            loadToGrid();
        }

        private void btnMoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                string sr = row.Cells["colSr"].Value.ToString();
                string qty = row.Cells["colQty"].Value.ToString();
                string sqlcmd;
                db.cn.Open();
                cmd = new SQLiteCommand("select id from saledet_tmp where id like @id and srno like @sr and billno like @billno", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@sr", sr);
                cmd.Parameters.AddWithValue("@billno", lbToBillNo.Text);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    sqlcmd = "";  
                }
                else
                {
                    sqlcmd = "insert into saledet_tmp select id,srno,itemid,0,price,takeaway,printed,remark,cancel,@tobillno,foc,cost from saledet_tmp where id like @id and srno like @sr and billno like @frombillno; ";
                }
                dr.Close();
                db.cn.Close();


                if (dataGridView1.Rows.Count > 1) {
                    db.cn.Open();
                    cmd = new SQLiteCommand(sqlcmd +
                        "update saledet_tmp set qty = qty - @qty where id like @id and srno like @sr and billno like @frombillno; " +
                        "update saledet_tmp set qty = qty + @qty   where id like @id and srno like @sr and billno like @tobillno; ", db.cn);
                    cmd.Parameters.AddWithValue("@tobillno", lbToBillNo.Text);
                    cmd.Parameters.AddWithValue("@sr", sr);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@frombillno", lbFromBillNo.Text);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();

                    loadFromGrid();
                    loadToGrid();
                }               
            }
            catch   
            {
              
                db.cn.Close();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView2.Columns[e.ColumnIndex].Name;
            if (colName == "colDelete")
            {
                try
                {
                    DataGridViewRow row = dataGridView2.CurrentRow;
                    string sr = row.Cells["colSr2"].Value.ToString();
                    string qty = row.Cells["colQty2"].Value.ToString();

                    string sqlcmd;
                    db.cn.Open();
                    cmd = new SQLiteCommand("select id from saledet_tmp where id like @id and srno like @sr and billno like @billno", db.cn);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@sr", sr);
                    cmd.Parameters.AddWithValue("@billno", lbFromBillNo.Text);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        sqlcmd = "";
                    }
                    else
                    {
                        sqlcmd = "insert into saledet_tmp select id,srno,itemid,0,price,takeaway,printed,remark,cancel,@frombillno,foc,cost from saledet_tmp where id like @id and srno like @sr and billno like @tobillno; ";
                    }
                    dr.Close();
                    db.cn.Close();



                    db.cn.Open();
                    cmd = new SQLiteCommand(sqlcmd + "update saledet_tmp set qty = qty - @qty where id like @id and srno like @sr and billno like @tobillno; " +
                        "update saledet_tmp set qty = qty + @qty where id like @id and srno like @sr and billno like @frombillno; ", db.cn);
                    cmd.Parameters.AddWithValue("@tobillno", lbToBillNo.Text);
                    cmd.Parameters.AddWithValue("@sr", sr);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@frombillno", lbFromBillNo.Text);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();

                    loadFromGrid();
                    loadToGrid();
                }
                catch   
                {
                  
                    db.cn.Close();
                }
            }
        }
       
        private void btnMoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                string sr = row.Cells["colSr"].Value.ToString();
                string qty = row.Cells["colQty"].Value.ToString();
                string sqlcmd;
                db.cn.Open();
                cmd = new SQLiteCommand("select id from saledet_tmp where id like @id and srno like @sr and billno like @billno", db.cn);
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@sr", sr);
                cmd.Parameters.AddWithValue("@billno", lbToBillNo.Text);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    sqlcmd = "";
                }
                else
                {
                    sqlcmd = "insert into saledet_tmp select id,srno,itemid,0,price,takeaway,printed,remark,cancel,@tobillno,foc,cost from saledet_tmp where id like @id and srno like @sr and billno like @frombillno; ";
                }
                dr.Close();
                db.cn.Close();


                if (dataGridView1.Rows.Count == 1 && qty == "1")
                {
                    return;
                }
                else 
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand(sqlcmd +
                        "update saledet_tmp set qty = qty - 1 where id like @id and srno like @sr and billno like @frombillno; " +
                        "update saledet_tmp set qty = qty + 1   where id like @id and srno like @sr and billno like @tobillno; ", db.cn);
                    cmd.Parameters.AddWithValue("@tobillno", lbToBillNo.Text);
                    cmd.Parameters.AddWithValue("@sr", sr);
                    cmd.Parameters.AddWithValue("@id", invid);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@frombillno", lbFromBillNo.Text);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();

                    loadToGrid();

                    if (qty == "1")
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                    else
                    {
                        row.Cells["colQty"].Value = Convert.ToInt32(qty) - 1;
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
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            db.cn.Open();           

            cmd = new SQLiteCommand("select id from splitbill where id like @id and billno like @billno", db.cn);
            cmd.Parameters.AddWithValue("@id", invid);
            cmd.Parameters.AddWithValue("@billno", lbToBillNo.Text);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows) {
                cmd2 = new SQLiteCommand("delete from saledet where id like @id; " +
                    "delete from saledet_tmp where qty <= 0; " +
                    "insert into saledet select * from saledet_tmp; ", db.cn);
                cmd2.Parameters.AddWithValue("@id", invid);
                cmd2.Parameters.AddWithValue("@billno", lbToBillNo.Text);
                cmd2.ExecuteNonQuery();
            }
            else
            {
                cmd2 = new SQLiteCommand("delete from saledet where id like @id; " +
                "delete from saledet_tmp where qty <= 0; " +
                "insert into saledet select * from saledet_tmp; " +
                "insert into splitbill(id,billno) values(@id,@billno);  ", db.cn); 
                cmd2.Parameters.AddWithValue("@id", invid);
                cmd2.Parameters.AddWithValue("@billno", lbToBillNo.Text);
                cmd2.ExecuteNonQuery();
            }
            dr.Close();

            cmd = new SQLiteCommand("select id,billno from splitbill where id like @id", db.cn);
            cmd.Parameters.AddWithValue("@id", invid);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cmd2 = new SQLiteCommand("update splitbill set totalQty=(select sum(qty) from saledet where id like @id and billno like @billno)," +
                    "totalAmount=(select sum(qty*price) from saledet where id like @id and billno like @billno)," +
                    "grandtotal=(select sum(qty*price) from saledet where id like @id and billno like @billno)," +
                    "balance=(select sum(qty*price) from saledet where id like @id and billno like @billno) where id like @id and billno like @billno;", db.cn);
                cmd2.Parameters.AddWithValue("@id", invid);
                cmd2.Parameters.AddWithValue("@billno", dr["billno"]);
                cmd2.ExecuteNonQuery();
            }
            db.cn.Close();     

            

            this.Close();
        }
    }
}
