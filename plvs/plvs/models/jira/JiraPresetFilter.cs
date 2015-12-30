using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models.jira {
    public abstract class JiraPresetFilter : JiraFilter {
        private readonly JiraServer server;

        private JiraProject project;
        private readonly string baseName;

        public JiraProject Project {
            get { return project; } 
            set { setProject(value); }
        }

        private void setProject(JiraProject p) {
            project = p;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            string paramName = getSettingName();
            store.storeParameter(paramName, project != null ? project.Key : null);
            setNameAndProject();
        }

        private void setNameAndProject() {
            if (project != null) {
                Name = baseName + " (" + project.Key + ")";
            } else {
                Name = baseName;
            }
        }

        private string getSettingName() {
            return "JiraPresetFilter_" + GetType() + "_" + server.GUID;
        }

        public string Name { get; private set; }

        protected JiraPresetFilter(JiraServer server, string name) {
            this.server = server;
            baseName = name;

            Name = baseName;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            string paramName = getSettingName();
            string projectKey = store.loadParameter(paramName, null);

            if (projectKey != null) {
                SortedDictionary<string, JiraProject> dictionary = JiraServerCache.Instance.getProjects(server);
                foreach (JiraProject p in dictionary.Values) {
                    if (!p.Key.Equals(projectKey)) continue;
                    project = p;
                    break;
                }
            }
            setNameAndProject();
        }

        public string getOldstyleFilterQueryString() {
            var query = getFilterQueryStringNoProject();
            if (Project != null) {
                return query + "&pid=" + Project.Id;
            }
            return query;
        }

        public string getJql() {
            var query = getJqlNoProject();
            if (Project != null) {
                return query + " and project = " + Project.Key;
            }
            return query;
        }

        public abstract string getFilterQueryStringNoProject();

        public abstract string getJqlNoProject();

        public abstract string getSortBy();
    }
}