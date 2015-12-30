using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.fields {
    public interface FieldFiller {
        List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject);
        string getSettablePropertyName(string id, JiraIssue issue, object rawIssueObject);
    }
}