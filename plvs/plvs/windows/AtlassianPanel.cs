using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.bamboo;
using Atlassian.plvs.ui.crucible;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using EnvDTE;

namespace Atlassian.plvs.windows {
    public partial class AtlassianPanel : ToolWindowFrame {

        public static AtlassianPanel Instance { get; private set; }

        private Autoupdate.UpdateAction updateAction;
        private Exception updateException;

        private const string UPDATE_BALOON_TITLE = "Atlassian Connector for Visual Studio";
        private const int UPDATE_BALOON_TIMEOUT = 60000;

        public TabJira Jira { get { return tabJira; } }
        public TabBamboo Bamboo { get { return tabBamboo; } }
        public TabCrucible Crucible { get { return tabCrucible; } }

        private bool jiraTabVisible;

        public bool JiraTabVisible {
            get { return jiraTabVisible; }
            set { 
                jiraTabVisible = value;
                reinsertTabs();
            }
        }

        private bool bambooTabVisible;

        public bool BambooTabVisible {
            get { return bambooTabVisible; }
            set {
                bambooTabVisible = value; 
                reinsertTabs();
            }
        }

        private bool crucibleTabVisible;

        public bool CrucibleTabVisible {
            get { return crucibleTabVisible; }
            set {
                crucibleTabVisible = value;
                reinsertTabs();
            }
        }

        private LinkLabel linkAddServer;

        public AtlassianPanel() {
            InitializeComponent();

            productTabs.ImageList = new ImageList();
            productTabs.ImageList.Images.Add(Resources.tab_jira);
            productTabs.ImageList.Images.Add(Resources.tab_bamboo);

            notifyUpdate.Visible = false;

            Instance = this;

            Jira.AddNewServerLinkClicked += jira_AddNewServerLinkClicked;
            Bamboo.AddNewServerLinkClicked += bamboo_AddNewServerLinkClicked;

            reinsertTabs();
        }

        private void reinsertTabs() {
            productTabs.TabPages.Clear();

            if (JiraTabVisible) {
                productTabs.TabPages.Add(tabIssues);
            }
            if (BambooTabVisible) {
                productTabs.TabPages.Add(tabBuilds);
            }
            if (CrucibleTabVisible) {
                productTabs.TabPages.Add(tabReviews);
            }

            bool tabPanelVisible = productTabs.TabPages.Count > 0;
            if (tabPanelVisible) {
                if (linkAddServer != null && mainContainer.ContentPanel.Controls.Contains(linkAddServer)) {
                    mainContainer.ContentPanel.Controls.Remove(linkAddServer);
                }
            } else {
                if (linkAddServer == null || !mainContainer.ContentPanel.Controls.Contains(linkAddServer)) {
                    linkAddServer = new LinkLabel {
                        Dock = DockStyle.Fill,
                        Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238),
                        Image = Resources.ide_plugin_16_with_padding,
                        Location = new Point(0, 0),
                        Name = "linkAddServers",
                        Size = new Size(1120, 510),
                        TabIndex = 0,
                        TabStop = true,
                        Text = "Add or Enable Servers",
                        BackColor = Color.White,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    linkAddServer.Click += (s, e) => openProjectProperties(null);
                    mainContainer.ContentPanel.Controls.Add(linkAddServer);
                }
            }
            productTabs.Visible = tabPanelVisible;
        }

        private void bamboo_AddNewServerLinkClicked(object sender, EventArgs e) {
            openProjectProperties(Server.BambooServerTypeGuid);
        }

        private void jira_AddNewServerLinkClicked(object sender, EventArgs e) {
            openProjectProperties(Server.JiraServerTypeGuid);
        }

        private void buttonProjectProperties_Click(object sender, EventArgs e) {
            openProjectProperties(null);
        }

        public void openProjectProperties(Guid? serverTypeToCreate) {
            ProjectConfiguration dialog = 
                new ProjectConfiguration(serverTypeToCreate, JiraServerModel.Instance, BambooServerModel.Instance, tabJira.Facade, tabBamboo.Facade);
            dialog.ShowDialog(this);
            if (dialog.SomethingChanged) {
                // todo: only do this for changed servers - add server model listeners
                // currently this code blows :)
                tabJira.reinitialize(null);
                tabBamboo.reinitialize();
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e) {
            new About().ShowDialog(this);
        }

        private void buttonReportBug_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser("https://ecosystem.atlassian.net/secure/CreateIssueDetails!init.jspa?pid=13773&issuetype=1");
// ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
        }

        private void buttonGlobalProperties_Click(object sender, EventArgs e) {
            openGlobalProperties();
        }

        public void openGlobalProperties() {
            GlobalSettings globals = new GlobalSettings();
            globals.ShowDialog();
        }

        private void notifyUpdate_MouseDoubleClick(object sender, MouseEventArgs e) {
            updateIconOrBaloonClicked();
        }

        private void notifyUpdate_BalloonTipClicked(object sender, EventArgs e) {
            updateIconOrBaloonClicked();
        }

        private void updateIconOrBaloonClicked() {
            notifyUpdate.Visible = false;
            if (updateAction != null) {
                updateAction();
            } else if (updateException != null) {
                PlvsUtils.showError("Unable to retrieve autoupdate information", updateException);
            }
        }

        public void setAutoupdateAvailable(Autoupdate.UpdateAction action) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = action;
                                             updateException = null;
                                             notifyUpdate.Visible = true;
                                             notifyUpdate.Icon = Icon.FromHandle(Resources.ico_updateavailable.GetHicon());
                                             notifyUpdate.Text = "New version of the connector available, double-click to install";
                                             notifyUpdate.BalloonTipIcon = ToolTipIcon.Info;
                                             notifyUpdate.BalloonTipTitle = UPDATE_BALOON_TITLE;
                                             notifyUpdate.BalloonTipText = "New version of the connector is available, click here to install";
                                             notifyUpdate.ShowBalloonTip(UPDATE_BALOON_TIMEOUT);
                                         }));
        }

        public void setAutoupdateUnavailable(Exception exception) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = null;
                                             updateException = exception;
                                             notifyUpdate.Visible = true;
                                             notifyUpdate.Icon = Icon.FromHandle(Resources.ico_updatenotavailable.GetHicon());
                                             notifyUpdate.Text = "Unable to retrieve update information, double-click for details";
                                             notifyUpdate.BalloonTipIcon = ToolTipIcon.Error;
                                             notifyUpdate.BalloonTipTitle = UPDATE_BALOON_TITLE;
                                             notifyUpdate.BalloonTipText = "Unable to retrieve connector update information, click here for details";
                                             notifyUpdate.ShowBalloonTip(UPDATE_BALOON_TIMEOUT);
                                         }));
        }

        public void reinitialize(DTE dte, PlvsPackage package) {

            PlvsUtils.updateKeyBindingsInformation(dte, new Dictionary<string, ToolStripItem>
                                            {
                                                { "Tools.AtlassianProjectConfiguration", buttonProjectProperties },
                                                { "Tools.AtlassianGlobalConfiguration", buttonGlobalProperties }
                                            });

            tabJira.reinitialize(dte);
            tabBamboo.reinitialize();
            tabCrucible.reinitialize(dte, package);
        }

        public void shutdown() {
            tabJira.reinitialize(null);
            tabBamboo.shutdown();
        }
    }
}