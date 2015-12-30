using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira.gh {
    public class Sprint {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public bool Closed { get; private set; }
        public int BoardId { get; private set; }
        
        public Sprint(int boardId, JToken sprint, bool newerThan6301) {
            BoardId = boardId;
            Id = sprint["id"].Value<int>();
            Name = sprint["name"].Value<string>();
            Closed = newerThan6301 
                ? sprint["state"].Value<string>().Equals("CLOSED")
                : sprint["closed"].Value<bool>();
        }
    }
}
