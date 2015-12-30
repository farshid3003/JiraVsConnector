using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira.gh {
    public class RapidBoard {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<Sprint> Sprints { get; private set; }
        public RapidBoard(JToken view) {
            Id = view["id"].Value<int>();
            Name = view["name"].Value<string>();
            Sprints = new List<Sprint>();
        }
    }
}
