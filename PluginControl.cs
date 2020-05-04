using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Metadata;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using XrmToolBox.Extensibility.Interfaces;

namespace CRMProcessExplorer
{
    public partial class PluginControl : PluginControlBase, IGitHubPlugin
    {
        private Settings mySettings;
        private EntityDetail entityDetail;
        private string _layoutXml = string.Empty;

        private ProcessDetailTN _selectedNode = null;

        public PluginControl()
        {
            InitializeComponent();
            gbRecord.Visible = true;
            gbDates.Visible = false;
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            lblInfo.Visible = false;

            dgvMain.ReadOnly = true;
            dgvMain.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMain.AllowUserToAddRows = false;
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            dgvMain.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(dgvMain_CellMouseEnter);
            dgvMain.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(dgvMain_CellMouseLeave);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                AsyncArgument = null,
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void btnRetrieveEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(FetchEntities);
        }

        private void FetchEntities()
        {
            cboEntities.Items.Clear();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Fetching Entities...",
                AsyncArgument = null,
                Work = (worker, args) =>
                {
                    using (var cpm = new CRMProcessExplorerManager(this.Service))
                    {
                        args.Result = cpm.GetEntities();
                    }
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as List<EntityMetadata>;
                    if (result != null)
                    {
                        var lst = new List<EntityDetail>();
                        foreach (var em in result)
                        {
                            lst.Add(new EntityDetail(em));
                        }

                        cboEntities.DataSource = lst.Where(l => !l.DisplayName.Equals("NULL")).ToList();
                        cboEntities.DisplayMember = "Name";

                        cboEntities.SelectedIndex = 0;
                    }
                }
            });
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!IsCheckConnection()) return;

            if (cboEntities.SelectedIndex < 0)
            {
                MessageBox.Show(ParentForm, "Please select an entity in the list before using the search action",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var search = new SearchFrm(this.Service, ((EntityDetail)cboEntities.SelectedItem).Metadata, this.ConnectionDetail.WebApplicationUrl);
            search.StartPosition = FormStartPosition.CenterParent;
            if (search.ShowDialog() == DialogResult.OK && search.SelectedRecord != null)
            {
                txtID.Text = search.SelectedRecord.Id.ToString();
                txtName.Text = search.SelectedRecord.Name;

                // load process for the selected record
                tsbProcess_Click(sender, e);
            }
        }

        private void txtName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cboEntities.SelectedItem == null || string.IsNullOrEmpty(txtID.Text)) return;

            var url = this.ConnectionDetail.WebApplicationUrl.GetCRMUrl(((EntityDetail)cboEntities.SelectedItem).Metadata.LogicalName, txtID.Text);
            System.Diagnostics.Process.Start(url);
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show($@"Hi All...Thanks for using this tool.{Environment.NewLine}{Environment.NewLine}Using this tool you can view Background/Real-time Processes & Plug-in Trace Logs by records/by dates. Double clicking the record/column will open relevent CRM forms also double clicking the column (long messages) will open up a pop-up view with full details.{Environment.NewLine}{Environment.NewLine}I'm Murali...I'm working for an Institution called CEWA. I'm a CRM Developer. You can reach me at [murali.tk@gmail.com].{Environment.NewLine}{Environment.NewLine}Please let me know if you find any issues/needs enhancement.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsbProcess_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMain.ColumnCount <= 0)
                    ExecuteMethod(InitilizeDataGridView);

                if (dgvMain.ColumnCount > 0)
                    ExecuteMethod(LoadDataGridView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Error(tsbProcess_Click): " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = cboEntities.SelectedItem != null;
            btnWFE.Enabled = cboEntities.SelectedItem != null;
            txtID.Text = string.Empty;
            txtName.Text = string.Empty;
            dgvMain.Rows.Clear();
            dgvMain.Refresh();
        }

        private void LoadDataGridView()
        {
            lblInfo.Visible = false;
            dgvMain.Rows.Clear();
            dgvMain.Refresh();
            if (rbRecord.Enabled && rbRecord.Checked && string.IsNullOrEmpty(txtID.Text.Trim())) return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Data...",
                AsyncArgument = new List<string>() { (rbRecord.Enabled ? txtID.Text.Trim() : ""), dtFrom.Value.ToString("yyyy-MM-dd"), dtTo.Value.ToString("yyyy-MM-dd"),
                    cbOperationType.Enabled && cbOperationType.Items.Count > 0 ? cbOperationType.SelectedValue.ToString() : "-1",
                    cboState.Enabled && cboState.Items.Count > 0? cboState.SelectedValue.ToString(): "-1",
                    cboStatus.Enabled && cboStatus.Items.Count > 0? cboStatus.SelectedValue.ToString() : "-1" },
                Work = (worker, args) =>
                {
                    var items = args.Argument as List<string>;

                    // load data
                    var resultXml = string.Empty;
                    using (var cpm = new CRMProcessExplorerManager(Service))
                    {
                        if (rbRecord.Enabled && rbRecord.Checked)
                            resultXml = cpm.GetProcessesResult(items[0]);
                        else if (rbDates.Checked && rbPTL.Checked)
                            resultXml = cpm.GetProcessesResult(null, items[1], items[2], items[5], items[4], items[3]);
                        else if (rbDates.Checked)
                            resultXml = cpm.GetProcessesResult(null, items[1], items[2], items[3], items[4], items[5]);
                    }
                    args.Result = resultXml;
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // load data
                    var resultXml = args.Result as string;
                    var isMoreRecords = false;
                    var rowList = Helper.ProcessXML(entityDetail.Metadata, _layoutXml, resultXml, out isMoreRecords);
                    rowList.ForEach(r =>
                    {
                        dgvMain.Rows.Add(r);
                    });

                    if (isMoreRecords)
                    {
                        lblInfo.Text = "There are more than 5000 records that match your search! Please refine your search. Showing top 5000 results.";
                        lblInfo.Visible = true;
                    }

                    ShowHeader(rowList.Count);
                }
            });
        }

        private void ShowHeader(int count)
        {
            var msg = (count >= 5000 ? "+" : "");
            if (rbBGP.Checked)
                gbMain.Text = $"Background Process:- {count}{msg} record(s)";
            else if (rbRTP.Checked)
                gbMain.Text = $"Real-time Process:- {count}{msg} record(s)";
            else
                gbMain.Text = $"Plug-in Trace Log:- {count}{msg} record(s)";
        }

        private void InitilizeDataGridView()
        {
            lblInfo.Visible = false;

            using (var cpm = new CRMProcessExplorerManager(this.Service))
            {
                if (!QueryEntityInfo.IsBackgroundProcess.HasValue) // Plug-n Trace log
                {
                    if (QueryEntityInfo.TLMetadata == null)
                    {
                        var em = cpm.GetEntities("plugintracelog").FirstOrDefault();
                        QueryEntityInfo.TLMetadata = new EntityDetail(em);
                    }
                    entityDetail = QueryEntityInfo.TLMetadata;
                    _layoutXml = QueryEntityInfo.TLLayoutXML;
                }
                else if (QueryEntityInfo.IsBackgroundProcess.Value) // background process
                {
                    if (QueryEntityInfo.AOMetadata == null)
                    {
                        var em = cpm.GetEntities("asyncoperation").FirstOrDefault();
                        QueryEntityInfo.AOMetadata = new EntityDetail(em);
                    }
                    entityDetail = QueryEntityInfo.AOMetadata;
                    _layoutXml = QueryEntityInfo.AOLayoutXML;
                }
                else // real-time process
                {
                    if (QueryEntityInfo.PSMetadata == null)
                    {
                        var em = cpm.GetEntities("processsession").FirstOrDefault();
                        QueryEntityInfo.PSMetadata = new EntityDetail(em);
                    }
                    entityDetail = QueryEntityInfo.PSMetadata;
                    _layoutXml = QueryEntityInfo.PSLayoutXML;
                }
            }

            dgvMain.Columns.Clear();
            dgvMain.Rows.Clear();
            dgvMain.Refresh();

            var layout = new XmlDocument();
            layout.LoadXml(_layoutXml);

            var cols = layout.SelectNodes("//cell");
            dgvMain.ColumnCount = cols.Count + 4;
            var loop = 0;

            dgvMain.Columns[loop].Name = "#";
            dgvMain.Columns[loop].Visible = false;
            loop++;
            foreach (XmlNode cell in cols)
            {
                var ch = dgvMain.Columns[loop++];
                try
                {
                    ch.Width = int.Parse(cell.Attributes["width"].Value);
                    if (entityDetail != null && entityDetail.Metadata != null)
                        ch.Name = entityDetail.Metadata.Attributes.FirstOrDefault(a => a.LogicalName == cell.Attributes["name"].Value).DisplayName.UserLocalizedLabel.Label;
                }
                catch
                {
                    ch.Name = cell.Attributes["name"].Value;
                }
            }
            // primary id field
            dgvMain.Columns[loop].Name = entityDetail.Metadata.PrimaryIdAttribute;
            dgvMain.Columns[loop].Visible = false;
            // primary name field
            dgvMain.Columns[++loop].Name = "pkn";
            dgvMain.Columns[loop].Visible = false;
            // xml field
            dgvMain.Columns[++loop].Name = "xml";
            dgvMain.Columns[loop].Visible = false;
        }

        private void rbRecord_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbRecord.Checked) return;
            if (!IsCheckConnection()) return;

            lblInfo.Visible = false;
            gbRecord.Visible = true;
            gbDates.Visible = false;
        }

        private void rbDates_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbDates.Checked) return;
            if (!IsCheckConnection()) return;

            lblInfo.Visible = false;
            gbDates.Visible = true;
            gbRecord.Visible = false;
            dgvMain.Rows.Clear();
            dgvMain.Refresh();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Load Info",
                AsyncArgument = null,
                Work = (worker, args) =>
                {
                    using (var cpm = new CRMProcessExplorerManager(this.Service))
                    {
                        if (entityDetail == null)
                        {
                            var em = cpm.GetEntities("asyncoperation").FirstOrDefault();
                            entityDetail = new EntityDetail(em);
                        }

                        if (cbOperationType.Items.Count <= 0)
                        {
                            args.Result = cpm.GetStringMap("asyncoperation", "operationtype");
                        }
                    }
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (cbOperationType.Items.Count <= 0)
                    {
                        var lst = args.Result;
                        cbOperationType.DataSource = new BindingSource(lst, null);
                        cbOperationType.DisplayMember = "Value";
                        cbOperationType.ValueMember = "Key";
                    }

                    LoadState();
                    LoadStatus();
                }
            });

            if (dgvMain.ColumnCount <= 0) ExecuteMethod(InitilizeDataGridView);
        }

        private void LoadState()
        {
            var comboSource = new Dictionary<string, string>();
            comboSource.Add("-1", "All");
            if (!QueryEntityInfo.IsBackgroundProcess.HasValue)
            {

            }
            else if (QueryEntityInfo.IsBackgroundProcess.Value)
            {
                comboSource.Add("0", "Ready");
                comboSource.Add("1", "Suspended");
                comboSource.Add("2", "Locked");
                comboSource.Add("3", "Completed");
            }
            else
            {
                comboSource.Add("0", "Incomplete");
                comboSource.Add("1", "Complete");
            }
            cboState.DataSource = new BindingSource(comboSource, null);
            cboState.DisplayMember = "Value";
            cboState.ValueMember = "Key";
        }

        private void LoadStatus()
        {
            var comboSource = new Dictionary<string, string>();
            comboSource.Add("-1", "All");
            if (!QueryEntityInfo.IsBackgroundProcess.HasValue)
            {
                comboSource.Add("0", "Unknown");
                comboSource.Add("1", "Plug-in");
                comboSource.Add("2", "Workflow Activity");
            }
            else if (QueryEntityInfo.IsBackgroundProcess.Value)
            {
                comboSource.Add("0", "Waiting For Resources");
                comboSource.Add("10", "Waiting");
                comboSource.Add("20", "In Progress");
                comboSource.Add("21", "Pausing");
                comboSource.Add("22", "Canceling");
                comboSource.Add("30", "Succeeded");
                comboSource.Add("31", "Failed");
                comboSource.Add("32", "Canceled");
            }
            else
            {
                comboSource.Add("1", "Waiting");
                comboSource.Add("2", "In Progress");
                comboSource.Add("3", "Paused");
                comboSource.Add("4", "Completed");
                comboSource.Add("5", "Canceled");
                comboSource.Add("6", "Failed");
            }
            cboStatus.DataSource = new BindingSource(comboSource, null);
            cboStatus.DisplayMember = "Value";
            cboStatus.ValueMember = "Key";
        }

        private void dgvMain_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (IsValidCell(e.RowIndex, e.ColumnIndex))
                dgvMain.Cursor = Cursors.Hand;
        }
        private void dgvMain_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (IsValidCell(e.RowIndex, e.ColumnIndex))
                dgvMain.Cursor = Cursors.Default;
        }

        private bool IsValidCell(int rowIndex, int columnIndex)
        {
            return rowIndex >= 0 && rowIndex < dgvMain.RowCount &&
                columnIndex >= 0 && columnIndex <= dgvMain.ColumnCount;
        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var header = dgvMain.Columns[e.ColumnIndex].Name;
            var rowXml = $"<root>{dgvMain.Rows[e.RowIndex].Cells[dgvMain.ColumnCount - 1].Value.ToStringNullSafe()}</root>";
            var tr = new StringReader(rowXml);
            var doc = XDocument.Load(tr);
            var regardingObjectId = string.Empty;
            var regardingObjectCode = string.Empty;
            var asyncOperationId = string.Empty;
            var operationType = string.Empty;
            var url = string.Empty;
            var msg = string.Empty;

            if (doc.Root.Element("result").Element("regardingobjectid") != null)
                regardingObjectId = doc.Root.Element("result").Element("regardingobjectid").Value;

            if (doc.Root.Element("result").Element("regardingobjectid") != null)
                regardingObjectCode = doc.Root.Element("result").Element("regardingobjectid").Attribute("type").Value;

            if (doc.Root.Element("result").Element("asyncoperationid") != null)
                asyncOperationId = doc.Root.Element("result").Element("asyncoperationid").Value;
            else if (doc.Root.Element("result").Element("processsessionid") != null)
                asyncOperationId = doc.Root.Element("result").Element("processsessionid").Value;
            else if (doc.Root.Element("result").Element("plugintracelogid") != null)
            {
                asyncOperationId = doc.Root.Element("result").Element("plugintracelogid").Value;
                regardingObjectCode = "4619";
            }

            if (doc.Root.Element("result").Element("operationtype") != null)
                operationType = doc.Root.Element("result").Element("operationtype").Value;
            else if (doc.Root.Element("result").Element("name") != null)
                operationType = doc.Root.Element("result").Element("name").Value;

            if ((header.Equals("Regarding")) && !string.IsNullOrEmpty(regardingObjectId))
                url = this.ConnectionDetail.WebApplicationUrl.GetCRMUrl(regardingObjectCode, regardingObjectId, true, true);
            else if (!string.IsNullOrEmpty(asyncOperationId) && operationType.Equals("10"))
                url = $"{this.ConnectionDetail.WebApplicationUrl}tools/workflowinstance/edit.aspx?id={asyncOperationId}";
            else if (!string.IsNullOrEmpty(asyncOperationId) && operationType.Equals("workflow", StringComparison.InvariantCultureIgnoreCase))
                url = $"{this.ConnectionDetail.WebApplicationUrl}sfa/workflowsession/edit.aspx?id={asyncOperationId}";
            else if (!string.IsNullOrEmpty(asyncOperationId) && !QueryEntityInfo.IsBackgroundProcess.HasValue)
                url = this.ConnectionDetail.WebApplicationUrl.GetCRMUrl(regardingObjectCode, asyncOperationId, true, true);
            else if (!string.IsNullOrEmpty(asyncOperationId) && QueryEntityInfo.IsBackgroundProcess.Value)
                url = $"{this.ConnectionDetail.WebApplicationUrl}tools/asyncoperation/edit.aspx?id={asyncOperationId}";

            if (header.Equals("Exception Details") && doc.Root.Element("result").Element("exceptiondetails") != null)
                msg = doc.Root.Element("result").Element("exceptiondetails").Value;
            else if (header.Equals("Configuration") && doc.Root.Element("result").Element("configuration") != null)
                msg = doc.Root.Element("result").Element("configuration").Value;
            else if (header.Equals("Secure Configuration") && doc.Root.Element("result").Element("secureconfiguration") != null)
                msg = doc.Root.Element("result").Element("secureconfiguration").Value;
            else if (header.Equals("Message Block") && doc.Root.Element("result").Element("messageblock") != null)
                msg = doc.Root.Element("result").Element("messageblock").Value;
            else if (header.Equals("Message") && doc.Root.Element("result").Element("message") != null)
                msg = doc.Root.Element("result").Element("message").Value;
            else if (header.Equals("Friendly message") && doc.Root.Element("result").Element("friendlymessage") != null)
                msg = doc.Root.Element("result").Element("friendlymessage").Value;
            else if (header.Equals("Comments") && doc.Root.Element("result").Element("comments") != null)
                msg = doc.Root.Element("result").Element("comments").Value;

            if (!string.IsNullOrEmpty(msg))
                ShowDetailMsg(header, msg);
            else if (!string.IsNullOrEmpty(url))
                System.Diagnostics.Process.Start(url);
        }

        private void rbBGP_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbBGP.Checked) return;
            if (!IsCheckConnection()) return;

            QueryEntityInfo.IsBackgroundProcess = true;
            rbRecord.Enabled = true;
            ShowHideFilters();

            if (rbRecord.Checked)
                rbRecord_CheckedChanged(sender, e);
            else
                rbDates_CheckedChanged(sender, e);

            ExecuteMethod(InitilizeDataGridView);
            if (rbDates.Checked && dgvMain.ColumnCount > 0) ExecuteMethod(LoadDataGridView);
        }

        private void rbRTP_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbRTP.Checked) return;
            if (!IsCheckConnection()) return;

            QueryEntityInfo.IsBackgroundProcess = false;
            rbRecord.Enabled = true;
            ShowHideFilters();

            if (rbRecord.Checked)
                rbRecord_CheckedChanged(sender, e);
            else
                rbDates_CheckedChanged(sender, e);

            ExecuteMethod(InitilizeDataGridView);
            if (rbDates.Checked && dgvMain.ColumnCount > 0) ExecuteMethod(LoadDataGridView);
        }

        private void rbPTL_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbPTL.Checked) return;
            if (!IsCheckConnection()) return;

            QueryEntityInfo.IsBackgroundProcess = null;
            rbRecord.Enabled = false;
            rbDates.Checked = true;
            ShowHideFilters();

            ExecuteMethod(InitilizeDataGridView);
            if (rbDates.Checked && dgvMain.ColumnCount > 0) ExecuteMethod(LoadDataGridView);
        }

        private void ShowHideFilters()
        {
            cboState.Enabled = true;
            cboStatus.Enabled = true;
            cbOperationType.Enabled = true;
            if (cbOperationType.Items.Count > 0) cbOperationType.SelectedIndex = 0;
            if (cboState.Items.Count > 0) cboState.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
            lblOperationType.Text = "System Job Type:";
            lblStatus.Text = "Status Reason:";

            if (!QueryEntityInfo.IsBackgroundProcess.HasValue)
            {
                gbMain.Text = "Plug-in Trace Logs";
                lblStatus.Text = "Operation Type:";
                cboState.Enabled = false;
                cbOperationType.Enabled = false;
            }
            else if (QueryEntityInfo.IsBackgroundProcess.Value)
            {
                gbMain.Text = "Background Process Info";
            }
            else
            {
                cbOperationType.Enabled = false;
                gbMain.Text = "Real-time Process Info";
            }
        }

        private void ShowDetailMsg(string title, string msg)
        {
            var mf = new MsgFrm(title, msg);
            mf.StartPosition = FormStartPosition.CenterParent;
            mf.Show();
        }

        private void CheckConnection()
        {
            if (this.Service == null)
            {
                MessageBox.Show(ParentForm, "Please connect to CRM first!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool IsCheckConnection()
        {
            if (this.Service == null)
            {
                MessageBox.Show(ParentForm, "Please connect to CRM first!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void tsbCSV_Click(object sender, EventArgs e)
        {
            if (dgvMain.Rows.Count > 0)
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = $"Process Info {DateTime.Now.ToString("yyyyMMddTHHmmss")}.csv";
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
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            var sb = new StringBuilder();

                            var headers = dgvMain.Columns.Cast<DataGridViewColumn>();
                            sb.AppendLine(string.Join(",", headers.Take(headers.Count() - 2).Select(column => "\"" + column.HeaderText + "\"").ToArray()));

                            foreach (DataGridViewRow row in dgvMain.Rows)
                            {
                                var cells = row.Cells.Cast<DataGridViewCell>();
                                sb.AppendLine(string.Join(",", cells.Take(headers.Count() - 2).Select(cell => "\"" + (cell.Value != null ? cell.Value.ToStringNullSafe().Replace("\"", "\'")  : string.Empty)  + "\"").ToArray()));
                            }
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                            MessageBox.Show("Data Exported Successfully !!!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void btnWFE_Click(object sender, EventArgs e)
        {
            entityDetail = ((EntityDetail)cboEntities.SelectedItem);
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting Process Info...",
                AsyncArgument = null,
                Work = (worker, args) =>
                {
                    using (var cpm = new CRMProcessExplorerManager(this.Service))
                    {
                        args.Result = cpm.GetWorkflowsByEntity(entityDetail);
                    }
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as List<ProcessDetail>;

                    var rootComponent = new ProcessDetailTN();
                    rootComponent.Type = ProcessDetail.eTypes.Entity;
                    rootComponent.Text = entityDetail.DisplayName;
                    result.ForEach(pd =>
                    {
                        AppendProcessNode(rootComponent, pd, 5);
                    });
                    _selectedNode = rootComponent;

                    var pdf = new ProcessDiagram(((EntityDetail)cboEntities.SelectedItem), _selectedNode);
                    pdf.StartPosition = FormStartPosition.CenterParent;
                    pdf.Show();
                }
            });
        }

        private void AppendProcessNode(ProcessDetailTN node, ProcessDetail pd, int depth)
        {
            depth--;
            if (depth < 0) return;

            var  childNode = new ProcessDetailTN();
            childNode.Text = pd.Name;
            childNode.Id = pd.Id;
            childNode.Category = pd.Category;
            childNode.Type = pd.Type;
            childNode.PrimaryEntityName = pd.PrimaryEntityName;

            node.Nodes.Add(childNode);
            childNode.IsVisited = IsComponentVisited(childNode, childNode.Id);

            if (!childNode.IsVisited)
            {
                foreach (var cpd in pd.ChildProcess)
                {
                    AppendProcessNode(childNode, cpd, depth);
                }
            }
        }

        private bool IsComponentVisited(ProcessDetailTN node, Guid id)
        {
            var parent = (ProcessDetailTN)node.Parent;
            if (parent != null)
            {
                if (parent.Id == id)
                    return true;
                else
                    return IsComponentVisited(parent, id);
            }

            return false;
        }

        #region Github implementation
        public string RepositoryName
        {
            get { return "CRMProcessExplorer"; }
        }

        public string UserName
        {
            get { return "muralitk"; }
        }
        #endregion Github implementation
    }
}