using System;
using System.Collections.Generic;
using System.Diagnostics;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models.jira {
    internal class RecentlyViewedIssuesModel {
        private readonly List<RecentlyViewedIssue> issues = new List<RecentlyViewedIssue>();

        private static readonly RecentlyViewedIssuesModel INSTANCE = new RecentlyViewedIssuesModel();
        private const string RECENTLY_VIEWED_COUNT = "recentlyViewedIssuesCount_";
        private const string RECENTLY_VIEWED_ISSUE_KEY = "recentlyViewedIssueKey_";
        private const string RECENTLY_VIEWED_ISSUE_SERVER_GUID = "recentlyViewedIssueServerGuid_";

        private const int MAX_ITEMS = 10;

        public static RecentlyViewedIssuesModel Instance {
            get { return INSTANCE; }
        }

        public void add(JiraIssue issue) {
            lock (this) {
                if (moveToFrontIfContains(issue)) {
                    return;
                }
                while (issues.Count >= MAX_ITEMS) {
                    issues.RemoveAt(issues.Count - 1);
                }
                issues.Insert(0, new RecentlyViewedIssue(issue));
                save();
            }
        }

        private bool moveToFrontIfContains(JiraIssue issue) {
            foreach (RecentlyViewedIssue rvi in issues) {
                if (!rvi.ServerGuid.Equals(issue.Server.GUID) || !rvi.IssueKey.Equals(issue.Key)) continue;
                issues.Remove(rvi);
                issues.Insert(0, rvi);
                save();
                return true;
            }
            return false;
        }

        public ICollection<RecentlyViewedIssue> Issues {
            get { return issues; }
        }

        public void load() {
            lock (this) {
                issues.Clear();

                ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

                int count = store.loadParameter(RECENTLY_VIEWED_COUNT, -1);
                if (count != -1) {
                    try {
                        if (count > MAX_ITEMS)
                            count = MAX_ITEMS;

                        for (int i = 1; i <= count; ++i) {
                            string guidStr = store.loadParameter(RECENTLY_VIEWED_ISSUE_SERVER_GUID  + i, null);
                            Guid guid = new Guid(guidStr);
                            string key = store.loadParameter(RECENTLY_VIEWED_ISSUE_KEY + i, null);
                            RecentlyViewedIssue issue = new RecentlyViewedIssue(guid, key);
                            issues.Add(issue);
                        }
                    }
                    catch (Exception e) {
                        Debug.WriteLine(e);
                    }
                }
            }
        }

        public void save() {
            lock (this) {
                ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

                try {
                    store.storeParameter(RECENTLY_VIEWED_COUNT, issues.Count);

                    int i = 1;
                    foreach (RecentlyViewedIssue issue in issues) {
                        string var = RECENTLY_VIEWED_ISSUE_SERVER_GUID + i;
                        store.storeParameter(var, issue.ServerGuid.ToString());
                        var = RECENTLY_VIEWED_ISSUE_KEY + i;
                        store.storeParameter(var, issue.IssueKey);
                        ++i;
                    }
                }
                catch (Exception e) {
                    Debug.WriteLine(e);
                }
            }
        }
    }
}