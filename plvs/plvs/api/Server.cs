using System;
using System.Text;

namespace Atlassian.plvs.api {
    public abstract class Server {
        private Guid guid;
        private string name;
        private string url;
        private string userName;
        private string password;
        private bool enabled;
        private bool noProxy;

        public bool IsShared { get; set; }

        public static Guid JiraServerTypeGuid = new Guid("0C644383-BC4C-406d-B325-CA0AB1815B38");
        public static Guid BambooServerTypeGuid = new Guid("1C7A224E-52C4-4575-9212-7D731C13CFE9");

        protected Server(string name, string url, string userName, string password, bool noProxy)
            : this(Guid.NewGuid(), name, url, userName, password, noProxy, true) {}

        protected Server(Guid guid, string name, string url, string userName, string password, bool noProxy, bool enabled) {
            this.guid = guid;
            this.name = name;
            this.url = url;
            this.userName = userName;
            this.password = password;
            this.enabled = enabled;
            this.noProxy = noProxy;
        }

        protected Server(Server other) {
            if (other != null) {
                guid = other.guid;
                name = other.name;
                url = other.url;
                userName = other.userName;
                password = other.password;
                enabled = other.enabled;
                noProxy = other.noProxy;
            }
            else {
                guid = Guid.NewGuid();
                enabled = true;
            }
        }

        public abstract Guid Type { get; }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string Url {
            get { return url; }
            set { url = value; }
        }

        public string UserName {
            get { return userName; }
            set { userName = value; }
        }

        public string Password {
            get { return password; }
            set { password = value; }
        }

        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool NoProxy {
            get { return noProxy; }
            set { noProxy = value; }
        }

// ReSharper disable InconsistentNaming
        public Guid GUID
// ReSharper restore InconsistentNaming
        {
            get { return guid; }
            set { guid = value; }
        }

        public virtual string serverDetailsHtmlTable() {
            return serverdetailsHtmlTableStart() + serverBaseDetailsHtml() + serverDetailsHtmlTableEnd();
        }

        protected string serverBaseDetailsHtml() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr valign=top><td width=\"200\">Name</td><td>").Append(Name).Append("</td></tr>\r\n");
            sb.Append("<tr valign=top><td width=\"200\">Enabled</td><td>").Append(Enabled ? "Yes" : "No").Append("</td></tr>\r\n");
            sb.Append("<tr valign=top><td width=\"200\">Shared Between Solutions</td><td>").Append(IsShared ? "Yes" : "No").Append("</td></tr>\r\n");
            sb.Append("<tr valign=top><td width=\"200\">URL</td><td><a href=\"").Append(Url).Append("\">").Append(Url).Append("</a></td></tr>\r\n");
            sb.Append("<tr valign=top><td width=\"200\">User Name</td><td>").Append(UserName).Append("</td></tr>\r\n");
            sb.Append("<tr valign=top><td width=\"200\">Use Proxy</td><td>").Append(NoProxy ? "No" : "Yes").Append("</td></tr>\r\n");
            return sb.ToString();
        }

        protected string serverdetailsHtmlTableStart() {
            return "<table width=\"100%\" class=\"summary\">";
        }

        protected string serverDetailsHtmlTableEnd() {
            return "</table>";
        }
    }
}
