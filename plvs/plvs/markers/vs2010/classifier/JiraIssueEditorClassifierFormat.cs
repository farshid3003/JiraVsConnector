using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.classifier {
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "JiraIssueEditorClassifier")]
    [Name("JiraIssueEditorClassifier")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class JiraIssueEditorClassifierFormat : ClassificationFormatDefinition {
        public JiraIssueEditorClassifierFormat() {
            DisplayName = "Jira Issue Editor Classifier";
        }
    }
}


