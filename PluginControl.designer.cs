namespace CRMProcessExplorer
{
    partial class PluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbProcess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.gbRecord = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.LinkLabel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.btnRetrieveEntities = new System.Windows.Forms.Button();
            this.lblEntityName = new System.Windows.Forms.Label();
            this.cboEntities = new System.Windows.Forms.ComboBox();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.gbChoice = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbDates = new System.Windows.Forms.RadioButton();
            this.rbRecord = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbPTL = new System.Windows.Forms.RadioButton();
            this.rbRTP = new System.Windows.Forms.RadioButton();
            this.rbBGP = new System.Windows.Forms.RadioButton();
            this.lblInfo = new System.Windows.Forms.Label();
            this.gbDates = new System.Windows.Forms.GroupBox();
            this.lblState = new System.Windows.Forms.Label();
            this.cboState = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblOperationType = new System.Windows.Forms.Label();
            this.cbOperationType = new System.Windows.Forms.ComboBox();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStripMenu.SuspendLayout();
            this.gbRecord.SuspendLayout();
            this.gbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.gbChoice.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbDates.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.toolStripSeparator1,
            this.tsbProcess,
            this.toolStripSeparator2,
            this.tsbAbout});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1236, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(28, 28);
            this.tsbClose.Text = "Close Me";
            this.tsbClose.ToolTipText = "Close Me";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbProcess
            // 
            this.tsbProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbProcess.Image = ((System.Drawing.Image)(resources.GetObject("tsbProcess.Image")));
            this.tsbProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbProcess.Name = "tsbProcess";
            this.tsbProcess.Size = new System.Drawing.Size(28, 28);
            this.tsbProcess.Text = "Retrieve Process";
            this.tsbProcess.Click += new System.EventHandler(this.tsbProcess_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbAbout
            // 
            this.tsbAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAbout.Image = ((System.Drawing.Image)(resources.GetObject("tsbAbout.Image")));
            this.tsbAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Size = new System.Drawing.Size(28, 28);
            this.tsbAbout.Text = "About";
            this.tsbAbout.Click += new System.EventHandler(this.tsbAbout_Click);
            // 
            // gbRecord
            // 
            this.gbRecord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRecord.Controls.Add(this.txtName);
            this.gbRecord.Controls.Add(this.btnSearch);
            this.gbRecord.Controls.Add(this.lblName);
            this.gbRecord.Controls.Add(this.lblID);
            this.gbRecord.Controls.Add(this.txtID);
            this.gbRecord.Controls.Add(this.btnRetrieveEntities);
            this.gbRecord.Controls.Add(this.lblEntityName);
            this.gbRecord.Controls.Add(this.cboEntities);
            this.gbRecord.Location = new System.Drawing.Point(3, 88);
            this.gbRecord.Name = "gbRecord";
            this.gbRecord.Size = new System.Drawing.Size(1230, 76);
            this.gbRecord.TabIndex = 5;
            this.gbRecord.TabStop = false;
            this.gbRecord.Text = "Processes by record";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.AutoSize = true;
            this.txtName.Location = new System.Drawing.Point(618, 51);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(35, 13);
            this.txtName.TabIndex = 21;
            this.txtName.TabStop = true;
            this.txtName.Text = "Name";
            this.txtName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.txtName_LinkClicked);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(1119, 45);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(105, 23);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(573, 52);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 19;
            this.lblName.Text = "Name:";
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(6, 49);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(21, 13);
            this.lblID.TabIndex = 17;
            this.lblID.Text = "ID:";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(137, 46);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(425, 20);
            this.txtID.TabIndex = 16;
            // 
            // btnRetrieveEntities
            // 
            this.btnRetrieveEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRetrieveEntities.Location = new System.Drawing.Point(1119, 17);
            this.btnRetrieveEntities.Name = "btnRetrieveEntities";
            this.btnRetrieveEntities.Size = new System.Drawing.Size(105, 23);
            this.btnRetrieveEntities.TabIndex = 15;
            this.btnRetrieveEntities.Text = "Retrieve Entities";
            this.btnRetrieveEntities.UseVisualStyleBackColor = true;
            this.btnRetrieveEntities.Click += new System.EventHandler(this.btnRetrieveEntities_Click);
            // 
            // lblEntityName
            // 
            this.lblEntityName.AutoSize = true;
            this.lblEntityName.Location = new System.Drawing.Point(6, 22);
            this.lblEntityName.Name = "lblEntityName";
            this.lblEntityName.Size = new System.Drawing.Size(67, 13);
            this.lblEntityName.TabIndex = 14;
            this.lblEntityName.Text = "Entity Name:";
            // 
            // cboEntities
            // 
            this.cboEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEntities.FormattingEnabled = true;
            this.cboEntities.Location = new System.Drawing.Point(137, 19);
            this.cboEntities.Name = "cboEntities";
            this.cboEntities.Size = new System.Drawing.Size(976, 21);
            this.cboEntities.Sorted = true;
            this.cboEntities.TabIndex = 0;
            this.cboEntities.SelectedIndexChanged += new System.EventHandler(this.cboEntities_SelectedIndexChanged);
            // 
            // gbMain
            // 
            this.gbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMain.Controls.Add(this.dgvMain);
            this.gbMain.Location = new System.Drawing.Point(3, 172);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(1230, 308);
            this.gbMain.TabIndex = 6;
            this.gbMain.TabStop = false;
            this.gbMain.Text = "Process Info";
            // 
            // dgvMain
            // 
            this.dgvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Location = new System.Drawing.Point(9, 19);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.Size = new System.Drawing.Size(1215, 283);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMain_CellDoubleClick);
            // 
            // gbChoice
            // 
            this.gbChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbChoice.Controls.Add(this.panel2);
            this.gbChoice.Controls.Add(this.panel1);
            this.gbChoice.Controls.Add(this.lblInfo);
            this.gbChoice.Location = new System.Drawing.Point(3, 34);
            this.gbChoice.Name = "gbChoice";
            this.gbChoice.Size = new System.Drawing.Size(1230, 48);
            this.gbChoice.TabIndex = 7;
            this.gbChoice.TabStop = false;
            this.gbChoice.Text = "Filter";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Controls.Add(this.rbDates);
            this.panel2.Controls.Add(this.rbRecord);
            this.panel2.Location = new System.Drawing.Point(439, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(204, 29);
            this.panel2.TabIndex = 1;
            // 
            // rbDates
            // 
            this.rbDates.AutoSize = true;
            this.rbDates.Location = new System.Drawing.Point(121, 6);
            this.rbDates.Name = "rbDates";
            this.rbDates.Size = new System.Drawing.Size(68, 17);
            this.rbDates.TabIndex = 3;
            this.rbDates.Text = "By Dates";
            this.rbDates.UseVisualStyleBackColor = true;
            this.rbDates.CheckedChanged += new System.EventHandler(this.rbDates_CheckedChanged);
            // 
            // rbRecord
            // 
            this.rbRecord.AutoSize = true;
            this.rbRecord.Checked = true;
            this.rbRecord.Location = new System.Drawing.Point(16, 6);
            this.rbRecord.Name = "rbRecord";
            this.rbRecord.Size = new System.Drawing.Size(75, 17);
            this.rbRecord.TabIndex = 2;
            this.rbRecord.TabStop = true;
            this.rbRecord.Text = "By Record";
            this.rbRecord.UseVisualStyleBackColor = true;
            this.rbRecord.CheckedChanged += new System.EventHandler(this.rbRecord_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Controls.Add(this.rbPTL);
            this.panel1.Controls.Add(this.rbRTP);
            this.panel1.Controls.Add(this.rbBGP);
            this.panel1.Location = new System.Drawing.Point(9, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 29);
            this.panel1.TabIndex = 6;
            // 
            // rbPTL
            // 
            this.rbPTL.AutoSize = true;
            this.rbPTL.Location = new System.Drawing.Point(297, 6);
            this.rbPTL.Name = "rbPTL";
            this.rbPTL.Size = new System.Drawing.Size(114, 17);
            this.rbPTL.TabIndex = 2;
            this.rbPTL.Text = "Plug-in Trace Logs";
            this.rbPTL.UseVisualStyleBackColor = true;
            this.rbPTL.CheckedChanged += new System.EventHandler(this.rbPTL_CheckedChanged);
            // 
            // rbRTP
            // 
            this.rbRTP.AutoSize = true;
            this.rbRTP.Location = new System.Drawing.Point(162, 6);
            this.rbRTP.Name = "rbRTP";
            this.rbRTP.Size = new System.Drawing.Size(110, 17);
            this.rbRTP.TabIndex = 1;
            this.rbRTP.Text = "Real-time Process";
            this.rbRTP.UseVisualStyleBackColor = true;
            this.rbRTP.CheckedChanged += new System.EventHandler(this.rbRTP_CheckedChanged);
            // 
            // rbBGP
            // 
            this.rbBGP.AutoSize = true;
            this.rbBGP.Checked = true;
            this.rbBGP.Location = new System.Drawing.Point(12, 6);
            this.rbBGP.Name = "rbBGP";
            this.rbBGP.Size = new System.Drawing.Size(124, 17);
            this.rbBGP.TabIndex = 0;
            this.rbBGP.TabStop = true;
            this.rbBGP.Text = "Background Process";
            this.rbBGP.UseVisualStyleBackColor = true;
            this.rbBGP.CheckedChanged += new System.EventHandler(this.rbBGP_CheckedChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.ForeColor = System.Drawing.Color.Red;
            this.lblInfo.Location = new System.Drawing.Point(646, 13);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(579, 28);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Text = "Search:";
            this.lblInfo.Visible = false;
            // 
            // gbDates
            // 
            this.gbDates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDates.Controls.Add(this.lblState);
            this.gbDates.Controls.Add(this.cboState);
            this.gbDates.Controls.Add(this.lblStatus);
            this.gbDates.Controls.Add(this.cboStatus);
            this.gbDates.Controls.Add(this.lblOperationType);
            this.gbDates.Controls.Add(this.cbOperationType);
            this.gbDates.Controls.Add(this.dtTo);
            this.gbDates.Controls.Add(this.dtFrom);
            this.gbDates.Controls.Add(this.label2);
            this.gbDates.Controls.Add(this.label3);
            this.gbDates.Location = new System.Drawing.Point(3, 88);
            this.gbDates.Name = "gbDates";
            this.gbDates.Size = new System.Drawing.Size(1230, 76);
            this.gbDates.TabIndex = 6;
            this.gbDates.TabStop = false;
            this.gbDates.Text = "Processes by dates";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(830, 46);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(40, 13);
            this.lblState.TabIndex = 25;
            this.lblState.Text = "Status:";
            // 
            // cboState
            // 
            this.cboState.FormattingEnabled = true;
            this.cboState.Location = new System.Drawing.Point(931, 45);
            this.cboState.Name = "cboState";
            this.cboState.Size = new System.Drawing.Size(200, 21);
            this.cboState.TabIndex = 24;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(790, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(80, 13);
            this.lblStatus.TabIndex = 23;
            this.lblStatus.Text = "Status Reason:";
            // 
            // cboStatus
            // 
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(931, 19);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(200, 21);
            this.cboStatus.TabIndex = 22;
            // 
            // lblOperationType
            // 
            this.lblOperationType.AutoSize = true;
            this.lblOperationType.Location = new System.Drawing.Point(294, 22);
            this.lblOperationType.Name = "lblOperationType";
            this.lblOperationType.Size = new System.Drawing.Size(91, 13);
            this.lblOperationType.TabIndex = 21;
            this.lblOperationType.Text = "System Job Type:";
            // 
            // cbOperationType
            // 
            this.cbOperationType.FormattingEnabled = true;
            this.cbOperationType.Location = new System.Drawing.Point(439, 19);
            this.cbOperationType.Name = "cbOperationType";
            this.cbOperationType.Size = new System.Drawing.Size(314, 21);
            this.cbOperationType.TabIndex = 20;
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(137, 45);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(97, 20);
            this.dtTo.TabIndex = 19;
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(137, 19);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(97, 20);
            this.dtFrom.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "To:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "From:";
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbDates);
            this.Controls.Add(this.gbChoice);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.gbRecord);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "PluginControl";
            this.Size = new System.Drawing.Size(1236, 483);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.gbRecord.ResumeLayout(false);
            this.gbRecord.PerformLayout();
            this.gbMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.gbChoice.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbDates.ResumeLayout(false);
            this.gbDates.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.GroupBox gbRecord;
        private System.Windows.Forms.Label lblEntityName;
        private System.Windows.Forms.ComboBox cboEntities;
        private System.Windows.Forms.Button btnRetrieveEntities;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.LinkLabel txtName;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbProcess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbAbout;
        private System.Windows.Forms.GroupBox gbChoice;
        private System.Windows.Forms.GroupBox gbDates;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.ComboBox cbOperationType;
        private System.Windows.Forms.Label lblOperationType;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.ComboBox cboState;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbBGP;
        private System.Windows.Forms.RadioButton rbRTP;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbDates;
        private System.Windows.Forms.RadioButton rbRecord;
        private System.Windows.Forms.RadioButton rbPTL;
    }
}
