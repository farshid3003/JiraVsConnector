using System;
using System.Collections.Generic;
using System.Text;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServer : Server {

        public bool UseFavourites { get; set; }

        public bool ShowBranches { get; set; }
        public bool ShowMyBranchesOnly { get; set; }

        public List<string> PlanKeys { get; set; }

        public BambooServer(string name, string url, string userName, string password, bool noProxy) 
            : base(name, url, userName, password, noProxy) {
        }
        
        public BambooServer(Guid guid, string name, string url, string userName, string password, bool noProxy, bool enabled) 
            : base(guid, name, url, userName, password, noProxy, enabled) {
        }

        public BambooServer(BambooServer other) : base(other) {
            if (other == null) return;
            UseFavourites = other.UseFavourites;
            ShowBranches = other.ShowBranches;
            ShowMyBranchesOnly = other.ShowMyBranchesOnly;

            if (other.PlanKeys != null) {
                PlanKeys = new List<string>(other.PlanKeys);
            }
        }

        public override Guid Type { get { return BambooServerTypeGuid; } }

        public override string serverDetailsHtmlTable() {
            var sb = new StringBuilder();

            sb.Append(serverdetailsHtmlTableStart());
            sb.Append(serverBaseDetailsHtml());

            sb.Append("<tr VALIGN=TOP><td width=\"200\">Show Plan Branches</td><td>").Append(ShowBranches ? "Yes" : "No").Append("</td></tr>\r\n");
            if (ShowBranches) {
                sb.Append("<tr VALIGN=TOP><td width=\"200\">Show My Branches Only</td><td>").Append(ShowMyBranchesOnly ? "Yes" : "No").Append("</td></tr>\r\n");
            }
            sb.Append("<tr VALIGN=TOP><td width=\"200\">Monitor Favourite Plans</td><td>").Append(UseFavourites ? "Yes" : "No").Append("</td></tr>\r\n");
            if (!UseFavourites) {
                if (PlanKeys != null && PlanKeys.Count > 0) {
                    sb.Append("<tr VALIGN=TOP><td width=\"200\">Monitored Plans</td><td>");
                    int i = 1;
                    foreach (var key in PlanKeys) {
                        sb.Append(key);
                        if (i < PlanKeys.Count) {
                            sb.Append(", ");
                        }
                        ++i;
                    }
                    sb.Append("</td></tr>");
                } else {
                    sb.Append("<tr VALIGN=TOP><td colspan=2>No plans monitored</td></tr>");
                }
            }

            sb.Append(serverDetailsHtmlTableEnd());
            return sb.ToString();
        }
    }
}