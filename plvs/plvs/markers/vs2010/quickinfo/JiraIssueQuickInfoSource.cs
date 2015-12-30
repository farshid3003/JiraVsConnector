using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Atlassian.plvs.markers.vs2010.quickinfo {
    internal class JiraIssueQuickInfoSource : IQuickInfoSource {
        private readonly JiraIssueQuickInfoSourceProvider provider;
        private readonly ITextBuffer subjectBuffer;

        public JiraIssueQuickInfoSource(JiraIssueQuickInfoSourceProvider provider, ITextBuffer subjectBuffer) {
            this.provider = provider;
            this.subjectBuffer = subjectBuffer;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan) {
            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(subjectBuffer.CurrentSnapshot);
            if (!subjectTriggerPoint.HasValue) {
                applicableToSpan = null;
                return;
            }
            
            string issueKey = provider.CurrentIssueKey;

            if (issueKey != null) {
                ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
                SnapshotSpan querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);

                applicableToSpan = currentSnapshot.CreateTrackingSpan(querySpan.Start.Add(0).Position, issueKey.Length, SpanTrackingMode.EdgeInclusive);
                qiContent.Add(createIssueTextFromKey(issueKey));
            } else {
                applicableToSpan = null;
            }
        }

        private static string createIssueTextFromKey(string issueKey) {
            JiraServer server = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            if (server != null) {
                JiraIssue issue = JiraIssueListModelImpl.Instance.getIssue(issueKey, server);
                if (issue != null) {
                    return "Issue Key: " + issueKey
                           + "\r\n\r\nSummary: " + issue.Summary
                           + "\r\nType: " + issue.IssueType
                           + "\r\nStatus: " + issue.Status
                           + "\r\nPriority: " + issue.Priority
                           + "\r\nReporter: " + JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Reporter)
                           + "\r\nAssignee: " + JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Assignee)
                           + "\r\nLast Updated: " + issue.UpdateDate 
                           + getCrtlClickText();
                }
                return "Issue Key: " + issueKey + getCrtlClickText();
            }
            return "Issue Key: " + issueKey + "\r\nNo JIRA server selected";
        }

        private static string getCrtlClickText() {
            return "\r\n\r\nCTRL + click to open issue";
        }

        private bool disposed;

        public void Dispose() {
            if (disposed) return;
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
