using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using System.Linq;
using Atlassian.plvs.util.jira;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.models.jira.fields {
    public class AffectsVersionsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            object[] value = JiraIssueUtils.getRawIssueObjectPropertyValueArray<object>(rawIssueObject, "affectsVersions");

            if (value == null || value.Length == 0) {
                return null;
            }

            if (rawIssueObject is JToken) {
                return value.Select(v => v as JToken).Select(vt => vt["id"].Value<string>()).ToList();
            }

            return (from v in value.ToList()
                    let prop = v.GetType().GetProperty("id")
                    where prop != null
                    select (string)prop.GetValue(v, null)).ToList();
        }

        public string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject) {
            return "id";
        }
    }
}