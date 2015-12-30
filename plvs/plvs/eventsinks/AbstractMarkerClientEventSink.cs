using Atlassian.plvs.markers;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public abstract class AbstractMarkerClientEventSink : IVsTextMarkerClient {
        public IVsTextLineMarker Marker { get; set; }

        #region Implementation of IVsTextMarkerClient

        public abstract int GetTipText(IVsTextMarker pMarker, string[] pbstrText);

        public abstract int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf);

        public abstract int ExecMarkerCommand(IVsTextMarker pMarker, int iItem);

        public void MarkerInvalidated() {
            JiraEditorLinkManager.OnMarkerInvalidated(Marker);
        }

        public void OnBufferSave(string pszFileName) { }

        public void OnBeforeBufferClose() { }

        public void OnAfterSpanReload() {
        }

        public int OnAfterMarkerChange(IVsTextMarker pMarker) {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
