using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using soapconnecttest.soap2;

namespace soapconnecttest {

    public class SoapSession : IDisposable {

        public string Token { get; set; }
        private readonly JiraSoapServiceService service;

        public SoapSession(string url, Action<WebResponse> webResponseHandler) {
            service = new JiraSoapServiceService(url + "/rpc/soap/jirasoapservice-v2", webResponseHandler);
//            service.Url = url + "/rpc/soap/jirasoapservice-v2";
            service.Timeout = 10000;
            service.Proxy = null;
        }

        public string getWriterXml() {
            return service.WriterXml;
        }

        public string login(string userName, string password) {
            try {
                service.Credentials = new NetworkCredential(CredentialUtils.getUserNameWithoutDomain(userName), password, CredentialUtils.getUserDomain(userName));
                Token = service.login(CredentialUtils.getUserNameWithoutDomain(userName), password);
                return Token;
            } catch (Exception e) {
                throw new Exception(e.ToString());
            }
        }

        public void logout() {
            service.logout(Token);
            Token = null;
        }

        public void Dispose() {
            service.Dispose();
        }

        public List<JiraProject> getProjects() {

            object[] results = service.getProjectsNoSchemes(Token);

            return (from pobj in results.ToList()
                    let id = pobj.GetType().GetProperty("id")
                    let key = pobj.GetType().GetProperty("key")
                    let name = pobj.GetType().GetProperty("name")
                    select new JiraProject(
                        int.Parse((string) id.GetValue(pobj, null)),
                        (string) key.GetValue(pobj, null),
                        (string) name.GetValue(pobj, null)))
                    .ToList();
        }

        public List<JiraSavedFilter> getSavedFilters() {
            object[] results = service.getSavedFilters(Token);
            return (from pobj in results.ToList()
                    let id = pobj.GetType().GetProperty("id")
                    let name = pobj.GetType().GetProperty("name")
                    select new JiraSavedFilter(
                        int.Parse((string) id.GetValue(pobj, null)), 
                        (string) name.GetValue(pobj, null)))
                    .ToList();
        }

        public List<JiraNamedEntity> getIssueTypes() {
            return createEntityListFromConstants(service.getIssueTypes(Token));
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes() {
            return createEntityListFromConstants(service.getSubTaskIssueTypes(Token));
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraProject project) {
            return createEntityListFromConstants(service.getSubTaskIssueTypesForProject(Token, project.Id.ToString()));
        }

        public List<JiraNamedEntity> getIssueTypes(JiraProject project) {
            return createEntityListFromConstants(service.getIssueTypesForProject(Token, project.Id.ToString()));
        }

        public List<JiraNamedEntity> getPriorities() {
            return createEntityListFromConstants(service.getPriorities(Token));
        }

        public List<JiraNamedEntity> getStatuses() {
            return createEntityListFromConstants(service.getStatuses(Token));
        }

        public List<JiraNamedEntity> getComponents(JiraProject project) {
            return createEntityList(service.getComponents(Token, project.Key));
        }

        public List<JiraNamedEntity> getVersions(JiraProject project) {
            return getVersions(project.Key);
        }

        private List<JiraNamedEntity> getVersions(string projectKey) {
            return createEntityList(service.getVersions(Token, projectKey));
        }

        private static List<JiraNamedEntity> createEntityList(IEnumerable<object> entities) {
            return entities == null
                ? new List<JiraNamedEntity>()
                : (from val in entities where val != null select createNamedEntity(val)).ToList();
        }

        private static List<JiraNamedEntity> createEntityListFromConstants(IEnumerable<object> vals) {
            return createEntityList(vals);
        }

        private static JiraNamedEntity createNamedEntity(object o) {
            int id = int.Parse((string)o.GetType().GetProperty("id").GetValue(o, null));
            string name = (string)o.GetType().GetProperty("name").GetValue(o, null);
            PropertyInfo propertyInfo = o.GetType().GetProperty("icon");
            string icon = null;
            if (propertyInfo != null) {
                icon = (string)propertyInfo.GetValue(o, null);
            }
            return new JiraNamedEntity(id, name, icon);
        }
    }
}