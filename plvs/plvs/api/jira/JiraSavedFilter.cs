using Atlassian.plvs.util;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraSavedFilter : JiraNamedEntity {
        public string Jql { get; private set; }
        public string ViewUrl { get; private set; }
        public string SearchUrl { get; private set; }

        public JiraSavedFilter(int id, string name)
            : base(id, name, null) {}

        public JiraSavedFilter(JToken filter) : this(filter["id"].Value<int>(), filter["name"].Value<string>()) {
            var json = filter["jql"];
            Jql = json != null ? json.Value<string>().unescape() : "";
            json = filter["viewUrl"];
            ViewUrl = json != null ? json.Value<string>() : "";
            json = filter["searchUrl"];
            SearchUrl = json != null ? json.Value<string>() : "";
        }
    }
}