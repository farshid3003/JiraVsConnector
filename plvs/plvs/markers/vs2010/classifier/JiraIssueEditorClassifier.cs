using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.classifier {

    [Export(typeof(IClassifierProvider))]
    [ContentType("code")]
    internal class JiraIssueEditorClassifierProvider : IClassifierProvider {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry; 

        public IClassifier GetClassifier(ITextBuffer buffer) {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new JiraIssueEditorClassifier(ClassificationRegistry));
        }
    }

    class JiraIssueEditorClassifier : IClassifier {
        private readonly IClassificationType classificationType;

        internal JiraIssueEditorClassifier(IClassificationTypeRegistryService registry) {
            classificationType = registry.GetClassificationType("JiraIssueEditorClassifier");
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            List<ClassificationSpan> classifications = new List<ClassificationSpan>
                                                       {
                                                           new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), classificationType)
                                                       };
            return classifications;
        }

#pragma warning disable 67
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
}


