using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRMProcessExplorer
{
    public class EntityDetail
    {
        public string DisplayName { get; private set; }
        public string LogicalName { get; private set; }
        public string Name { get; private set; }
        public EntityMetadata Metadata { get; }
        public List<ProcessDetail> PluginAssemblies { get; } = new List<ProcessDetail>();

        public EntityDetail(EntityMetadata em)
        {
            Metadata = em;

            LogicalName = em.LogicalName;
            DisplayName = em.DisplayName?.UserLocalizedLabel?.Label ?? "NULL";
            Name = string.Format("{0} [{1}]", DisplayName, LogicalName);
        }
    }

    public class ViewDetail
    {
        public Entity Entity { get; set; }

        public override string ToString()
        {
            return Entity.GetAttributeValueSafe<string>("name");
        }
    }

    public class ProcessDetail
    {
        public enum eCategories
        {
            Workflow = 0,
            Dialog = 1,
            BusinessRule = 2,
            Action = 3,
            BusinessProcessFlow = 4,
        };

        public enum eTypes 
        {
            Entity = 1,
            Workflow = 29,
            PluginType = 90
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PrimaryEntityName { get; set; }
        public List<ProcessDetail> ChildProcess { get; set; } = new List<ProcessDetail>();
        public eCategories Category { get; set; }
        public eTypes Type { get; set; }

        public bool IsVisited { get; set; } = false;

        public bool IsWorkflow
        {
            get
            {
                return this.Category == eCategories.Workflow;
            }
        }

        public bool IsAssembly
        {
            get
            {
                return this.Type == eTypes.PluginType;
            }
        }

        public bool IsEntity
        {
            get
            {
                return this.Type == eTypes.Entity;
            }
        }

        public ProcessDetail()
        {
        }

        public eCategories GetTypeByCategory(int category)
        {
            eCategories type;
            switch (category)
            {
                case 0:
                    type = eCategories.Workflow;
                    break;
                case 1:
                    type = eCategories.Dialog;
                    break;
                case 2:
                    type = eCategories.BusinessRule;
                    break;
                case 3:
                    type = eCategories.Action;
                    break;
                default:
                    type = eCategories.Workflow;
                    break;
            }

            return type;
        }

        public List<ProcessDetail> Nodes { get; set; }

        public ProcessDetail CloneMe()
        {
            var newNode = new ProcessDetail()
            {
                Name = this.Name,
                Id = this.Id,
                Category = this.Category,
                Type = this.Type,
                PrimaryEntityName = this.PrimaryEntityName,
                IsVisited = this.IsVisited
            };

            foreach (var childNode in this.Nodes)
            {
                if (childNode.Type != eTypes.PluginType)
                {
                    var newChildNode = childNode.CloneMe();
                    newNode.Nodes.Add(newChildNode);
                }
            }

            return newNode;
        }
    }

    public class ProcessDetailTN : TreeNode
    {
        public Guid Id { get; set; }
        public string ComponentName { get; set; }
        public string PrimaryEntityName { get; set; }
        private ProcessDetail.eCategories _category;
        public ProcessDetail.eCategories Category
        {
            get
            {
                return _category;
            }
            set
            {
                this._category = value;
                SetImageIndex();
            }
        }
        private ProcessDetail.eTypes _Type;
        public ProcessDetail.eTypes Type
        {
            get
            {
                return _Type;
            }
            set
            {
                this._Type = value;
                SetImageIndex();
            }
        }

        public void SetImageIndex()
        {
            if (Type == ProcessDetail.eTypes.Workflow)
            {
                switch (Category)
                {
                    case ProcessDetail.eCategories.Workflow:
                        this.Tag = "W";
                        this.ImageIndex = 1;
                        break;
                    case ProcessDetail.eCategories.Action:
                        this.Tag = "A";
                        this.ImageIndex = 2;
                        break;
                    case ProcessDetail.eCategories.Dialog:
                        this.Tag = "D";
                        this.ImageIndex = 4;
                        break;
                    case ProcessDetail.eCategories.BusinessRule:
                        this.Tag = "B";
                        this.ImageIndex = 5;
                        break;
                    case ProcessDetail.eCategories.BusinessProcessFlow:
                        this.Tag = "F";
                        this.ImageIndex = 6;
                        break;
                }
            }
            else if (Type == ProcessDetail.eTypes.Entity)
            {
                this.Tag = "E";
                this.ImageIndex = 0;
            }
            else if (Type == ProcessDetail.eTypes.PluginType)
            {
                this.Tag = "P";
                this.ImageIndex = 3;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public bool IsRoot { get; set; } = false;
        public bool IsHidden { get; set; } = false;
        public bool IsVisited { get; set; } = false;
        public bool IsWorkflow
        {
            get
            {
                return this.Type == ProcessDetail.eTypes.Workflow;
            }
        }
        public bool IsAssembly
        {
            get
            {
                return this.Type == ProcessDetail.eTypes.PluginType;
            }
        }
        public bool IsEntity
        {
            get
            {
                return this.Type == ProcessDetail.eTypes.Entity;
            }
        }

        public ProcessDetailTN CloneMe()
        {
            var  newNode = new ProcessDetailTN()
            {
                Name = this.Name,
                Id = this.Id,
                IsHidden = this.IsHidden,
                Category = this.Category,
                Type = this.Type,
                Text = this.Text,
                PrimaryEntityName = this.PrimaryEntityName,
                IsVisited = this.IsVisited
            };

            foreach (ProcessDetailTN childNode in this.Nodes)
            {
                var newChildNode = childNode.CloneMe();
                newNode.Nodes.Add(newChildNode);
            }

            return newNode;
        }
    }

    public static class QueryEntityInfo
    {
        public static bool? IsBackgroundProcess = true;
        public static EntityDetail AOMetadata = null;
        public static EntityDetail PSMetadata = null;
        public static EntityDetail TLMetadata = null;

        public static string AOLayoutXML {
            get
            {
                #region layoutxml
                return @"<grid name='asyncoperations' object='4700' jump='name' select='1' icon='1' preview='0'>
	                        <row name='asyncoperation' id='asyncoperationid' multiobjectidfield='operationtype'>
		                        <cell name='operationtype' width='100' />
		                        <cell name='name' width='300' />
		                        <cell name='regardingobjectid' width='150' />
		                        <cell name='statuscode' width='100' />
		                        <cell name='statecode' width='100' />
		                        <cell name='ownerid' width='200' />
		                        <cell name='startedon' width='125' />
		                        <cell name='executiontimespan' width='125' />
		                        <cell name='createdon' width='125' />
		                        <cell name='friendlymessage' width='125' />
		                        <cell name='hostid' width='125' />
		                        <cell name='messagename' width='125' />
		                        <cell name='depth' width='125' />
		                        <cell name='primaryentitytype' width='125' />
		                        <cell name='message' width='125' />
		                        <cell name='completedon' width='125' />
		                        <cell name='requestid' width='125' />
		                        <cell name='modifiedonbehalfby' width='125' />
		                        <cell name='workflowstagename' width='125' />
		                        <cell name='expanderstarttime' width='125' />
		                        <cell name='owningextensionid' width='125' />
		                        <cell name='createdonbehalfby' width='125' />
		                        <cell name='workload' width='125' />
		                        <cell name='modifiedon' width='125' />
		                        <cell name='dependencytoken' width='125' />
		                        <cell name='subtype' width='125' />
		                        <cell name='sequence' width='125' />
		                        <cell name='owningteam' width='125' />
		                        <cell name='createdby' width='125' />
		                        <cell name='modifiedby' width='125' />
		                        <cell name='workflowactivationid' width='125' />
		                        <cell name='owninguser' width='125' />
		                        <cell name='recurrencestarttime' width='125' />
		                        <cell name='owningbusinessunit' width='125' />
		                        <cell name='iswaitingforevent' width='125' />
		                        <cell name='recurrencepattern' width='125' />
		                        <cell name='errorcode' width='125' />
		                        <cell name='rootexecutioncontext' width='125' />
		                        <cell name='retrycount' width='125' />
		                        <cell name='postponeuntil' width='125' />
		                        <cell name='correlationid' width='125' />
		                        <cell name='correlationupdatedtime' width='125' />
	                        </row>
                        </grid>";
                #endregion
            }
        }

        public static string AOFetchXML {
            get
            {
                #region fetchxml
                return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'  >
                              <entity name='asyncoperation' >
                                <attribute name='parentpluginexecutionid' />
                                <attribute name='executiontimespan' />
                                <attribute name='createdon' />
                                <attribute name='primaryentitytypename' />
                                <attribute name='friendlymessage' />
                                <attribute name='statuscodename' />
                                <attribute name='hostid' />
                                <attribute name='startedon' />
                                <attribute name='messagename' />
                                <attribute name='owneridtype' />
                                <attribute name='asyncoperationid' />
                                <attribute name='depth' />
                                <attribute name='primaryentitytype' />
                                <attribute name='modifiedonbehalfbyyominame' />
                                <attribute name='message' />
                                <attribute name='regardingobjectidyominame' />
                                <attribute name='owneridname' />
                                <attribute name='timezoneruleversionnumber' />
                                <attribute name='completedon' />
                                <attribute name='requestid' />
                                <attribute name='createdonbehalfbyyominame' />
                                <attribute name='modifiedonbehalfby' />
                                <attribute name='regardingobjectid' />
                                <attribute name='statecode' />
                                <attribute name='workflowstagename' />
                                <attribute name='workflowactivationidname' />
                                <attribute name='expanderstarttime' />
                                <attribute name='owningextensionid' />
                                <attribute name='name' />
                                <attribute name='createdonbehalfbyname' />
                                <attribute name='createdonbehalfby' />
                                <attribute name='regardingobjectidname' />
                                <attribute name='utcconversiontimezonecode' />
                                <attribute name='workload' />
                                <attribute name='ownerid' />
                                <attribute name='modifiedon' />
                                <attribute name='owneridyominame' />
                                <attribute name='dependencytoken' />
                                <attribute name='createdbyname' />
                                <attribute name='regardingobjecttypecode' />
                                <attribute name='modifiedonbehalfbyname' />
                                <attribute name='owningextensiontypecode' />
                                <attribute name='subtype' />
                                <attribute name='sequence' />
                                <attribute name='operationtypename' />
                                <attribute name='owningteam' />
                                <attribute name='createdby' />
                                <attribute name='modifiedby' />
                                <attribute name='workflowactivationid' />
                                <attribute name='owningextensionidname' />
                                <attribute name='createdbyyominame' />
                                <attribute name='owninguser' />
                                <attribute name='recurrencestarttime' />
                                <attribute name='owningbusinessunit' />
                                <attribute name='modifiedbyyominame' />
                                <attribute name='iswaitingforevent' />
                                <attribute name='recurrencepattern' />
                                <attribute name='errorcode' />
                                <attribute name='operationtype' />
                                <attribute name='statuscode' />
                                <attribute name='modifiedbyname' />
                                <attribute name='rootexecutioncontext' />
                                <attribute name='retrycount' />
                                <attribute name='postponeuntil' />
                                <attribute name='correlationid' />
                                <attribute name='statecodename' />
                                <attribute name='correlationupdatedtime' />
                                {0}
                                <order attribute='startedon' descending='true' />
                              </entity>
                            </fetch>";
                #endregion
            }
        }

        public static string PSLayoutXML
        {
            get
            {
                #region layoutxml
                return @"<grid name='resultset' object='4710' jump='executedon' select='1' icon='1' preview='1'>
	                        <row name='result' id='processsessionid'>
		                        <cell name='name' width='125' />
                                <cell name='processid' width='250' />
                                <cell name='regardingobjectid' width='150' />
                                <cell name='statuscode' width='125' />
                                <cell name='statecode' width='125' />
                                <cell name='startedon' width='125' />
                                <cell name='startedby' width='125' />
                                <cell name='completedon' width='125' />
                                <cell name='stepname' width='125' />
                                <cell name='ownerid' width='125' />
                                <cell name='activityname' width='125' />
                                <cell name='createdby' width='125' />
                                <cell name='owningteam' width='125' />
                                <cell name='executedon' width='125' />
                                <cell name='processsessionid' width='125' />
                                <cell name='errorcode' width='125' />
                                <cell name='modifiedonbehalfby' width='125' />
                                <cell name='comments' width='125' />
                                <cell name='canceledon' width='125' />
                                <cell name='originatingsessionid' width='125' />
                                <cell name='createdon' width='125' />
                                <cell name='nextlinkedsessionid' width='125' />
                                <cell name='completedby' width='125' />
                                <cell name='executedby' width='125' />
                                <cell name='previouslinkedsessionid' width='125' />
                                <cell name='modifiedby' width='125' />
                                <cell name='processstagename' width='125' />
                                <cell name='processstate' width='125' />
                                <cell name='canceledby' width='125' />
                                <cell name='modifiedon' width='125' />
                                <cell name='owningbusinessunit' width='125' />
                                <cell name='createdonbehalfby' width='125' />
                                <cell name='owninguser' width='125' />
                                <cell name='inputarguments' width='125' />
	                        </row>
                        </grid>";
                #endregion
            }
        }

        public static string PSFetchXML
        {
            get
            {
                #region fetchxml
                return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true' >
                          <entity name='processsession' >
                            <attribute name='activityname' />
                            <attribute name='owneridtype' />
                            <attribute name='createdby' />
                            <attribute name='owningteam' />
                            <attribute name='executedon' />
                            <attribute name='statecodename' />
                            <attribute name='regardingobjectidname' />
                            <attribute name='processsessionid' />
                            <attribute name='errorcode' />
                            <attribute name='originatingsessionidname' />
                            <attribute name='modifiedonbehalfby' />
                            <attribute name='comments' />
                            <attribute name='canceledon' />
                            <attribute name='canceledbyname' />
                            <attribute name='statecode' />
                            <attribute name='originatingsessionid' />
                            <attribute name='createdon' />
                            <attribute name='nextlinkedsessionid' />
                            <attribute name='completedby' />
                            <attribute name='processid' />
                            <attribute name='stepname' />
                            <attribute name='previouslinkedsessionidname' />
                            <attribute name='regardingobjecttypecode' />
                            <attribute name='startedby' />
                            <attribute name='executedby' />
                            <attribute name='completedon' />
                            <attribute name='processidname' />
                            <attribute name='previouslinkedsessionid' />
                            <attribute name='statuscode' />
                            <attribute name='regardingobjectid' />
                            <attribute name='modifiedonbehalfbyname' />
                            <attribute name='startedbyname' />
                            <attribute name='modifiedby' />
                            <attribute name='processstagename' />
                            <attribute name='completedbyname' />
                            <attribute name='createdbyname' />
                            <attribute name='modifiedbyname' />
                            <attribute name='processstate' />
                            <attribute name='canceledby' />
                            <attribute name='ownerid' />
                            <attribute name='nextlinkedsessionidname' />
                            <attribute name='statuscodename' />
                            <attribute name='createdonbehalfbyname' />
                            <attribute name='name' />
                            <attribute name='modifiedon' />
                            <attribute name='startedon' />
                            <attribute name='owningbusinessunit' />
                            <attribute name='createdonbehalfby' />
                            <attribute name='owneridname' />
                            <attribute name='owninguser' />
                            <attribute name='inputarguments' />
                            <attribute name='executedbyname' />
                            {0}
                            <order attribute='startedon' descending='true' />
                          </entity>
                        </fetch>";
                #endregion
            }
        }

        public static string TLLayoutXML
        {
            get
            {
                #region layoutxml
                return @"<grid name='resultset' object='4619' jump='messagename' select='1' icon='1' preview='0'>
	                        <row name='result' id='plugintracelogid'>
		                        <cell name='issystemcreated' width='125' />
                                <cell name='operationtype' width='125' />
                                <cell name='typename' width='300' />
                                <cell name='messagename' width='200' />
                                <cell name='performanceexecutionstarttime' width='125' />
                                <cell name='performanceexecutionduration' width='125' />
                                <cell name='performanceconstructorduration' width='125' />
                                <cell name='depth' width='125' />
                                <cell name='mode' width='125' />
                                <cell name='configuration' width='125' />
                                <cell name='secureconfiguration' width='125' />
                                <cell name='createdon' width='125' />
                                <cell name='createdby' width='125' />
                                <cell name='organizationid' width='125' />
                                <cell name='pluginstepid' width='125' />
                                <cell name='profile' width='125' />
                                <cell name='primaryentity' width='125' />
                                <cell name='exceptiondetails' width='125' />
                                <cell name='messageblock' width='125' />
                                <cell name='createdonbehalfby' width='125' />
                                <cell name='requestid' width='125' />
                                <cell name='correlationid' width='125' />
	                        </row>
                        </grid>";
                #endregion
            }
        }

        public static string TLFetchXML
        {
            get
            {
                #region fetchxml
                return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='true' >
	                        <entity name='plugintracelog'>
		                        <attribute name='plugintracelogid' />
		                        <attribute name='messagename' />
		                        <attribute name='issystemcreated' />
		                        <attribute name='operationtype' />
		                        <attribute name='typename' />
		                        <attribute name='performanceexecutionstarttime' />
		                        <attribute name='performanceexecutionduration' />
	                            <attribute name='createdonbehalfby' />
		                        <attribute name='createdby' />
		                        <attribute name='configuration' />
		                        <attribute name='secureconfiguration' />
		                        <attribute name='modename' />
		                        <attribute name='organizationid' />
		                        <attribute name='pluginstepid' />
		                        <attribute name='profile' />
		                        <attribute name='primaryentity' />
		                        <attribute name='exceptiondetails' />
		                        <attribute name='messageblock' />
		                        <attribute name='createdbyname' />
		                        <attribute name='createdon' />
		                        <attribute name='typename' />
		                        <attribute name='createdonbehalfbyname' />
		                        <attribute name='requestid' />
		                        <attribute name='performanceconstructorduration' />
		                        <attribute name='issystemcreatedname' />
		                        <attribute name='depth' />
		                        <attribute name='mode' />
		                        <attribute name='issystemcreated' />
		                        <attribute name='correlationid' />
                                {0}
                                <order attribute='performanceexecutionstarttime' descending='true' />
                              </entity>
                            </fetch>";
                #endregion
            }
        }
    }
}
