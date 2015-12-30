using System.Collections.Generic;

namespace Atlassian.plvs.api.jira {
    public class JiraUserCache {

        private readonly JiraUser nullUser = new JiraUser(JiraUser.UNKNOWN_ID, "Unknown");
        protected JiraUser NullUser { get { return nullUser; } }

        private readonly Dictionary<string, JiraUser> users = new Dictionary<string, JiraUser>();

        public bool haveUser(string userId) {
            lock(this) {
                return users.ContainsKey(userId);
            }
        }

        public JiraUser getUser(string userId) {
            lock (this) {
                if (userId == null) {
                    return NullUser;
                }
                if (!users.ContainsKey(userId)) {
                    users[userId] = new JiraUser(userId, null);
                }
                return users[userId];
            }
        }

        public ICollection<JiraUser> getAllUsers() {
            lock(this) {
                // sortr values alphabetically by display name and skip the "unknown" user
                SortedDictionary<string, JiraUser> result = new SortedDictionary<string, JiraUser>();
                foreach (JiraUser u in users.Values) {
                    if (u.Id.Equals(JiraUser.UNKNOWN_ID)) continue;
                    result[u.ToString()] = u;
                }
                return result.Values;
            }
        }

        public void putUser(JiraUser user) {
            lock(this) {
                users[user.Id] = user;
            }
        }
    }
}