using System.Collections.Generic;
using Atlassian.plvs.api.bamboo.rest;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServerFacade : ServerFacade {
        private static readonly BambooServerFacade INSTANCE = new BambooServerFacade();

        public static BambooServerFacade Instance {
            get { return INSTANCE; }
        }

        private BambooServerFacade() {
            PlvsUtils.installSslCertificateHandler();
        }

        private static RestSession createSessionAndLogin(BambooServer server) {
            RestSession s = new RestSession(server);
            s.login(server.UserName, server.Password);
            return s;
        }

        private delegate T Wrapped<T>();
        private static T wrapExceptions<T>(RestSession session, Wrapped<T> wrapped) {
            T result = wrapped();
            session.logout();
            return result;
        }

        private delegate void WrappedVoid();
        private static void wrapExceptionsVoid(RestSession session, WrappedVoid wrapped) {
            wrapped();
            session.logout();
        }

        public void login(BambooServer server) {
            new RestSession(server).login(server.UserName, server.Password);
        }

        public int getServerBuildNumber(BambooServer server) {
            var session = createSessionAndLogin(server);
            return wrapExceptions<int>(session, () => session.getServerBuildNumber());
        }

        public ICollection<BambooPlan> getPlanList(BambooServer server) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getAllPlans());
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans(BambooServer server) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getLatestBuildsForFavouritePlans());
        }

        public ICollection<BambooBuild> getLatestBuildsForPlanKeys(BambooServer server, ICollection<string> keys) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getLatestBuildsForPlanKeys(keys));
        }

        public void runBuild(BambooServer server, string planKey) {
            RestSession session = createSessionAndLogin(server);
            wrapExceptionsVoid(session, () => session.runBuild(planKey));
        }

        public void addComment(BambooServer server, string planKey, int buildNumber, string comment) {
            RestSession session = createSessionAndLogin(server);
            wrapExceptionsVoid(session, () => session.addComment(planKey, buildNumber, comment));
        }

        public void addLabel(BambooServer server, string planKey, int buildNumber, string label) {
            RestSession session = createSessionAndLogin(server);
            wrapExceptionsVoid(session, () => session.addLabel(planKey, buildNumber, label));
        }

        public BambooBuild getBuildByKey(BambooServer server, string buildKey) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getBuildByKey(buildKey));
        }

        public ICollection<BambooBuild> getLastNBuildsForPlan(BambooServer server, string planKey, int howMany) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getLastNBuildsForPlan(planKey, howMany));
        }

//        public string getBuildLog(BambooBuild build) {
//            RestSession session = createSessionAndLogin(build.Server);
//            return wrapExceptions(session, () => session.getBuildLog(build));
//        }

        public string getBuildLog(BambooServer server, string logUrl) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getBuildLog(logUrl));
        }

        public ICollection<BambooTest> getTestResults(BambooBuild build) {
            RestSession session = createSessionAndLogin(build.Server);
            return wrapExceptions(session, () => session.getTestResults(build.Key));
        }

        public ICollection<BuildArtifact> getArtifacts(BambooBuild build) {
            RestSession session = createSessionAndLogin(build.Server);
            return wrapExceptions(session, () => session.getArtifacts(build.Key));
        }

        public void dropAllSessions() {}
    }
}
