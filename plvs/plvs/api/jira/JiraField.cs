using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraField {
        public List<string> Values { get; set; }

        public JiraField(string id, string name) {
            Id = id;
            Name = name;
            Values = new List<string>();
            // oh well
            Required = true;
        }

        public JiraField(JiraField other) {
            Id = other.Id;
            Name = other.Name;
            Values = new List<string>(other.Values);
            Required = other.Required;
            FieldDefinition = other.FieldDefinition;
        }

        public JiraField(JToken fields, string name) {
            FieldDefinition = fields[name];
            Id = name;
            Name = FieldDefinition["name"].Value<string>();
            Required = FieldDefinition["required"].Value<bool>();
            Values = new List<string>();
        }

        public void setRawIssueObject(object rawIssueObject) {
            var issue = rawIssueObject as JToken;
            if (issue == null) return;
            FieldDefinition = issue["editmeta"]["fields"][Id];
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string SettablePropertyName { get; set; }
        public JToken FieldDefinition { get; private set; }

        public object getJsonValue() {
            if (FieldDefinition == null) return null;
            
            var fieldType = FieldDefinition["schema"]["type"].Value<string>();

            if ("array".Equals(fieldType)) {
                if (Values == null) return null;
                if (Values.Count == 0) return new List<object>();

                var stringItems = "string".Equals(FieldDefinition["schema"]["items"].Value<string>());
                var res = Values.Select(value => stringItems ? value : getPair(SettablePropertyName, value)).ToList();
                return filterEmptyStrings(res);
            }

            var simple = !"user".Equals(fieldType) && !"timetracking".Equals(fieldType) && FieldDefinition["allowedValues"] == null;
            if (simple) {
                return Values == null || Values.Count == 0 ? null : Values[0];
            }

            if (SettablePropertyName == null) {
                return Values == null || Values.Count == 0 ? null : Values[0];
            }
            return getPair(SettablePropertyName, Values[0]);
        }

        private static List<object> filterEmptyStrings(List<object> list) {
            var result = new List<object>();
            foreach (var item in list) {
                if (!(item is string)) {
                    result.Add(item);
                } else {
                    if (((string)item).Length == 0) {
                        continue;
                    }
                    result.Add(item);
                }
            }
            return result.Count > 0 ? result : null;
        }

        private static object getPair(string key, object val) {
            var d = new Dictionary<string, object> {{key, val}};
            string there = JsonConvert.SerializeObject(d);
            var back = JsonConvert.DeserializeObject(there) as JContainer;
            return back;
        }
    }
}