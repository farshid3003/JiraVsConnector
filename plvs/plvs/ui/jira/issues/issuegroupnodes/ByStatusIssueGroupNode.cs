using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    class ByStatusIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByStatusIssueGroupNode(JiraServer server, JiraNamedEntity status)
            : base(server, status) {
        }
    }
}