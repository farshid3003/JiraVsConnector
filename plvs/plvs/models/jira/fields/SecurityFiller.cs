using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class SecurityFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {
            if (issue.SecurityLevel == null) {
                return null;
            }
            List<string> result = new List<string> {issue.SecurityLevel.Id.ToString()};
            return result;
        }

        public string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject) {
            return "id";
        }
    }
}