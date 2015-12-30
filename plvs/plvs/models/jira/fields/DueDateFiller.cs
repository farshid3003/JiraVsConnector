using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class DueDateFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            DateTime? dueDate = JiraIssueUtils.getRawIssueObjectPropertyValue<DateTime?>(rawIssueObject, "duedate");

            if (!dueDate.HasValue) {
                return new List<string>();
            }

            string dateString = JiraIssueUtils.getShortDateStringFromDateTime(issue.ServerLanguage, dueDate.Value);
            List<string> result = new List<string> {dateString};
            return result;
        }

        public string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject) {
            return null;
        }
    }
}