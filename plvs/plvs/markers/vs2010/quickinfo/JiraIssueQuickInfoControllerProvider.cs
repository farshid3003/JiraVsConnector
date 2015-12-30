using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.quickinfo {
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("JIRA Issue ToolTip QuickInfo Controller")]
    [ContentType("text")]
    internal class JiraIssueQuickInfoControllerProvider : IIntellisenseControllerProvider {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        [Import] 
        public JiraIssueQuickInfoSourceProvider InfoSourceProvider;

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers) {
            return new JiraIssueQuickInfoController(textView, subjectBuffers, this);
        }
    }
}
