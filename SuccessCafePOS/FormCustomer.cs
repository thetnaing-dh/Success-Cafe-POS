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
    public partial class FormCustomer: Form
    {
        string id = "0";
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        DataSet ds;
        MyanmartoEnglish mte = new MyanmartoEnglish();
     
        // ------------------- Send Key Tab ------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter) && !(ActiveControl is Button))
            {
                SendKeys.Send("{TAB}");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public FormCustomer()
        {
            InitializeComponent();
        }

        private void FormCustomer_Load(object sender, EventArgs e)
        {
            load_data();
        }

        // --------------------- Customer Group -----------------------
        private void load_group()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from CustGroup order by name", db.cn);
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboGroup.DataSource = ds.Tables[0];
                cboGroup.DisplayMember = "name";
                cboGroup.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {               
                db.cn.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormCustGroup frm = new FormCustGroup();
            frm.ShowDialog();
        }

        private void cboGroup_Enter(object sender, EventArgs e)
        {
            load_group();
        }

        private void cboGroup_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab || e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        // -------------------------- Load Data -----------------------------            
        private void load_data()
        {
            try
            {
                dataGridView1.Rows.Clear();             
                db.cn.Open();
                if (cboGroup.SelectedIndex >= 0)
                {
                    cmd = new SQLiteCommand("select c.id,srno,c.name,phone,address,gp.name gpname,memcard,discount,inactive from customer c " +
                        "left join CustGroup gp on c.groupid = gp.id where groupid like @id order by groupid,srno,c.name;", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboGroup.SelectedValue);
                }
                else 
                {
                    cmd = new SQLiteCommand("select c.id,srno,c.name,phone,address,gp.name gpname,memcard,discount,inactive from customer c " +
                        "left join CustGroup gp on c.groupid = gp.id order by groupid,srno,c.name;", db.cn);
                }
                dr = cmd.ExecuteReader();
                while (dr.Read()) {                    
                    dataGridView1.Rows.Add(dr["id"], "",dr["srno"], dr["name"], dr["phone"], dr["address"], dr["gpname"], dr["memcard"], dr["discount"].ToString() == "0" ? "" : dr["discount"], dr["inactive"]);             
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {             
                db.cn.Close();
            }
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                MessageBox.Show("Please use English Letter and Number only.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handled = true;
            }
        }

        //private void txtMemCard_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    var regex = new Regex(@"[^a-zA-Z0-9\b]");
        //    if (regex.IsMatch(e.KeyChar.ToString()))
        //    {
        //        MessageBox.Show("Please use English Letter and Number only.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        e.Handled = true;
        //    }
        //}

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtMemCard.Text))
            {
                e.Handled = true;
                MessageBox.Show("Please fill member card ID.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMemCard.Focus();
                return;
            }

            var regex = new Regex(@"[^0-9၀-၉.\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && txtDiscount.Text.Contains('.'))
            {
                e.Handled = true;
                return;
            }

            string newText = txtDiscount.Text.Insert(txtDiscount.SelectionStart, e.KeyChar.ToString());
            if (!string.IsNullOrEmpty(newText))
            {
                if (double.TryParse(mte.convert_text(newText), out double value))
                {
                    if (value > 100)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        // -------------------------- Check Duplicate -------------------------------
        private bool is_duplicate_id()
        {
            bool isDuplicate = false;
            try
            {
                db.cn.Open();
                if (id == "0")
                {
                    cmd = new SQLiteCommand("select id from customer where srno like @srno;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from customer where srno like @srno and id != @id;", db.cn);
                }
                cmd.Parameters.AddWithValue("@srno", txtID.Text);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    isDuplicate = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch 
            {              
                db.cn.Close();
            }
            return isDuplicate;
        }

        private bool is_duplicate_name()
        {
            bool isDuplicate = false;
            try
            {
                db.cn.Open();
                if (id == "0")
                {
                    cmd = new SQLiteCommand("select id from customer where name like @name;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from customer where name like @name and id != @id;", db.cn);
                }
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    isDuplicate = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch 
            {               
                db.cn.Close();
            }
            return isDuplicate;
        }

        private bool is_duplicate_member_card()
        {
            bool isDuplicate = false;
            try
            {
                db.cn.Open();
                if (id == "0")
                {
                    cmd = new SQLiteCommand("select id from customer where memcard like @card;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from customer where memcard like @card and id != @id;", db.cn);
                }
                cmd.Parameters.AddWithValue("@card", txtMemCard.Text);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    isDuplicate = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch 
            {               
                db.cn.Close();
            }
            return isDuplicate;
        }

        // ------------------------------- Insert/Update Data ---------------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboGroup.SelectedValue == null)
                {
                    MessageBox.Show("Please select a customer group.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    load_group();
                    cboGroup.Focus();
                    return;
                }

                if (is_duplicate_id() && !String.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Duplicate customer ID!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtID.Focus();
                    return;
                }                                

                if (String.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please fill customer name.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                    return;
                }

                if (is_duplicate_name())
                {
                    MessageBox.Show("Duplicate customer name!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();                   
                    return;
                }

                if (is_duplicate_member_card() && !String.IsNullOrWhiteSpace(txtMemCard.Text))
                {
                    MessageBox.Show("Duplicate member card ID!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMemCard.Focus();
                }
                else
                {
                    string dis = mte.convert_text(txtDiscount.Text);
                    db.cn.Open();
                    if (id == "0")
                    {
                        cmd = new SQLiteCommand("insert into customer(srno,name,groupid,phone,address,memcard,discount,inactive) values(@srno,@name,@groupid,@phone,@address,@memcard,@discount,@inactive)", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("update customer set srno=@srno,name=@name,groupid=@groupid,phone=@phone,address=@address,memcard=@memcard,discount=@discount,inactive=@inactive where id like @id", db.cn);
                    }                 
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@srno", txtID.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@groupid", cboGroup.SelectedValue);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@memcard", txtMemCard.Text);
                    cmd.Parameters.AddWithValue("@discount", dis);
                    cmd.Parameters.AddWithValue("@inactive", cbInActive.Checked);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    
                    load_data();

                    clear();
                }               
            }
            catch
            {
                db.cn.Close();

            }          
        }

        // ------------------------- Edit/Delete Data ------------------------------
        private void delete_data(DataGridView d)
        {
            try
            {
                DataGridViewRow row = d.CurrentRow;
                if (row != null)
                {
                    var id = row.Cells["colID"].Value;

                    if (MessageBox.Show("Are you sure to delete this customer?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("delete from customer where id = @id;", db.cn);
                        cmd.Parameters.AddWithValue("@id", id.ToString());
                        cmd.ExecuteNonQuery();
                        db.cn.Close();
                        d.Rows.Remove(row);
                        clear();
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
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dataGridView1.CurrentRow;            
            if (colName == "colEdit")
            {
                if(row != null) 
                {               
                    cboGroup.SelectedItem = cboGroup.Text = row.Cells["colGroup"].Value.ToString();
                    id = row.Cells["colID"].Value.ToString();
                    txtID.Text = row.Cells["colCustID"].Value.ToString();
                    txtName.Text = row.Cells["colName"].Value.ToString();
                    txtPhone.Text = row.Cells["colPhone"].Value.ToString();                      
                    txtAddress.Text = row.Cells["colAddress"].Value.ToString();
                    txtMemCard.Text = row.Cells["colMemCard"].Value.ToString();
                    txtDiscount.Text = row.Cells["colDiscount"].Value.ToString();
                    cbInActive.Checked = Convert.ToBoolean(row.Cells["colInActive"].Value);
                }            
            }
            else if (colName == "colDelete")
            {
                delete_data(dataGridView1);
            }                          
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                delete_data(dataGridView1);
            }
        }

        // ------------------------- Row Number -------------------------------
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "sr")
            {
                e.Value = e.RowIndex + 1;
            }
        }

        // ------------------------ Clear Fields -----------------------------
        private void clear()
        {
            id = "0";
            txtID.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtMemCard.Clear();
            txtDiscount.Clear();
            cbInActive.Checked = false;
            txtID.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_data();
        }
    }
}
