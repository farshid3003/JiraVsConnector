using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;

namespace Atlassian.plvs.explorer.treeNodes {
    class AssigneeDropZoneWorker : DropZone.DropZoneWorker {
        private readonly AbstractJiraServerFacade facade;
        private readonly JiraServer server;
        private readonly JiraUser user;

        public AssigneeDropZoneWorker(AbstractJiraServerFacade facade, JiraServer server, JiraUser user) {
            this.facade = facade;
            this.server = server;
            this.user = user;
        }

        public DropZone.PerformAction Action { get { return dropAction; } }

        private void dropAction(JiraIssue issue, bool add) {
            JiraField field = new JiraField("assignee", null) { Values = new List<string>() };
            if (add) {
                throw new ArgumentException("Unable to have multiple assignees in one issue");
            }

            // skip if issue already has this user
            if (field.Values.Contains(user.Id)) return;

            field.Values.Add(user.Id);
            facade.updateIssue(issue, new List<JiraField> { field });
        }

        public string ZoneName { get { return "Assign to: " + user; } }

        public string ZoneKey { get { return server.GUID + "_priority_" + user.Id; } }

        public bool CanAdd { get { return false; } }

        public string IssueWillBeAddedText { get { return "Unavailable"; } }

        public string issueWillBeMovedText { get { return "Issue will be assigned to \"" + user + "\""; } }

        public string InitialText { get { return "Drag issues here to assign them to \"" + user + "\""; } }
    }
}
