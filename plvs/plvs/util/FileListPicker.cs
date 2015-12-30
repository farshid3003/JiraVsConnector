using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EnvDTE;

namespace Atlassian.plvs.util {
    internal partial class FileListPicker : Form {

        private ProjectItemWrapper selectedItem;

        public ProjectItem SelectedFile { get { return selectedItem != null ? selectedItem.Item : null; }}

        internal class ProjectItemWrapper {
            public ProjectItemWrapper(ProjectItem item) {
                Item = item;
            }

            public ProjectItem Item { get; set; }

            public override string ToString() {
                return getFullName(Item);
            }

            private static string getFullName(ProjectItem item) {
                List<string> names = new List<string>();
                if (getParentNames(item, ref names)) {
                    names.Reverse();
                    StringBuilder sb = new StringBuilder();
                    foreach (string name in names) {
                        sb.Append(name).Append('\\');
                    }

                    return sb.Append(item.Name).ToString();
                }
                return "Unknown File Name";
            }

            private static bool getParentNames(ProjectItem item, ref List<string> names) {
                if (item.Collection.Parent is ProjectItem) {
                    ProjectItem parentItem = (ProjectItem)item.Collection.Parent;
                    names.Add(parentItem.Name);
                    return getParentNames(parentItem, ref names);
                }
                if (item.Collection.Parent is Project) {
                    Project parentProject = (Project)item.Collection.Parent;
                    string fullName = parentProject.FullName;
                    if (fullName.Contains("\\")) {
                        fullName = fullName.Substring(0, fullName.LastIndexOf("\\"));
                        names.Add(fullName);
                    }
                    return true;
                }
                return false;
            }
        }

        public FileListPicker(IEnumerable<ProjectItem> files) {
            InitializeComponent();

            foreach (ProjectItem file in files) {
                listFiles.Items.Add(new ProjectItemWrapper(file));
            }
        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listFiles.SelectedItem == null) return;

            selectedItem = listFiles.SelectedItem as ProjectItemWrapper;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listFiles_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char) Keys.Enter:
                    selectedItem = listFiles.SelectedItem as ProjectItemWrapper;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
                case (char) Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    break;
            }
        }
    }
}
