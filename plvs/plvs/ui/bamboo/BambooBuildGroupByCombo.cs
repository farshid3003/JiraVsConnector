using System;
using System.ComponentModel;
using System.Windows.Forms;
using Atlassian.plvs.store;

namespace Atlassian.plvs.ui.bamboo {
    public partial class BambooBuildGroupByCombo : ToolStripComboBox {
        private const string SELECTED_INDEX = "BambooBuildListGroupBy";

        public BambooBuildGroupByCombo() {
            InitializeComponent();

            init();
        }

        public BambooBuildGroupByCombo(IContainer container) {
            container.Add(this);

            InitializeComponent();

            init();
        }

        private void init() {
            SelectedIndexChanged += selectedIndexChanged;
        }

        void selectedIndexChanged(object sender, EventArgs e) {
            var store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(SELECTED_INDEX, SelectedIndex);
        }

        public void restoreSelectedIndex() {
            var store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            var selectedIndex = store.loadParameter(SELECTED_INDEX, 0);
            SelectedIndex = Items.Count > selectedIndex ? selectedIndex : 0;
        }
    }
}