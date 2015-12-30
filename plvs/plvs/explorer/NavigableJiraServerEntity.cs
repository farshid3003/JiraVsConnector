using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer {
    interface NavigableJiraServerEntity {
        string getUrl(string authString);
        void onClick(StatusLabel status);
    }
}
