using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira {
    internal class JiraNamedEntityListViewItem : ListViewItem {
        public JiraNamedEntity Entity { get; private set; }

        public JiraNamedEntityListViewItem(JiraNamedEntity entity, int imageIdx) : base(entity.Name, imageIdx) {
            Entity = entity;
        }
    }
}