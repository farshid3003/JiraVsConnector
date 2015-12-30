using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    class ByPriorityIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByPriorityIssueGroupNode(JiraServer server, JiraNamedEntity priority)
            : base(server, priority) {
        }
    }
}