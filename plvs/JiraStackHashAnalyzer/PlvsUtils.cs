using System.Collections.Generic;

namespace JiraStackHashAnalyzer {
    internal static class PlvsUtils {

        public static bool compareLists<T>(IList<T> lhs, IList<T> rhs) {
            if (lhs == null && rhs == null) return true;
            if (lhs == null || rhs == null) return false;

            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; ++i) {
                if (!lhs[i].Equals(rhs[i])) return false;
            }
            return true;
        }
    }
}
