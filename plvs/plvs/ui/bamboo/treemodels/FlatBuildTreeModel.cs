using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.util.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    public class FlatBuildTreeModel : AbstractBuildTreeModel {

        private readonly SortedDictionary<string, BuildNode> buildNodes = new SortedDictionary<string, BuildNode>();

        public FlatBuildTreeModel(BambooBuildListModel builds)
            : base(builds) {
        }

        protected override void modelModelChanged(object sender, EventArgs e) {
            fillModel(Model.Builds);
        }

        protected override void fillModel(ICollection<BambooBuild> builds) {
            if (builds == null || builds.Count == 0) {
                buildNodes.Clear();
                if (StructureChanged != null) {
                    StructureChanged(this, new TreePathEventArgs(TreePath.Empty));    
                }
                return;
            }
            foreach (var build in builds) {
                if (buildNodes.ContainsKey(getMapKeyFromBuild(build))) {
                    buildNodes[getMapKeyFromBuild(build)].Build = build;
                    if (NodesChanged != null) {
                        NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                } else {
                    buildNodes[getMapKeyFromBuild(build)] = new BuildNode(build);
                    if (NodesInserted != null) {
                        NodesInserted(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                }
                var toRemove = (from key in buildNodes.Keys 
                                let found = builds.Any(b => key.Equals(getMapKeyFromBuild(b))) 
                                where !found 
                                select key).ToList();
                
                foreach (var key in toRemove) {
                    var n = buildNodes[key];
                    buildNodes.Remove(key);
                    if (NodesRemoved != null) {
                        NodesRemoved(this, new TreeModelEventArgs(TreePath.Empty, new object[] {n}));
                    }
                }
            }
        }

        private static string getMapKeyFromBuild(BambooBuild build) {
            return build.Server.GUID + BambooBuildUtils.getPlanKey(build);
        }

        private int getIndex(BambooBuild build) {
            var i = 0;
            foreach (var key in buildNodes.Keys) {
                if (key.Equals(getMapKeyFromBuild(build))) {
                    return i;
                }
                ++i;
            }
            throw new ArgumentException("build node not found");
        }

        private object getNode(BambooBuild build) {
            return buildNodes[getMapKeyFromBuild(build)];
        }

        public override IEnumerable GetChildren(TreePath treePath) {
            return buildNodes.Values;
        }

        public override bool IsLeaf(TreePath treePath) {
            return true;
        }

        public override event EventHandler<TreeModelEventArgs> NodesChanged;
        public override event EventHandler<TreeModelEventArgs> NodesInserted;
        public override event EventHandler<TreeModelEventArgs> NodesRemoved;
        public override event EventHandler<TreePathEventArgs> StructureChanged;
    }
}
