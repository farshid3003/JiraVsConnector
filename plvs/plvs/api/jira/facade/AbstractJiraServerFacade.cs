using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira.gh;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira.facade {
    public abstract class AbstractJiraServerFacade : ServerFacade {
        protected Dictionary<string, IDictionary<string, string>> rssSessionCookieMap { get; private set; }

        protected AbstractJiraServerFacade() {
            PlvsUtils.installSslCertificateHandler();
            rssSessionCookieMap = new Dictionary<string, IDictionary<string, string>>();
        }

        public abstract void login(JiraServer server);

        public virtual void dropAllSessions() {
            lock (rssSessionCookieMap) {
                rssSessionCookieMap.Clear();
            }
        }

        public abstract bool supportsGh(JiraServer server);

        public abstract List<RapidBoard> getGhBoards(JiraServer server);

        public abstract List<Sprint> getGhSprints(JiraServer server, int boardId);

        public abstract List<string> getIssueKeysForSprint(JiraServer server, Sprint sprint);

        public abstract string getSoapToken(JiraServer server);

        public abstract List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count);

        public abstract List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count);

        public abstract JiraIssue getIssue(JiraServer server, string key);

        public abstract string getRenderedContent(JiraIssue issue, string markup);

        public abstract string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup);

        public abstract List<JiraProject> getProjects(JiraServer server);

        public abstract List<JiraNamedEntity> getIssueTypes(JiraServer server);

        public abstract List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server);

        public abstract List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project);

        public abstract List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project);

        public abstract List<JiraSavedFilter> getSavedFilters(JiraServer server);

        public abstract List<JiraNamedEntity> getPriorities(JiraServer server);

        public abstract List<JiraNamedEntity> getStatuses(JiraServer server);

        public abstract List<JiraNamedEntity> getResolutions(JiraServer server);

        public abstract void addComment(JiraIssue issue, string comment);

        public abstract List<JiraNamedEntity> getActionsForIssue(JiraIssue issue);

        public abstract List<JiraField> getFieldsForAction(JiraIssue issue, int actionId);

        public abstract void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action);

        public abstract void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment);

        public abstract List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project);

        public abstract List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project);

        public abstract string createIssue(JiraServer server, JiraIssue issue);

        public abstract object getRawIssueObject(JiraIssue issue);

        public abstract JiraNamedEntity getSecurityLevel(JiraIssue issue);

        public abstract void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment);

        public abstract void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment);

        public abstract void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment);

        public abstract void updateIssue(JiraIssue issue, ICollection<JiraField> fields);

        public abstract void uploadAttachment(JiraIssue issue, string name, byte[] attachment);

        protected delegate T Wrapped<T>();

        protected T setSessionCookieAndWrapExceptions<T>(JiraServer server, JiraAuthenticatedClient client, Wrapped<T> wrapped) {
            try {
                setSessionCookie(server, client);
                return wrapped();
            } catch (Exception) {
                removeSessionCookie(server);
                throw;
            }
        }

        private void setSessionCookie(JiraServer server, JiraAuthenticatedClient client) {
            if (server.OldSkoolAuth) {
                return;
            }

            lock (rssSessionCookieMap) {
                var key = CredentialUtils.getSessionOrTokenKey(server);
                if (rssSessionCookieMap.ContainsKey(key)) {
                    client.SessionTokens = rssSessionCookieMap[key];
                } else {
                    rssSessionCookieMap[key] = client.login();
                }
            }
        }

        private void removeSessionCookie(JiraServer server) {
            if (server.OldSkoolAuth) {
                return;
            }

            lock (rssSessionCookieMap) {
                rssSessionCookieMap.Remove(CredentialUtils.getSessionOrTokenKey(server));
            }
        }
    }
}
