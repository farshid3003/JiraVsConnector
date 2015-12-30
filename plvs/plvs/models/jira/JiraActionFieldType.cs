using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira.fields;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.models.jira {
    class JiraActionFieldType {

        public enum WidgetType {
            UNSUPPORTED,
            ASSIGNEE,
            REPORTER,
            ISSUE_TYPE,
            SUMMARY,
            DESCRIPTION,
            ENVIRONMENT,
            DUE_DATE,
            TIMETRACKING,
            VERSIONS,
            FIX_VERSIONS,
            RESOLUTION,
            PRIORITY,
            COMPONENTS,
            SECURITY
        }
 
        public sealed class WidgetTypeAndFieldFiller {
            public int SortOrder { get; private set; }
            public FieldFiller Filler { get; private set; }
            public WidgetType TypeOfWidget { get; private set; }

            private static int sortOrderCounter;

            public WidgetTypeAndFieldFiller(WidgetType widgetType, FieldFiller filler) {
                SortOrder = sortOrderCounter++;
                Filler = filler;
                TypeOfWidget = widgetType;
            }
        }

        private static readonly SortedDictionary<string, WidgetTypeAndFieldFiller> TypeMap = 
            new SortedDictionary<string, WidgetTypeAndFieldFiller> {
                                                                       { "issuetype", new WidgetTypeAndFieldFiller(WidgetType.ISSUE_TYPE, new IssueTypeFiller()) },
                                                                       { "summary", new WidgetTypeAndFieldFiller(WidgetType.SUMMARY, new SummaryFiller()) },
                                                                       { "priority", new WidgetTypeAndFieldFiller(WidgetType.PRIORITY, new PriorityFiller()) },
                                                                       { "description", new WidgetTypeAndFieldFiller(WidgetType.DESCRIPTION, new DescriptionFiller()) },
                                                                       { "resolution", new WidgetTypeAndFieldFiller(WidgetType.RESOLUTION, new ResolutionFiller()) },
                                                                       { "environment", new WidgetTypeAndFieldFiller(WidgetType.ENVIRONMENT, new EnvironmentFiller()) },
                                                                       { "components", new WidgetTypeAndFieldFiller(WidgetType.COMPONENTS, new ComponentsFiller()) },
                                                                       { "versions", new WidgetTypeAndFieldFiller(WidgetType.VERSIONS, new AffectsVersionsFiller()) },
                                                                       { "fixVersions", new WidgetTypeAndFieldFiller(WidgetType.FIX_VERSIONS, new FixVersionsFiller()) },
                                                                       { "assignee", new WidgetTypeAndFieldFiller(WidgetType.ASSIGNEE, new AssigneeFiller()) },
                                                                       { "reporter", new WidgetTypeAndFieldFiller(WidgetType.REPORTER, new ReporterFiller()) },
                                                                       { "duedate", new WidgetTypeAndFieldFiller(WidgetType.DUE_DATE, new DueDateFiller()) },
                                                                       { "timetracking", new WidgetTypeAndFieldFiller(WidgetType.TIMETRACKING, new TimeTrackingFiller()) },
                                                                       { "security", new WidgetTypeAndFieldFiller(WidgetType.SECURITY, new SecurityFiller()) }
                                                                   };

        private static readonly CustomFieldFiller CustomFieldFiller = new CustomFieldFiller();
        
        private const string TIMEORIGINALESTIMATE = "timeoriginalestimate";
        private const string TIMEESTIMATE = "timeestimate";
        private const string TIMESPENT = "timespent";

        private JiraActionFieldType() {}

        public static WidgetType getFieldTypeForField(JiraField field) {
            return getFieldTypeForFieldId(field.Id);
        }

        public static WidgetType getFieldTypeForFieldId(string fieldId) {
            return TypeMap.ContainsKey(fieldId) ? TypeMap[fieldId].TypeOfWidget : WidgetType.UNSUPPORTED;
        }

        public static List<JiraField> fillFieldValues(JiraIssue issue, object rawIssueObject, List<JiraField> fields) {
            List<JiraField> result = new List<JiraField>();

            if (fields == null) {
                return result;
            }

            result.AddRange(fields.Select(field => fillField(issue, rawIssueObject, field)).Where(filledField => filledField != null));

            addTimeFields(issue, rawIssueObject is JToken, result);

            return result;
        }

        private static void addTimeFields(JiraIssue issue, bool useRestApi, ICollection<JiraField> result) {
            if (useRestApi) return;

            var originalEstimate = issue.OriginalEstimateInSeconds;
            var remainingEstimate = issue.RemainingEstimateInSeconds;
            var timeSpent = issue.TimeSpentInSeconds;

            if (originalEstimate != 0) {
                var originalEstimateField = new JiraField(TIMEORIGINALESTIMATE, "Original Estimate");
                originalEstimateField.Values.Add(originalEstimate.ToString());
                result.Add(originalEstimateField);
            }
            if (remainingEstimate != 0) {
                var remainingEstimateField = new JiraField(TIMEESTIMATE, "Remaining Estimate");
                remainingEstimateField.Values.Add(remainingEstimate.ToString());
                result.Add(remainingEstimateField);
            }
            if (timeSpent != 0) {
                var timeSpentField = new JiraField(TIMESPENT, "Time Spent");
                timeSpentField.Values.Add(timeSpent.ToString());
                result.Add(timeSpentField);
            }
        }

        public static bool isTimeField(JiraField field) {
            return field.Id.Equals(TIMEESTIMATE) || field.Id.Equals(TIMEORIGINALESTIMATE) || field.Id.Equals(TIMESPENT);
        }

        private static JiraField fillField(JiraIssue issue, object rawIssueObject, JiraField field) {
            JiraField result = new JiraField(field);
            if (TypeMap.ContainsKey(field.Id)) {
                WidgetTypeAndFieldFiller widgetTypeAndFieldFiller = TypeMap[field.Id];
                result.Values = widgetTypeAndFieldFiller.Filler.getFieldValues(field.Id, issue, rawIssueObject);
                result.SettablePropertyName = widgetTypeAndFieldFiller.Filler.getSettablePropertyName(field.Id, issue, rawIssueObject);
            } else {
                try {
                    result.Values = CustomFieldFiller.getFieldValues(field.Id, issue, rawIssueObject);
                    result.SettablePropertyName = CustomFieldFiller.getSettablePropertyName(field.Id, issue, rawIssueObject);
                } catch (Exception) {
                    if (!field.Required) return null;
                    throw;
                }
            }
            return result;
        }

        public static ICollection<JiraField> sortFieldList(List<JiraField> fieldList) {
            int customFieldOffset = TypeMap.Count;
            SortedDictionary<int, JiraField> sorted = new SortedDictionary<int, JiraField>();
            foreach (JiraField field in fieldList) {
                if (TypeMap.ContainsKey(field.Id)) {
                    sorted[TypeMap[field.Id].SortOrder] = field;
                } else {
                    sorted[customFieldOffset++] = field;
                }
            }

            return sorted.Values;
        }
    }
}