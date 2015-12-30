using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class ProjectNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;

        public ProjectNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraProject project)
            : base(model, facade, server, project.ToString(), 0) {
            this.project = project;
            Server = server;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "?" + authString;
        }

        public override void onClick(StatusLabel status) { }

        public override string ToString() {
            return project.ToString();
        }
    }
}
