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
using System.Collections.Specialized;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Metadata;

namespace CRMProcessExplorer
{
    public class CRMProcessExplorerManager : IDisposable
    {
        private IOrganizationService Service = null;

        #region Disposable implementation
        private bool disposed = false; // to detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Service != null)
                        Service = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CRMProcessExplorerManager()
        {
            Dispose(false);
        }
        #endregion

        public CRMProcessExplorerManager(IOrganizationService service)
        {
            this.Service = service;
        }

        public bool GetEntityRecordInfo(string entityName, string primaryFieldName, Guid entityId, out string value)
        {
            value = string.Empty;
            var record = Service.RetrieveSafe(entityName, entityId, new ColumnSet(primaryFieldName));
            if (record != null)
            {
                value = record.GetAttributeValueSafe<string>(primaryFieldName);
                return true;
            }
            else
                return false;
        }

        public List<EntityMetadata> GetEntities(string entityname = null)
        {
            var entityQueryExpression = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "DisplayName", "Attributes",
                                "ObjectTypeCode", "PrimaryNameAttribute", "PrimaryIdAttribute"),
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("DisplayName", "LogicalName", "AttributeType",
                                    "AttributeOf", "OptionSet"),
                },
            };

            if (!string.IsNullOrEmpty(entityname))
                entityQueryExpression.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityname));

            return Service.GetAllMetaData(entityQueryExpression).ToList();
        }

        public List<Entity> RetrieveAsyncOperations(Guid? regardingObjectId)
        {
            var query = new QueryExpression
            {
                EntityName = "asyncoperation",
                ColumnSet = new ColumnSet(true),
            };
            query.Criteria.AddCondition("uniquename", ConditionOperator.NotNull);

            if (regardingObjectId.HasValue)
                query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, regardingObjectId.Value);

            query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));

            try
            {
                var response = Service.GetAllData(query);
                var results = response.Entities.ToList();
                return results;
            }
            catch
            {
                throw;
            }
        }

        public List<Entity> GetSavedQuery(string entityName)
        {
            var query = new QueryExpression
            {
                EntityName = "savedquery",
                ColumnSet = new ColumnSet(true),
            };
            query.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, entityName);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.Conditions.Add(new ConditionExpression("querytype", ConditionOperator.Equal, 4));
            filter.Conditions.Add(new ConditionExpression("querytype", ConditionOperator.Equal, 0));

            query.Criteria.AddFilter(filter);
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));

            return Service.GetAllData(query).Entities.ToList();
        }

        public string ExecuteFetchXML(string fetchXml)
        {
            return ((ExecuteFetchResponse)Service.Execute(new ExecuteFetchRequest { FetchXml = fetchXml })).FetchXmlResult;
        }

        public string GetProcessesResult(string regardingObjectId = null, string dtFrom = null, string dtTo = null,
            string operationType = null, string state = null, string status = null)
        {
            var addCondition = string.Empty;
            if(!string.IsNullOrEmpty(operationType) && operationType != "-1")
            {
                addCondition = $"<condition attribute='operationtype' operator='eq' value='{operationType}' />";
            }
            if (!string.IsNullOrEmpty(state) && state != "-1")
            {
                addCondition += $"<condition attribute='statecode' operator='eq' value='{state}' />";
            }
            if (!string.IsNullOrEmpty(status) && status != "-1")
            {
                addCondition += $"<condition attribute='statuscode' operator='eq' value='{status}' />";
            }

            var condition = string.Empty;
            if (!string.IsNullOrEmpty(regardingObjectId))
            {
                condition = $@"<filter type='and'>
                                    <condition attribute='regardingobjectid' operator='eq' value='{regardingObjectId}' />
                                    {addCondition}
                              </filter>";
            }
            else if (!string.IsNullOrEmpty(dtFrom) && !string.IsNullOrEmpty(dtTo))
            {
                condition = $@"<filter type='and'>
                                  <condition attribute='createdon' operator='on-or-after' value='{dtFrom}' />
                                  <condition attribute='createdon' operator='on-or-before' value='{dtTo}' />
                                  {addCondition}
                                </filter>";
            }
            else
                return string.Empty;

            #region fetchxml
            var isBGP = QueryEntityInfo.IsBackgroundProcess;
            var fetchXml = string.Format((!isBGP.HasValue ? QueryEntityInfo.TLFetchXML : (isBGP.Value ? QueryEntityInfo.AOFetchXML : QueryEntityInfo.PSFetchXML)), condition);
            #endregion

            return ExecuteFetchXML(fetchXml);
        }

        public Dictionary<string, string> GetStringMap(string entityName, string attributeName)
        {
            var result = new Dictionary<string, string>();
            var query = new QueryExpression
            {
                EntityName = "stringmap",
                ColumnSet = new ColumnSet("attributevalue", "value"),
            };
            query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, entityName);
            query.Criteria.AddCondition("attributename", ConditionOperator.Equal, attributeName);

            query.Orders.Add(new OrderExpression("displayorder", OrderType.Ascending));

            result.Add("-1", "All");
            foreach (var item in Service.RetrieveMultiple(query).Entities)
            {
                result.Add(item.GetAttributeValueSafe<int>("attributevalue").ToString(), item.GetAttributeValueSafe<string>("value"));
            }

            return result;
        }

        public List<Entity> RetrieveDependentComponents(Guid workflowId, string workflowName)
        {
            var request = new RetrieveDependentComponentsRequest
            {
                ObjectId = workflowId,
                ComponentType = 29
            };

            var response = (RetrieveDependentComponentsResponse)Service.Execute(request);
            var result = (EntityCollection)response.Results["EntityCollection"];
            return result.Entities.ToList();
        }

        public List<Entity> RetrieveRequiredComponents(Guid workflowId)
        {
            var request = new RetrieveRequiredComponentsRequest
            {
                ObjectId = workflowId,
                ComponentType = 29
            };

            var response = (RetrieveRequiredComponentsResponse)Service.Execute(request);
            var result = (EntityCollection)response.Results["EntityCollection"];
            return result.Entities.ToList();
        }

        public ProcessDetail GetPluginTypeById(EntityDetail entityDetail, Guid id)
        {
            var  pa = entityDetail.PluginAssemblies.Where(x => x.Id == id).FirstOrDefault();
            if (pa == null)
            {
                var pt = Service.Retrieve("plugintype", id, new ColumnSet(new string[] { "name",  }));
                pa = new ProcessDetail();
                pa.Id = id;
                pa.Name = pt.GetAttributeValueSafe<string>("name");
                pa.Type = ProcessDetail.eTypes.CustomCode;
                entityDetail.PluginAssemblies.Add(pa);
            }
            return pa;
        }

        public ProcessDetail GetComponentByType(EntityDetail entityDetail, List<ProcessDetail> pdList, int type, Guid id)
        {
            ProcessDetail pd = null;
            switch (type)
            {
                case 29:
                    pd = pdList.Where(x => x.Id == id).FirstOrDefault();
                    break;
                case 90:
                    pd = GetPluginTypeById(entityDetail, id);
                    pd.Type = ProcessDetail.eTypes.CustomCode;
                    break;
            }
            return pd;
        }

        public List<ProcessDetail> GetWorkflowsByEntity(EntityDetail entityDetail)
        {
            if (entityDetail == null) return null;

            List<ProcessDetail> processInfo = new List<ProcessDetail>();
            QueryExpression query = new QueryExpression("workflow");
            query.ColumnSet.AddColumns(new string[] {"mode", "primaryentity","name","scope","statecode","type",
                                                     "uniquename","solutionid","category"});
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("type", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("primaryentity", ConditionOperator.Equal, entityDetail.Metadata.LogicalName);

            var wfList = Service.RetrieveMultiple(query);
            foreach (var wf in wfList.Entities)
            {
                var workflow = new ProcessDetail();
                var category = wf.GetAttributeValueSafe<OptionSetValue>("category").Value;
                workflow.Category = (ProcessDetail.eCategories)category;
                workflow.Type = ProcessDetail.eTypes.Workflow;
                workflow.Id = wf.Id;
                workflow.Name = wf.GetAttributeValueSafe<string>("name");
                workflow.PrimaryEntityName = wf.GetAttributeValueSafe<string>("primaryentity");


                processInfo.Add(workflow);
            }

            processInfo.ForEach(p => {
                var rcList = RetrieveRequiredComponents(p.Id);

                rcList.ForEach(r =>
                {
                    int componentType = r.GetAttributeValueSafe<OptionSetValue>("requiredcomponenttype").Value;
                });

                foreach (var rc in rcList)
                {
                    var cType = rc.GetAttributeValue<OptionSetValue>("requiredcomponenttype").Value;
                    var rcId = rc.GetAttributeValue<Guid>("requiredcomponentobjectid");

                    if (cType == (int)ProcessDetail.eTypes.Workflow || cType == (int)ProcessDetail.eTypes.CustomCode)
                    {
                        var w = GetComponentByType(entityDetail, processInfo, cType, rcId);
                        if (w != null)
                        {
                            p.ChildProcess.Add(w);
                        }
                    }
                }
            });

            // get plugins info
            var ppInfo = GetPluginsByEntity(entityDetail);
            if (ppInfo != null && ppInfo.Count > 0) processInfo.AddRange(ppInfo);

            return processInfo;
        }

        public List<ProcessDetail> GetPluginsByEntity(EntityDetail entityDetail)
        {
            if (entityDetail == null) return null;
            List<ProcessDetail> processInfo = new List<ProcessDetail>();
            try
            {

                var layoutXml = @"<grid name='plugintype'>
	                        <row>
		                        <cell name='name' width='300' />
		                        <cell name='assemblyname' width='150' />
		                        <cell name='step.rank' width='100' />
		                        <cell name='step.stage' width='100' />
		                        <cell name='step.description' width='200' />
		                        <cell name='step.impersonatinguserid' width='125' />
		                        <cell name='step.sdkmessageid' width='125' />
		                        <cell name='filter.primaryobjecttypecode' width='125' />
                            </row></grid>";

                var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='true' >
                      <entity name='plugintype' >
                        <attribute name='plugintypeid' />
                        <attribute name='assemblyname' />
                        <attribute name='name' />
                        <filter>
                          <condition attribute='componentstate' operator='eq' value='0' />
                        </filter>
                        <link-entity name='sdkmessageprocessingstep' from='plugintypeid' to='plugintypeid' link-type='inner' alias='step' >
                          <attribute name='rank' />
                          <attribute name='stage' />
                          <attribute name='description' />
                          <attribute name='impersonatinguserid' />
                          <attribute name='sdkmessageid' />
                          <filter>
                            <condition attribute='ishidden' operator='eq' value='0' />
                            <condition attribute='iscustomizable' operator='eq' value='1' />
                            <condition attribute='invocationsource' operator='eq' value='0' />
                          </filter>
                          <link-entity name='sdkmessagefilter' from='sdkmessagefilterid' to='sdkmessagefilterid' link-type='outer' alias='filter' >
                            <attribute name='primaryobjecttypecode' />
                            <filter type='and' >
                              <condition attribute='primaryobjecttypecode' operator='in' >
                                <value>0</value>
                                <value>{entityDetail.Metadata.ObjectTypeCode}</value>
                              </condition>
                              <condition attribute='isvisible' operator='eq' value='1' />
                              <condition attribute='componentstate' operator='eq' value='0' />
                            </filter>
                          </link-entity>
                        </link-entity>
                        <link-entity name='pluginassembly' from='pluginassemblyid' to='pluginassemblyid' link-type='inner' alias='assembly' >
                          <filter>
                            <condition attribute='componentstate' operator='eq' value='0' />
                            <condition attribute='iscustomizable' operator='eq' value='1' />
                            <condition attribute='ishidden' operator='eq' value='0' />
                            <condition attribute='ismanaged' operator='eq' value='0' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

                var resultXml = ExecuteFetchXML(fetchXml);
                var isMoreRecords = false;
                var rowList = Helper.ProcessXML(entityDetail.Metadata, layoutXml, resultXml, out isMoreRecords);
                rowList.ForEach(r =>
                {
                    var rLength = r.Length;
                    var workflow = new ProcessDetail();
                    workflow.Category = ProcessDetail.eCategories.Plugins;
                    workflow.Type = ProcessDetail.eTypes.CustomCode;
                    workflow.Id = new Guid(r[rLength - 3].ToStringNullSafe());
                    workflow.Name = GetName(r, rLength);
                    workflow.PrimaryEntityName = entityDetail.Metadata.LogicalName;

                    processInfo.Add(workflow);
                });
            }
            catch { }
            return processInfo;
        }

        private string GetName(object[] row, int rLength)
        {
            if (rLength > 11)
            {
                //Name::Message::Calling User/??
                return $"{row[1]}::{row[7]}::{(string.IsNullOrEmpty(row[6].ToStringNullSafe()) ? "Calling User" : row[6])}";
            }
            else
            {
                // Name
                return row[rLength - 2].ToStringNullSafe();
            }
        }
    }
}
