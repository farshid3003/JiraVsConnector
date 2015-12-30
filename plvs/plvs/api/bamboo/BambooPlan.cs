namespace Atlassian.plvs.api.bamboo {
    public class BambooPlan {
        public BambooPlan(string key, string name, bool enabled, bool favourite) {
            Key = key;
            Name = name;
            Enabled = enabled;
            Favourite = favourite;
        }

        public string Key { get; private set; }
        public string Name { get; private set; }
        public bool Favourite { get; private set; }
        public bool Enabled { get; private set; }

        public override string ToString() {
            return "[" + Key + "] " + Name;
        }
    }
}
