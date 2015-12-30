using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public sealed class GhBoardTreeNode : TreeNodeWithJiraServer, TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {
        private readonly JiraServer server;

        public GhBoardTreeNode(JiraServer server, RapidBoard board, int imageIdx)
            : base(board.Name, imageIdx) {

            this.server = server;
            this.Board = board;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public RapidBoard Board { get; private set; }

        public string NodeKey {
            get { return "JIRA_Gh_Board_Node_" + Server.GUID + "_" + Board.Id; }
        }

        public bool NodeExpanded {
            get { return IsExpanded; }
            set { if (value) Expand(); else Collapse(); }
        }
    }
}