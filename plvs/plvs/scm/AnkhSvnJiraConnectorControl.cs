using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;

namespace Atlassian.plvs.scm {
    public partial class AnkhSvnJiraConnectorControl : UserControl {
        public AnkhSvnJiraConnectorControl() {
            InitializeComponent();
        }

        private readonly MySettings settings = new MySettings("jira");

        public IssueRepositorySettings Settings {
            get { return settings; }
            set {
                if (value.CustomProperties == null) return;
                foreach (KeyValuePair<string, object> property in value.CustomProperties) {
                    settings.CustomProperties[property.Key] = property.Value;
                }
                checkIntegrate.Enabled = GlobalSettings.AnkhSvnIntegrationEnabled;
                checkIntegrate.Checked = GlobalSettings.AnkhSvnIntegrationEnabled
                    && settings.CustomProperties.ContainsKey(Constants.INTEGRATE_WITH_ANKH)
                    && settings.CustomProperties[Constants.INTEGRATE_WITH_ANKH].Equals("1");
            }
        }

        public event EventHandler<ConfigPageEventArgs> PageChanged;

        private void InvokePageChanged(ConfigPageEventArgs e) {
            EventHandler<ConfigPageEventArgs> handler = PageChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        private class MySettings : IssueRepositorySettings {
            private readonly Dictionary<string, object> customProperties;

            public MySettings(string connectorName) : base(connectorName) {
                customProperties = new Dictionary<string, object>();
            }

            public override Uri RepositoryUri {
                get { return new Uri("http://www.atlassian.com/software/jira"); }
            }

            public override IDictionary<string, object> CustomProperties {
                get { return customProperties; }
            }
        }

        private void checkIntegrate_CheckedChanged(object sender, EventArgs e) {
            settings.CustomProperties[Constants.INTEGRATE_WITH_ANKH] = 
                GlobalSettings.AnkhSvnIntegrationEnabled && checkIntegrate.Checked ? "1" : "0";
            InvokePageChanged(new ConfigPageEventArgs { IsComplete = true });
        }
    }
}
