namespace JiraStackHashAnalyzer {
    public interface JiraFilter {
        string getFilterQueryString();
        string getSortBy();
    }
}
