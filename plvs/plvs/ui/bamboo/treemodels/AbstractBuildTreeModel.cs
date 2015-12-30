using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.models.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    public abstract class AbstractBuildTreeModel : ITreeModel {
        protected BambooBuildListModel Model { get; private set; }

        protected AbstractBuildTreeModel(BambooBuildListModel model) {
            Model = model;
        }

        public void init() {
            fillModel(Model.Builds);
            Model.ModelChanged += modelModelChanged;
        }

        public void shutdown() {
            Model.ModelChanged -= modelModelChanged;
        }

        protected abstract void fillModel(ICollection<BambooBuild> builds);
        protected abstract void modelModelChanged(object sender, EventArgs e);

        public abstract IEnumerable GetChildren(TreePath treePath);
        public abstract bool IsLeaf(TreePath treePath);
        public abstract event EventHandler<TreeModelEventArgs> NodesChanged;
        public abstract event EventHandler<TreeModelEventArgs> NodesInserted;
        public abstract event EventHandler<TreeModelEventArgs> NodesRemoved;
        public abstract event EventHandler<TreePathEventArgs> StructureChanged;
    }
}
