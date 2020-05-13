/*
 * Thanks to Hicham Wahbi, got insperation and idea from his earlier tool called "CRM Workflow Explorer".
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRMProcessExplorer
{
    public partial class ProcessDiagram : Form
    {
        private readonly EntityDetail entityDetail = null;
        private readonly ProcessDetailTN rootProcess = null;
        private readonly string webApplicationUrl;

        public ProcessDiagram(EntityDetail eDetail, ProcessDetailTN rProcess, string url)
        {
            InitializeComponent();
            entityDetail = eDetail;
            rootProcess = rProcess;
            webApplicationUrl = url;
        }

        private void ProcessDiagram_Load(object sender, EventArgs e)
        {
            tvProcess.Nodes.Add(rootProcess);
            tvProcess.ShowNodeToolTips = true;
            this.Text = entityDetail.DisplayName;
            SetNodeText(tvProcess.Nodes);


            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Save Diagram", new EventHandler(SaveDiagram_Click));
            cm.MenuItems.Add("Close", new EventHandler(Close_Click));

            pbProcess.ContextMenu = cm;
            tPage.ContextMenu = cm;
        }

        private void SaveDiagram_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "JPEG (*.jpg)|*.jpg";
            sfd.FileName = $"{entityDetail.DisplayName} {DateTime.Now.ToString("yyyyMMddTHHmmss")}.jpg";
            bool fileError = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(sfd.FileName))
                {
                    try
                    {
                        File.Delete(sfd.FileName);
                    }
                    catch (IOException ex)
                    {
                        fileError = true;
                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (!fileError)
                {
                    try
                    {
                        pbProcess.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                        MessageBox.Show("Process Diagram Successfully !!!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            btnCancel_Click(sender, e);
        }

        public void DisplayProcess()
        {
            var selectedNode = (ProcessDetailTN)tvProcess.SelectedNode;
            selectedNode.IsRoot = true;
            var db = new DiagramBuilder();

            int w = -1;
            int h = -1;

            Image img = Image.FromStream(db.PaintDiagram(selectedNode, ref w, ref h, "1", System.Drawing.Imaging.ImageFormat.Bmp));
            pbProcess.Image = (Image)(new Bitmap(img, new Size(w, h)));
            selectedNode.IsRoot = false;
        }

        private void tvProcess_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DisplayProcess();
        }

        private void SetNodeText(TreeNodeCollection nodes)
        {
            foreach(TreeNode node in nodes)
            {
                node.Text = $"({node.Tag}) {node.Text}";

                var pNode = node as ProcessDetailTN;
                if (pNode.Type == ProcessDetail.eTypes.Entity)
                {
                    node.Expand();
                    node.ToolTipText = $"Double click to open CRM - {pNode.Type.ToStringNullSafe()}";
                }
                else if (pNode.Type == ProcessDetail.eTypes.CustomCode && pNode.Category == ProcessDetail.eCategories.Workflow)
                    node.ToolTipText = $"{pNode.Type.ToStringNullSafe()}";
                else if (pNode.Type == ProcessDetail.eTypes.CustomCode && pNode.Category == ProcessDetail.eCategories.Plugins)
                    node.ToolTipText = $"{pNode.Category.ToStringNullSafe()}";
                else
                    node.ToolTipText = $"Double click to open CRM - {pNode.Category.ToStringNullSafe()}";

                if (node.Nodes.Count > 0) SetNodeText(node.Nodes);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tvProcess_DoubleClick(object sender, EventArgs e)
        {
            var node = tvProcess.SelectedNode as ProcessDetailTN;
            if (node == null || node.Tag.Equals("P") || node.Id.Equals(Guid.Empty)) return;

            var url = "";
            if(node.Tag.Equals("E"))
                url = $"{this.webApplicationUrl}tools/systemcustomization/Entities/manageEntity.aspx?entityId={node.Id}";
            else
                url = $"{this.webApplicationUrl}sfa/workflow/edit.aspx?id={node.Id}";

            System.Diagnostics.Process.Start(url);
        }

        private void pbProcess_MouseHover(object sender, EventArgs e)
        {
            //var tt = new ToolTip();
            //tt.SetToolTip(this.pbProcess, "Right click to save diagram");
        }
    }
}
