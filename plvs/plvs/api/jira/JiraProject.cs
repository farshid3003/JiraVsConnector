using System;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraProject : JiraNamedEntity {
        public JiraProject(int id, string key, string name) :
            base(id, name, null) {
            Key = key;
        }

        public JiraProject(JToken project) : base(project["id"].Value<int>(), project["name"].Value<string>(), null) {
            Key = project["key"].Value<string>();
        }

        public string Key { get; private set; }

        public override string ToString() {
            return "[" + Key + "] " + Name;
        }
    }
}