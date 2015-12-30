using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace Atlassian.plvs.markers.vs2010 {
    internal class TagCache {
        private readonly ITextBuffer buffer;
        private readonly IClassifier classifier;
        private readonly string propertyName;

        public bool CacheFilled { get; private set; }

        private readonly List<TagEntry> cache = new List<TagEntry>();

        public IEnumerable<TagEntry> Entries { get { return cache; } }

        public TagCache(ITextBuffer buffer, IClassifier classifier, string propertyName) {
            this.buffer = buffer;
            this.classifier = classifier;
            this.propertyName = propertyName;
        }

        public class TagEntry {
            internal TagEntry(SnapshotPoint start, SnapshotPoint end, string issueKey) {
                Start = start;
                End = end;
                IssueKey = issueKey;
            }

            public SnapshotPoint Start { get; private set; }
            public SnapshotPoint End { get; private set; }
            public string IssueKey { get; private set; }

            public override string ToString() {
                return "Tag: [" + Start + ", " + End + "]: " + IssueKey;
            }
        }

        private void clear() {
            cache.Clear();
            CacheFilled = false;
            if (buffer.Properties.ContainsProperty(propertyName)) {
                buffer.Properties.RemoveProperty(propertyName);
            }
        }

        public void fill() {
            CacheFilled = true;

            JiraServer selectedServer = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            if (selectedServer == null) return;


            foreach (ITextSnapshotLine line in buffer.CurrentSnapshot.Lines) {
                SnapshotSpan span = new SnapshotSpan(line.Start, line.End);

                foreach (ClassificationSpan classification in classifier.GetClassificationSpans(span)) {
                    if (!classification.ClassificationType.Classification.ToLower().Contains("comment")) continue;

                    MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(classification.Span.GetText());

                    foreach (Match match in matches.Cast<Match>().Where(match => match.Success)) {
                        SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(selectedServer);
                        if (!projects.ContainsKey(match.Groups[2].Value)) continue;

                        SnapshotSpan snapshotSpan = new SnapshotSpan(classification.Span.Start + match.Index, match.Length);

                        TagEntry tagEntry = new TagEntry(snapshotSpan.Start, snapshotSpan.End, match.Groups[0].Value);
//                        DebugMon.Instance().addText("adding tag entry: " + tagEntry);
                        cache.Add(tagEntry);
                    }
                }
            }
            if (buffer.Properties.ContainsProperty(propertyName)) {
                buffer.Properties.RemoveProperty(propertyName);
            }
            buffer.Properties.AddProperty(propertyName, Entries);
        }

        private Timer clearCacheTimer;

        public void clearAndRunActionAfterTimeout(Action action) {
            if (clearCacheTimer != null) {
                clearCacheTimer.Stop();
                clearCacheTimer.Dispose();
            }
            clearCacheTimer = new Timer { Interval = 1000 };
            clearCacheTimer.Tick += (s, e) => {
                clearCacheTimer.Stop();
                clearCacheTimer.Dispose();
                clearCacheTimer = null;
                clear();
                action();
            };
            clearCacheTimer.Start();
        }
    }
}
