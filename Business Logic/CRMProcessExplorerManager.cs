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
    }
}
