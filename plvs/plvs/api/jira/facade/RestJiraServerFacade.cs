using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.api.jira.facade {
    public class RestJiraServerFacade : AbstractJiraServerFacade {
        public override void login(JiraServer server) {
            restSupported(server);
            using (var rest = new RestClient(server)) {
                rest.restLogin();
            }
        }

        public override void dropAllSessions() {
            RestClient.clearSessions();
            base.dropAllSessions();
        }

        public override string getSoapToken(JiraServer server) {
            throw new NotImplementedException();
        }

        public override bool supportsGh(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.supportsGh();
            }
        }

        public override List<RapidBoard> getGhBoards(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getGhBoards();
            }
        }

        public override List<Sprint> getGhSprints(JiraServer server, int boardId) {
            using (var rest = new RestClient(server)) {
                return rest.getGhSprints(boardId);
            }
        }

        public override List<string> getIssueKeysForSprint(JiraServer server, Sprint sprint) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueKeysForSprint(sprint.BoardId, sprint.Id);
            }
        }

        public override List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            using (var rest = new RestClient(server)) {
                return rest.getSavedFilterIssues(filter, "priority", "desc", start, count);
            }
        }

        public override List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            using (var rest = new RestClient(server)) {
                return rest.getCustomFilterIssues(filter, "desc", start, count);
            }
        }

        public override JiraIssue getIssue(JiraServer server, string key) {
            using (var rest = new RestClient(server)) {
                return rest.getIssue(key);
            }
        }

        public override string getRenderedContent(JiraIssue issue, string markup) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getRenderedContent(issue.Key, -1, -1, markup);
//                return setSessionCookieAndWrapExceptions(issue.Server, rest, () => rest.getRenderedContent(issue.Key, -1, -1, markup));
            }
        }

        public override string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            using (var rest = new RestClient(server)) {
                return rest.getRenderedContent(null, issueTypeId, project.Id, markup);
//                return setSessionCookieAndWrapExceptions(server, rest, () => rest.getRenderedContent(null, issueTypeId, project.Id, markup));
            }
        }


        public override List<JiraProject> getProjects(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getProjects();
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(false);
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(true);
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(true, project);
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(false, project);
            }
        }

        public override List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getSavedFilters();
            }
        }

        public override List<JiraNamedEntity> getPriorities(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getPriorities();
            }
        }

        public override List<JiraNamedEntity> getStatuses(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getStatuses();
            }
        }

        public override List<JiraNamedEntity> getResolutions(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getResolutions();
            }
        }

        public override void addComment(JiraIssue issue, string comment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.addComment(issue, comment);
            }
        }

        public override List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getActionsForIssue(issue);
            }
        }

        public override List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getFieldsForAction(issue, actionId);
            }
        }

        public override void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            using (var rest = new RestClient(issue.Server)) {
                rest.runIssueActionWithoutParams(issue, action);
            }
        }

        public override void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.runIssueActionWithParams(issue, action, fields, comment);
            }
        }

        public override List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getComponents(project);
            }
        }

        public override List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getVersions(project);
            }
        }

        public override string createIssue(JiraServer server, JiraIssue issue) {
            using (var rest = new RestClient(server)) {
                return rest.createIssue(issue);
            }
        }

        public override object getRawIssueObject(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getRawIssueObject(issue.Key);
            }
        }

        public override JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getSecurityLevel(issue);
            }
        }

        public override void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.logWorkAndAutoUpdateRemaining(issue, timeSpent, startDate, comment);
            }
        }

        public override void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.logWorkAndLeaveRemainingUnchanged(issue, timeSpent, startDate, comment);
            }
        }

        public override void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.logWorkAndUpdateRemainingManually(issue, timeSpent, startDate, remainingEstimate, comment);
            }
        }

        public override void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            using (var rest = new RestClient(issue.Server)) {
                rest.updateIssue(issue, fields);
            }
        }

        public override void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            using (var rest = new RestClient(issue.Server)) {
                rest.uploadAttachment(issue, name, attachment);
            }
        }

        public bool restSupported(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.restSupported();
            }
        }
    }
}
