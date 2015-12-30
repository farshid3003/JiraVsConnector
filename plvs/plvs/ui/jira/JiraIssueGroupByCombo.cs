using System;
using System.ComponentModel;
using System.Windows.Forms;
using Atlassian.plvs.store;

namespace Atlassian.plvs.ui.jira {
    public partial class JiraIssueGroupByCombo : ToolStripComboBox {
        private const string SELECTED_INDEX = "JiraIssueListGroupBy";

        public JiraIssueGroupByCombo() {
            InitializeComponent();

            init();
        }

        public JiraIssueGroupByCombo(IContainer container) {
            container.Add(this);

            InitializeComponent();

            init();
        }

        private void init() {
            SelectedIndexChanged += selectedIndexChanged;
        }

        void selectedIndexChanged(object sender, EventArgs e) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(SELECTED_INDEX, SelectedIndex);
        }

        public void restoreSelectedIndex() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            int selectedIndex = store.loadParameter(SELECTED_INDEX, 0);
            SelectedIndex = Items.Count > selectedIndex ? selectedIndex : 0;
        }
    }
}