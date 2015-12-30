using Atlassian.plvs.util.jira;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextMarkerClientEventSink : AbstractMarkerClientEventSink {
        private readonly string issueKey;

        public TextMarkerClientEventSink(string issueKey) {
            this.issueKey = issueKey;
        }

        public override int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            if (issueKey != null) {
                pbstrText[0] = "Double click to try open " + issueKey +
                               "\non the currently selected server,\nRight click for more options";
            }

            return VSConstants.S_OK;
        }

        public override int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
            // For each command we add we have to specify that we support it.
            // Furthermore it should always be enabled.
// ReSharper disable BitwiseOperatorOnEnumWihtoutFlags
            const uint menuItemFlags = (uint) (OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
// ReSharper restore BitwiseOperatorOnEnumWihtoutFlags

            if (pcmdf == null) {
                return VSConstants.S_OK;
            }

            switch (iItem) {
                case 0:
                    if (issueKey != null && pbstrText != null) {
                        pbstrText[0] = "Open Issue " + issueKey + " in IDE";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;
                case 1:
                    if (issueKey != null && pbstrText != null) {
                        pbstrText[0] = "View Issue " + issueKey + " in the Browser";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    pcmdf[0] = menuItemFlags;
                    return VSConstants.S_OK;

                default:
                    pcmdf[0] = 0;
                    return VSConstants.S_FALSE;
            }
        }

        public override int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
            switch (iItem) {
                case 0:
                    JiraIssueUtils.openInIde(issueKey);
                    return VSConstants.S_OK;
                case 1:
                    JiraIssueUtils.launchBrowser(issueKey);
                    return VSConstants.S_OK;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    JiraIssueUtils.openInIde(issueKey);
                    return VSConstants.S_OK;

                default:
                    return VSConstants.S_OK;
            }
        }
    }
}