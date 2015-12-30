using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Atlassian.plvs.windows;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;

namespace Atlassian.plvs.ui.crucible {
    public partial class TabCrucible : UserControl {

        private PlvsPackage package;
        private Solution solution;
        private DTE dte;

        public TabCrucible() {
            InitializeComponent();
        }

        public void reinitialize(DTE d, PlvsPackage p) {
            AtlassianPanel.Instance.CrucibleTabVisible = false;
            package = p;
            dte = d;
            solution = dte.Solution;
        }
        
        private void buttonShowChanges_Click(object sender, EventArgs e) {
            Debug.WriteLine(string.Format("{0}={1}", solution.FileName, solution.FullName));

            string dir = solution.FileName.Substring(0, solution.FileName.LastIndexOf("\\"));
            try {

                dte.ExecuteCommand("File.SaveAll", "");

                ProcessStartInfo psi = new ProcessStartInfo("svn", "stat \"" + dir + "\"") {
                                           CreateNoWindow = true,
                                           UseShellExecute = false,
                                           RedirectStandardOutput = true
                                       };
                psi.EnvironmentVariables["Lang"] = "C";
                Process process = Process.Start(psi);
                string filesChanged = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                psi = new ProcessStartInfo("svn", "info \"" + dir + "\"") {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                psi.EnvironmentVariables["Lang"] = "C";
                process = Process.Start(psi);
                string solutionInfo = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            if (package == null) return;

//            IVsSccManager2 sscMgr = (IVsSccManager2)package.GetService(typeof(SVsSccManager));

#if false
            if (sscMgr == null) return;
            int installed;
            int hr = sscMgr.IsInstalled(out installed);
            if (VSConstants.S_OK != hr || installed == 0) return;

            foreach (Project project in solution.Projects) {
                getFileSscStatus(project.ProjectItems, sscMgr);
            }
            Debug.WriteLine(sscMgr);
#endif
        }

#if false
        private void getFileSscStatus(ProjectItems projectItems, IVsSccManager2 sscMgr) {
            if (projectItems == null || projectItems.Count == 0) return;
            foreach (ProjectItem item in projectItems) {
                if (item.Properties != null) {
                    Property fullName = item.Properties.Item("FullPath");
                    if (fullName != null) {
                        VsStateIcon[] icons = new VsStateIcon[1];
                        uint[] status = new uint[1];
                        if (VSConstants.S_OK == sscMgr.GetSccGlyph(1, new[] { fullName.Value.ToString() }, icons, status)) {
                            Debug.WriteLine(string.Format("status {0} - file {1}", status[0], fullName.Value));
                        }
                    }
                }
                getFileSscStatus(item.ProjectItems, sscMgr);
            }
        }
#endif
    }
}
