using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira {
    class JiraIssueListModelImpl: JiraIssueListModel {

        #region member fields
        
        private static readonly JiraIssueListModel INSTANCE = new JiraIssueListModelImpl();

        private readonly Dictionary<string, JiraIssue> issues = new Dictionary<string, JiraIssue>();

        #endregion

        public ICollection<JiraIssue> Issues {
            get { lock(issues) { return new List<JiraIssue>(issues.Values); } }
        }

        public JiraIssue getIssue(string key, JiraServer server) {
            lock (issues) {
                return issues.ContainsKey(server.GUID + key) ? issues[server.GUID + key] : null;
            }
        }

        public static JiraIssueListModel Instance {
            get { return INSTANCE; }
        }

        public void removeAllListeners() {
            ModelChanged = null;
            IssueChanged = null;
        }

        public void clear(bool notify) {
            lock (issues) {
                issues.Clear();
            }
            if (notify) {
                notifyListenersOfModelChange();
            }
        }

        public void addIssues(ICollection<JiraIssue> newIssues) {
            lock (issues) {
                foreach (var issue in newIssues) {
                    issues[issue.Server.GUID + issue.Key] = issue;
                }
            }
            notifyListenersOfModelChange();
        }

        public void updateIssue(JiraIssue issue) {
            bool found = false;
            bool differs = false;
            lock (issues) {
                foreach (var i in Issues) {
                    if (!i.Id.Equals(issue.Id)) continue;
                    if (!i.Server.GUID.Equals(issue.Server.GUID)) continue;
                    found = true;
                    if (!i.Equals(issue)) {
                        issues[issue.Server.GUID + issue.Key] = issue;
                        differs = true;
                    }
                    break;
                }
            }
            // issue might not be in the model, but somebody still modified it. 
            // Let's tell other parties that just might be interested in that
            if (!found || differs) {
                notifyListenersOfIssueChange(issue);
            }
        }

        public event EventHandler<EventArgs> ModelChanged;
        public event EventHandler<IssueChangedEventArgs> IssueChanged;

        #region private parts

        private void notifyListenersOfIssueChange(JiraIssue issue) {
            if (IssueChanged != null) {
                IssueChanged(this, new IssueChangedEventArgs(issue));
            }
        }

        private void notifyListenersOfModelChange() {
            if (ModelChanged != null) {
                ModelChanged(this, new EventArgs());
            }
        }

        private JiraIssueListModelImpl() { }

        #endregion
    }
}