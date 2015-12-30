using System.Windows;
using System.Windows.Controls;
using Atlassian.plvs.util;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    class JiraIssueGlyphFactory : IGlyphFactory {
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag) {
            if (tag == null || !(tag is JiraIssueLineGlyphTag)) {
                return null;
            }

            Image image = new Image { Source = PlvsUtils.bitmapSourceFromPngImage(Resources.tab_jira) };
            return image;
        }
    }
}


