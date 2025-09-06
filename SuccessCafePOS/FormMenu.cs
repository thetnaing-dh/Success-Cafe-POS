using SuccessCafePOS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SuccessCafePOS
{
    public partial class FormMenu : Form
    {
        string id = "0";
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        DataSet ds; 
        int   ItemId = 0;
        byte[] img;
        MyanmartoEnglish mte = new MyanmartoEnglish();
        MemoryStream ms = new MemoryStream();  

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

        public FormMenu()
        {
            InitializeComponent();
        }                   

        private void FormMenu_Load(object sender, EventArgs e)
        {           
            load_data();
        }

        // -------------------------  Category -----------------------------
        private void load_group()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from Category order by name;", db.cn);
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

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            FormCategory frm = new FormCategory();
            frm.ShowDialog();
        }

        private void cboCategory_Enter(object sender, EventArgs e)
        {
            load_group();
        }

        private void cboCategory_KeyPress(object sender, KeyPressEventArgs e)
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

        // ------------------------- Price & Cost --------------------------
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9၀-၉\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        // ---------------------- Image ---------------------------
        private void pbImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image (*.JPG;*.PNG,*.GIF|*.jpg;*.png;*.gif";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                ms = new MemoryStream();
                pbImage.Image = Image.FromFile(opf.FileName);
                pbImage.Image.Save(ms, pbImage.Image.RawFormat);
                img = ms.ToArray();
                pbDelete.Show();
            }
        }

        private void pbDelete_Click(object sender, EventArgs e)
        {
            img = null;
            pbImage.Image = (Image)Properties.Resources.ResourceManager.GetObject("browse");
            pbDelete.Hide();
        }   

        // ------------------------- Kitchen Printer -----------------------
        private void load_printer()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from printer order by name", db.cn);
                da = new SQLiteDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cboPrinter.DataSource = ds.Tables[0];
                cboPrinter.DisplayMember = "name";
                cboPrinter.ValueMember = "id";
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        private void cboPrinter_Enter(object sender, EventArgs e)
        {
            load_printer();
        }

        private void cboPrinter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                cboPrinter.DataSource = null;
            }
        }

        private void btnAddPrinter_Click(object sender, EventArgs e)
        {
            FormAddPrinter frm = new FormAddPrinter();
            frm.ShowDialog();
        }

        // ----------------------- Item Remark -----------------------
        private void btnRemark_Click(object sender, EventArgs e)
        {
            FormItemRemark frm = new FormItemRemark();
            frm.ShowDialog();
        }

        // -------------------------- Load Data -----------------------------            
        private void load_data()
        {
            try
            {
                dataGridView1.Rows.Clear();
                db.cn.Open();
                if (cboCategory.SelectedIndex >= 0)
                {
                    cmd = new SQLiteCommand("select i.id,code,i.name,c.name catename,saleprice,cost,inactive,i.image,printer from Items i " +
                   "left join Category c on i.categoryid=c.id where categoryid like @id order by c.name,code,i.name;", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboCategory.SelectedValue);
                }
                else
                {
                    cmd = new SQLiteCommand("select i.id,code,i.name,c.name catename,saleprice,cost,inactive,i.image,printer from Items i " +
                                          "left join Category c on i.categoryid=c.id order by c.name,code,i.name;", db.cn);
                }                  
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataGridView1.Rows.Add(dr["id"], "", dr["code"], dr["name"], dr["catename"], dr["saleprice"], dr["cost"], dr["inactive"], dr["image"], dr["printer"]);
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

        // -------------------------- Check Duplicate -------------------------------
        private bool is_duplicate_id()
        {
            bool isDuplicate = false;
            try
            {
                db.cn.Open();
                if (id == "0")
                {
                    cmd = new SQLiteCommand("select id from items where code like @code;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from items where code like @code and id != @id;", db.cn);
                }
                cmd.Parameters.AddWithValue("@code", txtID.Text);
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
                    cmd = new SQLiteCommand("select id from items where name like @name;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from items where name like @name and id != @id;", db.cn);
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

        // ------------------------------- Insert/Update Data ---------------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCategory.SelectedValue == null)
                {
                    MessageBox.Show("Please select a category.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    load_group();
                    cboCategory.Focus();
                    return;
                }

                if (is_duplicate_id() && !String.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Duplicate item ID!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtID.Focus();
                    return;
                }

                if (String.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please fill item name.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                    return;
                }                              

                if (is_duplicate_name())
                {
                    MessageBox.Show("Duplicate item name!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                }
                else
                {
                    string price = mte.convert_text(txtPrice.Text);
                    string cost = mte.convert_text(txtCost.Text);
                    db.cn.Open();
                    if (id == "0")
                    {
                        cmd = new SQLiteCommand("INSERT INTO Items(code,name,categoryid,saleprice,cost,image,inactive,printer) VALUES(@code,@name,@categoryid,@saleprice,@cost,@image,@inactive,@printer)", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("UPDATE Items SET code=@code,name=@name,categoryid=@categoryid,saleprice=@saleprice,cost=@cost,image=@image,inactive=@inactive,printer=@printer WHERE id LIKE @id", db.cn);
                    }
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@code", txtID.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@categoryid", cboCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@saleprice", price);
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@image", img);
                    cmd.Parameters.AddWithValue("@inactive", cbInActive.Checked);
                    cmd.Parameters.AddWithValue("@printer", cboPrinter.SelectedValue);
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

        // ------------------------- Row Number -------------------------------
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "sr")
            {
                e.Value = e.RowIndex + 1;
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

                    if (MessageBox.Show("Are you sure to delete this item?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("delete from items where id = @id;", db.cn);
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
                if (row != null)
                {                    
                    id = row.Cells["colID"].Value.ToString();
                    txtID.Text = row.Cells["colItemID"].Value.ToString();
                    txtName.Text = row.Cells["colName"].Value.ToString();
                    cboCategory.SelectedItem = cboCategory.Text = row.Cells["colGroup"].Value.ToString();
                    txtPrice.Text = row.Cells["colPrice"].Value.ToString();
                    txtCost.Text = row.Cells["colCost"].Value.ToString();
                    cbInActive.Checked = Convert.ToBoolean(row.Cells["colInActive"].Value);
                    // ------------------------------- Load Image ---------------------------------------
                    if (row.Cells["colImage"].Value != DBNull.Value)
                    {
                        img = (byte[])row.Cells["colImage"].Value;
                        ms = new MemoryStream(img);
                        Image image = Image.FromStream(ms);
                        pbImage.Image = image;
                        pbDelete.Show();
                    }
                    else
                    {
                        img = null;
                        pbImage.Image = (Image)Properties.Resources.ResourceManager.GetObject("browse");
                        pbDelete.Hide();
                    }
                    // ----------------------------- Load Kitchen Printer --------------------------------
                    if (row.Cells["colPrinter"].Value != DBNull.Value)
                    {
                        load_printer();
                        cboPrinter.SelectedValue = row.Cells["colPrinter"].Value;
                    }
                    else
                    {
                        cboPrinter.DataSource = null;
                    }
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

        // ------------------------ Clear Fields -----------------------------
        private void clear()
        {
            id = "0";
            txtName.Clear();
            txtID.Clear();
            txtPrice.Clear();
            txtCost.Clear();
            ItemId = 0;
            cbInActive.Checked = false;
            cboPrinter.Text = "";
            pbImage.Image = (Image)Properties.Resources.ResourceManager.GetObject("browse");
            pbDelete.Hide();
            img = null;
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

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_data();   
        }
    }
}
