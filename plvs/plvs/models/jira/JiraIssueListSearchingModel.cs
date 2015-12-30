using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira {
    class JiraIssueListSearchingModel : JiraIssueListModel {
        private JiraIssueListModel model;
        private string query;
        public string Query { get { return query; } set { setQuery(value); } }

        public JiraIssueListSearchingModel(JiraIssueListModel model) {
            reinit(model);
        }

        public void reinit(JiraIssueListModel m) {
            model = m;
            shutdown();
            model.IssueChanged += model_IssueChanged;
            model.ModelChanged += model_ModelChanged;
        }

        public void shutdown() {
            model.IssueChanged -= model_IssueChanged;
            model.ModelChanged -= model_ModelChanged;
            removeAllListeners();
        }

        private void setQuery(string q) {
            if (q.Equals(Query)) {
                return;
            }
            query = q;
            if (ModelChanged != null) {
                ModelChanged(this, new EventArgs());
            }
        }

        public ICollection<JiraIssue> Issues {
            get { return filter(model.Issues); }
        }

        public JiraIssue getIssue(string key, JiraServer server) {
            JiraIssue issue = model.getIssue(key, server);
            if (issue == null) return null;
            return matches(issue) ? issue : null;
        }

        private ICollection<JiraIssue> filter(ICollection<JiraIssue> issues) {
            if (string.IsNullOrEmpty(Query)) {
                return issues;
            }
            List<JiraIssue> list = new List<JiraIssue>();
            foreach (var issue in issues) {
                if (matches(issue)) {
                    list.Add(issue);
                }
            }
            return list;
        }

        private bool matches(JiraIssue issue) {
            return issue.Key.ToLower().Contains(Query.ToLower()) || issue.Summary.ToLower().Contains(Query.ToLower());
        }

        public event EventHandler<EventArgs> ModelChanged;
        public event EventHandler<IssueChangedEventArgs> IssueChanged;

        public void removeAllListeners() {
            ModelChanged = null;
            IssueChanged = null;
        }

        public void clear(bool notify) {
            model.clear(notify);
        }

        public void addIssues(ICollection<JiraIssue> newIssues) {
            model.addIssues(newIssues);
        }

        public void updateIssue(JiraIssue issue) {
            model.updateIssue(issue);
        }

        private void model_ModelChanged(object sender, EventArgs e) {
            if (ModelChanged != null) {
                ModelChanged(this, new EventArgs());
            }
        }

        private void model_IssueChanged(object sender, IssueChangedEventArgs e) {
            if (!string.IsNullOrEmpty(Query) && !matches(e.Issue)) {
                return;
            }
            if (IssueChanged != null) {
                IssueChanged(this, new IssueChangedEventArgs(e.Issue));
            }
        }
    }
}