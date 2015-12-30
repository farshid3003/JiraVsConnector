using System;
using System.Diagnostics;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.explorer;
using Atlassian.plvs.markers;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.bamboo;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class SolutionEventSink : IVsSolutionEvents {
        public delegate ToolWindowPane CreateToolWindow();

        private readonly PlvsPackage package;
        private readonly CreateToolWindow createAtlassianWindow;
        private readonly CreateToolWindow createIssueDetailsWindow;
        private readonly CreateToolWindow createBuildDetailsWindow;

        public SolutionEventSink(
            PlvsPackage package, 
            CreateToolWindow createAtlassianWindow, 
            CreateToolWindow createIssueDetailsWindow, 
            CreateToolWindow createBuildDetailsWindow) {

            this.package = package;
            this.createAtlassianWindow = createAtlassianWindow;
            this.createIssueDetailsWindow = createIssueDetailsWindow;
            this.createBuildDetailsWindow = createBuildDetailsWindow;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            PlvsLogger.log("SolutionEventSink.OnAfterOpenSolution()");
            try {
                GlobalSettings.checkFirstRun();

                JiraServerModel.Instance.clear();
                JiraServerModel.Instance.load();
                BambooServerModel.Instance.clear();
                BambooServerModel.Instance.load();
                RecentlyViewedIssuesModel.Instance.load();
                JiraCustomFilter.load();

                DTE dte = (DTE)package.GetService(typeof(DTE));
                PlvsUtils.Dte = dte;

                ToolWindowManager.Instance.AtlassianWindow = createAtlassianWindow();

                AtlassianPanel.Instance.reinitialize(dte, package);

                ToolWindowManager.Instance.IssueDetailsWindow = createIssueDetailsWindow();
                ToolWindowManager.Instance.BuildDetailsWindow = createBuildDetailsWindow();

                IssueDetailsWindow.Instance.Solution = dte.Solution;
                BuildDetailsWindow.Instance.Solution = dte.Solution;

                JiraEditorLinkManager.OnSolutionOpened();
                Autoupdate.Instance.initialize();
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                new ExceptionViewer("Failed to initialize Atlassian tool windows", e).ShowDialog();
            }

            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved) {
            try {
                JiraEditorLinkManager.OnSolutionClosed();
                if (ToolWindowManager.Instance.AtlassianWindow == null) return VSConstants.S_OK;
                JiraServerModel.Instance.clear();
                BambooServerModel.Instance.clear();
                AtlassianPanel.Instance.shutdown();
                AtlassianPanel.Instance.FrameVisible = false;
                ParameterStoreManager.Instance.clear();
                JiraIssueListModelImpl.Instance.removeAllListeners();
                IssueDetailsWindow.Instance.clearAllIssues();
                IssueDetailsWindow.Instance.FrameVisible = false;
                BuildDetailsWindow.Instance.clearAllBuilds();
                BuildDetailsWindow.Instance.FrameVisible = false;
                ToolWindowManager.Instance.AtlassianWindow = null;
                Autoupdate.Instance.shutdown();
                DropZone.closeAll();
                JiraServerExplorer.closeAll();
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved) {
            return VSConstants.S_OK;
        }
    }
}