using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.util.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    public class ProjectAndBranchesBuildTreeModel : AbstractBuildTreeModel {

        private readonly SortedDictionary<string, ProjectNode> projectNodes = new SortedDictionary<string, ProjectNode>();
        private readonly SortedDictionary<string, BuildNode> masterNodes = new SortedDictionary<string, BuildNode>();

        public ProjectAndBranchesBuildTreeModel(BambooBuildListModel builds)
            : base(builds) {
        }

        protected override void modelModelChanged(object sender, EventArgs e) {
            fillModel(Model.Builds);
        }

        protected override void fillModel(ICollection<BambooBuild> builds) {
            projectNodes.Clear();
            masterNodes.Clear();
            if (builds != null) {
                fillProjects(builds);
                fillMasters(builds);
                fillBranches(builds);
            }
            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        private void fillBranches(IEnumerable<BambooBuild> builds) {
            foreach (var build in builds) {
                if (build.MasterPlanKey == null) continue;
                var masterKey = build.Server.GUID + build.MasterPlanKey;
                if (!masterNodes.ContainsKey(masterKey)) continue;
                if (masterNodes[masterKey].BranchNodes == null) {
                    masterNodes[masterKey].BranchNodes = new List<BuildNode>();
                }
                masterNodes[masterKey].BranchNodes.Add(new BuildNode(build));
            }
        }

        private void fillMasters(IEnumerable<BambooBuild> builds) {
            foreach (var build in builds) {
                if (build.MasterPlanKey != null) continue;
                var key = getMapPlanKeyFromBuild(build);
                if (!masterNodes.ContainsKey(key)) {
                    masterNodes[key] = new BuildNode(build); 
                }
            }
        }

        private void fillProjects(IEnumerable<BambooBuild> builds) {
            foreach (var build in builds) {
                var proj = getMapProjectKeyFromBuild(build);
                if (!projectNodes.ContainsKey(proj)) {
                    projectNodes[proj] = new ProjectNode(build.Server.GUID.ToString(), build.ProjectName + " (" + build.ProjectKey + ")", build.ProjectKey);
                }
            }
        }

        private static string getMapProjectKeyFromBuild(BambooBuild build) {
            return build.Server.GUID + build.ProjectKey;
        }

        private static string getMapPlanKeyFromBuild(BambooBuild build) {
            return build.Server.GUID + BambooBuildUtils.getPlanKey(build);
        }

        public override IEnumerable GetChildren(TreePath treePath) {
            if (treePath.IsEmpty()) {
                return projectNodes.Values;
            }
            if (treePath.LastNode is ProjectNode) {
                var pn = treePath.LastNode as ProjectNode;
                var res = new SortedDictionary<string, BuildNode>();
                foreach (var m in masterNodes.Values.Where(m => m.NodeKey.StartsWith(pn.NodeKey))) {
                    res[m.Key] = m;
                }
                return res.Values;
            }
            var n = treePath.LastNode as BuildNode;
            return n != null ? n.BranchNodes : null;
        }

        public override bool IsLeaf(TreePath treePath) {
            if (treePath.LastNode is ProjectNode) {
                return false;
            }

            var n = treePath.LastNode as BuildNode;
            if (n != null) {
                return n.BranchNodes == null  || n.BranchNodes.Count == 0;
            }

            return false;
        }

        public override event EventHandler<TreeModelEventArgs> NodesChanged;
        public override event EventHandler<TreeModelEventArgs> NodesInserted;
        public override event EventHandler<TreeModelEventArgs> NodesRemoved;
        public override event EventHandler<TreePathEventArgs> StructureChanged;
    }
}
