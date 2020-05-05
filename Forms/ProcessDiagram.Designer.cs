namespace CRMProcessExplorer
{
    partial class ProcessDiagram
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
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.tvProcess = new System.Windows.Forms.TreeView();
            this.tcProcess = new System.Windows.Forms.TabControl();
            this.tPage = new System.Windows.Forms.TabPage();
            this.pbProcess = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.tcProcess.SuspendLayout();
            this.tPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProcess)).BeginInit();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.tvProcess);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.tcProcess);
            this.scMain.Size = new System.Drawing.Size(1156, 430);
            this.scMain.SplitterDistance = 404;
            this.scMain.TabIndex = 3;
            // 
            // tvProcess
            // 
            this.tvProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProcess.Location = new System.Drawing.Point(0, 0);
            this.tvProcess.Name = "tvProcess";
            this.tvProcess.Size = new System.Drawing.Size(404, 430);
            this.tvProcess.TabIndex = 1;
            this.tvProcess.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvProcess_AfterSelect);
            this.tvProcess.DoubleClick += new System.EventHandler(this.tvProcess_DoubleClick);
            // 
            // tcProcess
            // 
            this.tcProcess.Controls.Add(this.tPage);
            this.tcProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcProcess.Location = new System.Drawing.Point(0, 0);
            this.tcProcess.Name = "tcProcess";
            this.tcProcess.SelectedIndex = 0;
            this.tcProcess.Size = new System.Drawing.Size(748, 430);
            this.tcProcess.TabIndex = 3;
            // 
            // tPage
            // 
            this.tPage.AutoScroll = true;
            this.tPage.Controls.Add(this.pbProcess);
            this.tPage.Controls.Add(this.btnCancel);
            this.tPage.Location = new System.Drawing.Point(4, 22);
            this.tPage.Name = "tPage";
            this.tPage.Padding = new System.Windows.Forms.Padding(3);
            this.tPage.Size = new System.Drawing.Size(740, 404);
            this.tPage.TabIndex = 0;
            this.tPage.Text = "Process Hierarchy Diagram";
            this.tPage.UseVisualStyleBackColor = true;
            // 
            // pbProcess
            // 
            this.pbProcess.BackColor = System.Drawing.Color.Transparent;
            this.pbProcess.Location = new System.Drawing.Point(0, 0);
            this.pbProcess.Name = "pbProcess";
            this.pbProcess.Size = new System.Drawing.Size(390, 198);
            this.pbProcess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbProcess.TabIndex = 1;
            this.pbProcess.TabStop = false;
            this.pbProcess.MouseHover += new System.EventHandler(this.pbProcess_MouseHover);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(666, -23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ProcessDiagram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1156, 430);
            this.Controls.Add(this.scMain);
            this.MinimizeBox = false;
            this.Name = "ProcessDiagram";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Process Diagram";
            this.Load += new System.EventHandler(this.ProcessDiagram_Load);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.tcProcess.ResumeLayout(false);
            this.tPage.ResumeLayout(false);
            this.tPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProcess)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TreeView tvProcess;
        private System.Windows.Forms.TabControl tcProcess;
        private System.Windows.Forms.TabPage tPage;
        private System.Windows.Forms.PictureBox pbProcess;
        private System.Windows.Forms.Button btnCancel;
    }
}