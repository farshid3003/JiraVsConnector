using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs.api.jira.soap {

    public class SoapSession : IDisposable {

        public string Token { get; set; }
        private readonly JiraSoapServiceService service = new JiraSoapServiceService();

        public SoapSession(string url, bool noProxy) {
            service.Url = url + "/rpc/soap/jirasoapservice-v2";
            service.Timeout = GlobalSettings.NetworkTimeout * 1000;
            service.Proxy = noProxy ? null : GlobalSettings.Proxy;
        }

        public string login(string userName, string password) {
            try {
                service.Credentials = new NetworkCredential(CredentialUtils.getUserNameWithoutDomain(userName), password, CredentialUtils.getUserDomain(userName));
                Token = service.login(CredentialUtils.getUserNameWithoutDomain(userName), password);
                return Token;
            } catch (Exception e) {
                throw new LoginException(e);
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
#if PLVS_133_WORKAROUND

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

#else
            RemoteProject[] pTable = service.getProjectsNoSchemes(Token);
            return pTable.Select(p => new JiraProject(int.Parse(p.id), p.key, p.name)).ToList();
#endif
        }

        public List<JiraSavedFilter> getSavedFilters() {
#if PLVS_133_WORKAROUND
            object[] results = service.getSavedFilters(Token);
            return (from pobj in results.ToList()
                    let id = pobj.GetType().GetProperty("id")
                    let name = pobj.GetType().GetProperty("name")
                    select new JiraSavedFilter(
                        int.Parse((string) id.GetValue(pobj, null)), 
                        (string) name.GetValue(pobj, null)))
                    .ToList();
#else
            RemoteFilter[] fTable = service.getSavedFilters(Token);
            return fTable.Select(f => new JiraSavedFilter(int.Parse(f.id), f.name)).ToList();
#endif
        }

        public string createIssue(JiraIssue issue) {

#if PLVS_133_WORKAROUND
            object[] issuesTable = service.getIssuesFromTextSearch(Token, "<<<<<<<<<<<<<IHOPETHEREISNOSUCHTEXT>>>>>>>>>>>>>>");

            Type issueObjectType = issuesTable.GetType().GetElementType();
            object ri = issueObjectType.GetConstructor(new Type[] {}).Invoke(new object[] {});
            setObjectProperty(ri, "project", issue.ProjectKey);
            setObjectProperty(ri, "type", issue.IssueTypeId.ToString());
            setObjectProperty(ri, "priority", issue.PriorityId.ToString());
            setObjectProperty(ri, "summary", issue.Summary);
            setObjectProperty(ri, "description", issue.Description);

            if (issue.Assignee != null) {
                setObjectProperty(ri, "assignee", issue.Assignee);
            }

            if (issue.Components != null && issue.Components.Count > 0) {
                object[] components = service.getComponents(Token, issue.ProjectKey);
                setObjectTablePropertyFromObjectList(ri, "components", issue.Components, components);
            }

            object[] versions = service.getVersions(Token, issue.ProjectKey);

            if (issue.Versions != null && issue.Versions.Count > 0) {
                setObjectTablePropertyFromObjectList(ri, "affectsVersions", issue.Versions, versions);
            }

            if (issue.FixVersions != null && issue.FixVersions.Count > 0) {
                setObjectTablePropertyFromObjectList(ri, "fixVersions", issue.FixVersions, versions);
            }

            object createdIssue = service.createIssue(Token, ri);
            return (string) createdIssue.GetType().GetProperty("key").GetValue(createdIssue, null);

#else
            RemoteIssue ri = new RemoteIssue
                 {
                     project = issue.ProjectKey,
                     type = issue.IssueTypeId.ToString(),
                     priority = issue.PriorityId.ToString(),
                     summary = issue.Summary,
                     description = issue.Description,
                 };
            if (issue.Assignee != null) {
                ri.assignee = issue.Assignee;
            }

            if (issue.Components != null && issue.Components.Count > 0) {
                IEnumerable<JiraNamedEntity> components = getComponents(issue.ProjectKey);
//                RemoteComponent[] objects = service.getComponents(Token, issue.ProjectKey);
                List<RemoteComponent> comps = new List<RemoteComponent>();
                foreach (string t in issue.Components) {
                    // fixme: a bit problematic part. What if two objects have the same name?
                    // I suppose JiraIssue class has to be fixed, but that would require more problematic
                    // construction of it during server query
                    string tCopy = t;
                    foreach (JiraNamedEntity component in components.Where(component => component.Name.Equals(tCopy))) {
                        comps.Add(new RemoteComponent {id = component.Id.ToString(), name = component.Name});
                        break;
                    }
                }
                ri.components = comps.ToArray();
            }

            List<JiraNamedEntity> versions = getVersions(issue.ProjectKey);
//            RemoteVersion[] versions = service.getVersions(Token, issue.ProjectKey);
            
            if (issue.Versions != null && issue.Versions.Count > 0) {
                List<RemoteVersion> vers = new List<RemoteVersion>();
                foreach (string t in issue.Versions) {
                    // fixme: a bit problematic part. same as for objects
                    string tCopy = t;
                    foreach (JiraNamedEntity version in versions.Where(version => version.Name.Equals(tCopy))) {
                        vers.Add(new RemoteVersion {id = version.Id.ToString(), name = version.Name } );
                        break;
                    }
                }
                ri.affectsVersions = vers.ToArray();
            }

            if (issue.FixVersions != null && issue.FixVersions.Count > 0) {
                List<RemoteVersion> vers = new List<RemoteVersion>();
                foreach (string t in issue.FixVersions) {
                    // fixme: a bit problematic part. same as for objects
                    string tCopy = t;
                    foreach (JiraNamedEntity version in versions.Where(version => version.Name.Equals(tCopy))) {
                        vers.Add(new RemoteVersion { id = version.Id.ToString(), name = version.Name });
                        break;
                    }
                }
                ri.fixVersions = vers.ToArray();
            }

            RemoteIssue createdIssue = service.createIssue(Token, ri);
            return createdIssue.key;
#endif
        }

        public void addComment(JiraIssue issue, string comment) {
#if PLVS_133_WORKAROUND
            object[] comments = service.getComments(Token, issue.Key);
            if (comments == null) {
                throw new Exception("Unable to retrieve information about the RemoteComment type");
            }
            Type type = comments.GetType();
            Type commentType = type.GetElementType();
            ConstructorInfo constructor = commentType.GetConstructor(new Type[] {});
            object commentObject = constructor.Invoke(new object[] {});
            setObjectProperty(commentObject, "body", comment);
            service.addComment(Token, issue.Key, commentObject);
#else
            service.addComment(Token, issue.Key, new RemoteComment {body = comment});
#endif
        }

        public object getIssueSoapObject(string key) {
            return service.getIssue(Token, key);
        }

        public JiraNamedEntity getSecurityLevel(string key) {
            try {
#if PLVS_133_WORKAROUND
                object securityLevel = service.getSecurityLevel(Token, key);
                return securityLevel == null ? null : createNamedEntity(securityLevel);
#else
                RemoteSecurityLevel securityLevel = service.getSecurityLevel(Token, key);
                return securityLevel == null ? null : new JiraNamedEntity(int.Parse(securityLevel.id), securityLevel.name, null);
#endif
            } catch (Exception) {
                return null;
            }
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

        private IEnumerable<JiraNamedEntity> getComponents(string projectKey) {
            return createEntityList(service.getComponents(Token, projectKey));
        }

        public List<JiraNamedEntity> getVersions(JiraProject project) {
            return getVersions(project.Key);
        }

        private List<JiraNamedEntity> getVersions(string projectKey) {
            return createEntityList(service.getVersions(Token, projectKey));
        }

        public List<JiraNamedEntity> getResolutions() {
            return createEntityListFromConstants(service.getResolutions(Token));
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
#if PLVS_133_WORKAROUND
            object[] results = service.getAvailableActions(Token, issue.Key);
            return createEntityList(results);
#else
            RemoteNamedObject[] actions = service.getAvailableActions(Token, issue.Key);
            if (actions == null) return new List<JiraNamedEntity>();

            return (from action in actions 
                    where action != null 
                    select new JiraNamedEntity(int.Parse(action.id), action.name, null)).ToList();
#endif
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int id) {
#if PLVS_133_WORKAROUND
            object[] fields = service.getFieldsForAction(Token, issue.Key, id.ToString());
            if (fields == null) return new List<JiraField>();

            return (from field in fields
                    where field != null
                    let fieldId = field.GetType().GetProperty("id")
                    let name = field.GetType().GetProperty("name")
                    select new JiraField((string) fieldId.GetValue(field, null), (string) name.GetValue(field, null)))
                    .ToList();
#else
            RemoteField[] fields = service.getFieldsForAction(Token, issue.Key, id.ToString());
            if (fields == null) return new List<JiraField>();

            return (from field in fields 
                    where field != null 
                    select new JiraField(field.id, field.name)).ToList();
#endif
        }

        public void runIssueActionWithoutParams(JiraIssue issue, int id) {
            service.progressWorkflowAction(Token, issue.Key, id.ToString(), null);
        }

        public void runIssueActionWithParams(JiraIssue issue, int id, ICollection<JiraField> fields, string comment) {
#if PLVS_133_WORKAROUND
            List<object> fieldValues = new List<object>();
            foreach (var field in fields) {
                if (field.Values == null) continue;
                object rfv = createRemoteFieldValueObject(field.Id);
                setObjectProperty(rfv, "values", field.Values.ToArray());
                fieldValues.Add(rfv);
            }
            service.progressWorkflowAction(Token, issue.Key, id.ToString(), fieldValues.ToArray());
            if (comment != null) {
                addComment(issue, comment);
            }
#else
            if (fields == null || fields.Count == 0) {
                throw new Exception("Field values must not be empty");
            }
            service.progressWorkflowAction(Token, issue.Key, id.ToString(), 
                (from field in fields where field.Values != null 
                 select new RemoteFieldValue {id = field.Id, values = field.Values.ToArray()}).ToArray());
            if (!string.IsNullOrEmpty(comment)) {
                service.addComment(Token, issue.Key, new RemoteComment { body = comment });
            }
#endif
        }

        public void logWorkAndAutoUpdateRemaining(string key, string timeSpent, DateTime startDate, string comment) {
#if PLVS_133_WORKAROUND
            ConstructorInfo constructor = getRemoteWorklogConstructor(key);
            object worklog = createWorklogObject(constructor, timeSpent, startDate, comment);
            service.addWorklogAndAutoAdjustRemainingEstimate(Token, key, worklog);
#else
            RemoteWorklog worklog = new RemoteWorklog { timeSpent = timeSpent, startDate = startDate };
            service.addWorklogAndAutoAdjustRemainingEstimate(Token, key, worklog);
#endif
        }

        public void logWorkAndLeaveRemainingUnchanged(string key, string timeSpent, DateTime startDate, string comment) {
#if PLVS_133_WORKAROUND
            ConstructorInfo constructor = getRemoteWorklogConstructor(key);
            object worklog = createWorklogObject(constructor, timeSpent, startDate, comment);
            service.addWorklogAndRetainRemainingEstimate(Token, key, worklog);
#else
            RemoteWorklog worklog = new RemoteWorklog { timeSpent = timeSpent, startDate = startDate };
            service.addWorklogAndRetainRemainingEstimate(Token, key, worklog);
#endif
        }

        public void logWorkAndUpdateRemainingManually(string key, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
#if PLVS_133_WORKAROUND
            ConstructorInfo constructor = getRemoteWorklogConstructor(key);
            object worklog = createWorklogObject(constructor, timeSpent, startDate, comment);
            service.addWorklogWithNewRemainingEstimate(Token, key, worklog, remainingEstimate);
#else
            RemoteWorklog worklog = new RemoteWorklog { timeSpent = timeSpent, startDate = startDate };
            service.addWorklogWithNewRemainingEstimate(Token, key, worklog, remainingEstimate);
#endif
        }

        public void updateIssue(string key, ICollection<JiraField> fields) {
#if PLVS_133_WORKAROUND
            List<object> values = new List<object>();
            foreach (var field in fields) {
                object rfv = createRemoteFieldValueObject(field.Id);
                setObjectProperty(rfv, "values", field.Values.ToArray());
                values.Add(rfv);
            }
            service.updateIssue(Token, key, values.ToArray());
#else
            service.updateIssue(Token, key, fields.Select(field => new RemoteFieldValue {id = field.Id, values = field.Values.ToArray()}).ToArray());
#endif
        }

        public void uploadAttachment(string key, string name, byte[] attachment) {
            service.addBase64EncodedAttachmentsToIssue(Token, key, new[] {name}, new[] {Convert.ToBase64String(attachment)});
        }

#if PLVS_133_WORKAROUND
        private static List<JiraNamedEntity> createEntityList(IEnumerable<object> entities) {
            return entities == null 
                ? new List<JiraNamedEntity>() 
                : (from val in entities where val != null select createNamedEntity(val)).ToList();
        }

        private static List<JiraNamedEntity> createEntityListFromConstants(IEnumerable<object> vals) {
            return createEntityList(vals);
        }
#else 
        private static List<JiraNamedEntity> createEntityList(IEnumerable<AbstractNamedRemoteEntity> entities) {
            if (entities == null) return new List<JiraNamedEntity>();

            return (from val in entities 
                    where val != null 
                    select new JiraNamedEntity(int.Parse(val.id), val.name, null)).ToList();
        }

        private static List<JiraNamedEntity> createEntityListFromConstants(IEnumerable<AbstractRemoteConstant> vals) {
            if (vals == null) return new List<JiraNamedEntity>();

            return (from val in vals 
                    where val != null 
                    select new JiraNamedEntity(int.Parse(val.id), val.name, val.icon)).ToList();
        }

#endif

        private ConstructorInfo getRemoteWorklogConstructor(string key) {
            object[] remoteWorklogs = service.getWorklogs(Token, key);
            Type elementType = remoteWorklogs.GetType().GetElementType();
            return elementType.GetConstructor(new Type[] { });
        }

        private static object createWorklogObject(ConstructorInfo constructor, string timeSpent, DateTime startDate, string comment) {
            object worklog = constructor.Invoke(new object[] { });
            setObjectProperty(worklog, "timeSpent", timeSpent);
            setObjectProperty(worklog, "startDate", startDate);
            if (!string.IsNullOrEmpty(comment)) {
                setObjectProperty(worklog, "comment", comment);
            }

            return worklog;
        }

        private static object createRemoteFieldValueObject(string id) {
            MemberInfo info = typeof(JiraSoapServiceService);
            object[] attributes = info.GetCustomAttributes(true);
            Type rfvType = (from attribute in attributes
                            let property = attribute.GetType().GetProperty("Type")
                            where property != null
                            select property.GetValue(attribute, null) as Type)
                                .FirstOrDefault(type => type != null && type.Name.Equals("RemoteFieldValue"));
            if (rfvType == null) {
                throw new Exception("Unable to retrieve information about the RemoteFieldValue type");
            }
            ConstructorInfo constructor = rfvType.GetConstructor(new Type[] { });
            object rfv = constructor.Invoke(new object[] { });
            setObjectProperty(rfv, "id", id);
            return rfv;
        }

        private static void setObjectProperty(object o, string name, object value) {
            o.GetType().GetProperty(name).SetValue(o, value, null);
        }

        private static void setObjectTablePropertyFromObjectList(object o, string propertyName, IEnumerable<string> items, object[] availableObjects) {
            ConstructorInfo tableConstructor = availableObjects.GetType().GetConstructor(new[] { typeof(int) });
            ConstructorInfo itemConstructor = availableObjects.GetType().GetElementType().GetConstructor(new Type[] { });
            List<object> list = new List<object>();
            foreach (string item in items) {

                foreach (object obj in availableObjects) {

                    // fixme: a bit problematic part. What if two entities have the same name?
                    // I suppose JiraIssue class has to be fixed, but that would require more problematic
                    // construction of it during server query
                    if (!obj.GetType().GetProperty("name").GetValue(obj, null).Equals(item)) continue;

                    object newItem = itemConstructor.Invoke(new object[] { });
                    setObjectProperty(newItem, "id", obj.GetType().GetProperty("id").GetValue(obj, null));
                    setObjectProperty(newItem, "name", obj.GetType().GetProperty("name").GetValue(obj, null));
                    
                    list.Add(newItem);
                }
            }
            object[] table = tableConstructor.Invoke(new object[] { list.Count }) as object[];
            if (table == null) return;
            for (int i = 0; i < table.Length; ++i) {
                table[i] = list[i];
            }
            setObjectProperty(o, propertyName, table);
        }

        private static JiraNamedEntity createNamedEntity(object o) {
            var idVal = (string)o.GetType().GetProperty("id").GetValue(o, null);
            try {
                var id = int.Parse(idVal);
                var name = (string)o.GetType().GetProperty("name").GetValue(o, null);
                var propertyInfo = o.GetType().GetProperty("icon");
                string icon = null;
                if (propertyInfo != null) {
                    icon = (string)propertyInfo.GetValue(o, null);
                }
                return new JiraNamedEntity(id, name, icon);
            } catch(Exception) {
                return null;
            }
        }
    }
}