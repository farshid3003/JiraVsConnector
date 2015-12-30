using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.store;
using Microsoft.Win32;

namespace Atlassian.plvs.models.jira {
    public class JiraServerModel : AbstractServerModel<JiraServer> {

        private const string USE_OLDSKOOL_AUTH = "UseOldSkoolAuth";

        private static readonly JiraServer ServerForType = new JiraServer(null, null, null, null, false, false);

        private JiraServerModel() { }

        private static readonly JiraServerModel INSTANCE = new JiraServerModel();

        public static JiraServerModel Instance { get { return INSTANCE; } }

        protected override ParameterStoreManager.StoreType StoreType { get { return ParameterStoreManager.StoreType.JIRA_SERVERS; } }
        protected override Guid SupportedServerType { get { return ServerForType.Type; } }

        protected override void loadCustomServerParameters(ParameterStore store, JiraServer server) {
            server.OldSkoolAuth = store.loadParameter(USE_OLDSKOOL_AUTH + "_" + server.GUID, 0) > 0;
        }

        protected override void saveCustomServerParameters(ParameterStore store, JiraServer server) {
            store.storeParameter(USE_OLDSKOOL_AUTH + "_" + server.GUID, server.OldSkoolAuth ? 1 : 0);
        }

        protected override void loadCustomServerParameters(RegistryKey key, JiraServer server) {
            server.OldSkoolAuth = (int)key.GetValue(USE_OLDSKOOL_AUTH, 0) > 0;
        }

        protected override void saveCustomServerParameters(RegistryKey key, JiraServer server) {
            key.SetValue(USE_OLDSKOOL_AUTH, server.OldSkoolAuth ? 1 : 0);
        }

        protected override JiraServer createServer(Guid guid, string name, string url, string userName, string password, 
            bool noProxy, bool enabled) {
            return new JiraServer(guid, name, url, userName, password, noProxy, false, enabled);
        }
    }
}