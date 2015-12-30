using System;

namespace JiraStackHashAnalyzer {
    public class JiraServer : Server {
        public JiraServer(string name, string url, string userName, string password, bool noProxy) 
            : base(name, url, userName, password, noProxy) {}
        public JiraServer(Guid guid, string name, string url, string userName, string password, bool noProxy, bool enabled) 
            : base(guid, name, url, userName, password, noProxy, enabled) {}
        public JiraServer(Server other) : base(other) {}
        
        public override Guid Type { get { return JiraServerTypeGuid; } }
    }
}