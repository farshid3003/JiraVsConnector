using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class GhGroupTreeNode : JiraFilterGroupTreeNode {
        public GhGroupTreeNode(JiraServer server, int imageIdx)
            : base(server, "Agile", imageIdx) {
        }

        public override string NodeKey {
            get { return "JIRA_Gh_Node_" + Server.GUID; }
        }
    }
}