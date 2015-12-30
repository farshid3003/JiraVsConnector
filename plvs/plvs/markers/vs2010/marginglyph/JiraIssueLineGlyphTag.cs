using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    class JiraIssueLineGlyphTag : IGlyphTag {
        public SnapshotSpan Where { get; set; }
        public List<string> IssueKeys { get; private set; }

        public JiraIssueLineGlyphTag(SnapshotSpan where, List<string> issueKeys) {
            Where = where;
            IssueKeys = issueKeys;
        }
    }
}
