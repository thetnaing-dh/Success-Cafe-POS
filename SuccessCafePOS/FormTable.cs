using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace SuccessCafePOS
{
    public partial class FormTable : Form
    {
        string id = "0";
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        DataSet ds;

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

        public FormTable()
        {
            InitializeComponent();
        }

        private void FormTable_Load(object sender, EventArgs e)
        {
            load_data();
        }

        // -------------------- Table Group ---------------------------------
        private void load_group()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select id,name from TableGroup order by name;", db.cn);
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

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            FormTableGroup frm = new FormTableGroup();
            frm.ShowDialog();
        }

        private void cboTableGroup_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cboTableGroup_Enter(object sender, EventArgs e)
        {
            load_group();
        }

        // -------------------------- Load Data -----------------------------            
        private void load_data()
        {
            try
            {
                dataGridView1.Rows.Clear();
                db.cn.Open();
                if (cboTableGroup.SelectedIndex >= 0)
                {
                    cmd = new SQLiteCommand("select t.id,t.srno,t.name,g.name gpname,t.takeaway,inactive from Tables t " +
                       "left join TableGroup g on t.groupid=g.id where groupid like @id order by g.name,t.srno,t.name;", db.cn);
                    cmd.Parameters.AddWithValue("@id", cboTableGroup.SelectedValue);
                }
                else
                {
                    cmd = new SQLiteCommand("select t.id,t.srno,t.name,g.name gpname,t.takeaway,inactive from Tables t " +
                        "left join TableGroup g on t.groupid=g.id order by g.name,t.srno,t.name;", db.cn);
                }
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataGridView1.Rows.Add(dr["id"], "", dr["srno"], dr["name"], dr["gpname"], dr["takeaway"], dr["inactive"]);
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
                    cmd = new SQLiteCommand("select id from tables where srno like @srno;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from tables where srno like @srno and id != @id;", db.cn);
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
                    cmd = new SQLiteCommand("select id from tables where name like @name;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from tables where name like @name and id != @id;", db.cn);
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
                if (cboTableGroup.SelectedValue == null)
                {
                    MessageBox.Show("Please select a table group.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    load_group();
                    cboTableGroup.Focus();
                    return;
                }

                if (is_duplicate_id() && !String.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Duplicate table ID!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtID.Focus();
                    return;
                }

                if (String.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please fill table name.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                    return;
                }

                if (is_duplicate_name())
                {
                    MessageBox.Show("Duplicate table name!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                }
                else
                {
                    db.cn.Open();
                    if (id == "0")
                    {
                        cmd = new SQLiteCommand("INSERT INTO Tables(srno,name,groupid,takeaway,inactive) VALUES(@srno,@name,@groupid,@takeaway,@inactive)", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("UPDATE Tables SET srno=@srno,name=@name,groupid=@groupid,takeaway=@takeaway,inactive=@inactive WHERE id = @id", db.cn);
                    }                  
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@srno", txtID.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@groupid", cboTableGroup.SelectedValue);
                    cmd.Parameters.AddWithValue("@takeaway", cbTakeAway.Checked);
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

                    if (MessageBox.Show("Are you sure to delete this table?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.cn.Open();
                        cmd = new SQLiteCommand("delete from Tables where id = @id;", db.cn);
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
                    cboTableGroup.SelectedItem = cboTableGroup.Text = row.Cells["colGroup"].Value.ToString();
                    id = row.Cells["colID"].Value.ToString();
                    txtID.Text = row.Cells["colTableID"].Value.ToString();
                    txtName.Text = row.Cells["colName"].Value.ToString();
                    cbTakeAway.Checked = Convert.ToBoolean(row.Cells["colTakeAway"].Value);
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

        // ------------------------ Clear Fields -----------------------------
        private void clear()
        {
            id = "0";
            txtID.Clear();
            txtName.Clear();
            cbTakeAway.Checked = false;
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

        private void cboTableGroup_SelectedIndexChanged(object sender, EventArgs e)
        {          
            load_data();
        }
    }
}
