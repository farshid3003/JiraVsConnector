using System;

namespace Atlassian.plvs.api.jira {
    public class FiveOhThreeJiraException : Exception {
        public JiraServer Server { get; private set; }

        public FiveOhThreeJiraException(JiraServer server) {
            Server = server;
        }
    }
}
