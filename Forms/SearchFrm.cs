using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CRMProcessExplorer
{
    public partial class SearchFrm : Form
    {
        private readonly IOrganizationService service;
        private readonly string webApplicationUrl;
        private EntityMetadata metadata;

        public EntityReference SelectedRecord { get; private set; }

        public SearchFrm(IOrganizationService service, EntityMetadata em, string url)
        {
            InitializeComponent();

            this.service = service;
            this.metadata = em;
            this.webApplicationUrl = url;

            dgvSearch.ReadOnly = true;
            dgvSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSearch.AllowUserToAddRows = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedRecord = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvSearch.SelectedRows.Count <= 0)
                SelectedRecord = null;
            else
            {
                var row = dgvSearch.SelectedRows[0];
                var colCnt = row.Cells.Count;

                SelectedRecord = new EntityReference(metadata.LogicalName, new Guid(row.Cells[colCnt - 3].Value.ToString()));
                SelectedRecord.Name = row.Cells[colCnt - 2].Value.ToStringNullSafe();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SearchFrm_Load(object sender, EventArgs e)
        {
            var defaultIndex = -1;
            using (var cpm = new CRMProcessExplorerManager(this.service))
            {
                cboViews.Items.Clear();
                var views = cpm.GetSavedQuery(metadata.LogicalName);
                var loop = 0;
                views.ForEach(v =>
                {
                    if (v.GetAttributeValueSafe<bool>("isdefault") && v.GetAttributeValueSafe<int>("querytype").Equals(4)) defaultIndex = loop;
                    cboViews.Items.Add(new ViewDetail { Entity = v });
                    loop++;
                });
            }

            cboViews.SelectedIndex = defaultIndex;

            dgvSearch.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(dgvSearch_CellMouseEnter);
            dgvSearch.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(dgvSearch_CellMouseLeave);
        }

        private void dgvSearch_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (IsValidCell(e.RowIndex, e.ColumnIndex))
                dgvSearch.Cursor = Cursors.Hand;
        }
        private void dgvSearch_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (IsValidCell(e.RowIndex, e.ColumnIndex))
                dgvSearch.Cursor = Cursors.Default;
        }

        private bool IsValidCell(int rowIndex, int columnIndex)
        {
            return rowIndex >= 0 && rowIndex < dgvSearch.RowCount &&
                columnIndex >= 0 && columnIndex <= 0;
        }

        private void cboViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInfo.Visible = false;
            dgvSearch.Columns.Clear();
            dgvSearch.Rows.Clear();
            dgvSearch.Refresh();

            var view = ((ViewDetail)cboViews.SelectedItem).Entity;

            var layout = new XmlDocument();
            layout.LoadXml(view.GetAttributeValueSafe<string>("layoutxml"));

            var fetchDoc = new XmlDocument();
            fetchDoc.LoadXml(view.GetAttributeValueSafe<string>("fetchxml"));
            var cols = layout.SelectNodes("//cell");
            dgvSearch.ColumnCount = cols.Count + 4;
            var loop = 0;

            dgvSearch.Columns[loop].Name = "#";
            dgvSearch.Columns[loop].Width = 50;
            dgvSearch.Columns[loop].DefaultCellStyle.ForeColor = Color.Blue;
            dgvSearch.Columns[loop].DefaultCellStyle.Font = new Font(dgvSearch.DefaultCellStyle.Font, FontStyle.Underline);
            loop++;
            foreach (XmlNode cell in cols)
            {
                var ch = dgvSearch.Columns[loop++];
                try
                {
                    ch.Width = int.Parse(cell.Attributes["width"].Value);
                    ch.Name = GetAttributeMeta(cell.Attributes["name"].Value).DisplayName.UserLocalizedLabel.Label;
                }
                catch
                {
                    ch.Name = cell.Attributes["name"].Value;
                }

                // do this for link entity column header only
                if (ch.Name.Contains("."))
                    ch.Name = FixLinkEntityHeader(fetchDoc, ch.Name);
            }
            // primary id field
            dgvSearch.Columns[loop].Name = metadata.PrimaryIdAttribute;
            dgvSearch.Columns[loop].Visible = false;
            // primary name field
            dgvSearch.Columns[++loop].Name = "pkn";
            dgvSearch.Columns[loop].Visible = false;
            // xml field
            dgvSearch.Columns[++loop].Name = "xml";
            dgvSearch.Columns[loop].Visible = false;
        }

        private string FixLinkEntityHeader(XmlDocument doc, string header)
        {
            var linkEntityNodes = doc.SelectNodes("fetch/entity/link-entity");
            foreach (XmlNode node in linkEntityNodes)
            {
                var alias = node.Attributes["alias"].Value;
                if (header.StartsWith(alias))
                    return header.Replace(alias, node.Attributes["name"].Value);
            }

            return header;
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            try
            {
                lblInfo.Visible = false;
                dgvSearch.Rows.Clear();
                dgvSearch.Refresh();
                if (txtSearch.Text.Length == 0) txtSearch.Text = "*";
                var searchString = txtSearch.Text.Trim().Replace("*", "%");

                if(cboViews.SelectedItem == null)
                {
                    var dName = metadata.DisplayName.UserLocalizedLabel.Label;
                    MessageBox.Show(this,
                    $"The entity {dName}, {metadata.LogicalName} is not supported for query!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var view = ((ViewDetail)cboViews.SelectedItem).Entity;
                var layoutXml = view["layoutxml"].ToString();

                var fetchXml = view["fetchxml"].ToString();
                var fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(fetchXml);

                var filterNodes = fetchDoc.SelectNodes("fetch/entity/filter");
                if (filterNodes.Count > 0)
                {
                    foreach (XmlNode node in filterNodes)
                        FilerCondition(node, searchString);

                    var filterNode = filterNodes[0];
                    var cond = fetchDoc.CreateElement("condition");
                    cond.SetAttribute("attribute", metadata.PrimaryNameAttribute);
                    cond.SetAttribute("operator", "like");
                    cond.SetAttribute("value", $"{searchString}%");
                    filterNode.AppendChild(cond.Clone());
                }
                else
                {
                    var ele = fetchDoc.CreateElement("filter");
                    ele.SetAttribute("type", "and");
                    var cond = fetchDoc.CreateElement("condition");
                    cond.SetAttribute("attribute", metadata.PrimaryNameAttribute);
                    cond.SetAttribute("operator", "like");
                    cond.SetAttribute("value", $"{searchString}%");
                    ele.AppendChild(cond.Clone());

                    var entityNode = fetchDoc.SelectSingleNode("fetch/entity");
                    entityNode.AppendChild(ele.Clone());
                }

                var resultXml = string.Empty;
                using (var cpm = new CRMProcessExplorerManager(this.service))
                {
                    resultXml = cpm.ExecuteFetchXML(fetchDoc.OuterXml);
                }

                var isMoreRecords = false;
                var rowList = Helper.ProcessXML(metadata, view["layoutxml"].ToString(), resultXml, out isMoreRecords);
                rowList.ForEach(r =>
                {
                    dgvSearch.Rows.Add(r);
                });

                if (isMoreRecords)
                {
                    lblInfo.Text = "There are more than 5000 records that match your search! Please refine your search. Showing top 5000 results.";
                    lblInfo.Visible = true;
                    this.Text = $"Search:- {rowList.Count}+ records";
                }
                else
                    this.Text = $"Search:- {rowList.Count} record(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Error(btnFetch_Click): " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilerCondition(XmlNode node, string searchString)
        {
            foreach (XmlNode condition in node.SelectNodes("condition"))
            {
                if (condition.Attributes["value"] == null ||
                    !condition.Attributes["value"].Value.StartsWith("{"))
                    continue;

                var oper = condition.Attributes["operator"].Value;
                var attr = GetAttributeMeta(condition.Attributes["attribute"].Value);
                if (oper.Equals("like") && !searchString.Contains("%"))
                    searchString = $"{searchString}%";

                switch (attr.AttributeType.Value)
                {
                    case AttributeTypeCode.Boolean:
                        var bValue = false;
                        if (Boolean.TryParse(searchString, out bValue))
                            condition.Attributes["value"].Value = bValue.ToString();
                        else if (searchString == "0" || searchString == "1")
                            condition.Attributes["value"].Value = (searchString == "1").ToString();
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.Customer:
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Owner:
                        if (GetAttributeMeta(condition.Attributes["attribute"].Value + "name") == null)
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        else
                        {
                            condition.Attributes["attribute"].Value += "name";
                            condition.Attributes["value"].Value = searchString;
                        }
                        break;
                    case AttributeTypeCode.DateTime:
                        DateTime dt;
                        if (DateTime.TryParse(searchString, out dt))
                            condition.Attributes["value"].Value = dt.ToString("yyyy-MM-dd");
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.Decimal:
                    case AttributeTypeCode.Double:
                    case AttributeTypeCode.Money:
                        decimal d;
                        if (decimal.TryParse(searchString, out d))
                            condition.Attributes["value"].Value = d.ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.Integer:
                        int i;
                        if (int.TryParse(searchString, out i))
                            condition.Attributes["value"].Value = i.ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.Picklist:
                        var pOpt = ((PicklistAttributeMetadata)attr).OptionSet.Options.FirstOrDefault(o => o.Label.UserLocalizedLabel.Label == searchString);
                        if (pOpt != null)
                            condition.Attributes["value"].Value = pOpt.Value.Value.ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.State:
                        var sOpt = ((StateAttributeMetadata)attr).OptionSet.Options.FirstOrDefault(o => o.Label.UserLocalizedLabel.Label == searchString);
                        if (sOpt != null)
                            condition.Attributes["value"].Value = sOpt.Value.Value.ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    case AttributeTypeCode.Status:
                        var stOpt = ((StatusAttributeMetadata)attr).OptionSet.Options.FirstOrDefault(o => o.Label.UserLocalizedLabel.Label == searchString);
                        if (stOpt != null)
                            condition.Attributes["value"].Value = stOpt.Value.Value.ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            node.RemoveChild(condition);
                            continue;
                        }
                        break;
                    default:
                        condition.Attributes["value"].Value = searchString;
                        break;
                }
            }

            // if innter filter found
            foreach (XmlNode filter in node.SelectNodes("filter"))
                FilerCondition(filter, searchString);
        }

        private AttributeMetadata GetAttributeMeta(string name)
        {
            return metadata.Attributes.FirstOrDefault(a => a.LogicalName == name);
        }

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            if (e.ColumnIndex == 0)
            {
                var row = dgvSearch.Rows[e.RowIndex];
                var colCnt = dgvSearch.Columns.Count;

                var url = this.webApplicationUrl.GetCRMUrl(metadata.LogicalName, row.Cells[colCnt - 3].Value.ToString(), true);
                System.Diagnostics.Process.Start(url);
            }
            else
                btnOK_Click(sender, null);
        }
    }
}
