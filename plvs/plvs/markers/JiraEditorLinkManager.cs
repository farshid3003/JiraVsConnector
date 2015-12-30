using System.Collections.Generic;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.markers {
    internal class JiraEditorLinkManager {

        public enum BufferType {
            CSHARP_OR_C_OR_CPP,
            VISUAL_BASIC
        }

        private static readonly List<IVsTextLines> cssBuffers = new List<IVsTextLines>();
        private static readonly List<IVsTextLines> vbBuffers = new List<IVsTextLines>();

        private static readonly Regex BlockInOneLine = new Regex(@"/\*(.*)\*/");
        private static readonly Regex BlockCommentStarted = new Regex(@"/\*(.*)");
        private static readonly Regex BlockCommentEnded = new Regex(@"(.*)\*/");

        private class CommentStrings {
            public CommentStrings(string line, string blockOpen, string blockClose) {
                Line = line;
                BlockOpen = blockOpen;
                BlockClose = blockClose;
            }

            public CommentStrings() {}

            public readonly string Line;
            public readonly string BlockOpen;
            public readonly string BlockClose;
        }

        public static void OnSolutionOpened() {}

        public static void OnSolutionClosed() {}

        public static void OnDocumentClosed(IVsTextLines lines) {
            lock(cssBuffers) {
                if (cssBuffers.Contains(lines)) cssBuffers.Remove(lines);
            }

            lock (vbBuffers) {
                if (vbBuffers.Contains(lines)) vbBuffers.Remove(lines);
            }
        }

        public static void OnDocumentOpened(IVsTextLines lines, BufferType type) {
            switch (type) {
                case BufferType.CSHARP_OR_C_OR_CPP:
                    lock(cssBuffers) {
                        if (!cssBuffers.Contains(lines)) cssBuffers.Add(lines);
                    }
                    break;
                case BufferType.VISUAL_BASIC:
                    lock (vbBuffers) {
                        if (!vbBuffers.Contains(lines)) vbBuffers.Add(lines);
                    }
                    break;
            }

            if (AtlassianPanel.Instance.Jira != null && AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault != null) {
                addMarkersToDocument(lines);
            }
        }

        public static void OnDocumentSaved(uint cookie) {}

        public static void OnMarkerInvalidated(IVsTextLineMarker marker) {}

        public static void OnDocumentChanged(IVsTextLines textLines) {
            if (!(isCSharpOrCppOrC(textLines) || isVb(textLines))) return;

            cleanupMarkers(textLines, JiraLinkTextMarkerType.Id);
            cleanupMarkers(textLines, JiraLinkMarginMarkerType.Id);

            if (AtlassianPanel.Instance.Jira != null && AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault != null) {
                addMarkersToDocument(textLines);
            }
        }

        private static void addMarkersToDocument(IVsTextLines textLines) {
            int lineCount;
            textLines.GetLineCount(out lineCount);

            CommentStrings commentMarkers = getCommentMarkerStrings(textLines);

            if (!GlobalSettings.shouldShowIssueLinks(lineCount)) return;

            bool isInBlockComment = false;
            for (int lineNumber = 0; lineNumber < lineCount; ++lineNumber) {
                string text;
                int lineLength;

                List<string> issueKeys = new List<string>();

                textLines.GetLengthOfLine(lineNumber, out lineLength);
                textLines.GetLineText(lineNumber, 0, lineNumber, lineLength, out text);

                if (text == null) continue;

                if (commentMarkers.BlockOpen != null && commentMarkers.BlockClose != null) {
                    int current = 0;
                    MatchCollection matches;
                    if (isInBlockComment) {
                        matches = BlockCommentEnded.Matches(text);
                        if (matches.Count > 0) {
                            scanCommentedLine(textLines, lineNumber, matches[0].Value, 0, ref issueKeys);
                            current = matches[0].Length;
                            isInBlockComment = false;
                        } else {
                            scanCommentedLine(textLines, lineNumber, text, 0, ref issueKeys);
                            maybeAddMarginMarker(textLines, lineNumber, lineLength, issueKeys);
                            continue;
                        }
                    } else {
                        if (scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueKeys)) {
                            maybeAddMarginMarker(textLines, lineNumber, lineLength, issueKeys);
                            continue;
                        }
                    }

                    matches = BlockInOneLine.Matches(text, current);
                    for (int i = 0; i < matches.Count; ++i) {
                        scanCommentedLine(textLines, lineNumber, matches[i].Value, matches[i].Index, ref issueKeys);
                        current = matches[i].Index + matches[i].Length;
                    }

                    if (scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueKeys)) {
                        maybeAddMarginMarker(textLines, lineNumber, lineLength, issueKeys);
                        continue;
                    }

                    matches = BlockCommentStarted.Matches(text, current);
                    if (matches.Count > 0) {
                        isInBlockComment = true;
                        scanCommentedLine(textLines, lineNumber, matches[0].Value, matches[0].Index, ref issueKeys);
                    }
                } else {
                    if (commentMarkers.Line == null) {
                        maybeAddMarginMarker(textLines, lineNumber, lineLength, issueKeys);
                        continue;
                    }

                    scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueKeys);
                }
                maybeAddMarginMarker(textLines, lineNumber, lineLength, issueKeys);
            }
        }

        private static void maybeAddMarginMarker(IVsTextLines textLines, int lineNumber, int lineLength, List<string> issueKeys) {
            if (issueKeys.Count == 0) return;
            AbstractMarkerClientEventSink marginMarkerClientEventSink = new MarginMarkerClientEventSink(issueKeys);
            addMarker(textLines, lineNumber, 0, lineLength, JiraLinkMarginMarkerType.Id, marginMarkerClientEventSink);
        }

        private static bool scanForLineComment(IVsTextLines textLines, int lineNumber, string text, CommentStrings commentMarkers, ref List<string> issueKeys) {
            int lineCmtIdx = text.IndexOf(commentMarkers.Line);
            return lineCmtIdx != -1 && scanCommentedLine(textLines, lineNumber, text.Substring(lineCmtIdx), lineCmtIdx, ref issueKeys);
        }

        private static bool scanCommentedLine(IVsTextLines textLines, int lineNumber, string text, int offset, ref List<string> issueKeys) {
            MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(text);
            
            SortedDictionary<string, JiraProject> projects = 
                JiraServerCache.Instance.getProjects(AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault);

            int countOrg = issueKeys.Count;

            for (int j = 0; j < matches.Count; ++j) {
                if (!projects.ContainsKey(matches[j].Groups[2].Value)) continue;

                issueKeys.Add(matches[j].Value);

                int index = matches[j].Index + offset;
                AbstractMarkerClientEventSink textMarkerClientEventSink = new TextMarkerClientEventSink(matches[j].Value);
                addMarker(textLines, lineNumber, index, index + matches[j].Length, JiraLinkTextMarkerType.Id, textMarkerClientEventSink);
            }

            return issueKeys.Count - countOrg > 0;
        }

        private static CommentStrings getCommentMarkerStrings(IVsTextLines lines) {
            if (isCSharpOrCppOrC(lines)) return new CommentStrings("//", "/*", "*/");
            if (isVb(lines)) return new CommentStrings("'", null, null);
            return new CommentStrings();
        }


        private static bool isCSharpOrCppOrC(IVsTextLines textLines) {
            lock (cssBuffers) {
                return cssBuffers.Contains(textLines);
            }
        }

        private static bool isVb(IVsTextLines textLines) {
            lock (vbBuffers) {
                return vbBuffers.Contains(textLines);
            }
        }

        private static void addMarker(IVsTextLines textLines, int line, int start, int end, int markerType,
                                      AbstractMarkerClientEventSink client) {
            IVsTextLineMarker[] markers = new IVsTextLineMarker[1];
            int hr = textLines.CreateLineMarker(markerType, line, start, line, end, client, markers);
            if (!ErrorHandler.Succeeded(hr)) return;
            client.Marker = markers[0];
        }

        private static void cleanupMarkers(IVsTextLines textLines, int markerType) {
            IVsEnumLineMarkers markers;
            textLines.EnumMarkers(0, 0, 0, 0, markerType, (uint) ENUMMARKERFLAGS.EM_ENTIREBUFFER, out markers);

            int count;
            markers.GetCount(out count);

            for (int i = 0; i < count; ++i) {
                IVsTextLineMarker marker;
                markers.Next(out marker);
                if (marker != null) {
                    marker.Invalidate();
                }
            }
        }
    }
}