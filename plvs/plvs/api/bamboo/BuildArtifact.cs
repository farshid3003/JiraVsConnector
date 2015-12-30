namespace Atlassian.plvs.api.bamboo {
    public class BuildArtifact {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ResultKey { get; private set; }

        public BuildArtifact(string resultKey, string name, string url) {
            ResultKey = resultKey;
            Name = name;
            Url = url;
        }
    }
}
