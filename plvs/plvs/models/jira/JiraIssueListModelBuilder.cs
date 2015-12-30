using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.api.jira.gh;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs.models.jira {
    public class JiraIssueListModelBuilder {
        private readonly AbstractJiraServerFacade facade;

        public JiraIssueListModelBuilder(AbstractJiraServerFacade facade) {
            this.facade = facade;
        }

        public void rebuildModelWithSavedFilter(JiraIssueListModel model, JiraServer server, JiraSavedFilter filter) {
            List<JiraIssue> issues = facade.getSavedFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithSavedFilter(JiraIssueListModel model, JiraServer server, JiraSavedFilter filter) {
            List<JiraIssue> issues = facade.getSavedFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        private class JqlFilter : JiraFilter {
            private readonly string jql;

            public JqlFilter(IEnumerable<string> keys) {
                var sb = new StringBuilder();
                foreach (var key in keys) {
                    if (sb.Length > 0) {
                        sb.Append(" or ");
                    }
                    sb.Append("key = ").Append(key);
                }
                jql = sb.ToString();
            }

            public string getOldstyleFilterQueryString() {
                throw new NotImplementedException();
            }

            public string getSortBy() {
                return null;
            }

            public string getJql() {
                return jql;
            }
        }

        public void rebuildModelWithSprint(JiraIssueListModel model, JiraServer server, Sprint sprint) {
            var keys = facade.getIssueKeysForSprint(server, sprint);
            if (keys == null || keys.Count == 0) {
                lock (this) {
                    model.clear(true);
                    return;
                }
            }
            JiraFilter f = new JqlFilter(keys);

            var issues = facade.getCustomFilterIssues(server, f, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithSprint(JiraIssueListModel model, JiraServer server, Sprint sprint) {
            var keys = facade.getIssueKeysForSprint(server, sprint);
            if (keys == null || keys.Count == 0) {
                return;
            }

            JiraFilter f = new JqlFilter(keys);

            var issues = facade.getCustomFilterIssues(server, f, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithPresetFilter(JiraIssueListModel model, JiraServer server, JiraPresetFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithPresetFilter(JiraIssueListModel model, JiraServer server, JiraPresetFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithCustomFilter(JiraIssueListModel model, JiraServer server, JiraCustomFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithCustomFilter(JiraIssueListModel model, JiraServer server, JiraCustomFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithRecentlyViewedIssues(JiraIssueListModel model) {
            ICollection<RecentlyViewedIssue> issues = RecentlyViewedIssuesModel.Instance.Issues;
            ICollection<JiraServer> servers = JiraServerModel.Instance.getAllEnabledServers();

            List<JiraIssue> list = new List<JiraIssue>(issues.Count);
            list.AddRange(from issue in issues
                          let server = findServer(issue.ServerGuid, servers)
                          where server != null
                          select facade.getIssue(server, issue.IssueKey));

            lock (this) {
                model.clear(false);
                model.addIssues(list);
            }
        }

        private static JiraServer findServer(Guid guid, IEnumerable<JiraServer> servers) {
            return servers.FirstOrDefault(server => server.GUID.Equals(guid));
        }
    }
}