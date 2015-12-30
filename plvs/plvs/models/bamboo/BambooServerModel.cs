using System;
using System.Collections.Generic;
using System.Text;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.store;
using Microsoft.Win32;

namespace Atlassian.plvs.models.bamboo {
    public class BambooServerModel : AbstractServerModel<BambooServer> {
        private static readonly BambooServer ServerForType = new BambooServer(null, null, null, null, false);

        private BambooServerModel() { }

        private static readonly BambooServerModel INSTANCE = new BambooServerModel();
        
        private const string USE_FAVOURITES = "UseFavourites";
        private const string SHOW_BRANCHES = "ShowBranches";
        private const string MY_BRANCHES_ONLY = "MyBranchesOnly";
        private const string PLAN_KEYS = "PlanKeys";

        public static BambooServerModel Instance { get { return INSTANCE; } }

        protected override ParameterStoreManager.StoreType StoreType { get { return ParameterStoreManager.StoreType.BAMBOO_SERVERS; } }
        protected override Guid SupportedServerType { get { return ServerForType.Type; } }

        protected override void loadCustomServerParameters(ParameterStore store, BambooServer server) {
            server.UseFavourites = store.loadParameter(USE_FAVOURITES + "_" + server.GUID, 1) > 0;
            server.ShowBranches = store.loadParameter(SHOW_BRANCHES + "_" + server.GUID, 1) > 0;
            server.ShowMyBranchesOnly = store.loadParameter(MY_BRANCHES_ONLY + "_" + server.GUID, 0) > 0;
            string keyString = store.loadParameter(PLAN_KEYS + "_" + server.GUID, "");
            setPlanKeysFromString(server, keyString);
        }

        protected override void loadCustomServerParameters(RegistryKey key, BambooServer server) {
            server.UseFavourites = (int) key.GetValue(USE_FAVOURITES, 1) > 0;
            server.ShowBranches = (int)key.GetValue(SHOW_BRANCHES, 1) > 0;
            server.ShowMyBranchesOnly = (int)key.GetValue(MY_BRANCHES_ONLY, 0) > 0;
            string keyString = (string)key.GetValue(PLAN_KEYS, "");
            setPlanKeysFromString(server, keyString);
        }

        private static void setPlanKeysFromString(BambooServer server, string keyString) {
            if (keyString.Trim().Length <= 0) return;
            string[] keys = keyString.Split(new[] { ' ' });
            server.PlanKeys = new List<string>(keys.Length);
            server.PlanKeys.AddRange(keys);
        }

        protected override void saveCustomServerParameters(ParameterStore store, BambooServer server) {
            store.storeParameter(USE_FAVOURITES + "_" + server.GUID, server.UseFavourites ? 1 : 0);
            store.storeParameter(SHOW_BRANCHES + "_" + server.GUID, server.ShowBranches ? 1 : 0);
            store.storeParameter(MY_BRANCHES_ONLY + "_" + server.GUID, server.ShowMyBranchesOnly ? 1 : 0);

            string planKeys = getPlanKeysStringFromPlans(server);
            store.storeParameter(PLAN_KEYS + "_" + server.GUID, planKeys.Trim());
        }

        protected override void saveCustomServerParameters(RegistryKey key, BambooServer server) {
            key.SetValue(USE_FAVOURITES, server.UseFavourites ? 1 : 0);
            key.SetValue(SHOW_BRANCHES, server.ShowBranches ? 1 : 0);
            key.SetValue(MY_BRANCHES_ONLY, server.ShowMyBranchesOnly ? 1 : 0);

            string planKeys = getPlanKeysStringFromPlans(server);
            key.SetValue(PLAN_KEYS, planKeys.Trim());
        }

        private static string getPlanKeysStringFromPlans(BambooServer server) {
            StringBuilder sb = new StringBuilder();
            if (server.PlanKeys != null) {
                foreach (var key in server.PlanKeys) {
                    sb.Append(key).Append(" ");
                }
            }
            return sb.ToString();
        }

        protected override BambooServer createServer(Guid guid, string name, string url, string userName, string password, bool noProxy, bool enabled) {
            return new BambooServer(guid, name, url, userName, password, noProxy, enabled);
        }
    }
}