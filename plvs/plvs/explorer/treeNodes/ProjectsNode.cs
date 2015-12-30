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
    class ProjectsNode : AbstractNavigableTreeNodeWithServer {
        private readonly Control parent;

        private bool projectsLoaded;

        public ProjectsNode(Control parent, JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server)
            : base(model, facade, server, "Projects", 0) {

            this.parent = parent;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/secure/BrowseProjects.jspa?selectedCategory=all&" + authString ; 
        }

        public override void onClick(StatusLabel status) {
            if (projectsLoaded) return;
            projectsLoaded = true;
            Thread t = PlvsUtils.createThread(() => loadProjects(status));
            t.Start();
        }

        private void loadProjects(StatusLabel status) {
            try {
                List<JiraProject> projects = Facade.getProjects(Server);
                parent.Invoke(new MethodInvoker(() => populateProjects(projects)));
            } catch (Exception e) {
                status.setError("Failed to retrieve projects", e);
            }
        }

        private void populateProjects(IEnumerable<JiraProject> projects) {
            SortedDictionary<string, JiraProject> sorted = new SortedDictionary<string, JiraProject>();
            foreach (JiraProject project in projects) {
                sorted[project.Key] = project;
            }
            foreach (JiraProject project in sorted.Values) {
                ProjectNode projectNode = new ProjectNode(Model, Facade, Server, project);
                Nodes.Add(projectNode);
                projectNode.Nodes.Add(new ComponentsNode(parent, Model, Facade, Server, project));
                projectNode.Nodes.Add(new VersionsNode(parent, Model, Facade, Server, project));
            }
            Expand();
        }
    }
}
