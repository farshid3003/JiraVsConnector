using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.api.jira {
    internal class JiraServerCache {
        private static readonly JiraServerCache INSTANCE = new JiraServerCache();

        public static JiraServerCache Instance {
            get { return INSTANCE; }
        }

        private readonly SortedDictionary<Guid, SortedDictionary<string, JiraProject>> projectCache =
            new SortedDictionary<Guid, SortedDictionary<string, JiraProject>>();

        private readonly SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>> issueTypeCache =
            new SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>>();

        private readonly SortedDictionary<Guid, List<JiraNamedEntity>> priorityCache =
            new SortedDictionary<Guid, List<JiraNamedEntity>>();

        private readonly SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>> statusCache =
            new SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>>();

        private readonly SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>> resolutionCache =
            new SortedDictionary<Guid, SortedDictionary<int, JiraNamedEntity>>();

        private readonly SortedDictionary<Guid, SortedDictionary<int, RapidBoard>> ghBoardsCache = 
            new SortedDictionary<Guid, SortedDictionary<int, RapidBoard>>();

        private readonly Dictionary<Guid, JiraUserCache> userCaches = new Dictionary<Guid, JiraUserCache>();

        public SortedDictionary<string, JiraProject> getProjects(JiraServer server) {
            lock (projectCache) {
                if (projectCache.ContainsKey(server.GUID)) {
                    return projectCache[server.GUID];
                }
            }
            return new SortedDictionary<string, JiraProject>();
        }

        public void addRapidBoard(JiraServer server, RapidBoard rapidBoard) {
            lock (ghBoardsCache) {
                if (!ghBoardsCache.ContainsKey(server.GUID)) {
                    ghBoardsCache[server.GUID] = new SortedDictionary<int, RapidBoard>();
                }
                ghBoardsCache[server.GUID][rapidBoard.Id] = rapidBoard;
            }
        }

        public void clearGhBoards() {
            lock (ghBoardsCache) {
                ghBoardsCache.Clear();
            }
        }

        public SortedDictionary<int, RapidBoard> getGhBoards(JiraServer server) {
            lock (ghBoardsCache) {
                if (ghBoardsCache.ContainsKey(server.GUID)) {
                    return ghBoardsCache[server.GUID];
                }
            }
            return new SortedDictionary<int, RapidBoard>();
        }

        public void addProject(JiraServer server, JiraProject project) {
            lock (projectCache) {
                if (!projectCache.ContainsKey(server.GUID)) {
                    projectCache[server.GUID] = new SortedDictionary<string, JiraProject>();
                }
                projectCache[server.GUID][project.Key] = project;
            }
        }
        
        public void clearProjects() {
            lock (projectCache) {
                projectCache.Clear();
            }
        }

        public SortedDictionary<int, JiraNamedEntity> getIssueTypes(JiraServer server) {
            lock (issueTypeCache) {
                if (issueTypeCache.ContainsKey(server.GUID)) {
                    return issueTypeCache[server.GUID];
                }
            }
            return new SortedDictionary<int, JiraNamedEntity>();
        }

        public void addIssueType(JiraServer server, JiraNamedEntity issueType) {
            lock (issueTypeCache) {
                if (!issueTypeCache.ContainsKey(server.GUID)) {
                    issueTypeCache[server.GUID] = new SortedDictionary<int, JiraNamedEntity>();
                }
                issueTypeCache[server.GUID][issueType.Id] = issueType;
            }
        }

        public void clearIssueTypes() {
            lock (issueTypeCache) {
                issueTypeCache.Clear();
            }
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            lock (priorityCache) {
                if (priorityCache.ContainsKey(server.GUID)) {
                    return priorityCache[server.GUID];
                }
            }
            return new List<JiraNamedEntity>();
        }

        public void addPriority(JiraServer server, JiraNamedEntity priority) {
            lock (priorityCache) {
                if (!priorityCache.ContainsKey(server.GUID)) {
                    priorityCache[server.GUID] = new List<JiraNamedEntity>();
                }
                priorityCache[server.GUID].Add(priority);
            }
        }

        public void clearPriorities() {
            lock (priorityCache) {
                priorityCache.Clear();
            }
        }

        public SortedDictionary<int, JiraNamedEntity> getStatues(JiraServer server) {
            lock (statusCache) {
                if (statusCache.ContainsKey(server.GUID)) {
                    return statusCache[server.GUID];
                }
            }
            return new SortedDictionary<int, JiraNamedEntity>();
        }

        public void addStatus(JiraServer server, JiraNamedEntity status) {
            lock (statusCache) {
                if (!statusCache.ContainsKey(server.GUID)) {
                    statusCache[server.GUID] = new SortedDictionary<int, JiraNamedEntity>();
                }
                statusCache[server.GUID][status.Id] = status;
            }
        }

        public void clearStatuses() {
            lock (statusCache) {
                statusCache.Clear();
            }
        }

        public SortedDictionary<int, JiraNamedEntity> getResolutions(JiraServer server) {
            lock (resolutionCache) {
                if (resolutionCache.ContainsKey(server.GUID)) {
                    return resolutionCache[server.GUID];
                }
            }
            return new SortedDictionary<int, JiraNamedEntity>();
        }

        public void addResolution(JiraServer server, JiraNamedEntity resolution) {
            lock (resolutionCache) {
                if (!resolutionCache.ContainsKey(server.GUID)) {
                    resolutionCache[server.GUID] = new SortedDictionary<int, JiraNamedEntity>();
                }
                resolutionCache[server.GUID][resolution.Id] = resolution;
            }
        }

        public void clearResolutions() {
            lock(resolutionCache) {
                resolutionCache.Clear();
            }
        }

        public JiraUserCache getUsers(JiraServer server) {
            lock(userCaches) {
                if (!userCaches.ContainsKey(server.GUID)) {
                    userCaches[server.GUID] = new JiraUserCache();
                }
                return userCaches[server.GUID];
            }
        }
    }
}