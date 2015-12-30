using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.markers {
    public sealed class JiraLinkTextMarkerType : IVsPackageDefinedTextMarkerType, IVsMergeableUIItem
    {
        private const string JIRA_LINK_TEXT = "JIRA Link (Text)";

        public static int Id { get; internal set; }

        public int GetVisualStyle(out uint pdwVisualFlags) {
            pdwVisualFlags = (uint) MARKERVISUAL.MV_LINE | (uint) MARKERVISUAL.MV_TIP_FOR_BODY | (uint) MARKERVISUAL.MV_COLOR_ALWAYS;
            return VSConstants.S_OK;
        }

        public int GetDefaultColors(COLORINDEX[] piForeground, COLORINDEX[] piBackground) {
            piForeground[0] = COLORINDEX.CI_DARKBLUE;
            piBackground[0] = COLORINDEX.CI_WHITE;
            return VSConstants.S_OK;
        }

        public int GetDefaultLineStyle(COLORINDEX[] piLineColor, LINESTYLE[] piLineIndex) {
            piLineColor[0] = COLORINDEX.CI_DARKBLUE;
            piLineIndex[0] = LINESTYLE.LI_SOLID;

            return VSConstants.S_OK;
        }

        public int GetDefaultFontFlags(out uint pdwFontFlags) {
            pdwFontFlags = (uint) FONTFLAGS.FF_DEFAULT;

            return VSConstants.S_OK;
        }

        public int GetBehaviorFlags(out uint pdwFlags) {
            pdwFlags = (uint) MARKERBEHAVIORFLAGS.MB_TRACK_EDIT_ON_RELOAD;

            return VSConstants.S_OK;
        }

        public int GetPriorityIndex(out int piPriorityIndex) {
            piPriorityIndex = (int) MARKERTYPE.MARKER_READONLY;
            return VSConstants.S_OK;
        }

        public int DrawGlyphWithColors(IntPtr hdc, RECT[] pRect, int iMarkerType,
                                       IVsTextMarkerColorSet pMarkerColors, uint dwGlyphDrawFlags, int iLineHeight) {
            return VSConstants.S_OK;
        }

        public int GetCanonicalName(out string pbstrNonLocalizeName) {
            pbstrNonLocalizeName = JIRA_LINK_TEXT;

            return VSConstants.S_OK;
        }

        public int GetDisplayName(out string pbstrDisplayName) {
            // This string is displayed in the "Fonts and Colors" section
            // of the Visual Studio Options dialog.
            pbstrDisplayName = JIRA_LINK_TEXT;

            return VSConstants.S_OK;
        }

        public int GetMergingPriority(out int piMergingPriority) {
            piMergingPriority = (int) MARKERTYPE.MARKER_READONLY;

            return VSConstants.S_OK;
        }

        public int GetDescription(out string pbstrDesc) {
            pbstrDesc = JIRA_LINK_TEXT;

            return VSConstants.S_OK;
        }
    }
}