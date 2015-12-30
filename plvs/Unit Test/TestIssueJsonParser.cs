using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Atlassian.plvs.api.jira;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unit_Test {
    [TestClass]
    public class TestIssueJsonParser {
        private JiraServer server = new JiraServer(Guid.NewGuid(), "test", "http://localhost", "test", "test", false, false, true);

        [TestMethod]
        public void TestParseJsonPlvs384() {
            try {
                var json = Resource.plvs_384_issue_json_txt;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                var s = new JiraServer("a", "http:/a", "a", "a", true, false);
                var issueObject = new JiraIssue(s, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestPlvs374() {
            try {
                var json = Resource.plvs_374_no_priority;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                new JiraIssue(server, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestPlvs374BadDate() {
            try {
                var info = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
                var json = Resource.plvs_374_bad_date;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                new JiraIssue(server, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestPlvs389() {
            try {
                var info = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
                var json = Resource.plvs_389;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                new JiraIssue(server, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestPlvs313() {
            JToken current = null;
            try {
                var json = Resource.plvs_413;
                var issues = JsonConvert.DeserializeObject(json) as JToken;
                foreach (var issue in issues["issues"]) {
                    current = issue;
                    new JiraIssue(server, current);
                }
            } catch (Exception e) {
                Assert.Fail(current["key"].Value<string>() + ": " + e.Message);
            }
        }

    }
}
