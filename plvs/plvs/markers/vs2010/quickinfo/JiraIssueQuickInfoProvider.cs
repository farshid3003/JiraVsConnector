using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.quickinfo {
    [Export(typeof(IQuickInfoSourceProvider))]
    [Export(typeof(JiraIssueQuickInfoSourceProvider))]
    [Name("JIRA Issue ToolTip QuickInfo Source")]
    [Order(Before="default")]
    [ContentType("text")]
    internal class JiraIssueQuickInfoSourceProvider : IQuickInfoSourceProvider {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        [Import]
        public IViewTagAggregatorFactoryService tagAggregatorFactoryService;

        public string CurrentIssueKey { get; set; }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            return new JiraIssueQuickInfoSource(this, textBuffer);
        }
    }
}
