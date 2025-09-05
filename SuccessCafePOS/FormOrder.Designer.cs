namespace SuccessCafePOS
{
    partial class FormOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrder));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtAdvance = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lbTotalQty = new System.Windows.Forms.Label();
            this.lbTotalAmount = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.lbGrandTotal = new System.Windows.Forms.Label();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtSvrChr = new System.Windows.Forms.TextBox();
            this.txtDisChr = new System.Windows.Forms.TextBox();
            this.txtDisPer = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbTax = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboBillNo = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.cboTable = new System.Windows.Forms.ComboBox();
            this.calendar = new System.Windows.Forms.MonthCalendar();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cboCustomer = new System.Windows.Forms.ComboBox();
            this.btnDate = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtInvoice = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMemCard = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPrintBill = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrinted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCancel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTakeAway = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFOC = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBillno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelTitle.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(47)))), ((int)(((byte)(15)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2, 540);
            this.panel2.TabIndex = 11;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(47)))), ((int)(((byte)(15)))));
            this.panelTitle.Controls.Add(this.label16);
            this.panelTitle.Controls.Add(this.btnClose);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(767, 40);
            this.panelTitle.TabIndex = 10;
            // 
            // label16
            // 
            this.label16.Dock = System.Windows.Forms.DockStyle.Left;
            this.label16.Font = new System.Drawing.Font("Pyidaungsu", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(0, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(239, 40);
            this.label16.TabIndex = 3;
            this.label16.Text = "Take Out Order";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = global::SuccessCafePOS.Properties.Resources.close;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(727, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 0;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(47)))), ((int)(((byte)(15)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(2, 578);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(763, 2);
            this.panel3.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(47)))), ((int)(((byte)(15)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(765, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 540);
            this.panel1.TabIndex = 12;
            // 
            // txtAdvance
            // 
            this.txtAdvance.BackColor = System.Drawing.SystemColors.Info;
            this.txtAdvance.Location = new System.Drawing.Point(334, 481);
            this.txtAdvance.Name = "txtAdvance";
            this.txtAdvance.ReadOnly = true;
            this.txtAdvance.Size = new System.Drawing.Size(99, 33);
            this.txtAdvance.TabIndex = 193;
            this.txtAdvance.Text = "0";
            this.txtAdvance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(265, 484);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 26);
            this.label15.TabIndex = 192;
            this.label15.Text = "Advance";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel8.Location = new System.Drawing.Point(601, 456);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(140, 2);
            this.panel8.TabIndex = 191;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel7.Location = new System.Drawing.Point(408, 456);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(76, 2);
            this.panel7.TabIndex = 190;
            // 
            // lbTotalQty
            // 
            this.lbTotalQty.BackColor = System.Drawing.SystemColors.Info;
            this.lbTotalQty.Font = new System.Drawing.Font("Pyidaungsu", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalQty.Location = new System.Drawing.Point(408, 420);
            this.lbTotalQty.Name = "lbTotalQty";
            this.lbTotalQty.Size = new System.Drawing.Size(76, 35);
            this.lbTotalQty.TabIndex = 189;
            this.lbTotalQty.Text = "0";
            this.lbTotalQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbTotalAmount
            // 
            this.lbTotalAmount.BackColor = System.Drawing.SystemColors.Info;
            this.lbTotalAmount.Font = new System.Drawing.Font("Pyidaungsu", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalAmount.Location = new System.Drawing.Point(600, 420);
            this.lbTotalAmount.Name = "lbTotalAmount";
            this.lbTotalAmount.Size = new System.Drawing.Size(141, 35);
            this.lbTotalAmount.TabIndex = 188;
            this.lbTotalAmount.Text = "0";
            this.lbTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.panel6.Controls.Add(this.panel9);
            this.panel6.Controls.Add(this.lbGrandTotal);
            this.panel6.Location = new System.Drawing.Point(535, 471);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(206, 50);
            this.panel6.TabIndex = 187;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.DodgerBlue;
            this.panel9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel9.Location = new System.Drawing.Point(0, 48);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(206, 2);
            this.panel9.TabIndex = 73;
            // 
            // lbGrandTotal
            // 
            this.lbGrandTotal.Font = new System.Drawing.Font("Pyidaungsu", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGrandTotal.Location = new System.Drawing.Point(3, 5);
            this.lbGrandTotal.Name = "lbGrandTotal";
            this.lbGrandTotal.Size = new System.Drawing.Size(208, 35);
            this.lbGrandTotal.TabIndex = 70;
            this.lbGrandTotal.Text = "0";
            this.lbGrandTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTax
            // 
            this.txtTax.BackColor = System.Drawing.SystemColors.Info;
            this.txtTax.Location = new System.Drawing.Point(268, 420);
            this.txtTax.Name = "txtTax";
            this.txtTax.ReadOnly = true;
            this.txtTax.Size = new System.Drawing.Size(80, 33);
            this.txtTax.TabIndex = 186;
            this.txtTax.Text = "0";
            this.txtTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSvrChr
            // 
            this.txtSvrChr.BackColor = System.Drawing.SystemColors.Info;
            this.txtSvrChr.Location = new System.Drawing.Point(97, 420);
            this.txtSvrChr.Name = "txtSvrChr";
            this.txtSvrChr.ReadOnly = true;
            this.txtSvrChr.Size = new System.Drawing.Size(80, 33);
            this.txtSvrChr.TabIndex = 185;
            this.txtSvrChr.Text = "0";
            this.txtSvrChr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDisChr
            // 
            this.txtDisChr.BackColor = System.Drawing.SystemColors.Info;
            this.txtDisChr.Location = new System.Drawing.Point(160, 481);
            this.txtDisChr.Name = "txtDisChr";
            this.txtDisChr.ReadOnly = true;
            this.txtDisChr.Size = new System.Drawing.Size(80, 33);
            this.txtDisChr.TabIndex = 184;
            this.txtDisChr.Text = "0";
            this.txtDisChr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDisPer
            // 
            this.txtDisPer.BackColor = System.Drawing.SystemColors.Info;
            this.txtDisPer.Location = new System.Drawing.Point(99, 481);
            this.txtDisPer.Name = "txtDisPer";
            this.txtDisPer.ReadOnly = true;
            this.txtDisPer.Size = new System.Drawing.Size(30, 33);
            this.txtDisPer.TabIndex = 183;
            this.txtDisPer.Text = "0";
            this.txtDisPer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(25, 423);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 26);
            this.label13.TabIndex = 182;
            this.label13.Text = "Svr. Chr";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(497, 423);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 26);
            this.label11.TabIndex = 181;
            this.label11.Text = "Total Amount";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Pyidaungsu", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(458, 481);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 30);
            this.label12.TabIndex = 180;
            this.label12.Text = "Balance";
            // 
            // lbTax
            // 
            this.lbTax.AutoSize = true;
            this.lbTax.Location = new System.Drawing.Point(193, 424);
            this.lbTax.Name = "lbTax";
            this.lbTax.Size = new System.Drawing.Size(35, 26);
            this.lbTax.TabIndex = 179;
            this.lbTax.Text = "Tax";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(367, 423);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 26);
            this.label9.TabIndex = 178;
            this.label9.Text = "Qty";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 484);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 26);
            this.label10.TabIndex = 177;
            this.label10.Text = "Discount";
            // 
            // cboBillNo
            // 
            this.cboBillNo.BackColor = System.Drawing.SystemColors.Info;
            this.cboBillNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboBillNo.Font = new System.Drawing.Font("Pyidaungsu", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBillNo.FormattingEnabled = true;
            this.cboBillNo.Location = new System.Drawing.Point(372, 56);
            this.cboBillNo.Name = "cboBillNo";
            this.cboBillNo.Size = new System.Drawing.Size(47, 32);
            this.cboBillNo.TabIndex = 175;
            this.cboBillNo.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Info;
            this.textBox2.Location = new System.Drawing.Point(372, 56);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(47, 33);
            this.textBox2.TabIndex = 176;
            this.textBox2.Visible = false;
            // 
            // cboTable
            // 
            this.cboTable.BackColor = System.Drawing.SystemColors.Info;
            this.cboTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTable.FormattingEnabled = true;
            this.cboTable.Location = new System.Drawing.Point(141, 105);
            this.cboTable.Name = "cboTable";
            this.cboTable.Size = new System.Drawing.Size(172, 34);
            this.cboTable.TabIndex = 159;
            this.cboTable.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboTable_KeyPress);
            // 
            // calendar
            // 
            this.calendar.Location = new System.Drawing.Point(127, 89);
            this.calendar.Name = "calendar";
            this.calendar.TabIndex = 173;
            this.calendar.Visible = false;
            this.calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendar_DateSelected);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::SuccessCafePOS.Properties.Resources.add;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(713, 105);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(35, 36);
            this.btnAdd.TabIndex = 174;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Visible = false;
            // 
            // cboCustomer
            // 
            this.cboCustomer.BackColor = System.Drawing.SystemColors.Info;
            this.cboCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboCustomer.FormattingEnabled = true;
            this.cboCustomer.Location = new System.Drawing.Point(517, 105);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(185, 34);
            this.cboCustomer.TabIndex = 158;
            this.cboCustomer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboTable_KeyPress);
            // 
            // btnDate
            // 
            this.btnDate.BackgroundImage = global::SuccessCafePOS.Properties.Resources.calendar;
            this.btnDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDate.FlatAppearance.BorderSize = 0;
            this.btnDate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDate.Location = new System.Drawing.Point(319, 53);
            this.btnDate.Name = "btnDate";
            this.btnDate.Size = new System.Drawing.Size(35, 36);
            this.btnDate.TabIndex = 172;
            this.btnDate.UseVisualStyleBackColor = true;
            this.btnDate.Click += new System.EventHandler(this.btnDate_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Info;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colSr,
            this.Column1,
            this.colName,
            this.colQty,
            this.colPrice,
            this.colAmount,
            this.colPrinted,
            this.colCancel,
            this.colTakeAway,
            this.colFOC,
            this.colRemark,
            this.colItemID,
            this.colBillno});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Pyidaungsu", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Location = new System.Drawing.Point(27, 209);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(714, 200);
            this.dataGridView1.TabIndex = 171;
            // 
            // txtRemark
            // 
            this.txtRemark.BackColor = System.Drawing.SystemColors.Info;
            this.txtRemark.Location = new System.Drawing.Point(514, 157);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ReadOnly = true;
            this.txtRemark.Size = new System.Drawing.Size(188, 33);
            this.txtRemark.TabIndex = 161;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(448, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 26);
            this.label6.TabIndex = 170;
            this.label6.Text = "Remark";
            // 
            // txtInvoice
            // 
            this.txtInvoice.BackColor = System.Drawing.SystemColors.Info;
            this.txtInvoice.Location = new System.Drawing.Point(514, 58);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.ReadOnly = true;
            this.txtInvoice.Size = new System.Drawing.Size(188, 33);
            this.txtInvoice.TabIndex = 168;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(436, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 26);
            this.label7.TabIndex = 169;
            this.label7.Text = "InvoiceNo.";
            // 
            // txtMemCard
            // 
            this.txtMemCard.BackColor = System.Drawing.SystemColors.Info;
            this.txtMemCard.Location = new System.Drawing.Point(141, 157);
            this.txtMemCard.Name = "txtMemCard";
            this.txtMemCard.ReadOnly = true;
            this.txtMemCard.Size = new System.Drawing.Size(172, 33);
            this.txtMemCard.TabIndex = 160;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 26);
            this.label5.TabIndex = 167;
            this.label5.Text = "Mem.Card";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(439, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 26);
            this.label3.TabIndex = 166;
            this.label3.Text = "Customer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 26);
            this.label2.TabIndex = 164;
            this.label2.Text = "Table";
            // 
            // txtDate
            // 
            this.txtDate.BackColor = System.Drawing.SystemColors.Info;
            this.txtDate.Location = new System.Drawing.Point(141, 56);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(172, 33);
            this.txtDate.TabIndex = 157;
            this.txtDate.TextChanged += new System.EventHandler(this.txtDate_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 26);
            this.label4.TabIndex = 162;
            this.label4.Text = "Date";
            // 
            // btnPrintBill
            // 
            this.btnPrintBill.BackColor = System.Drawing.Color.Transparent;
            this.btnPrintBill.BackgroundImage = global::SuccessCafePOS.Properties.Resources.btnPrint;
            this.btnPrintBill.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrintBill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrintBill.FlatAppearance.BorderSize = 0;
            this.btnPrintBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintBill.Location = new System.Drawing.Point(466, 530);
            this.btnPrintBill.Name = "btnPrintBill";
            this.btnPrintBill.Size = new System.Drawing.Size(120, 40);
            this.btnPrintBill.TabIndex = 156;
            this.btnPrintBill.Text = " ";
            this.btnPrintBill.UseVisualStyleBackColor = false;
            this.btnPrintBill.Click += new System.EventHandler(this.btnPrintBill_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = global::SuccessCafePOS.Properties.Resources.btnCancel;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(620, 530);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 40);
            this.btnCancel.TabIndex = 155;
            this.btnCancel.Text = " ";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImage = global::SuccessCafePOS.Properties.Resources.btnTakeout;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(310, 530);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 40);
            this.btnSave.TabIndex = 154;
            this.btnSave.Text = " ";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // colID
            // 
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false;
            // 
            // colSr
            // 
            this.colSr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSr.HeaderText = "Sr";
            this.colSr.Name = "colSr";
            this.colSr.ReadOnly = true;
            this.colSr.Visible = false;
            this.colSr.Width = 31;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Sr";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 35;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colName.HeaderText = "Item Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 106;
            // 
            // colQty
            // 
            this.colQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colQty.HeaderText = "Qty";
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            this.colQty.Width = 59;
            // 
            // colPrice
            // 
            this.colPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colPrice.HeaderText = "Price";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            this.colPrice.Width = 69;
            // 
            // colAmount
            // 
            this.colAmount.HeaderText = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.ReadOnly = true;
            // 
            // colPrinted
            // 
            this.colPrinted.HeaderText = "Printed";
            this.colPrinted.Name = "colPrinted";
            this.colPrinted.ReadOnly = true;
            this.colPrinted.Visible = false;
            // 
            // colCancel
            // 
            this.colCancel.HeaderText = "Cancel";
            this.colCancel.Name = "colCancel";
            this.colCancel.ReadOnly = true;
            this.colCancel.Visible = false;
            // 
            // colTakeAway
            // 
            this.colTakeAway.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTakeAway.HeaderText = "T.A";
            this.colTakeAway.Name = "colTakeAway";
            this.colTakeAway.ReadOnly = true;
            this.colTakeAway.Width = 38;
            // 
            // colFOC
            // 
            this.colFOC.HeaderText = "FOC";
            this.colFOC.Name = "colFOC";
            this.colFOC.ReadOnly = true;
            this.colFOC.Width = 38;
            // 
            // colRemark
            // 
            this.colRemark.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRemark.HeaderText = "Remarks";
            this.colRemark.Name = "colRemark";
            this.colRemark.ReadOnly = true;
            this.colRemark.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRemark.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colItemID
            // 
            this.colItemID.HeaderText = "ItemID";
            this.colItemID.Name = "colItemID";
            this.colItemID.ReadOnly = true;
            this.colItemID.Visible = false;
            // 
            // colBillno
            // 
            this.colBillno.HeaderText = "billno";
            this.colBillno.Name = "colBillno";
            this.colBillno.ReadOnly = true;
            this.colBillno.Visible = false;
            // 
            // FormOrder
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(205)))), ((int)(((byte)(168)))));
            this.ClientSize = new System.Drawing.Size(767, 580);
            this.Controls.Add(this.calendar);
            this.Controls.Add(this.txtAdvance);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.lbTotalQty);
            this.Controls.Add(this.lbTotalAmount);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.txtTax);
            this.Controls.Add(this.txtSvrChr);
            this.Controls.Add(this.txtDisChr);
            this.Controls.Add(this.txtDisPer);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lbTax);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cboBillNo);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.cboTable);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.btnDate);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtInvoice);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtMemCard);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnPrintBill);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelTitle);
            this.Font = new System.Drawing.Font("Pyidaungsu", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Success Cafe POS";
            this.Load += new System.EventHandler(this.FormOrder_Load);
            this.panelTitle.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtAdvance;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lbTotalQty;
        private System.Windows.Forms.Label lbTotalAmount;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label lbGrandTotal;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtSvrChr;
        private System.Windows.Forms.TextBox txtDisChr;
        private System.Windows.Forms.TextBox txtDisPer;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbTax;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.ComboBox cboBillNo;
        private System.Windows.Forms.TextBox textBox2;
        public System.Windows.Forms.ComboBox cboTable;
        private System.Windows.Forms.MonthCalendar calendar;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cboCustomer;
        private System.Windows.Forms.Button btnDate;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtInvoice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMemCard;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPrintBill;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrinted;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTakeAway;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemark;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBillno;
    }
}