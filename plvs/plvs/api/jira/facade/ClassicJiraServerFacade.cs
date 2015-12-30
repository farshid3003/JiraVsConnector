using System;
using System.Collections.Generic;
using System.Diagnostics;
using Atlassian.plvs.api.jira.gh;
using Atlassian.plvs.api.jira.soap;

namespace Atlassian.plvs.api.jira.facade {
    public class ClassicJiraServerFacade : AbstractJiraServerFacade {

        private readonly Dictionary<string, string> soapTokenMap = new Dictionary<string, string>();

        public override void login(JiraServer server) {
            try {
                createSoapSession(server).login(server.UserName, server.Password);
            } catch (Exception e) {
                maybeHandle503(server, e);
                throw;
            }
        }

        public override void dropAllSessions() {
            lock (soapTokenMap) {
                soapTokenMap.Clear();
            }
            base.dropAllSessions();
        }

        public override bool supportsGh(JiraServer server) {
            return false;
        }

        public override List<RapidBoard> getGhBoards(JiraServer server) {
            throw new NotImplementedException();
        }

        public override List<Sprint> getGhSprints(JiraServer server, int boardId) {
            throw new NotImplementedException();
        }

        public override List<string> getIssueKeysForSprint(JiraServer server, Sprint sprint) {
            throw new NotImplementedException();
        }

        public override string getRenderedContent(JiraIssue issue, string markup) {
            throw new NotImplementedException();
        }

        public override string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            throw new NotImplementedException();
        }

        public override string getSoapToken(JiraServer server) {
            try {
                using (var session = createSoapSession(server)) {
                    return session.login(server.UserName, server.Password);
                }
            } catch (Exception e) {
                Debug.WriteLine("JiraServerFacade.getSoapToken() - exception: " + e.Message);
            }
            return null;
        }

        public override List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            using (var rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(
                    server, rss, () => rss.getSavedFilterIssues(filter.Id, "priority", "DESC", start, count));
            }
        }

        public override List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            using (var rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(
                    server, rss,
                    () => rss.getCustomFilterIssues(filter.getOldstyleFilterQueryString(), filter.getSortBy(), "DESC", start, count));
            }
        }

        public override JiraIssue getIssue(JiraServer server, string key) {
            using (var rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(server, rss, () => rss.getIssue(key));
            }
        }

        public override List<JiraProject> getProjects(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getProjects());
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getIssueTypes());
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSubtaskIssueTypes());
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSubtaskIssueTypes(project));
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getIssueTypes(project));
            }
        }

        public override List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSavedFilters());
            }
        }

        public override List<JiraNamedEntity> getPriorities(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getPriorities());
            }
        }

        public override List<JiraNamedEntity> getStatuses(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getStatuses());
            }
        }

        public override List<JiraNamedEntity> getResolutions(JiraServer server) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getResolutions());
            }
        }

        public override void addComment(JiraIssue issue, string comment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.addComment(issue, comment));
            }
        }

        public override List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            using (var s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getActionsForIssue(issue));
            }
        }

        public override List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            using (var s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getFieldsForAction(issue, actionId));
            }
        }

        public override void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithoutParams(issue, action.Id));
            }
        }

        public override void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithParams(issue, action.Id, fields, comment));
            }
        }

        public override List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getComponents(project));
            }
        }

        public override List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getVersions(project));
            }
        }

        public override string createIssue(JiraServer server, JiraIssue issue) {
            using (var s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.createIssue(issue));
            }
        }

        public override object getRawIssueObject(JiraIssue issue) {
            using (var s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getIssueSoapObject(issue.Key));
            }
        }

        public override JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            using (var s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getSecurityLevel(issue.Key));
            }
        }

        public override void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndAutoUpdateRemaining(issue.Key, timeSpent, startDate, comment));
            }
        }

        public override void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndLeaveRemainingUnchanged(issue.Key, timeSpent, startDate, comment));
            }
        }

        public override void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndUpdateRemainingManually(issue.Key, timeSpent, startDate, remainingEstimate, comment));
            }
        }

        public override void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.updateIssue(issue.Key, fields));
            }
        }

        public override void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            using (var s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.uploadAttachment(issue.Key, name, attachment));
            }
        }

        private static SoapSession createSoapSession(JiraServer server) {
            var s = new SoapSession(server.Url, server.NoProxy);
            return s;
        }

        private T setSoapTokenAndWrapExceptions<T>(JiraServer server, SoapSession session, Wrapped<T> wrapped) {
            try {
                setSoapSessionToken(server, session);
                return wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSoapSessionToken(server);
                try {
                    setSoapSessionToken(server, session);
                    return wrapped();
                } catch (Exception e) {
                    removeSoapSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSoapSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }


        private delegate void WrappedVoid();
        private void setSoapTokenAndWrapExceptionsVoid(JiraServer server, SoapSession session, WrappedVoid wrapped) {
            try {
                setSoapSessionToken(server, session);
                wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSoapSessionToken(server);
                try {
                    setSoapSessionToken(server, session);
                    wrapped();
                } catch (Exception e) {
                    removeSoapSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSoapSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }

        private static void maybeHandle503(JiraServer server, Exception e) {
            if (e is LoginException) {
                var inner = e.InnerException;
                if (inner.Message.Contains("503")) {
                    throw new FiveOhThreeJiraException(server);
                }
            }
        }

        private void setSoapSessionToken(JiraServer server, SoapSession session) {
            lock (soapTokenMap) {
                var key = CredentialUtils.getSessionOrTokenKey(server);
                if (soapTokenMap.ContainsKey(key)) {
                    session.Token = soapTokenMap[key];
                } else {
                    soapTokenMap[key] = session.login(server.UserName, server.Password);
                }
            }
        }

        private void removeSoapSessionToken(JiraServer server) {
            lock (soapTokenMap) {
                soapTokenMap.Remove(CredentialUtils.getSessionOrTokenKey(server));
            }
        }
    }
}