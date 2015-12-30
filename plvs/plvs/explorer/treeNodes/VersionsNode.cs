using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;

namespace Atlassian.plvs.explorer.treeNodes {
    class VersionsNode : AbstractNavigableTreeNodeWithServer {
        private readonly Control parent;
        private readonly JiraProject project;

        private bool versionsLoaded;

        public VersionsNode(Control parent, JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraProject project)
            : base(model, facade, server, "Versions", 0) {

            this.parent = parent;
            this.project = project;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key
                + "?" + authString 
                + "#selectedTab=com.atlassian.jira.plugin.system.project%3Aversions-panel"; 
        }

        public override void onClick(StatusLabel status) {
            if (versionsLoaded) return;
            versionsLoaded = true;
            Thread t = PlvsUtils.createThread(() => loadVersions(Facade, status));
            t.Start();
        }

        private void loadVersions(AbstractJiraServerFacade facade, StatusLabel status) {
            try {
                List<JiraNamedEntity> versions = facade.getVersions(Server, project);
                parent.Invoke(new MethodInvoker(()=> populateVersions(versions)));
            } catch (Exception e) {
                status.setError("Unable to retrieve versions list", e);
            }
        }

        private void populateVersions(List<JiraNamedEntity> versions) {
            versions.Reverse();
            foreach (JiraNamedEntity version in versions) {
                Nodes.Add(new VersionNode(Model, Facade, Server, project, version));
            }
            ExpandAll();
        }
    }
}
