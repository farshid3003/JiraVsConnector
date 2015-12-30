using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.windows;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.util.jira {
    public sealed class JiraIssueUtils {

        public static readonly Regex ISSUE_REGEX = new Regex(@"(([A-Z]+)-\d+)");

        private const string JiraFormat = "ddd, d MMM yyyy HH:mm:ss zzz";
        private const string ShortFormatFromJira = "dd/MM/yy";
        private const string ShortFormatToJira = "dd/MMM/yy";
        private const string ShortRestFormatToJira = "yyyy-MM-dd";

        public static DateTime getDateTimeFromJiraTimeString(string locale, string value) {
            int bracket = value.LastIndexOf("(");
            if (bracket != -1) {
                value = value.Substring(0, bracket);
            }

            try {
                return DateTime.ParseExact(value.Trim(), JiraFormat, new CultureInfo(fixLocale(locale) ?? "en-US"), DateTimeStyles.None);
            } catch (Exception) {
                return DateTime.MinValue;
            } 
        }

        public static string getJiraFormattedTimeString(DateTime dt) {
            return dt.ToString(JiraFormat, CultureInfo.InvariantCulture);
        }

        private static string fixLocale(string locale) {
            if (locale != null && !locale.ToLower().Equals("en-us") && locale.ToLower().StartsWith("en-")) {
                return "en-gb";
            }
            return locale;
        }

        public static DateTime getDateTimeFromShortString(string locale, string value) {
            // let's try both formats
            try {
                return DateTime.ParseExact(value.Trim(), ShortFormatFromJira, new CultureInfo(fixLocale(locale) ?? "en-US"), DateTimeStyles.None);
            } catch (Exception) {
                try {
                    return DateTime.ParseExact(value.Trim(), ShortFormatToJira, new CultureInfo(fixLocale(locale) ?? "en-US"), DateTimeStyles.None);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
        }

        public static string getTimeStringFromIssueDateTime(DateTime time) {
            if (time.Equals(DateTime.MinValue)) {
                return "Unknown";
            }
            return time.ToShortDateString() + " " + time.ToShortTimeString();
        }

        public static string getShortDateStringFromDateTime(string locale, DateTime time) {
            return time.ToString(ShortFormatToJira, new CultureInfo(fixLocale(locale) ?? "en-US"));
        }

        public static string getShortRestDateStringFromDateTime(DateTime time) {
            return time.ToString(ShortRestFormatToJira, new CultureInfo("en-US"));
        }

        public static string addSpacesToTimeSpec(string text) {
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);

            if (!regex.IsMatch(text)) {
                throw new ArgumentException("Time specification must be in the " + Constants.TIME_TRACKING_SYNTAX + " format");
            }

            Match match = regex.Match(text);
            Group @groupWeeks = match.Groups[2];
            Group @groupDays = match.Groups[4];
            Group @groupHours = match.Groups[6];
            Group @groupMinutes = match.Groups[8];

            string result = "";
            if (groupWeeks != null && groupWeeks.Success) result = result + groupWeeks.Value + "w ";
            if (groupDays != null && groupDays.Success) result = result + groupDays.Value + "d ";
            if (groupHours != null && groupHours.Success) result = result + groupHours.Value + "h ";
            if (groupMinutes != null && groupMinutes.Success) result = result + groupMinutes.Value + "m";
            return result.Trim();
        }

        public static T getRawIssueObjectPropertyValue<T>(object rawObject, string name) {

            if (rawObject == null) {
                return default(T);
            }

            var t = rawObject as JToken;
            if (t != null) {
                var prop = t["fields"][name];
                if (prop == null) return default(T);
                return prop.HasValues ? prop["name"].Value<T>() : prop.Value<T>();
            }

            var property = rawObject.GetType().GetProperty(name);
            if (property == null) {
                return default(T);
            }
            return (T) property.GetValue(rawObject, null);
        }

        public static T[] getRawIssueObjectPropertyValueArray<T>(object rawObject, string name)  {

            if (rawObject == null) {
                return default(T[]);
            }

            var t = rawObject as JToken;
            if (t != null) {
                var prop = t["fields"][name];
                return prop == null 
                    ? default(T[]) 
                    : prop.Select(ch => (T) ((object) ch)).ToArray();
            }

            var property = rawObject.GetType().GetProperty(name);
            if (property == null) {
                return default(T[]);
            }
            return (T[])property.GetValue(rawObject, null);
        }

        public static void openInIde(string issueKey) {
            if (issueKey == null) return;

            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModelImpl.Instance.Issues.Where(issue => issue.Key.Equals(issueKey))) {
                IssueDetailsWindow.Instance.openIssue(issue, AtlassianPanel.Instance.Jira.ActiveIssueManager);
                found = true;
                break;
            }
            if (!found) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(issueKey, findFinished);
            }
        }

        private static void findFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
        }

        public static void launchBrowser(string issueKey) {
            if (issueKey == null) {
                return;
            }
            try {
                JiraServer server = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
                if (server != null) {
                    PlvsUtils.runBrowser(server.Url + "/browse/" + issueKey);
                } else {
                    MessageBox.Show("No JIRA server selected", Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
// ReSharper disable EmptyGeneralCatchClause
            } catch { }
// ReSharper restore EmptyGeneralCatchClause
        }
    }
}