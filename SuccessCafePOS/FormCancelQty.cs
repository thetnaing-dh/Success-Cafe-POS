using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormCancelQty: Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public string invid {  get; set; }

        public string srno { get; set; }

        public string qty {  get; set; }

        public FormCancelQty()
        {
            InitializeComponent();
        }

        private void FormCancelQty_Load(object sender, EventArgs e)
        {
            txtQty.Text = qty;
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }     
            
            string newText = txtQty.Text;
            
            int selectionStart = txtQty.SelectionStart;
            int selectionLength =  txtQty.SelectionLength;

            newText = newText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart,e.KeyChar.ToString());

            if (int.TryParse(newText, out int result))
            {
                if (result > Convert.ToInt32(qty)) {
                    e.Handled = true;
                }
            }
        }

        private void save()
        {
            try
            {
                db.cn.Open();
                if (qty == txtQty.Text) {
                    cmd = new SQLiteCommand("update saledet set printed = 0, cancel = 1 where id like @id and srno like @sr; ", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("update saledet set qty = qty - @cancelqty where id like @id and srno like @sr; " +
                  "CREATE TEMPORARY TABLE tmp AS select id,(select (max(srno)+1) from saledet where id like @id),itemid,@cancelqty," +
                  "price,takeaway,0,remark,1,billno,foc,cost from saledet where id like @id and srno like @sr; " +
                  "Insert into saledet select * from tmp;" +
                  "drop table tmp;", db.cn);
                }
                cmd.Parameters.AddWithValue("@cancelqty", Convert.ToInt32(txtQty.Text));
                cmd.Parameters.AddWithValue("@id", invid);
                cmd.Parameters.AddWithValue("@sr", srno);
                cmd.ExecuteNonQuery();             

                db.cn.Close();
            }
            catch
            {             
                db.cn.Close();
            }
        }

        private void btnItemSave_Click(object sender, EventArgs e)
        {
            save();
            this.Close();
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                save();
                this.Close();
            }
        }
    }
}
