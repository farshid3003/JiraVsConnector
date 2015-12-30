using Atlassian.plvs.markers;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks
{
    class TextLinesEventSink : IVsTextLinesEvents {

        public IVsTextLines TextLines { get; set; }

        public IConnectionPoint ConnectionPoint { get; set; }

        public uint Cookie { get; set; }

        public void OnChangeLineText(TextLineChange[] pTextLineChange, int fLast) {
            JiraEditorLinkManager.OnDocumentChanged(TextLines);
        }

        public void OnChangeLineAttributes(int iFirstLine, int iLastLine) {
        }
    }
}
