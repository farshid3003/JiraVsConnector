using System.ComponentModel.Composition;
using Atlassian.plvs.markers.vs2010.texttag;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("JiraIssueGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(JiraIssueLineGlyphTag))]
    internal sealed class JiraIssueGlyphFactoryProvider : IGlyphFactoryProvider {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin) {
            return new JiraIssueGlyphFactory();
        }
    }
}


