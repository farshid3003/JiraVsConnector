namespace Atlassian.plvs.api.jira {
    public interface JiraFilter {
        string getOldstyleFilterQueryString();
        string getJql();
        string getSortBy();
    }
}
