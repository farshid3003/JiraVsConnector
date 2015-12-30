using System;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraNamedEntity {
        public JiraNamedEntity(int id, string name, string iconUrl) {
            Id = id;
            Name = name;
            IconUrl = iconUrl;
        }

        public JiraNamedEntity(JToken entity) {
            Id = entity["id"] != null ? entity["id"].Value<int>() : 0;
            Name = entity["name"] != null ? entity["name"].Value<string>() : null;
            IconUrl = entity["iconUrl"] != null ? entity["iconUrl"].Value<string>() : null;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IconUrl { get; private set; }

        public override string ToString() {
            return Name;
        }
    }
}