using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.api.jira.facade {
    public class SmartJiraServerFacade : AbstractJiraServerFacade {
        private static readonly SmartJiraServerFacade INSTANCE = new SmartJiraServerFacade();

        public static SmartJiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private readonly ClassicJiraServerFacade classicFacade = new ClassicJiraServerFacade();
        private readonly RestJiraServerFacade restFacade = new RestJiraServerFacade();

        private readonly List<Guid> restServers = new List<Guid>();

        public override void login(JiraServer server) {
            if (restFacade.restSupported(server)) {
                restServers.Add(server.GUID);   
            }
            delegatedVoid(server, delegate { restFacade.login(server); }, delegate { classicFacade.login(server); });
        }

        public override string getSoapToken(JiraServer server) {
            return delegated(server, delegate { return restFacade.getSoapToken(server); }, delegate { return classicFacade.getSoapToken(server); });
        }

        public override bool supportsGh(JiraServer server) {
            return delegated(server,
                             delegate { return restFacade.supportsGh(server); },
                             delegate { return classicFacade.supportsGh(server); });
        }

        public override List<RapidBoard> getGhBoards(JiraServer server) {
            return delegated(server,
                             delegate { return restFacade.getGhBoards(server); },
                             delegate { return classicFacade.getGhBoards(server); });
        }

        public override List<string> getIssueKeysForSprint(JiraServer server, Sprint sprint) {
            return delegated(server,
                             delegate { return restFacade.getIssueKeysForSprint(server, sprint); },
                             delegate { return classicFacade.getIssueKeysForSprint(server, sprint); });
        }

        public override List<Sprint> getGhSprints(JiraServer server, int boardId) {
            return delegated(server,
                             delegate { return restFacade.getGhSprints(server, boardId); },
                             delegate { return classicFacade.getGhSprints(server, boardId); });
        }

        public override List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            return delegated(server,
                             delegate { return restFacade.getSavedFilterIssues(server, filter, start, count); },
                             delegate { return classicFacade.getSavedFilterIssues(server, filter, start, count); });
        }

        public override List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            return delegated(server,
                             delegate { return restFacade.getCustomFilterIssues(server, filter, start, count); },
                             delegate { return classicFacade.getCustomFilterIssues(server, filter, start, count); });
        }

        public override JiraIssue getIssue(JiraServer server, string key) {
            return delegated(server,
                             delegate { return restFacade.getIssue(server, key); },
                             delegate { return classicFacade.getIssue(server, key); });
        }

        public override string getRenderedContent(JiraIssue issue, string markup) {
            return restFacade.getRenderedContent(issue, markup);
        }

        public override string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            return restFacade.getRenderedContent(server, issueTypeId, project, markup);
        }

        public override List<JiraProject> getProjects(JiraServer server) {
            return delegated(server, 
                delegate { return restFacade.getProjects(server); }, 
                delegate { return classicFacade.getProjects(server); });
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getIssueTypes(server); },
                delegate { return classicFacade.getIssueTypes(server); });
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getSubtaskIssueTypes(server); },
                delegate { return classicFacade.getSubtaskIssueTypes(server); });
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getSubtaskIssueTypes(server, project); },
                delegate { return classicFacade.getSubtaskIssueTypes(server, project); });
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getIssueTypes(server, project); },
                delegate { return classicFacade.getIssueTypes(server, project); });
        }

        public override List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getSavedFilters(server); },
                delegate { return classicFacade.getSavedFilters(server); });
        }

        public override List<JiraNamedEntity> getPriorities(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getPriorities(server); },
                delegate { return classicFacade.getPriorities(server); });
        }

        public override List<JiraNamedEntity> getStatuses(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getStatuses(server); },
                delegate { return classicFacade.getStatuses(server); });
        }

        public override List<JiraNamedEntity> getResolutions(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getResolutions(server); },
                delegate { return classicFacade.getResolutions(server); });
        }

        public override void addComment(JiraIssue issue, string comment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.addComment(issue, comment); },
                delegate { classicFacade.addComment(issue, comment); });
        }

        public override List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            return delegated(issue.Server,
                             delegate { return restFacade.getActionsForIssue(issue); },
                             delegate { return classicFacade.getActionsForIssue(issue); });
        }

        public override List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            return delegated(issue.Server,
                             delegate { return restFacade.getFieldsForAction(issue, actionId); },
                             delegate { return classicFacade.getFieldsForAction(issue, actionId); });
        }

        public override void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            delegatedVoid(issue.Server,
                delegate { restFacade.runIssueActionWithoutParams(issue, action); },
                delegate { classicFacade.runIssueActionWithoutParams(issue, action); });
        }

        public override void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.runIssueActionWithParams(issue, action, fields, comment); },
                delegate { classicFacade.runIssueActionWithParams(issue, action, fields, comment); });
        }

        public override List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getComponents(server, project); },
                delegate { return classicFacade.getComponents(server, project); });
        }

        public override List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getVersions(server, project); },
                delegate { return classicFacade.getVersions(server, project); });
        }

        public override string createIssue(JiraServer server, JiraIssue issue) {
            return delegated(server,
                             delegate { return restFacade.createIssue(server, issue); },
                             delegate { return classicFacade.createIssue(server, issue); });
        }

        public override object getRawIssueObject(JiraIssue issue) {
            return delegated(issue.Server,
                             delegate { return restFacade.getRawIssueObject(issue); },
                             delegate { return classicFacade.getRawIssueObject(issue); });
        }

        public override JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            return delegated(issue.Server,
                             delegate { return restFacade.getSecurityLevel(issue); },
                             delegate { return classicFacade.getSecurityLevel(issue); });
        }

        public override void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.logWorkAndAutoUpdateRemaining(issue, timeSpent, startDate, comment); },
                delegate { classicFacade.logWorkAndAutoUpdateRemaining(issue, timeSpent, startDate, comment); });
        }

        public override void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.logWorkAndLeaveRemainingUnchanged(issue, timeSpent, startDate, comment); },
                delegate { classicFacade.logWorkAndLeaveRemainingUnchanged(issue, timeSpent, startDate, comment); });
        }

        public override void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.logWorkAndUpdateRemainingManually(issue, timeSpent, startDate, remainingEstimate, comment); },
                delegate { classicFacade.logWorkAndUpdateRemainingManually(issue, timeSpent, startDate, remainingEstimate, comment); });
        }

        public override void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            delegatedVoid(issue.Server,
                delegate { restFacade.updateIssue(issue, fields); },
                delegate { classicFacade.updateIssue(issue, fields); });
        }

        public override void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            delegatedVoid(issue.Server,
                delegate { restFacade.uploadAttachment(issue, name, attachment); },
                delegate { classicFacade.uploadAttachment(issue, name, attachment); });
        }

        protected delegate T Delegate<T>(JiraServer server);
        protected delegate void DelegateVoid(JiraServer server);

        private T delegated<T>(JiraServer server, Delegate<T> rest, Delegate<T> classic) {
            return restServers.Contains(server.GUID) ? rest(server) : classic(server);
        }

        private void delegatedVoid(JiraServer server, DelegateVoid rest, DelegateVoid classic) {
            if (restServers.Contains(server.GUID)) {
                rest(server);
            } else {
                classic(server);
            }
        }
    }
}
