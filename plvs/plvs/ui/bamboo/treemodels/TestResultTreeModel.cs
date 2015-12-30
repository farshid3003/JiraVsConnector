using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    internal class TestResultTreeModel : ITreeModel {
        private readonly ICollection<BambooTest> tests;

        private bool failedOnly;
        public bool FailedOnly {
            get { return failedOnly; } 
            set {
                failedOnly = value;
                updateModel();
            }
        }

        public TestResultTreeModel(ICollection<BambooTest> tests, bool failedOnly) {
            this.tests = tests;
            FailedOnly = failedOnly;
        }

        private void updateModel() {
            if (StructureChanged != null) StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
        }

        public IEnumerable GetChildren(TreePath treePath) {
            List<object> children = new List<object>();
            object[] fullPath = treePath.FullPath;
            foreach (BambooTest test in tests) {
                if (failedOnly && !test.Result.Equals(BambooTest.TestResult.FAILED)) continue;

                string[] testClassStrings = splitTestClass(test);
                int pathPartsCount = fullPath.Count();
                if (pathPartsCount > testClassStrings.Count()) continue;
                StringBuilder sbPath = new StringBuilder();
                StringBuilder sbClass = new StringBuilder();
                for (int i = 0; i < pathPartsCount; ++i) {
                    sbPath.Append(fullPath[i]).Append('.');
                    sbClass.Append(testClassStrings[i]).Append('.');
                }
                if (!sbPath.ToString().Equals(sbClass.ToString())) continue;

                if (testClassStrings.Count() > pathPartsCount) {
                    bool isPackage = testClassStrings.Count() > 1 && pathPartsCount < testClassStrings.Count() - 1;
                    TestPackageOrClassNode node = new TestPackageOrClassNode(testClassStrings[pathPartsCount], isPackage);
                    if (!children.Contains(node)) {
                        children.Add(node);
                    }
                } else {
                    children.Add(new TestMethodNode(test));
                }
            }
            return children;
        }

        private static string[] splitTestClass(BambooTest test) {
            string[] strings = test.ClassName.Split(new[] {'.'});
            return strings;
        }

        public bool IsLeaf(TreePath treePath) {
            return treePath.LastNode is TestMethodNode;
        }

        public event EventHandler<TreePathEventArgs> StructureChanged;

#pragma warning disable 67
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
    }
}
