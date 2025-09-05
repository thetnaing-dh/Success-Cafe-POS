using SuccessCafePOS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuccessCafePOS
{
    public partial class FormItemRemark : Form
    {
        DBConnection db = new DBConnection();
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FormItemRemark()
        {
            InitializeComponent();
        }

        private void FormItemRemark_Load(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[0];
            row.Height = 30;
            insert_into_tmp();
            load_data();
        }

        private void insert_into_tmp()
        {
            try
            {
                db.cn.Open();
                cmd = new SQLiteCommand("delete from remarks_tmp;" +
                    "insert into remarks_tmp select * from remarks;", db.cn);
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
                dataGridView1.Rows.Clear();
                db.cn.Open();
                cmd = new SQLiteCommand("select id,remark from remarks_tmp;", db.cn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dataGridView1.Rows.Add(dr["id"], dr["remark"]);
                }
                dr.Close();
                db.cn.Close();
            }
            catch 
            {                
                db.cn.Close();
            }
        }  

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            object O = Resources.ResourceManager.GetObject("delete");
            Image image = (Image)O;

            if (e.ColumnIndex == 2 && e.Value != null)
            {
                e.Value = image;
            }
        }

        // ------------------------------- Insert/Update Data ---------------------------------
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                if (row != null)
                {
                    var id = row.Cells["colID"].Value;
                    var name = row.Cells["colName"].Value;

                    if (name == null)
                    {
                        return;
                    }

                    // Check White Space
                    if (String.IsNullOrWhiteSpace(name.ToString()) == true)
                    {
                        MessageBox.Show("Please enter a remark.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        load_data();
                        return;
                    }

                    // ------------------------------------- Check Duplicate ----------------------------------------
                    var isDuplicate = false;
                    db.cn.Open();

                    // Check For Insert
                    if (id == null)
                    {
                        cmd = new SQLiteCommand("select id from remarks_tmp where remark like @name;", db.cn);
                        cmd.Parameters.AddWithValue("@name", name);
                        dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            isDuplicate = true;
                        }
                        dr.Close();
                    }

                    //Check For Update
                    else
                    {
                        cmd = new SQLiteCommand("select id from remarks_tmp where remark like @name and id <> @id;", db.cn);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@id", id);
                        dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            isDuplicate = true;
                        }
                        dr.Close();
                    }
                    db.cn.Close();               

                    if (isDuplicate)
                    {
                        MessageBox.Show("This remark already exists.", "Success Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        load_data();
                        return;
                    }

                    // -------------------------------------- End Check Duplicate -------------------------------------------

                    db.cn.Open();
                    // -------------------------------------- Insert Data ------------------------------------------
                    if (id == null)
                    {
                        cmd = new SQLiteCommand("insert into remarks_tmp(id,remark) values((select ifnull(max(id),0)+1 from remarks_tmp),@name);", db.cn);
                    }
                    // -------------------------------------- Update Data ------------------------------------------
                    else
                    {
                        cmd = new SQLiteCommand("update remarks_tmp set remark=@name where id = @id;", db.cn);
                        cmd.Parameters.AddWithValue("@id", id);
                    }
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                    db.cn.Close();
                }
            }
            catch 
            {               
                db.cn.Close();
            }
        }

        // ----------------------------------- Delete Data -------------------------------------
        private void delete_data(DataGridView d)
        {
            try
            {
                DataGridViewRow row = d.SelectedRows[0];
                if (row != null)
                {                  
                    var name = row.Cells["colName"].Value;

                    if (name == null) { return; }

                    if (MessageBox.Show("Are you sure to delete this remark?", "Success Cafe POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {                      
                        db.cn.Open();
                        cmd = new SQLiteCommand("delete from remarks_tmp where remark like @name;", db.cn);                      
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.ExecuteNonQuery();
                        db.cn.Close();                       
                        d.Rows.Remove(row);
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
            if (colName == "colDelete")
            {
                delete_data(dataGridView1);
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {          
            delete_data(dataGridView1);
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            load_data();
        }


        // ------------------------------ Save & Close -----------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {               
                db.cn.Open();
                cmd = new SQLiteCommand("delete from remarks;" +
                    "insert into remarks select * from remarks_tmp;", db.cn);
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
