using System;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.util.bamboo {
    public static class BambooBuildUtils {
        
        public static string getPlanKey(BambooBuild build) {
            return getPlanKey(build.Key);
        }

        public static string getPlanKey(string buildKey) {
            int idx = buildKey.LastIndexOf("-");
            if (idx < 0) {
                throw new ArgumentException("Build key does not seem to contain plan key: " + buildKey);
            }
            return buildKey.Substring(0, idx);
        }

    }
}
