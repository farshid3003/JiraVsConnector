using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira.issues.menus {
    public sealed class SavedFilterContextMenu : ContextMenuStrip {
        private readonly JiraSavedFilter filter;

        public SavedFilterContextMenu(JiraSavedFilter filter) {
            this.filter = filter;
            if (filter.ViewUrl != null) {
                Items.Add(new ToolStripMenuItem("View Filter in Browser", Resources.view_in_browser, new EventHandler(browseFilter)));
            }
        }

        private void browseFilter(object sender, EventArgs e) {
            var url = filter.ViewUrl;
            try {
                PlvsUtils.runBrowser(url);
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
