using System;
using System.Collections.Generic;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.models.bamboo {
    public class BambooBuildListModelImpl : BambooBuildListModel {
        private static readonly BambooBuildListModel INSTANCE = new BambooBuildListModelImpl();

        public static BambooBuildListModel Instance {
            get { return INSTANCE; }
        }

        private readonly Dictionary<string, BambooBuild> builds = new Dictionary<string, BambooBuild>();

        public ICollection<BambooBuild> Builds {
            get { lock(builds) { return new List<BambooBuild>(builds.Values); } }
        }

        public BambooBuild getBuild(string key, BambooServer server) {
            lock (builds) {
                return builds.ContainsKey(server.GUID + key) ? builds[server.GUID + key] : null;
            }
        }

        public event EventHandler<EventArgs> ModelChanged;
        
        public void removeAllListeners() {
            ModelChanged = null;
        }

        public void clear(bool notify) {
            lock (builds) {
                builds.Clear();
            }
            if (notify) {
                notifyListenersOfModelChange();
            }
        }

        private void notifyListenersOfModelChange() {
            if (ModelChanged != null) {
                ModelChanged(this, new EventArgs());
            }
        }

        public void addBuilds(ICollection<BambooBuild> newBuilds) {
            lock (builds) {
                foreach (var build in newBuilds) {
                    builds[build.Server.GUID + build.Key] = build;
                }
            }
            notifyListenersOfModelChange();
        }
    }
}
