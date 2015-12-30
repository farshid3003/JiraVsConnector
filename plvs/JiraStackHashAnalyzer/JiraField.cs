using System.Collections.Generic;

namespace JiraStackHashAnalyzer {
    public class JiraField {
        public List<string> Values { get; set; }

        public JiraField(string id, string name) {
            Id = id;
            Name = name;
            Values = new List<string>();
        }

        public JiraField(JiraField other) {
            Id = other.Id;
            Name = other.Name;
            Values = new List<string>(other.Values);
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
    }
}