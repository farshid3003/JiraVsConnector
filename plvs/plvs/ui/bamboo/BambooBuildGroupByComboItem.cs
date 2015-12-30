using System.Collections.Generic;
using Atlassian.plvs.attributes;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.ui.bamboo.treemodels;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.bamboo {
    internal class BambooBuildGroupByComboItem {
        private readonly BambooBuildListModel model;
        public GroupBy By { get; private set; }

        public enum GroupBy {
            [StringValue("Project And Branch")] PROJECT_AND_BRANCH,
            [StringValue("None")] NONE
        }

        private delegate AbstractBuildTreeModel CreateTreeModel(BambooBuildListModel model);

        private static readonly SortedDictionary<GroupBy, CreateTreeModel> TREE_MODEL_CREATORS =
            new SortedDictionary<GroupBy, CreateTreeModel> {
                {GroupBy.PROJECT_AND_BRANCH, model => new ProjectAndBranchesBuildTreeModel(model)},
                {GroupBy.NONE, model => new FlatBuildTreeModel(model)}
            };

        public BambooBuildGroupByComboItem(GroupBy groupBy, BambooBuildListModel model) {
            this.model = model;
            By = groupBy;
        }

        public override string ToString() {
            return By.GetStringValue();
        }

        public AbstractBuildTreeModel TreeModel {
            get {
                return TREE_MODEL_CREATORS.ContainsKey(By) 
                    ? TREE_MODEL_CREATORS[By](model) 
                    : new FlatBuildTreeModel(model);
            }
        }
    }
}