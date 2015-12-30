using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.texttag {

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "JiraIssueUnderlineClassification")]
    [Name("JiraIssueUnderlineClassificationFormat")]
    [UserVisible(true)]
    [Order(After = Priority.Low)]
    internal sealed class UnderlineFormatDefinition : ClassificationFormatDefinition {
        public UnderlineFormatDefinition() {
            DisplayName = "JIRA Issue Underline";
            TextDecorations = System.Windows.TextDecorations.Underline;
            ForegroundColor = Colors.Blue;
        }
    }

    public class JiraIssueTextTag : ClassificationTag {

        public SnapshotSpan Where { get; set; }
        public string IssueKey { get; private set; }

        public JiraIssueTextTag(SnapshotSpan where, string issueKey)
            : base(JiraIssueTagProvider.JiraIssueTaggerProvider.UnderlineClassification) {
            Where = where;
            IssueKey = issueKey;
        }
    }
}


