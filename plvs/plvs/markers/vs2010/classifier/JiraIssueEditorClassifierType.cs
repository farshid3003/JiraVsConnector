using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.classifier {
    internal static class JiraIssueEditorClassifierClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("JiraIssueEditorClassifier")]
        internal static ClassificationTypeDefinition EditorClassifierTestType;
    }
}


