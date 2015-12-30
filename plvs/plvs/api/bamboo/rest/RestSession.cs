using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;

namespace Atlassian.plvs.api.bamboo.rest {
    public class RestSession {

        private readonly BambooServer server;
        private string authToken;
        private string userName;
        private string password;

        private string cookie;

        private const string BUILDS_XML_SUBTREE = "/builds/builds";
        private const string BUILDS_XML_SUBTREE_3_0 = "/results/results";

        private const string INFO = "/rest/api/latest/info";

        private const string LATEST_BUILDS_FOR_FAVOURITE_PLANS_ACTION = "/rest/api/latest/build?favourite&expand=builds.build";
        private const string LATEST_BUILDS_FOR_FAVOURITE_PLANS_ACTION_3_0 = "/rest/api/latest/result?favourite&expand=results.result";

        private const string LATEST_BUILDS_FOR_PLANS_ACTION = "/rest/api/latest/build/{0}?expand=builds[0].build";
        private const string LATEST_BUILDS_FOR_PLANS_ACTION_3_0 = "/rest/api/latest/result/{0}?expand=results[0].result";
        private const string BUILD_BY_KEY_ACTION = "/rest/api/latest/build/{0}?expand=artifacts.artifact,jiraIssues.jiraIssue";
        private const string BUILD_BY_KEY_ACTION_3_0 = "/rest/api/latest/result/{0}?expand=artifacts.artifact,jiraIssues.jiraIssue";
        private const string LAST_N_BUILDS_ROM_PLAN = "/rest/api/latest/build/{0}?expand=builds[0:{1}].build";
        private const string LAST_N_BUILDS_ROM_PLAN_3_0 = "/rest/api/latest/result/{0}?expand=results[0:{1}].result";
        
        private const string ALL_TESTS = "/rest/api/latest/build/{0}?expand=testResults.all,stages.stage.results.result.testResults.all";
        private const string ALL_TESTS_3_0 = "/rest/api/latest/result/{0}-{1}?expand=testResults.allTests";
        private const string ALL_ARTIFACTS = "/rest/api/latest/build/{0}?expand=artifacts.artifact,stages.stage.results.result.artifacts.artifact";
        private const string ALL_ARTIFACTS_3_0 = "/rest/api/latest/result/{0}?expand=artifacts.artifact,stages.stage.results.result.artifacts.artifact";

        private const string PLAN_JOBS = "/rest/api/latest/plan/{0}?expand=stages.stage.plans";

        private const string ALL_PLANS_ACTION = "/rest/api/latest/plan?expand=plans.plan";
        private const string FAVOURITE_PLANS_ACTION = "/rest/api/latest/plan?favourite&expand=plans.plan";
        private const string BRANCHES_ACTION = "/rest/api/latest/plan/{0}?expand=branches.branch";
        private const string PLAN_DETAILS = "/rest/api/latest/plan/{0}";

        private const string RUN_BUILD_ACTION_NEW = "/rest/api/latest/queue";
        private const string RUN_BUILD_ACTION_OLD = "/api/rest/executeBuild.action";

       	private const string ADD_COMMENT_ACTION = "/api/rest/addCommentToBuildResults.action";
        private const string ADD_LABEL_ACTION = "/api/rest/addLabelToBuildResults.action";

        private const string LOGIN_ACTION = "/api/rest/login.action";
    	private const string LOGOUT_ACTION = "/api/rest/logout.action";

        public const int BUILD_NUMBER_3_0 = 2212;
        public const int BUILD_NUMBER_4_0 = 2906;
        public const int BUILD_NUMBER_5_0 = 3600;

        private int serverBuildNumber;

        public RestSession(BambooServer server) {
            this.server = server;
            serverBuildNumber = 0;
        }

        public bool LoggedIn { get; private set; }

        public RestSession login(string username, string pwd) {

#if OLDSKOOL_AUTH
            string endpoint = server.Url + LOGIN_ACTION 
                + "?username=" + HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(username), Encoding.UTF8) 
                + "&password=" + HttpUtility.UrlEncode(pwd, Encoding.UTF8) 
                + "&os_username=" + HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(username), Encoding.UTF8) 
                + "&os_password=" + HttpUtility.UrlEncode(pwd, Encoding.UTF8);
#else
            string endpoint = server.Url + LOGIN_ACTION + getBasicAuthParameter(server.Url);
#endif

            userName = username;
            password = pwd;

#if OLDSKOOL_AUTH
            using (Stream stream = getQueryResultStream(endpoint, false)) {
#else
            using (Stream stream = getQueryResultStream(endpoint, true)) {
#endif
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string exceptions = getRemoteExceptionMessages(doc);
                if (exceptions != null) {
                    throw new Exception(exceptions);
                }

                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile("/response/auth");
                XPathNodeIterator it = nav.Select(expr);
                if (it.Count == 0) {
                    throw new Exception("Server did not return any authentication token");
                }
                if (it.Count != 1) {
                    throw new Exception("Server returned unexpected number of authentication tokens (" + it.Count + ")");
                }
                it.MoveNext();
                authToken = it.Current.Value;

                getServerBuildNumber();

                LoggedIn = true;
                return this;
            }
        }

        // achtung - gobble all exceptions
        public int getServerBuildNumber() {
            string endpoint = server.Url + INFO;

            try {
                using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                    XPathDocument doc = XPathUtils.getXmlDocument(stream);

                    string code = getRestErrorStatusCode(doc);
                    if (code != null) {
                        return 0;
                    }

                    XPathNavigator nav = doc.CreateNavigator();

                    XPathExpression expr = nav.Compile("/info/buildNumber");
                    XPathNodeIterator it = nav.Select(expr);

                    if (it.MoveNext()) {
                        serverBuildNumber = it.Current.ValueAsInt;
                    }
                }
            } catch (Exception e) {
                Debug.WriteLine("RestSession.getServerBuildNumber() - exception (ignored): " + e.Message);    
            }
            return serverBuildNumber;
        }

        public void logout() {
            if (!LoggedIn) return;
            try {
                string endpoint = server.Url + LOGOUT_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);
                Stream stream = getQueryResultStream(endpoint, false);
                stream.Close();
            } catch (Exception e) {
                Debug.WriteLine("RestSession.logout() - exception (ignored): " + e.Message);
            }
            authToken = null;
            userName = null;
            password = null;
            LoggedIn = false;
        }

        public ICollection<BambooPlan> getAllPlans() {
            return getPlansFromUrl(server.Url + ALL_PLANS_ACTION);
        }

        public ICollection<BambooPlan> getFavouritePlans() {
            return getPlansFromUrl(server.Url + FAVOURITE_PLANS_ACTION);
        }

        private ICollection<BambooPlan> getPlansFromUrl(string endpoint) {
            return getPlansFromUrlWithStartIndex(endpoint, 0);
        }

        private ICollection<BambooPlan> getPlansFromUrlWithStartIndex(string endpoint, int start) {
            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint) + "&start-index=" + start, true)) {

                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/plans/plans");
                XPathNodeIterator it = nav.Select(expr);
                int totalPlansCount = 0;
                int maxResult = 0;
                int startIndex = 0;
                if (it.MoveNext()) {
                    totalPlansCount = int.Parse(XPathUtils.getAttributeSafely(it.Current, "size", "0"));
                    maxResult = int.Parse(XPathUtils.getAttributeSafely(it.Current, "max-result", "0"));
                    startIndex = int.Parse(XPathUtils.getAttributeSafely(it.Current, "start-index", "0"));
                }

                expr = nav.Compile("/plans/plans/plan");
                it = nav.Select(expr);

                List<BambooPlan> plans = new List<BambooPlan>();

                while (it.MoveNext()) {
                    string enabledValue = XPathUtils.getAttributeSafely(it.Current, "enabled", "true");
                    string key = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    string name = XPathUtils.getAttributeSafely(it.Current, "name", null);
                    bool enabled = true;
                    if (enabledValue != null) {
                        enabled = Boolean.Parse(enabledValue);
                    }
                    it.Current.MoveToFirstChild();
                    bool favourite = false;
                    do {
                        switch (it.Current.Name) {
                            case "isFavourite":
                                favourite = it.Current.Value.Equals("true");
                                break;
                        }
                    } while (it.Current.MoveToNext());
                    if (key == null || name == null) continue;
                    BambooPlan plan = new BambooPlan(key, name, enabled, favourite);
                    plans.Add(plan);
                }

                // Yes, recursion here. I hope it works as I think it should. If not, we are all doomed
                if (totalPlansCount > maxResult + startIndex) {
                    plans.AddRange(getPlansFromUrlWithStartIndex(endpoint, startIndex + maxResult));
                }

                return plans;
            }
        }

        private string getCorrectEnpointOrXPath(string oldOne, string newOne) {
            return serverBuildNumber >= BUILD_NUMBER_3_0 ? newOne : oldOne;
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans() {
            var favouritePlans = getFavouritePlans();
            return getLatestBuildsForPlanKeys(favouritePlans.Select(p => p.Key).ToList());
        }
    
        private IEnumerable<string> getBranchKeys(string planKey) {
            var my = server.ShowMyBranchesOnly ? "&my" : "";
            var endpoint = server.Url + string.Format(BRANCHES_ACTION + my, planKey);
            var result = new List<string>();
            using (var stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                var xdoc = XDocument.Load(XmlReader.Create(stream));
                foreach (var branch in xdoc.XPathSelectElements("/plan/branches/branch")) {
                    if (server.UseFavourites) {
                        if (branch.Descendants("isFavourite").First().Value.Equals("false")) continue;
                    } 
                    var a = branch.Attribute("key");
                    if (a != null) result.Add(a.Value);
                }
            }
            return result;
        }

        public ICollection<BambooBuild> getLatestBuildsForPlanKeys(ICollection<string> keys) {
            var src = new List<string>();
            if (server.ShowBranches) {
                foreach (var key in keys) {
                    src.AddRange(getBranchKeys(key));
                    src.Add(key);
                }
            } else {
                src.AddRange(keys);
            }
            var result = new List<BambooBuild>();
            foreach (var builds in
                src.Select(key => string.Format(getCorrectEnpointOrXPath(LATEST_BUILDS_FOR_PLANS_ACTION, LATEST_BUILDS_FOR_PLANS_ACTION_3_0), key))
                    .Select(buildUrl => server.Url + buildUrl)
                    .Select(endpoint => getBuildsFromUrl(endpoint, true, false, getCorrectEnpointOrXPath(BUILDS_XML_SUBTREE, BUILDS_XML_SUBTREE_3_0)))
                    .Where(builds => builds != null)) {
                result.AddRange(builds);
            }
            return result;
        }

        private static BuildArtifact getArtifact(string resultKey, XPathNavigator artifact) {
            if (!artifact.HasChildren) return null;
            XPathNavigator art = artifact.Clone();
            art.MoveToFirstChild();
            string name = null;
            string link = null;
            do {
                switch (art.Name) {
                    case "name":
                        name = art.Value;
                        break;
                    case "link":
                        link = XPathUtils.getAttributeSafely(art, "href", null);
                        break;
                }
            } while (art.MoveToNext());
            if (name == null | link == null) return null;
            BuildArtifact a = new BuildArtifact(resultKey, name, link);
            return a;
        }

        public ICollection<BuildArtifact> getArtifacts(string buildKey) {
            string buildUrl = string.Format(getCorrectEnpointOrXPath(ALL_ARTIFACTS, ALL_ARTIFACTS_3_0), buildKey);
            string endpoint = server.Url + buildUrl;

            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                List<BuildArtifact> artifacts = new List<BuildArtifact>();

                XPathExpression expr = nav.Compile("/build/artifacts/artifact");
                XPathNodeIterator it = nav.Select(expr);

                if (it.MoveNext()) {
                    artifacts.Add(getArtifact(null, it.Current));
                }

                expr = nav.Compile("/result/artifacts/artifact");
                it = nav.Select(expr);

                if (it.MoveNext()) {
                    artifacts.Add(getArtifact(null, it.Current));
                }

                expr = nav.Compile("/result/stages/stage/results/result");
                it = nav.Select(expr);

                while (it.MoveNext()) {
                    string resultKey = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    XPathNodeIterator it2 = it.Clone();
                    XPathExpression expr3 = it2.Current.Compile("artifacts/artifact");

                    XPathNodeIterator it3 = it2.Current.Select(expr3);

                    while (it3.MoveNext()) {
                        artifacts.Add(getArtifact(resultKey, it3.Current));
                    }
                }

                return artifacts;
            }
        }

        public ICollection<BambooTest> getTestResults(string buildKey) {
            return serverBuildNumber < BUILD_NUMBER_3_0 ? getTestResultsOlderThan30(buildKey) : getTestResults30(buildKey);
        }

        private ICollection<BambooTest> getTestResults30(string buildKey) {
            string number = buildKey.Substring(buildKey.LastIndexOf("-") + 1);
            string planKey = buildKey.Substring(0, buildKey.Length - number.Length - 1);
            IEnumerable<string> jobs = getPlanJobs(planKey);
            List<BambooTest> allTests = new List<BambooTest>();
            foreach (string job in jobs) {
                allTests.AddRange(getTestsForJob(job, number));
            }
            return allTests;
        }

        private IEnumerable<BambooTest> getTestsForJob(string job, string number) {
            string endpoint = server.Url + string.Format(ALL_TESTS_3_0, job, number);

            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/result/testResults/allTests/testResult");
                XPathNodeIterator it = nav.Select(expr);

                return retrieveTestsFromXPath(it);
            }

        }

        private IEnumerable<string> getPlanJobs(string planKey) {
            string endpoint = server.Url + string.Format(PLAN_JOBS, planKey);
            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                List<string> result = new List<string>();

                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile("/plan/stages/stage/plans/plan");
                XPathNodeIterator it = nav.Select(expr);
                while (it.MoveNext()) {
                    string jobKey = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    if (jobKey != null) {
                        result.Add(jobKey);
                    }
                }
                return result;
            }
        }

        public ICollection<BambooTest> getTestResultsOlderThan30(string buildKey) {

            string buildUrl = string.Format(ALL_TESTS, buildKey);
            string endpoint = server.Url + buildUrl;

            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/build/testResults/all/testResult");
                XPathNodeIterator it = nav.Select(expr);

                if (it.Count == 0) {
                    expr = nav.Compile("/result/stages/stage/results/result/testResults/all/testResult");
                    it = nav.Select(expr);
                }

                return retrieveTestsFromXPath(it);
            }
        }

        private static ICollection<BambooTest> retrieveTestsFromXPath(XPathNodeIterator it) {
            List<BambooTest> tests = new List<BambooTest>();

            while (it.MoveNext()) {
                string className = XPathUtils.getAttributeSafely(it.Current, "className", null);
                string methodName = XPathUtils.getAttributeSafely(it.Current, "methodName", null);
                string status = XPathUtils.getAttributeSafely(it.Current, "status", null);
                if (className == null || methodName == null || status == null) continue;
                BambooTest.TestResult res = BambooTest.TestResult.UNKNOWN;
                switch (status.ToLower()) {
                    case "successful":
                        res = BambooTest.TestResult.SUCCESSFUL;
                        break;
                    case "failed":
                        res = BambooTest.TestResult.FAILED;
                        break;
                }
                BambooTest test = new BambooTest(className, methodName, res);
                tests.Add(test);
            }

            return tests;
        }

        public BambooBuild getBuildByKey(string buildKey) {
            List<BambooBuild> result = new List<BambooBuild>();
            string buildUrl = string.Format(getCorrectEnpointOrXPath(BUILD_BY_KEY_ACTION, BUILD_BY_KEY_ACTION_3_0), buildKey);
            string endpoint = server.Url + buildUrl;
            ICollection<BambooBuild> builds = getBuildsFromUrl(endpoint, false, false, "");
            if (builds != null) {
                result.AddRange(builds);
            }
            return result.Count == 0 ? null : result[0];
        }

        public ICollection<BambooBuild> getLastNBuildsForPlan(string planKey, int howMany) {
            if (howMany <= 0) {
                throw new ArgumentException("\"howMany\" parameter must be greater than 0");
            }
            string endpoint = server.Url + string.Format(getCorrectEnpointOrXPath(LAST_N_BUILDS_ROM_PLAN, LAST_N_BUILDS_ROM_PLAN_3_0), planKey, howMany - 1);
            return getBuildsFromUrl(endpoint, false, false, getCorrectEnpointOrXPath(BUILDS_XML_SUBTREE, BUILDS_XML_SUBTREE_3_0));
        }

        private ICollection<BambooBuild> getBuildsFromUrl(string endpoint, bool getPlanState, bool withRecursion, string prefix) {
            return getBuildsFromUrlWithStartIndex(endpoint, 0, getPlanState, withRecursion, prefix);
        }

        private ICollection<BambooBuild> getBuildsFromUrlWithStartIndex(string endpoint, int start, bool getPlanState, bool withRecursion, string prefix) {

            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint) + (withRecursion ? "&start-index=" + start : ""), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr;
                XPathNodeIterator it;

                int totalBuildsCount = 0;
                int maxResult = 0;
                int startIndex = 0;

                if (!string.IsNullOrEmpty(prefix)) {
                    expr = nav.Compile(prefix);
                    it = nav.Select(expr);
                    if (it.MoveNext()) {
                        totalBuildsCount = int.Parse(XPathUtils.getAttributeSafely(it.Current, "size", "0"));
                        maxResult = int.Parse(XPathUtils.getAttributeSafely(it.Current, "max-result", "0"));
                        startIndex = int.Parse(XPathUtils.getAttributeSafely(it.Current, "start-index", "0"));
                    }
                }

                expr = nav.Compile(prefix + "/build");
                it = nav.Select(expr);

                if (it.Count == 0) {
                    expr = nav.Compile(prefix + "/result");
                    it = nav.Select(expr);
                }

                List<BambooBuild> builds = new List<BambooBuild>();

                while (it.MoveNext()) {
                    int number = int.Parse(XPathUtils.getAttributeSafely(it.Current, "number", "-1"));
                    string key = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    string state = XPathUtils.getAttributeSafely(it.Current, "state", null);
                    it.Current.MoveToFirstChild();
                    string buildRelativeTime = null;
                    string buildDurationDescription = null;
                    string masterKey = null;
                    int successfulTestCount = 0;
                    int failedTestCount = 0;
                    string projectName = null;
                    string buildReason = null;
                    List<BambooBuild.RelatedIssue> relatedIssues = new List<BambooBuild.RelatedIssue>();

                    do {
                        switch (it.Current.Name) {
                            case "master":
                                masterKey = XPathUtils.getAttributeSafely(it.Current, "key", null);
                                break;
                            case "projectName":
                                projectName = it.Current.Value;
                                break;
                            case "buildRelativeTime":
                                buildRelativeTime = it.Current.Value;
                                break;
                            case "buildDurationDescription":
                                buildDurationDescription = it.Current.Value;
                                break;
                            case "successfulTestCount":
                                successfulTestCount = int.Parse(it.Current.Value);
                                break;
                            case "failedTestCount":
                                failedTestCount = int.Parse(it.Current.Value);
                                break;
                            case "buildReason":
                                buildReason = it.Current.Value;
                                break;
                            case "jiraIssues":
                                if (it.Current.HasChildren) {
                                    var chnav = it.Clone().Current;
                                    if (chnav != null) {
                                        chnav.MoveToFirstChild();
                                        do {
                                            var issue = getRelatedIssue(chnav);
                                            if (issue != null) relatedIssues.Add(issue);
                                        } while (chnav.MoveToNext());
                                    }
                                }
                                break;
                        }
                    } while (it.Current.MoveToNext());
                    if (key == null) continue;

                    var planState = getPlanState ? getPlanStateForBuild(key) : BambooBuild.PlanState.IDLE;
                    var build = new BambooBuild(server,
                        key, projectName, masterKey, BambooBuild.stringToResult(state), number, buildRelativeTime,
                        buildDurationDescription, successfulTestCount, failedTestCount, buildReason, planState, relatedIssues);
                    builds.Add(build);
                }

                // Yes, recursion here. I hope it works as I think it should. If not, we are all doomed
                if (withRecursion && totalBuildsCount > maxResult + startIndex) {
                    builds.AddRange(getBuildsFromUrlWithStartIndex(endpoint, startIndex + maxResult, getPlanState, true, getCorrectEnpointOrXPath(BUILDS_XML_SUBTREE, BUILDS_XML_SUBTREE_3_0)));
                }

                return builds;
            }
        }

        private static BambooBuild.RelatedIssue getRelatedIssue(XPathNavigator issue) {
            string key = XPathUtils.getAttributeSafely(issue, "key", null);
            if (key == null || !issue.HasChildren) return null;
            XPathNavigator art = issue.Clone();
            art.MoveToFirstChild();
            string url = null;
            do {
                switch (art.Name) {
                    case "url":
                        url = XPathUtils.getAttributeSafely(art, "href", null);
                        break;
                }
            } while (art.MoveToNext());
            if (url == null) return null;
            BambooBuild.RelatedIssue i = new BambooBuild.RelatedIssue(key, url);
            return i;
        }

        private BambooBuild.PlanState getPlanStateForBuild(string buildKey) {
            BambooBuild.PlanState state;

            string endpoint = server.Url + string.Format(PLAN_DETAILS, BambooBuildUtils.getPlanKey(buildKey));
            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/plan/isInBuildQueue");
                XPathNodeIterator it = nav.Select(expr);
                string inQueue = "false";
                string isBuilding = "false";
                if (it.Count > 0) {
                    it.MoveNext();
                    inQueue = it.Current.Value;
                }
                expr = nav.Compile("/plan/isBuilding");
                it = nav.Select(expr);
                if (it.Count > 0) {
                    it.MoveNext();
                    isBuilding = it.Current.Value;
                }
                if (inQueue.Equals("true")) {
                    state = BambooBuild.PlanState.IN_QUEUE;
                } else if (isBuilding.Equals("true")) {
                    state = BambooBuild.PlanState.BUILDING;
                } else {
                    state = BambooBuild.PlanState.IDLE;
                }
            }
            return state;
        }

        public void runBuild(string planKey) {
            if (serverBuildNumber >= BUILD_NUMBER_4_0) {
                string endpoint = server.Url + RUN_BUILD_ACTION_NEW + "/" + planKey;

                Stream stream = postWithNullBody(endpoint + getBasicAuthParameter(endpoint), true);

                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }
            } else {
                string endpoint = server.Url + RUN_BUILD_ACTION_OLD 
                    + "?buildKey=" + planKey 
                    + "&auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);

                using (Stream stream = getQueryResultStream(endpoint, false)) {
                    XPathDocument doc = XPathUtils.getXmlDocument(stream);

                    string code = getRemoteExceptionMessages(doc);
                    if (code != null) {
                        throw new Exception(code);
                    }
                }
            }
        }

        public void addComment(string planKey, int buildNumber, string comment) {
            string endpoint = server.Url + ADD_COMMENT_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8)
                    + "&buildKey=" + HttpUtility.UrlEncode(planKey) + "&buildNumber=" + buildNumber + "&content="
                    + HttpUtility.UrlEncode(comment);

            using (Stream stream = getQueryResultStream(endpoint, false)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRemoteExceptionMessages(doc);
                if (code != null) {
                    throw new Exception(code);
                }
            }
        }

        public void addLabel(string planKey, int buildNumber, string label) {
            string endpoint = server.Url + ADD_LABEL_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8)
                    + "&buildKey=" + HttpUtility.UrlEncode(planKey) + "&buildNumber=" + buildNumber + "&label="
                    + HttpUtility.UrlEncode(label);

            using (Stream stream = getQueryResultStream(endpoint, false)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRemoteExceptionMessages(doc);
                if (code != null) {
                    throw new Exception(code);
                }
            }
        }

        public string getBuildLog(string logUrl) {
            string endpoint = logUrl;
            endpoint = endpoint + getBasicAuthParameter(endpoint);
            using (Stream stream = getQueryResultStream(endpoint, true)) {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

//        public string getBuildLog(BambooBuild build) {
//            string endpoint = server.Url + "/download/" + BambooBuildUtils.getPlanKey(build) + "/build_logs/" + build.Key + ".log";
//            endpoint = endpoint + getBasicAuthParameter(endpoint);
//            using (Stream stream = getQueryResultStream(endpoint, true)) {
//                StreamReader reader = new StreamReader(stream);
//                return reader.ReadToEnd();
//            }
//        }

        private Stream getQueryResultStream(string endpoint, bool setBasicAuth) {
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;
            req.UserAgent = Constants.USER_AGENT;
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.ContentType = "application/xml";
            req.Method = "GET";
            // required for PLVS-83
            req.Accept = "application/xml";

            if (setBasicAuth) {
                setBasicAuthHeader(endpoint, req);
            } else {
                restoreSessionContext(req);
            }
            var resp = (HttpWebResponse)req.GetResponse();
            if (!setBasicAuth) {
                saveSessionContext(resp);
            }
            return resp.GetResponseStream();
        }

        private Stream postWithNullBody(string endpoint, bool setBasicAuth) {
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.Method = "POST";
            req.ContentType = "application/xml";
            // required for PLVS-83
            req.Accept = "application/xml";
            req.ContentLength = 0;
            req.UserAgent = Constants.USER_AGENT;

            const string postData = "";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            if (setBasicAuth) {
                setBasicAuthHeader(endpoint, req);
            } else {
                restoreSessionContext(req);
            }

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var resp = (HttpWebResponse)req.GetResponse();
            if (!setBasicAuth) {
                saveSessionContext(resp);
            }
            return resp.GetResponseStream();
        }

        private static string getBasicAuthParameter(string url) {
            return url.Contains("?") ? "&os_authType=basic" : "?os_authType=basic";
        }

        private void setBasicAuthHeader(string url, WebRequest req) {
            if (userName == null || password == null) {
                return;
            }
#if true
            req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(url, userName, password);
#else
            string authInfo = CredentialUtils.getUserNameWithoutDomain(userName) + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
#endif
        }

       	private static string getRemoteExceptionMessages(IXPathNavigable doc) {
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/errors/error");
            XPathNodeIterator it = nav.Select(expr);

       	    return messagesFromNode(it);
	    }

        private static string getRestErrorStatusCode(IXPathNavigable doc) {
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/status/status-code");
            XPathNodeIterator it = nav.Select(expr);
            string code = messagesFromNode(it);
            expr = nav.Compile("/status/message");
            it = nav.Select(expr);
            string message = messagesFromNode(it);
            if (code == null || message == null) {
                return null;
            }
            return "Status code: " + code + ", Message: " + message;
        }

        private static string messagesFromNode(XPathNodeIterator it) {
            if (it.Count <= 0) {
                return null;
            }
            StringBuilder msg = new StringBuilder();
            while (it.MoveNext()) {
                msg.Append(it.Current.Value);
                msg.Append("\n");
            }
            return msg.ToString().Trim(new[] { '\n' });
        }

        private void saveSessionContext(WebResponse resp) {
            if (cookie != null) {
                return;
            }

            if (resp.Headers["Set-Cookie"] == null) {
                return;
            }

            cookie = resp.Headers["Set-Cookie"];
        }

        private void restoreSessionContext(WebRequest req) {
            if (cookie != null) {
                req.Headers["Cookie"] = cookie;
            }
        }
    }
}
