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
    class PrioritiesNode : AbstractNavigableTreeNodeWithServer {
        private readonly Control parent;

        private bool prioritiesLoaded;

        public PrioritiesNode(Control parent, JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server)
            : base(model, facade, server, "Priorities", 0) {

            this.parent = parent;
        }

        public override string getUrl(string authString) {
            return Server.Url + "?" + authString;
        }

        public override void onClick(StatusLabel status) {
            if (prioritiesLoaded) return;
            prioritiesLoaded = true;
            Thread t = PlvsUtils.createThread(() => loadPriorities(Facade, status));
            t.Start();
        }

        private void loadPriorities(AbstractJiraServerFacade facade, StatusLabel status) {
            try {
                List<JiraNamedEntity> priorities = facade.getPriorities(Server);
                parent.Invoke(new MethodInvoker(()=> populatePriorities(priorities)));
            } catch (Exception e) {
                status.setError("Unable to retrieve priorities", e);
            }
        }

        private void populatePriorities(IEnumerable<JiraNamedEntity> priorities) {
            foreach (JiraNamedEntity priority in priorities) {
                Nodes.Add(new PriorityNode(Model, Facade, Server, priority));
            }
            ExpandAll();
        }
    }
}
