using System;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Atlassian.plvs.markers {
    [Guid(GuidList.GUID_JIRA_LINK_MARKER_SERVICE_STRING)]
    public sealed class JiraLinkMarkerTypeProvider : IVsTextMarkerTypeProvider {
        private readonly JiraLinkMarginMarkerType marginMarkerType = new JiraLinkMarginMarkerType();
        private readonly JiraLinkTextMarkerType textMarkerType = new JiraLinkTextMarkerType();

        public int GetTextMarkerType(ref Guid pguidMarker, out IVsPackageDefinedTextMarkerType ppMarkerType) {
            // This method is called by Visual Studio when it needs the marker
            // type information in order to retrieve the implementing objects.
            if (pguidMarker == GuidList.JiraLinkMarginMarker) {
                ppMarkerType = marginMarkerType;
                return VSConstants.S_OK;
            }

            if (pguidMarker == GuidList.JiraLinkTextMarker) {
                ppMarkerType = textMarkerType;
                return VSConstants.S_OK;
            }

            ppMarkerType = null;
            return VSConstants.E_FAIL;
        }

        internal static void InitializeMarkerIds(PlvsPackage package) {
#if !VS2010
            // Retrieve the Text Marker IDs. We need them to be able to create instances.
            IVsTextManager textManager = (IVsTextManager) package.GetService(typeof (SVsTextManager));

            try {
                int markerId;
                Guid markerGuid = GuidList.JiraLinkMarginMarker;
                textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId);
                JiraLinkMarginMarkerType.Id = markerId;

                markerGuid = GuidList.JiraLinkTextMarker;
                textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId);
                JiraLinkTextMarkerType.Id = markerId;
            } catch (COMException e) {
                Debug.WriteLine("JiraLinkMarkerTypeProvider.InitializeMarkerids() - COMException: " + e.Message);
            }
#endif
        }
    }
}