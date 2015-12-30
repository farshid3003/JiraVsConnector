using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.models.jira.fields {
    public class CustomFieldFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            var issueToken = rawIssueObject as JToken;
            if (issueToken != null) {
                var value = JiraIssueUtils.getRawIssueObjectPropertyValue<object>(rawIssueObject, field);
                if (value == null || (value is JToken && !(value as JToken).HasValues)) return null;
                var arr = value as JArray;
                return arr != null ? arr.Select(el => el.ToString()).ToList() : new List<string> { value.ToString() };
            }

            var customFieldValues = JiraIssueUtils.getRawIssueObjectPropertyValueArray<object>(rawIssueObject, "customFieldValues");

            if (customFieldValues == null || customFieldValues.Length == 0) {
                return null;
            }

            foreach (var customFieldValue in customFieldValues) {
                var property = customFieldValue.GetType().GetProperty("customfieldId");
                if (property == null) continue;
                var fieldId = property.GetValue(customFieldValue, null) as string;
                if (fieldId == null || !fieldId.Equals(field)) continue;
                property = customFieldValue.GetType().GetProperty("values");
                if (property == null) continue;
                var values = property.GetValue(customFieldValue, null) as object[];

                return (from val in values select val.ToString()).ToList();
            }
            return null;
        }

        public string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject) {
            return null;
        }
    }
}