using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SuccessCafePOS
{
    public partial class FormAddPrinter : Form
    {      
        string id = "0";
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;     

        public FormAddPrinter()
        {
            InitializeComponent();
        }

        private void FormAddPrinter_Load(object sender, EventArgs e)
        {
            insert_into_tmp();
            load_data();
        }

        private void insert_into_tmp()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from printer_tmp;" +
                    "insert into printer_tmp select * from printer;", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        // ----------------------------------- Load Data --------------------------------------
        private void load_data()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from printer_tmp", db.cn);
                dr = cmd.ExecuteReader();              
                dataGridView1.Rows.Clear();
                while (dr.Read())
                {                   
                    dataGridView1.Rows.Add(dr["id"], "", dr["name"], dr["printer"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
        }

        // ------------------------------- cboSystemPrinter ---------------------------------
        private void cboSystemPrinter_DropDown(object sender, EventArgs e)
        {
            cboSystemPrinter.Items.Clear();
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                cboSystemPrinter.Items.Add(printer);
            }
        }

        private void cboSystemPrinter_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        // ------------------------- Check is Duplicate ----------------------------
        private bool is_duplicate_name()
        {
            bool isDuplicate = false;
            try
            {
                db.cn.Open();
                if (id == "0")
                {
                    cmd = new SQLiteCommand("select id from printer_tmp where name like @name;", db.cn);
                }
                else
                {
                    cmd = new SQLiteCommand("select id from printer_tmp where name like @name and id != @id;", db.cn);
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtName.Text)) {
                    MessageBox.Show("Please fill printer name!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                    return;
                }
                if (is_duplicate_name())
                {
                    MessageBox.Show("Duplicate printer name!", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtName.Focus();
                }
                else
                {
                    db.cn.Open();
                    if (id == "0")
                    {
                        cmd = new SQLiteCommand("insert into printer_tmp(id,name,printer) " +
                            "values((select ifnull(max(id),0)+1 from printer_tmp),@name,@printer)", db.cn);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("update printer_tmp set name=@name,printer=@printer where id like @id", db.cn);
                    }
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@printer", cboSystemPrinter.Text);
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

        private void clear()
        {
            id = "0";
            txtName.Clear();
            cboSystemPrinter.Items.Clear();
            cboSystemPrinter.Text = "";
        }

        // ------------------------ Check is Used ----------------------------------
        private bool check_isUsed(string id)
        {           
            bool isUsed = false;
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("select * from items where printer like @id", db.cn);
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    isUsed = true;
                }
                dr.Close();
                db.cn.Close();
            }
            catch
            {
                db.cn.Close();
            }
            return isUsed;
        }

        // ------------------------- Edit/Delete Data ------------------------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row != null)
            {
                if (colName == "colEdit")
                {
                    id = row.Cells["colID"].Value.ToString();
                    txtName.Text = row.Cells["colName"].Value.ToString();
                    cboSystemPrinter.SelectedItem = cboSystemPrinter.Text = row.Cells["colPrinter"].Value.ToString();
                }
                else if (colName == "colDelete")
                {
                    delete_data(dataGridView1);                    
                }
            }
        }                   

        private void delete_data(DataGridView d)
        {
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            id = row.Cells["colID"].Value.ToString();

            if (check_isUsed(id))
            {
                MessageBox.Show("You can not delete used Printer.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete Printer?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    db.cn.Open();
                    cmd = new SQLiteCommand("delete from printer_tmp where id like @id", db.cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                    dataGridView1.Rows.Remove(row);
                }
                catch
                {
                    db.cn.Close();
                }
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

        // ------------------------------ Save & Close -----------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from printer;" +
                    "insert into printer select * from printer_tmp;", db.cn);
                cmd.ExecuteNonQuery();
                db.cn.Close();
                this.Close();
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
    }
}
