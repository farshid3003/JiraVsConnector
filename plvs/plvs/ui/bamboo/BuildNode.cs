using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo {
    public class BuildNode : TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {

        private const int PROBABLE_GARBAGE_REASON_LENGTH = 300;

        public BambooBuild Build { get; set; }

        public BuildNode(BambooBuild build) {
            Build = build;
        }

        public bool? IsInProgress {
            get { return Build.State == BambooBuild.PlanState.IN_QUEUE || Build.State == BambooBuild.PlanState.BUILDING; }
        }

        public Image Icon {
            get {
                switch (Build.Result) {
                    case BambooBuild.BuildResult.SUCCESSFUL:
                        return Resources.icn_plan_passed;
                    case BambooBuild.BuildResult.FAILED:
                        return Resources.icn_plan_failed;
                    default:
                        // todo: fixme - this is not right, disabled plans should be disabled, 
                        // unknown results should have a separate icon
                        return Resources.icn_plan_disabled;
                }
            }
        }

        public string Key { get { return Build.Key; } }

        public string Tests { 
            get {
                if (Build.SuccessfulTests == 0 && Build.FailedTests == 0) {
                    return "No tests";
                }
                return Build.SuccessfulTests + "/" + (Build.SuccessfulTests + Build.FailedTests) + " tests passed";
            } 
        }

        public string Reason {
            get {
                string txt = Build.Reason.Length > PROBABLE_GARBAGE_REASON_LENGTH ? "[garbage received?]" : stripHtml(Build.Reason);
                return txt.Replace("&", "&&");
            }
        }

        private static string stripHtml(string html) {
            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty); 
        }

        public string Completed { get { return Build.RelativeTime; } }

        public string Duration { get { return Build.Duration; } }

        public string Server { get { return Build.Server.Name; } }

        public List<BuildNode> BranchNodes { get; set; }

        public string NodeKey {
            get { return Build.Server.GUID + Build.Key; }
        }

        public bool NodeExpanded { get; set; }
    }
}
