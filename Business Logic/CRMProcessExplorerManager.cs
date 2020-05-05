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

        public List<Entity> RetrieveRequiredComponents(Guid workflowId, string workflowName)
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
                pa.Type = ProcessDetail.eTypes.PluginType;
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
                    pd.Type = ProcessDetail.eTypes.PluginType;
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
                var rcList = RetrieveRequiredComponents(p.Id, p.Name);

                rcList.ForEach(r =>
                {
                    int componentType = r.GetAttributeValueSafe<OptionSetValue>("requiredcomponenttype").Value;
                });

                foreach (var rc in rcList)
                {
                    var cType = rc.GetAttributeValue<OptionSetValue>("requiredcomponenttype").Value;
                    var rcId = rc.GetAttributeValue<Guid>("requiredcomponentobjectid");

                    if (cType == (int)ProcessDetail.eTypes.Workflow || cType == (int)ProcessDetail.eTypes.PluginType)
                    {
                        var w = GetComponentByType(entityDetail, processInfo, cType, rcId);
                        if (w != null)
                        {
                            p.ChildProcess.Add(w);
                        }
                    }
                }
            });

            return processInfo;
        }
    }
}
