using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.util;
using Microsoft.Win32;

namespace Atlassian.plvs.autoupdate {
    public class UsageCollector {

        private static readonly UsageCollector INSTANCE = new UsageCollector();
        public static UsageCollector Instance { get { return INSTANCE; }}

        private const string JIRA_ISSUES_OPEN_KEY = "JiraIssuesOpen";
        private const string BAMBOO_BUILDS_OPEN_KEY = "BambooBuildsOpen";
        private const string INSTANCE_GUID_KEY = "InstanceGuid";
        private const string UNKNOWN = "unknown";

        private readonly string instanceGuid;

        private UsageCollector() {
            RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY + "\\UsageStatistics");
            if (root == null) {
                instanceGuid = UNKNOWN;
                return;
            }
            instanceGuid = (string) root.GetValue(INSTANCE_GUID_KEY);
            if (instanceGuid != null) return;

            instanceGuid = Guid.NewGuid().ToString();
            root.SetValue(INSTANCE_GUID_KEY, instanceGuid);
        }

        public string getUsageReportingUrl(string url) {
            lock (this) {
                if (!GlobalSettings.ReportUsage || instanceGuid.Equals(UNKNOWN)) return url;
                var sb = new StringBuilder(url);

                sb.Append("?uid=").Append(instanceGuid);

                sb.Append("&version=").Append(PlvsVersionInfo.Version);

                var jiras = JiraServerModel.Instance.getAllServers();
                sb.Append("&jiraServers=").Append(jiras != null ? jiras.Count : 0);

                var bamboos = BambooServerModel.Instance.getAllServers();
                sb.Append("&bambooServers=").Append(bamboos != null ? bamboos.Count : 0);

                // todo - fix this when we handle crucible
                sb.Append("&crucibleServers=0");

                try {
                    var root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY + "\\UsageStatistics");

                    if (root != null) {
                        var jirasOpen = (int)root.GetValue(JIRA_ISSUES_OPEN_KEY, 0);
                        sb.Append("&i=").Append(jirasOpen);
                        root.SetValue(JIRA_ISSUES_OPEN_KEY, 0);
                        var bamboosOpen = (int)root.GetValue(BAMBOO_BUILDS_OPEN_KEY, 0);
                        sb.Append("&b=").Append(bamboosOpen);
                        root.SetValue(BAMBOO_BUILDS_OPEN_KEY, 0);
                        root.Close();

                        // todo - fix this when we handle crucible
                        sb.Append("&r=0");

                        // todo - fix this when we handle issue activation
                        sb.Append("&a=0");
                    }
                } catch (Exception e) {
                    Debug.WriteLine("UsageCollector.getUsageReportingUrl() - failed to read registry: " + e.Message);
                }

                return sb.ToString();
            }
        }

        public void bumpJiraIssuesOpen() {
            lock(this) {
                try {
                    var root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY + "\\UsageStatistics");
                    if (root != null) {
                        var issues = (int) root.GetValue(JIRA_ISSUES_OPEN_KEY, 0);
                        root.SetValue(JIRA_ISSUES_OPEN_KEY, ++issues);
                    }
                }
                catch (Exception e) {
                    Debug.WriteLine("UsageCollector.bumpJiraIssuesOpen() - failed to write registry: " + e.Message);
                }
            }
        }

        public void bumpBambooBuildsOpen() {
            lock(this) {
                try {
                    var root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY + "\\UsageStatistics");
                    if (root != null) {
                        var builds = (int)root.GetValue(BAMBOO_BUILDS_OPEN_KEY, 0);
                        root.SetValue(BAMBOO_BUILDS_OPEN_KEY, ++builds);
                    }
                } catch (Exception e) {
                    Debug.WriteLine("UsageCollector.bumpBambooBuildsOpen() - failed to write registry: " + e.Message);
                }
            }
        }

        public void sendOptInOptOut(bool usage) {
            var t = PlvsUtils.createThread(() => sendOptInOptOutWorker(usage));
            t.Start();
        }

        private void sendOptInOptOutWorker(bool usage) {
            if (instanceGuid.Equals(UNKNOWN)) {
                Debug.WriteLine("UsageCollector.sendOptInOptOutWorker() - unknown instance guid");
                return;
            }

            var url = Autoupdate.STABLE_URL + "?uid=" + instanceGuid + "&userOptedIn=" + (usage ? "1" : "0");    
            try {
                var req = (HttpWebRequest)WebRequest.Create(url);

                req.Timeout = 5000;
                req.ReadWriteTimeout = 20000;
                var resp = (HttpWebResponse)req.GetResponse();
                // ignore response
                resp.GetResponseStream();
            } catch (Exception e) {
                Debug.WriteLine("UsageCollector.sendOptInOptOutWorker() - exception: " + e.Message);
            }
        }
    }
}
