using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.attributes;
using Atlassian.plvs.store;
using Atlassian.plvs.util;

namespace Atlassian.plvs.models.jira {
    public class JiraCustomFilter : JiraFilter {
        private JiraServer server;
        public readonly Guid Guid;

        private const string ISSUE_NAVIGATOR = "/secure/IssueNavigator.jspa?refreshFilter=false&reset=update&show=View+%3E%3E";

        private const string BROWSER_QUERY_SUFFIX = "&pager/start=-1&tempMax=100";

        private const string ISSUE_NAVIGATOR_JQL = "/secure/IssueNavigator.jspa?reset=true&jqlQuery=";

        private const string FILTER_COUNT = "_jiraCustomFilterCount";
        private const string FILTER_GUID = "_jiraCustormFilterGuid_";
        private const string FILTER_SERVER_GUID = "_jiraCustomFilterServerGuid_";

        private const string FILTER_PROJECT_COUNT = "_jiraCustomFilterProjectCount_";
        private const string FILTER_PROJECT_ID = "_jiraCustomFilterProjectId_";
        private const string FILTER_PROJECT_KEY = "_jiraCustomFilterProjectKey_";

        private const string FILTER_ISSUE_TYPE_COUNT = "_jiraCustomFilterIssueTypeCount_";
        private const string FILTER_ISSUE_TYPE_ID = "_jiraCustomFilterIssueTypeId_";
        private const string FILTER_ISSUE_TYPE_NAME = "_jiraCustomFilterIssueTypeName_";

        private const string FILTER_FIXFORVERSIONS_COUNT = "_jiraCustomFilterFixForVersionsCount_";
        private const string FILTER_FIXFORVERSIONS_ID = "_jiraCustomFilterFixForVersionsId_";
        private const string FILTER_FIXFORVERSIONS_NAME = "_jiraCustomFilterFixForVersionsName_";

        private const string FILTER_AFFECTVERSIONS_COUNT = "_jiraCustomFilterAffectsVersionsCount_";
        private const string FILTER_AFFECTVERSIONS_ID = "_jiraCustomFilterAffectsVersionsId_";
        private const string FILTER_AFFECTVERSIONS_NAME = "_jiraCustomFilterAffectsVersionsName_";

        private const string FILTER_COMPONENTS_COUNT = "_jiraCustomFilterComponentsCount_";
        private const string FILTER_COMPONENTS_ID = "_jiraCustomFilterComponentsId_";
        private const string FILTER_COMPONENTS_NAME = "_jiraCustomFilterComponentsName_";

        private const string FILTER_STATUSES_COUNT = "_jiraCustomFilterStatusesCount_";
        private const string FILTER_STATUSES_ID = "_jiraCustomFilterStatusesId_";
        private const string FILTER_STATUSES_NAME = "_jiraCustomFilterStatusesName_";

        private const string FILTER_RESOLUTIONS_COUNT = "_jiraCustomFilterResolutionsCount_";
        private const string FILTER_RESOLUTIONS_ID = "_jiraCustomFilterResolutionsId_";
        private const string FILTER_RESOLUTIONS_NAME = "_jiraCustomFilterResolutionsName_";

        private const string FILTER_PRIORITIES_COUNT = "_jiraCustomFilterPrioritiesCount_";
        private const string FILTER_PRIORITIES_ID = "_jiraCustomFilterPrioritiesId_";
        private const string FILTER_PRIORITIES_NAME = "_jiraCustomFilterPrioritiesName_";
        
        private const string FILTER_REPORTER = "_jiraCustomFilterReporter_";

        private const string FILTER_ASSIGNEE = "_jiraCustomFilterAssignee_";

        private const string FILTER_NAME = "_jiraCustomFilterName_";

        private const string CUSTOM_FILTER_DEFAULT_NAME = "Local Filter";

        public enum UserType {
            [StringValue("")]
            UNDEFINED = 0,
            [StringValue("Any User")]
            ANY = 1,
            [StringValue("Current User")]
            CURRENT = 2,
            [StringValue("Unassigned")]
            UNASSIGNED
        }

        public List<JiraProject> Projects { get; private set; }
        public List<JiraNamedEntity> IssueTypes { get; private set; }
        public List<JiraNamedEntity> FixForVersions { get; private set; }
        public List<JiraNamedEntity> AffectsVersions { get; private set; }
        public List<JiraNamedEntity> Components { get; private set; }
        public List<JiraNamedEntity> Statuses { get; private set; }
        public List<JiraNamedEntity> Resolutions { get; private set; }
        public List<JiraNamedEntity> Priorities { get; private set; }
        public UserType Reporter;
        public UserType Assignee;
        public string Name { get; set; }

        private static readonly Dictionary<Guid, JiraCustomFilter> FILTERS = new Dictionary<Guid, JiraCustomFilter>();

        public bool Empty {
            get {
                return 0 == Projects.Count + IssueTypes.Count + FixForVersions.Count + AffectsVersions.Count
                            + Components.Count + Resolutions.Count + Statuses.Count + Priorities.Count
                       && Reporter == UserType.UNDEFINED && Assignee == UserType.UNDEFINED;
            }
        }

        public JiraCustomFilter(JiraServer server, Guid guid) {
            init(server);
            Guid = guid;
        }

        public JiraCustomFilter(JiraServer server) {
            init(server);
            Guid = Guid.NewGuid();
        }

        private void init(JiraServer srv) {
            server = srv;

            Projects = new List<JiraProject>();
            IssueTypes = new List<JiraNamedEntity>();
            FixForVersions = new List<JiraNamedEntity>();
            AffectsVersions = new List<JiraNamedEntity>();
            Components = new List<JiraNamedEntity>();
            Statuses = new List<JiraNamedEntity>();
            Priorities = new List<JiraNamedEntity>();
            Resolutions = new List<JiraNamedEntity>();
            Reporter = UserType.UNDEFINED;
            Assignee = UserType.UNDEFINED;
            Name = CUSTOM_FILTER_DEFAULT_NAME;
        }

        public static List<JiraCustomFilter> getAll(JiraServer server) {
            return FILTERS.Values.Where(filter => filter.server.GUID.Equals(server.GUID)).ToList();
        }

        public static void add(JiraCustomFilter filter) {
            if (FILTERS.ContainsKey(filter.Guid)) {
                throw new Exception("Filter Exists");
            }
            FILTERS[filter.Guid] = filter;
            save();
        }

        public static void remove(JiraCustomFilter filter) {
            if (!FILTERS.ContainsKey(filter.Guid)) return;
            FILTERS.Remove(filter.Guid);
            save();
        }

        public static void clear() {
            FILTERS.Clear();
        }

        public string getOldstyleBrowserQueryString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(ISSUE_NAVIGATOR).Append("&");

            sb.Append(getOldStyleQueryParameters());

            sb.Append(BROWSER_QUERY_SUFFIX);

            return sb.ToString();
        }

        public string getBrowserJqlQueryString() {
            return ISSUE_NAVIGATOR_JQL + getJql();
        }

        public string getOldstyleFilterQueryString() {
            var sb = new StringBuilder();

            sb.Append(getOldStyleQueryParameters());

            return sb.ToString();
        }

        public string getJql() {
            var sb = new StringBuilder();
            var cnt = joinEnumerables(sb, Projects, "project", p => p.Key);
            cnt += joinEnumerables(sb, IssueTypes, "type", t => t.Name);
            cnt += joinEnumerables(sb, AffectsVersions, "affectedVersion", v => v.Name);
            cnt += joinEnumerables(sb, FixForVersions, "fixVersion", f => f.Name);
            cnt += joinEnumerables(sb, Components, "component", c => c.Name);
            cnt += joinEnumerables(sb, Resolutions, "resolution", r => r.Id != -1 ? r.Name : "Unresolved");
            cnt += joinEnumerables(sb, Statuses, "status", s => s.Name);
            cnt += joinEnumerables(sb, Priorities, "priority", p => p.Name);
            if (Reporter == UserType.CURRENT)
                sb.Append(cnt++ == 0 ? "" : " and ").Append("reporter = ").Append(server.UserName);
            switch (Assignee) {
                case UserType.CURRENT:
                    sb.Append(cnt == 0 ? "" : " and ").Append("assignee = currentUser()");
                    break;
                case UserType.UNASSIGNED:
                    sb.Append(cnt == 0 ? "" : " and ").Append("assignee is EMPTY");
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        private delegate string GetVal<T>(T ent);
        private static int joinEnumerables<T>(StringBuilder sb, IEnumerable<T> ents, string name, GetVal<T> gv) {
            var cnt = 0;
            joinJqlGroups(sb, ents, () => { foreach (var ent in ents) sb.Append(cnt++ == 0 ? "" : " or ").Append(name + " = \"").Append(gv(ent)).Append('"'); });
            return cnt;
        }

//        private static int joinEnumerables<T>(StringBuilder sb, IEnumerable<T> ents, string name) {
//            var cnt = 0;
//            joinJqlGroups(sb, ents, () => { foreach (var ent in ents) sb.Append(cnt++ == 0 ? "" : " or ").Append(name + " = \"").Append(ent.Name).Append('"'); });
//            return cnt;
//        }

        private static void joinJqlGroups<T>(StringBuilder sb, IEnumerable<T> what, Action a) {
            if (what.Count() <= 0) return;

            if (sb.Length > 0) sb.Append(" and ");
            sb.Append("(");
            a();
            sb.Append(")");
        }

        public string getSortBy() {
            return "priority";
        }

        private string getOldStyleQueryParameters() {
            var sb = new StringBuilder();
            var first = 0;
            foreach (var project in Projects) sb.Append(first++ == 0 ? "" : "&").Append("pid=").Append(project.Id);
            foreach (var issueType in IssueTypes) sb.Append(first++ == 0 ? "" : "&").Append("type=").Append(issueType.Id);
            foreach (var version in AffectsVersions) sb.Append(first++ == 0 ? "" : "&").Append("version=").Append(version.Id);
            foreach (var version in FixForVersions) sb.Append(first++ == 0 ? "" : "&").Append("fixfor=").Append(version.Id);
            foreach (var comp in Components) sb.Append(first++ == 0 ? "" : "&").Append("component=").Append(comp.Id);
            foreach (var resolution in Resolutions) sb.Append(first++ == 0 ? "" : "&").Append("resolution=").Append(resolution.Id);
            foreach (var status in Statuses) sb.Append(first++ == 0 ? "" : "&").Append("status=").Append(status.Id);
            foreach (var priority in Priorities) sb.Append(first++ == 0 ? "" : "&").Append("priority=").Append(priority.Id);
            if (Reporter == UserType.CURRENT) sb.Append(first++ == 0 ? "" : "&").Append("reporter=").Append(server.UserName);
            switch (Assignee) {
                case UserType.CURRENT:
                    sb.Append(first == 0 ? "" : "&").Append("assigneeSelect=issue_current_user");
                    break;
                case UserType.UNASSIGNED:
                    sb.Append(first == 0 ? "" : "&").Append("assigneSelect=unassigned");
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string ToString() {
            var sb = new StringBuilder();

            if (Empty)
                return "Filter not defined\n\nRight-click to define the filter";

            sb.Append("Server URL: ").Append(server.Url);

            if (Projects.Count > 0) {
                sb.Append("\nProjects: ");
                foreach (var project in Projects) sb.Append(project.Key).Append(" ");
            }
            if (IssueTypes.Count > 0) {
                sb.Append("\nIssue Types: ");
                foreach (var issueType in IssueTypes) sb.Append(issueType.Name).Append(" ");
            }
            if (AffectsVersions.Count > 0) {
                sb.Append("\nAffects Versions: ");
                foreach (var version in AffectsVersions) sb.Append(version.Name).Append(" ");
            }
            if (FixForVersions.Count > 0) {
                sb.Append("\nFix For Versions: ");
                foreach (var version in FixForVersions) sb.Append(version.Name).Append(" ");
            }
            if (Components.Count > 0) {
                sb.Append("\nComponents: ");
                foreach (var comp in Components) sb.Append(comp.Name).Append(" ");
            }
            if (Reporter != UserType.UNDEFINED) {
                sb.Append("\nReporter: ").Append(Reporter.GetStringValue());
            }
            if (Assignee != UserType.UNDEFINED) {
                sb.Append("\nAssignee: ").Append(Assignee.GetStringValue());
            }
            if (Statuses.Count > 0) {
                sb.Append("\nStatuses: ");
                foreach (var status in Statuses) sb.Append(status.Name).Append(" ");
            }
            if (Resolutions.Count > 0) {
                sb.Append("\nResolutions: ");
                foreach (var resolution in Resolutions) sb.Append(resolution.Name).Append(" ");
            }
            if (Priorities.Count > 0) {
                sb.Append("\nPriorities: ");
                foreach (var priority in Priorities) sb.Append(priority.Name).Append(" ");
            }
 
            sb.Append("\n\nRight-click for filter operations");

            return sb.ToString();
        }

        public static void load() {
            FILTERS.Clear();

            var store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            var filtersCount = store.loadParameter(FILTER_COUNT, 0);
            var servers = JiraServerModel.Instance.getAllServers();

            for (int i = 0; i < filtersCount; ++i) {
                var filterGuidStr = store.loadParameter(FILTER_GUID + i, null);
                var filterGuid = new Guid(filterGuidStr);
                var filterServerGuidStr = store.loadParameter(getParamKey(filterGuid, FILTER_SERVER_GUID + filterGuidStr), null);
                var serverGuid = new Guid(filterServerGuidStr);
                var server = servers.FirstOrDefault(s => s.GUID.Equals(serverGuid));
                if (server == null) continue;

                var filter = new JiraCustomFilter(server, filterGuid)
                                          {
                                              Name = store.loadParameter(getParamKey(filterGuid, FILTER_NAME + filterGuidStr), CUSTOM_FILTER_DEFAULT_NAME)
                                          };

                loadProjects(store, filterGuid, filter);
                loadIssueTypes(store, filterGuid, filter);
                loadFixVersions(store, filterGuid, filter);
                loadAffectsVersions(store, filterGuid, filter);
                loadComponents(store, filterGuid, filter);
                loadReporter(store, filterGuid, filter);
                loadAssignee(store, filterGuid, filter);
                loadStatuses(store, filterGuid, filter);
                loadResolutions(store, filterGuid, filter);
                loadPriorities(store, filterGuid, filter);

                FILTERS[filterGuid] = filter;
            }
        }

        public static void save() {
            var store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            store.storeParameter(FILTER_COUNT, FILTERS.Count);
            var i = 0;
            foreach (var filter in FILTERS) {
                store.storeParameter(FILTER_GUID + i, filter.Key.ToString());
                store.storeParameter(getParamKey(filter.Key, FILTER_SERVER_GUID + filter.Key), filter.Value.server.GUID.ToString());

                var f = filter.Value;

                store.storeParameter(getParamKey(filter.Key, FILTER_NAME + filter.Key), filter.Value.Name);

                storeProjects(store, filter.Key, f);
                storeIssueTypes(store, filter.Key, f);
                storeFixVersions(store, filter.Key, f);
                storeAffectsVersions(store, filter.Key, f);
                storeComponents(store, filter.Key, f);
                storeReporter(store, filter.Key, f);
                storeAssignee(store, filter.Key, f);
                storeStatuses(store, filter.Key, f);
                storeResolutions(store, filter.Key, f);
                storePriorities(store, filter.Key, f);

                ++i;
            }
        }

        private static void storeAssignee(ParameterStore store, Guid key, JiraCustomFilter f) {
            store.storeParameter(getParamKey(key, FILTER_ASSIGNEE), f.Assignee.ToString());
        }

        private static void loadAssignee(ParameterStore store, Guid key, JiraCustomFilter f) {
            string assigneeString = store.loadParameter(getParamKey(key, FILTER_ASSIGNEE), UserType.UNDEFINED.ToString());
            try {
                f.Assignee = (UserType)Enum.Parse(typeof(UserType), assigneeString);
            } catch (Exception) {
                f.Assignee = UserType.UNDEFINED;
            }
        }

        private static void storeReporter(ParameterStore store, Guid key, JiraCustomFilter f) {
            store.storeParameter(getParamKey(key, FILTER_REPORTER), f.Reporter.ToString());
        }

        private static void loadReporter(ParameterStore store, Guid key, JiraCustomFilter f) {
            string reporterString = store.loadParameter(getParamKey(key, FILTER_REPORTER), UserType.UNDEFINED.ToString());
            try {
                f.Reporter = (UserType)Enum.Parse(typeof(UserType), reporterString);
            } catch (Exception) {
                f.Reporter = UserType.UNDEFINED;
            }
        }


        private static void storeResolutions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_RESOLUTIONS_COUNT), f.Resolutions.Count);
            foreach (JiraNamedEntity resolution in f.Resolutions) {
                store.storeParameter(getParamKey(key, FILTER_RESOLUTIONS_ID + i), resolution.Id);
                store.storeParameter(getParamKey(key, FILTER_RESOLUTIONS_NAME + i), resolution.Name);
                ++i;
            }
        }

        private static void loadResolutions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_RESOLUTIONS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_RESOLUTIONS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_RESOLUTIONS_NAME + i), null);
                JiraNamedEntity resolution = new JiraNamedEntity(id, name, null);
                f.Resolutions.Add(resolution);
            }
        }

        private static void storeStatuses(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_STATUSES_COUNT), f.Statuses.Count);
            foreach (JiraNamedEntity status in f.Statuses) {
                store.storeParameter(getParamKey(key, FILTER_STATUSES_ID + i), status.Id);
                store.storeParameter(getParamKey(key, FILTER_STATUSES_NAME + i), status.Name);
                ++i;
            }
        }

        private static void loadStatuses(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_STATUSES_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_STATUSES_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_STATUSES_NAME + i), null);
                JiraNamedEntity status = new JiraNamedEntity(id, name, null);
                f.Statuses.Add(status);
            }
        }

        private static void storePriorities(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_PRIORITIES_COUNT), f.Priorities.Count);
            foreach (JiraNamedEntity priority in f.Priorities) {
                store.storeParameter(getParamKey(key, FILTER_PRIORITIES_ID + i), priority.Id);
                store.storeParameter(getParamKey(key, FILTER_PRIORITIES_NAME + i), priority.Name);
                ++i;
            }
        }

        private static void loadPriorities(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_PRIORITIES_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_PRIORITIES_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_PRIORITIES_NAME + i), null);
                JiraNamedEntity priority = new JiraNamedEntity(id, name, null);
                f.Priorities.Add(priority);
            }
        }

        private static void storeComponents(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_COMPONENTS_COUNT), f.Components.Count);
            foreach (JiraNamedEntity comp in f.Components) {
                store.storeParameter(getParamKey(key, FILTER_COMPONENTS_ID + i), comp.Id);
                store.storeParameter(getParamKey(key, FILTER_COMPONENTS_NAME + i), comp.Name);
                ++i;
            }
        }

        private static void loadComponents(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_NAME + i), null);
                JiraNamedEntity comp = new JiraNamedEntity(id, name, null);
                f.Components.Add(comp);
            }
        }

        private static void storeAffectsVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;
            store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_COUNT), f.AffectsVersions.Count);
            foreach (JiraNamedEntity version in f.AffectsVersions) {
                store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_ID + i), version.Id);
                store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_NAME + i), version.Name);
                ++i;
            }
        }

        private static void loadAffectsVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_NAME + i), null);
                JiraNamedEntity affectsVersion = new JiraNamedEntity(id, name, null);
                f.AffectsVersions.Add(affectsVersion);
            }
        }

        private static void storeFixVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_COUNT), f.FixForVersions.Count);
            foreach (JiraNamedEntity version in f.FixForVersions) {
                store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_ID + i), version.Id);
                store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_NAME + i), version.Name);
                ++i;
            }
        }

        private static void loadFixVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_NAME + i), null);
                JiraNamedEntity fixVersion = new JiraNamedEntity(id, name, null);
                f.FixForVersions.Add(fixVersion);
            }
        }

        private static void storeIssueTypes(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_COUNT), f.IssueTypes.Count);
            foreach (JiraNamedEntity issueType in f.IssueTypes) {
                store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_ID + i), issueType.Id);
                store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_NAME + i), issueType.Name);
                ++i;
            }
        }

        private static void loadIssueTypes(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_NAME + i), null);
                JiraNamedEntity issueType = new JiraNamedEntity(id, name, null);
                f.IssueTypes.Add(issueType);
            }
        }

        private static void storeProjects(ParameterStore store, Guid key, JiraCustomFilter f) {
            store.storeParameter(getParamKey(key, FILTER_PROJECT_COUNT), f.Projects.Count);
            int i = 0;
            foreach (JiraProject project in f.Projects) {
                store.storeParameter(getParamKey(key, FILTER_PROJECT_ID + i), project.Id);
                store.storeParameter(getParamKey(key, FILTER_PROJECT_KEY + i), project.Key);
                ++i;
            }
        }

        private static void loadProjects(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_PROJECT_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_PROJECT_ID + i), 0);
                string projectKey = store.loadParameter(getParamKey(key, FILTER_PROJECT_KEY + i), null);
                JiraProject proj = new JiraProject(id, projectKey, projectKey);
                f.Projects.Add(proj);
            }
        }

        private static string getParamKey(Guid serverGuid, string paramName) {
            return paramName + serverGuid;
        }
    }
}