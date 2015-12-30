using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010 {
    internal abstract class LineTagger<T> : ITagger<T> where T : ITag {
        private readonly ITextBuffer buffer;
        private bool disposed;

        private readonly TagCache tagCache;

        internal LineTagger(ITextBuffer buffer, IClassifier classifier, string cachePropertyName) {
            this.buffer = buffer;
            tagCache = new TagCache(buffer, classifier, cachePropertyName);

            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
            GlobalSettings.SettingsChanged += globalSettingsChanged;

            buffer.Changed += buffer_Changed;
        }

        private void buffer_Changed(object sender, TextContentChangedEventArgs e) {
//             DebugMon.Instance().addText(GetType().Name + " buffer_Changed(): " + buffer + " changed, clearing tag cache");
            tagCache.clearAndRunActionAfterTimeout(updateTags);
        }

        private void globalSettingsChanged(object sender, EventArgs e) {
            tagCache.clearAndRunActionAfterTimeout(updateTags);
        }

        private void jiraSelectedServerChanged(object sender, EventArgs e) {
            tagCache.clearAndRunActionAfterTimeout(updateTags);
        }

        private void updateTags() {
            var snapshot = buffer.CurrentSnapshot;

            EventHandler<SnapshotSpanEventArgs> handler = TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }
        }

        IEnumerable<ITagSpan<T>> ITagger<T>.GetTags(NormalizedSnapshotSpanCollection spans) {
            if (GlobalSettings.AllIssueLinksDisabled) {
                return new List<ITagSpan<T>>();
            }

            if (!tagCache.CacheFilled) {
                tagCache.fill();
            }

            return getTagsFromCacheFor(spans);
        }

        private IEnumerable<ITagSpan<T>> getTagsFromCacheFor(IEnumerable<SnapshotSpan> spans) {
            List<ITagSpan<T>> result = new List<ITagSpan<T>>();

            if (!GlobalSettings.shouldShowIssueLinks(buffer.CurrentSnapshot.LineCount)) {
                return result;
            }

            JiraServer selectedServer = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            if (selectedServer == null) {
                return result;
            }

            int lastLine = -1;
            foreach (SnapshotSpan span in spans) {
                foreach (TagCache.TagEntry tagEntry in tagCache.Entries) {
                    if (tagEntry.Start >= span.Start && tagEntry.End <= span.End) {
                        TagSpan<T> tag = getTagForKey(new SnapshotSpan(tagEntry.Start, tagEntry.End), tagEntry.IssueKey, lastLine);
                        lastLine = tagEntry.Start.GetContainingLine().LineNumber;
                        if (tag != null) {
                            result.Add(tag);
                        }
                    }
                }    
            }
            return result;
        }

        protected abstract TagSpan<T> getTagForKey(SnapshotSpan span, string issueKey, int lastLine);

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                AtlassianPanel.Instance.Jira.SelectedServerChanged -= jiraSelectedServerChanged;
                GlobalSettings.SettingsChanged -= globalSettingsChanged;
                buffer.Changed -= buffer_Changed;
            }

            disposed = true;
        }
    }
}
