using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CRMProcessExplorer
{
    public static class Helper
    {
        public static Entity RetrieveSafe(this IOrganizationService service, string entityName, Guid id, ColumnSet columnSet)
        {
            Entity entity = null;
            try
            {
                entity = service.Retrieve(entityName, id, columnSet);
            }
            catch (Exception)
            {
            }
            return entity;
        }
        public static T GetAttributeValueSafe<T>(this Entity entity, string attribute)
        {
            return entity != null && entity.Contains(attribute) ? entity.GetAttributeValue<T>(attribute) : default(T);
        }
        public static string GetFormattedValueSafe(this Entity entity, string attribute)
        {
            return entity != null && entity.FormattedValues.Keys.Contains(attribute) ? entity.FormattedValues[attribute] : null;
        }
        public static string ToStringNullSafe(this object obj)
        {
            return (obj ?? String.Empty).ToString();
        }
        public static T GetSafe<T>(this Entity entity, string attribute)
        {
            attribute = attribute.ToLower();
            if (entity != null && entity.Contains(attribute))
                return entity.GetAttributeValue<T>(attribute);
            return default(T);
        }

        public static EntityCollection GetAllData(this IOrganizationService service, QueryExpression queryExp)
        {
            return service.GetAllData(queryExp, 5000);
        }
        public static EntityCollection GetAllData(this IOrganizationService service, QueryExpression queryExp, int recCount, bool isSetNoLock = true)
        {
            var entityCol = new EntityCollection();
            try
            {
                int pageNumber = 1;
                RetrieveMultipleRequest multiRequest;
                var multiResponse = new RetrieveMultipleResponse();

                do
                {
                    queryExp.NoLock = isSetNoLock;
                    queryExp.PageInfo.Count = recCount;
                    queryExp.PageInfo.PagingCookie = (pageNumber == 1) ? null : multiResponse.EntityCollection.PagingCookie;
                    queryExp.PageInfo.PageNumber = pageNumber++;

                    multiRequest = new RetrieveMultipleRequest();
                    multiRequest.Query = queryExp;
                    multiResponse = (RetrieveMultipleResponse)service.Execute(multiRequest);

                    entityCol.Entities.AddRange(multiResponse.EntityCollection.Entities);

                    // if the record count is less than 5k no need to check for morerecords, just terminate
                    if (recCount < 5000) break;
                }
                while (multiResponse.EntityCollection.MoreRecords);
            }
            catch
            {
                throw;
            }
            return entityCol;
        }

        public static EntityMetadataCollection GetAllMetaData(this IOrganizationService service, EntityQueryExpression queryExp)
        {
            var entityCol = new EntityMetadataCollection();
            try
            {
                var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest
                {
                    Query = queryExp,
                    ClientVersionStamp = null
                };
                entityCol = ((RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest)).EntityMetadata;
            }
            catch
            {
                throw;
            }
            return entityCol;
        }

        public static string GetCRMUrl(this string url, string entityName, string entityId, bool isForceUCI = false, bool isETC = false)
        {
            var urlFormat = string.Empty;

            if (isETC)
                urlFormat = $"{url}main.aspx?pagetype=entityrecord&etc={entityName}&id={entityId}";
            else
                urlFormat = $"{url}main.aspx?pagetype=entityrecord&etn={entityName}&id={entityId}";

            if (isForceUCI)
                return $"{urlFormat}&forceUCI=1";
            else
                return $"{urlFormat}&settingsonly=true";
        }

        public static List<object[]> ProcessXML(EntityMetadata metadata, string layoutXml, string resultXml, out bool isMoreRecords)
        {
            isMoreRecords = false;
            if (string.IsNullOrEmpty(resultXml)) return null;

            List<object[]> rowList = new List<object[]>();
            var layout = new XmlDocument();
            layout.LoadXml(layoutXml);

            var resultDoc = new XmlDocument();
            resultDoc.LoadXml(resultXml);

            object[] row;
            var rows = resultDoc.SelectNodes("//result");
            var loop = 0;
            foreach (XmlNode node in rows)
            {
                var col = 0;
                var cols = layout.SelectNodes("//cell");
                row = new object[cols.Count + 4];
                row[col++] = ++loop; // SNo
                foreach (XmlNode cell in cols)
                {
                    var attributeNode = node.SelectSingleNode(cell.Attributes["name"].Value);
                    string value = "";
                    if (attributeNode == null)
                        value = string.Empty;
                    else
                    {
                        var attr = attributeNode.Attributes["name"];
                        value = attr?.Value ?? attributeNode.InnerText;
                    }
                    row[col++] = value;
                }
                row[col++] = node.SelectSingleNode(metadata.PrimaryIdAttribute)?.InnerText ?? node.SelectSingleNode(metadata.PrimaryIdAttribute)?.Value ?? string.Empty;
                row[col++] = node.SelectSingleNode(metadata.PrimaryNameAttribute)?.InnerText ?? string.Empty;
                row[col] = rows[loop - 1].OuterXml;
                rowList.Add(row);
            }

            if (resultDoc.SelectSingleNode("resultset").Attributes["morerecords"].Value == "1")
                isMoreRecords = true;
            return rowList;
        }
    }
}
