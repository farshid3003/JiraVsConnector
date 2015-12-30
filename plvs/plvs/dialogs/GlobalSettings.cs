using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using Atlassian.plvs.attributes;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.store;
using Atlassian.plvs.util;
using Microsoft.Win32;

namespace Atlassian.plvs.dialogs {
    public partial class GlobalSettings : Form {
        private const int DEFAULT_BAMBOO_POLLING_INTERVAL = 60;
        private const int DEFAULT_ISSUE_BATCH_SIZE = 25;
        private const int DEFAULT_JIRA_TIMEOUT = 10;
        private const int DEFAULT_MAX_ISSUE_LINKS_FILE_LEN = 10000;

        private const string REG_AUTOUPDATE = "AutoupdateEnabled";
        private const string REG_BAMBOO_POLLING_INTERVAL = "BambooPollingInterval";
        private const string REG_CHECK_SNAPSHOTS = "AutoupdateCheckSnapshots";
        private const string REG_FIRST_RUN = "FirstRun";
        private const string REG_ISSUE_BATCH_SIZE = "JiraIssueBatchSize";
        private const string REG_MANUAL_UPDATE_STABLE_ONLY = "ManualUpdateCheckStableOnly";
        private const string REG_REPORT_USAGE = "AutoupdateReportUsage";
        private const string REG_JIRA_SERVER_EXPLORER = "JiraServerExplorer";
        private const string REG_ANKH_SNV_ENABLED = "AnkhSVNIntegrationEnabled";
        private const string REG_NETWORK_TIMEOUT = "NetworkTimeout";
        private const string REG_ISSUE_LINKS_DISABLED = "JiraIssueLinksInEditorDisabled";
        private const string REG_ISSUE_LINKS_MAX_FILE_LENGTH = "JiraIssueLinksInEditorMaxFileLength";

        private const string REG_PROXY_TYPE = "ProxyType";
        private const string REG_PROXY_HOST = "ProxyHost";
        private const string REG_PROXY_PORT = "ProxyPort";
        private const string REG_PROXY_USE_AUTH = "ProxyUseAuth";
        private const string REG_PROXY_USER = "ProxyUser";
        private const string REG_PROXY_PASSWORD = "ProxyPassword";

        private bool isRunningManualUpdateQuery;

        private const string PASSWORD_ENTROPY = "borgriubdfasd";

        private enum ProxyType {
            [StringValue("System")]
            SYSTEM, 
            [StringValue("Custom")]
            CUSTOM,
            [StringValue("None")]
            NONE
        }

        private static ProxyType currentProxyType;
        private static ProxyType proxyTypeToSave;
        private static string proxyHost;
        private static int proxyPort;
        private static bool proxyUseAuth;
        private static string proxyUser;
        private static string proxyPassword;

        static GlobalSettings() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                JiraIssuesBatch = (int) root.GetValue(REG_ISSUE_BATCH_SIZE, DEFAULT_ISSUE_BATCH_SIZE);
                AutoupdateEnabled = (int) root.GetValue(REG_AUTOUPDATE, 1) > 0;
                AutoupdateSnapshots = (int) root.GetValue(REG_CHECK_SNAPSHOTS, 0) > 0;
                ReportUsage = (int) root.GetValue(REG_REPORT_USAGE, 1) > 0;
                CheckStableOnlyNow = (int) root.GetValue(REG_MANUAL_UPDATE_STABLE_ONLY, 1) > 0;
                BambooPollingInterval = (int) root.GetValue(REG_BAMBOO_POLLING_INTERVAL, DEFAULT_BAMBOO_POLLING_INTERVAL);
                JiraServerExplorerEnabled = (int)root.GetValue(REG_JIRA_SERVER_EXPLORER, 0) > 0;
                AnkhSvnIntegrationEnabled = (int) root.GetValue(REG_ANKH_SNV_ENABLED, 0) > 0;
                NetworkTimeout = (int) root.GetValue(REG_NETWORK_TIMEOUT, 10);
                IssueLinksInEditorDisabled = (int) root.GetValue(REG_ISSUE_LINKS_DISABLED, 0) > 0;
                IssueLinksInEditorDisabledForAllFiles = (int) root.GetValue(REG_ISSUE_LINKS_DISABLED, 0) > 1;
                MaxIssueLinksInEditorFileSize = (int) root.GetValue(REG_ISSUE_LINKS_MAX_FILE_LENGTH, DEFAULT_MAX_ISSUE_LINKS_FILE_LEN);

                string proxy = (string) root.GetValue(REG_PROXY_TYPE, ProxyType.SYSTEM.GetStringValue());
                if (proxy.Equals(ProxyType.CUSTOM.GetStringValue())) {
                    currentProxyType = ProxyType.CUSTOM;
                } else if (proxy.Equals(ProxyType.NONE.GetStringValue())) {
                    currentProxyType = ProxyType.NONE;
                } else {
                    currentProxyType = ProxyType.SYSTEM;
                }
                proxyHost = (string) root.GetValue(REG_PROXY_HOST, "");
                proxyPort = (int) root.GetValue(REG_PROXY_PORT, 0);
                proxyUseAuth = ((int) root.GetValue(REG_PROXY_USE_AUTH, 0)) == 1;
                proxyUser = (string) root.GetValue(REG_PROXY_USER, "");
                proxyPassword = DPApi.decrypt((string) root.GetValue(REG_PROXY_PASSWORD, ""), PASSWORD_ENTROPY);
            } catch (Exception) {
                JiraIssuesBatch = DEFAULT_ISSUE_BATCH_SIZE;
                AutoupdateEnabled = true;
                AutoupdateSnapshots = false;
                ReportUsage = true;
                CheckStableOnlyNow = true;
                BambooPollingInterval = DEFAULT_BAMBOO_POLLING_INTERVAL;
                JiraServerExplorerEnabled = false;
                AnkhSvnIntegrationEnabled = false;
                NetworkTimeout = DEFAULT_JIRA_TIMEOUT;
                IssueLinksInEditorDisabled = false;
                IssueLinksInEditorDisabledForAllFiles = false;
                MaxIssueLinksInEditorFileSize = DEFAULT_MAX_ISSUE_LINKS_FILE_LEN;
                currentProxyType = ProxyType.SYSTEM;
                proxyHost = "";
                proxyPort = 0;
                proxyUseAuth = false;
                proxyUser = "";
                proxyPassword = "";
            }

            proxyTypeToSave = currentProxyType;
        }

        public GlobalSettings() {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            initializeWidgets();

            buttonOk.Enabled = false;
        }

        public static int JiraIssuesBatch { get; private set; }
        public static bool AutoupdateEnabled { get; private set; }
        public static bool AutoupdateSnapshots { get; private set; }
        public static bool ReportUsage { get; private set; }
        public static bool CheckStableOnlyNow { get; private set; }
        public static int BambooPollingInterval { get; private set; }
        public static bool JiraServerExplorerEnabled { get; private set; }
        public static bool AnkhSvnIntegrationEnabled { get; private set; }
        public static int NetworkTimeout { get; private set; }
        public static IWebProxy Proxy { get { return getProxy(); } }
        public static bool IssueLinksInEditorDisabled { get; private set; }
        public static bool IssueLinksInEditorDisabledForAllFiles { get; private set; }
        public static int MaxIssueLinksInEditorFileSize { get; private set; }

        private static IWebProxy getProxy() {
            switch (proxyTypeToSave) {
                case ProxyType.NONE:
                    return null;
                case ProxyType.CUSTOM:
                    WebProxy proxy = new WebProxy
                                     {
                                         Address = new Uri("http://" + proxyHost + (proxyPort > 0 ? (":" + proxyPort) : ""))
                                     };
                    if (proxyUseAuth) {
                        string domain = proxyUser.Contains("\\") && !proxyUser.EndsWith("\\")
                                            ? proxyUser.Substring(0, proxyUser.IndexOf("\\"))
                                            : null;
                        string user = proxyUser.Contains("\\") && !proxyUser.EndsWith("\\")
                                          ? proxyUser.Substring(proxyUser.IndexOf("\\") + 1)
                                          : proxyUser.Trim(new[] { '\\' });
                        proxy.Credentials = new NetworkCredential(user, proxyPassword, domain);
                    }
                    return proxy;
            }
            return WebRequest.DefaultWebProxy;
        }

        private void initializeWidgets() {
            numericJiraBatchSize.Value = Math.Min(Math.Max(JiraIssuesBatch, 10), 1000);
            numericBambooPollingInterval.Value = Math.Min(Math.Max(BambooPollingInterval, 10), 3600);
            checkAutoupdate.Checked = AutoupdateEnabled;
            checkUnstable.Checked = AutoupdateSnapshots;
            checkStats.Checked = ReportUsage;
            checkUnstable.Enabled = AutoupdateEnabled;
            checkStats.Enabled = AutoupdateEnabled;
            radioStable.Checked = CheckStableOnlyNow;
            radioUnstable.Checked = !CheckStableOnlyNow;
            checkJiraExplorer.Checked = JiraServerExplorerEnabled;
            checkAnkhSvn.Checked = AnkhSvnIntegrationEnabled;
            numericNetworkTimeout.Value = Math.Min(Math.Max(NetworkTimeout, 2), 100);
            radioUseSystemProxy.Checked = currentProxyType == ProxyType.SYSTEM;
            radioUseCustomProxy.Checked = currentProxyType == ProxyType.CUSTOM;
            radioUseNoProxy.Checked = currentProxyType == ProxyType.NONE;
            textProxyHost.Text = proxyHost;
            numericProxyPort.Value = proxyPort;
            checkUseProxyAuth.Checked = proxyUseAuth;
            textProxyUserName.Text = proxyUser;
            textProxyPassword.Text = proxyPassword;

            checkDisableIssueLinks.Checked = IssueLinksInEditorDisabled;
            radioDisableIssueLinksForAllFiles.Checked = IssueLinksInEditorDisabledForAllFiles;
            radioDisableIssueLinksForLargeFiles.Checked = !radioDisableIssueLinksForAllFiles.Checked;
            radioDisableIssueLinksForAllFiles.Enabled = IssueLinksInEditorDisabled;
            radioDisableIssueLinksForLargeFiles.Enabled = IssueLinksInEditorDisabled;
            numericMaxIssueLinksFileLength.Value = MaxIssueLinksInEditorFileSize;
        }

        public static void checkFirstRun() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                var firstRun = (int) root.GetValue(REG_FIRST_RUN, 1);
                if (firstRun > 0) {
                    root.SetValue(REG_FIRST_RUN, 0);
                    handleFirstRun();
                }
            } catch (Exception e) {
                MessageBox.Show("Unable to read registry: " + e.Message, Constants.ERROR_CAPTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void handleFirstRun() {
            DialogResult result = MessageBox.Show(
                "We would greatly appreciate it if you would allow us to collect anonymous"
                + " usage statistics to help us provide a better quality product. Is this OK?",
                Constants.QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            ReportUsage = result == DialogResult.Yes;

            UsageCollector.Instance.sendOptInOptOut(ReportUsage);

            saveValues();
        }

        private void globalSettingsKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && !isRunningManualUpdateQuery) {
                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            JiraIssuesBatch = (int) numericJiraBatchSize.Value;
            NetworkTimeout = (int) numericNetworkTimeout.Value;
            AutoupdateEnabled = checkAutoupdate.Checked;
            AutoupdateSnapshots = checkUnstable.Checked;
            if (ReportUsage != checkStats.Checked) {
                ReportUsage = checkStats.Checked;
                UsageCollector.Instance.sendOptInOptOut(ReportUsage);
            }
            CheckStableOnlyNow = radioStable.Checked;
            BambooPollingInterval = (int) numericBambooPollingInterval.Value;
            JiraServerExplorerEnabled = checkJiraExplorer.Checked;
            AnkhSvnIntegrationEnabled = checkAnkhSvn.Checked;
            proxyTypeToSave = currentProxyType;
            proxyHost = textProxyHost.Text;
            proxyPort = (int) numericProxyPort.Value;
            proxyUseAuth = checkUseProxyAuth.Checked;
            proxyUser = textProxyUserName.Text;
            proxyPassword = textProxyPassword.Text;

            IssueLinksInEditorDisabled = checkDisableIssueLinks.Checked;
            IssueLinksInEditorDisabledForAllFiles = radioDisableIssueLinksForAllFiles.Checked;
            MaxIssueLinksInEditorFileSize = (int) numericMaxIssueLinksFileLength.Value;

            saveValues();

            if (SettingsChanged != null) {
                SettingsChanged(this, null);
            }

            Close();
        }

        public static event EventHandler<EventArgs> SettingsChanged;

        private static void saveValues() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                root.SetValue(REG_ISSUE_BATCH_SIZE, JiraIssuesBatch);
                root.SetValue(REG_AUTOUPDATE, AutoupdateEnabled ? 1 : 0);
                root.SetValue(REG_CHECK_SNAPSHOTS, AutoupdateSnapshots ? 1 : 0);
                root.SetValue(REG_REPORT_USAGE, ReportUsage ? 1 : 0);
                root.SetValue(REG_MANUAL_UPDATE_STABLE_ONLY, CheckStableOnlyNow ? 1 : 0);
                root.SetValue(REG_BAMBOO_POLLING_INTERVAL, BambooPollingInterval);
                root.SetValue(REG_JIRA_SERVER_EXPLORER, JiraServerExplorerEnabled ? 1 : 0);
                root.SetValue(REG_ANKH_SNV_ENABLED, AnkhSvnIntegrationEnabled ? 1 : 0);
                root.SetValue(REG_NETWORK_TIMEOUT, NetworkTimeout);
                root.SetValue(REG_PROXY_TYPE, proxyTypeToSave.GetStringValue());
                root.SetValue(REG_PROXY_HOST, proxyHost);
                root.SetValue(REG_PROXY_PORT, proxyPort);
                root.SetValue(REG_PROXY_USE_AUTH, proxyUseAuth ? 1 : 0);
                root.SetValue(REG_PROXY_USER, proxyUser);
                root.SetValue(REG_PROXY_PASSWORD, DPApi.encrypt(proxyPassword, PASSWORD_ENTROPY));
                int issueLinksDisabled = IssueLinksInEditorDisabled
                                             ? (IssueLinksInEditorDisabledForAllFiles ? 2 : 1)
                                             : 0;
                root.SetValue(REG_ISSUE_LINKS_DISABLED, issueLinksDisabled);
                root.SetValue(REG_ISSUE_LINKS_MAX_FILE_LENGTH, MaxIssueLinksInEditorFileSize);
            } catch (Exception e) {
                MessageBox.Show("Unable to save values to registry: " + e.Message, Constants.ERROR_CAPTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCheckNow_Click(object sender, EventArgs e) {
            setCloseButtonsEnabled(false);
            isRunningManualUpdateQuery = true;
            Autoupdate.Instance.runManualUpdate(radioStable.Checked, this, delegate
                                                                               {
                                                                                   setCloseButtonsEnabled(true);
                                                                                   isRunningManualUpdateQuery = false;
                                                                               });
        }

        private void setCloseButtonsEnabled(bool enabled) {
            buttonOk.Enabled = enabled;
            buttonCancel.Enabled = enabled;
        }

        private void linkUsageStatsDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                PlvsUtils.runBrowser(
                    "http://confluence.atlassian.com/display/IDEPLUGIN/Collecting+Usage+Statistics+for+the+Visual+Studio+Connector");
            } catch (Exception ex) {
                Debug.WriteLine("GlobalSettings.linkUsageStatsDetails_LinkClicked() - exception: " + ex.Message);
            }
        }

        private void updateOkButton() {
            bool changed = false;

            changed |= JiraIssuesBatch != (int) numericJiraBatchSize.Value;
            changed |= AutoupdateEnabled != checkAutoupdate.Checked;
            changed |= AutoupdateSnapshots != checkUnstable.Checked;
            changed |= ReportUsage != checkStats.Checked;
            changed |= CheckStableOnlyNow != radioStable.Checked;
            changed |= BambooPollingInterval != (int) numericBambooPollingInterval.Value;
            changed |= JiraServerExplorerEnabled != checkJiraExplorer.Checked;
            changed |= AnkhSvnIntegrationEnabled != checkAnkhSvn.Checked;
            changed |= NetworkTimeout != (int) numericNetworkTimeout.Value;
            changed |= proxyTypeToSave != currentProxyType;
            changed |= !proxyHost.Equals(textProxyHost.Text);
            changed |= proxyPort != numericProxyPort.Value;
            changed |= proxyUseAuth != checkUseProxyAuth.Checked;
            changed |= !proxyUser.Equals(textProxyUserName.Text);
            changed |= !proxyPassword.Equals(textProxyPassword.Text);
            changed |= IssueLinksInEditorDisabled != checkDisableIssueLinks.Checked;
            changed |= IssueLinksInEditorDisabledForAllFiles != radioDisableIssueLinksForAllFiles.Checked;
            changed |= MaxIssueLinksInEditorFileSize != numericMaxIssueLinksFileLength.Value;

            buttonOk.Enabled = changed;
        }

        private void checkAutoupdate_CheckedChanged(object sender, EventArgs e) {
            checkUnstable.Enabled = checkAutoupdate.Checked;
            checkStats.Enabled = checkAutoupdate.Checked;

            updateOkButton();
        }

        private void checkJiraExplorer_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void numericJiraBatchSize_ValueChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void numericBambooPollingInterval_ValueChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void checkUnstable_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void checkStats_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void radioStable_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void radioUnstable_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void checkAnkhSvn_CheckedChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void numericJiraBatchSize_KeyUp(object sender, KeyEventArgs e) {
            updateOkButton();
        }

        private void numericBambooPollingInterval_KeyUp(object sender, KeyEventArgs e) {
            updateOkButton();
        }

        private void numericNetworkTimeout_ValueChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void radioUseSystemProxy_CheckedChanged(object sender, EventArgs e) {
            setProxyType();
            updateOkButton();
        }

        private void radioUseCustomProxy_CheckedChanged(object sender, EventArgs e) {
            setProxyType();
            updateOkButton();
        }

        private void radioUseNoProxy_CheckedChanged(object sender, EventArgs e) {
            setProxyType();
            updateOkButton();
        }

        private void textProxyHost_TextChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void numericProxyPort_ValueChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void checkUseProxyAuth_CheckedChanged(object sender, EventArgs e) {
            setProxyType();
            updateOkButton();
        }

        private void textProxyUserName_TextChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void textProxyPassword_TextChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void setProxyType() {
            if (radioUseNoProxy.Checked) {
                currentProxyType = ProxyType.NONE;
            } else if (radioUseCustomProxy.Checked) {
                currentProxyType = ProxyType.CUSTOM;
            } else {
                currentProxyType = ProxyType.SYSTEM;
            }

            updateCustomProxyControls();
        }

        private void updateCustomProxyControls() {
            bool enabled = currentProxyType == ProxyType.CUSTOM;
            textProxyHost.Enabled = enabled;
            numericProxyPort.Enabled = enabled;
            checkUseProxyAuth.Enabled = enabled;
            textProxyUserName.Enabled = enabled && checkUseProxyAuth.Checked;
            textProxyPassword.Enabled = enabled && checkUseProxyAuth.Checked;
        }

        private void checkDisableIssueLinks_CheckedChanged(object sender, EventArgs e) {
            radioDisableIssueLinksForAllFiles.Enabled = checkDisableIssueLinks.Checked;
            radioDisableIssueLinksForLargeFiles.Enabled = checkDisableIssueLinks.Checked;
            numericMaxIssueLinksFileLength.Enabled = radioDisableIssueLinksForLargeFiles.Checked && checkDisableIssueLinks.Checked;
            updateOkButton();
        }

        private void radioDisableIssueLinksForAllFiles_CheckedChanged(object sender, EventArgs e) {
            numericMaxIssueLinksFileLength.Enabled = radioDisableIssueLinksForLargeFiles.Checked && checkDisableIssueLinks.Checked;
            updateOkButton();
        }

        private void radioDisableIssueLInksForLargeFiles_CheckedChanged(object sender, EventArgs e) {
            numericMaxIssueLinksFileLength.Enabled = radioDisableIssueLinksForLargeFiles.Checked && checkDisableIssueLinks.Checked;
            updateOkButton();
        }

        private void numericMaxIssueLinksFileLength_ValueChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        public static bool shouldShowIssueLinks(int editorLineCount) {
            if (!IssueLinksInEditorDisabled) return true;

            if (IssueLinksInEditorDisabledForAllFiles) return false;

            return editorLineCount < MaxIssueLinksInEditorFileSize;
        }

        public static bool AllIssueLinksDisabled {
            get {
                return IssueLinksInEditorDisabled && IssueLinksInEditorDisabledForAllFiles;
            }
        }
    }
}