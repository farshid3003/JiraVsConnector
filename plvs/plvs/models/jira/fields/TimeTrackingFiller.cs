using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class TimeTrackingFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {
            return issue.TimeSpent == null 
                       ? new List<string> {translate(issue.OriginalEstimate)} 
                       : new List<string> {translate(issue.RemainingEstimate)};
        }

        public string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject) {
            return issue.TimeSpent == null ? "originalEstimate" : "remainingEstimate";
        }

        public static string translate(string displayValue) {
            if (displayValue != null) {
                displayValue = displayValue.Replace(" weeks", "w");
                displayValue = displayValue.Replace(" week", "w");
                displayValue = displayValue.Replace(" days", "d");
                displayValue = displayValue.Replace(" day", "d");
                displayValue = displayValue.Replace(" hours", "h");
                displayValue = displayValue.Replace(" hour", "h");
                displayValue = displayValue.Replace(" minutes", "m");
                displayValue = displayValue.Replace(" minute", "m");
                displayValue = displayValue.Replace(",", "");
            }
            return displayValue;
        }
    }
}