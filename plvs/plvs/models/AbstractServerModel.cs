using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Atlassian.plvs.api;
using Atlassian.plvs.store;
using Atlassian.plvs.util;
using Microsoft.Win32;

namespace Atlassian.plvs.models {
    public abstract class AbstractServerModel<T> where T : Server {

        private const string DEFAULT_SERVER_GUID = "defaultServerGuid_";
        private const string SERVER_COUNT = "serverCount";
        private const string SERVER_GUID = "serverGuid_";
        private const string SERVER_NAME = "serverName";
        private const string SERVER_URL = "serverUrl";
        private const string SERVER_TYPE = "serverType_";
        private const string SERVER_ENABLED = "serverEnabled_";
        private const string SERVER_DONT_USE_PROXY = "serverNoProxy";

        public class ModelException : Exception {
            public ModelException(string message) : base(message) {}
        }

        private readonly SortedDictionary<Guid, T> serverMap = new SortedDictionary<Guid, T>();

        protected abstract ParameterStoreManager.StoreType StoreType { get; }
        protected abstract Guid SupportedServerType { get; }
        protected abstract void loadCustomServerParameters(ParameterStore store, T server);
        protected abstract void saveCustomServerParameters(ParameterStore store, T server);
        protected abstract void loadCustomServerParameters(RegistryKey key, T server);
        protected abstract void saveCustomServerParameters(RegistryKey key, T server);

        protected abstract T createServer(Guid guid, string name, string url, string userName, string password, bool noProxy, bool enabled);

        private Guid defaultServerGuid;

        public Guid DefaultServerGuid { get { return defaultServerGuid; } }

        public event EventHandler<EventArgs> DefaultServerChanged;

        public T DefaultServer {
            get {
                lock(serverMap) {
                    return serverMap.ContainsKey(defaultServerGuid) ? serverMap[defaultServerGuid] : null;
                }
            }
            set {
                lock(serverMap) {
                    if (!serverMap.ContainsKey(value.GUID)) {
                        return;
                    }
                    defaultServerGuid = value.GUID;
                    save();
                    if (DefaultServerChanged != null) {
                        DefaultServerChanged(this, new EventArgs());
                    }
                }
            }
        }

        public ICollection<T> getAllServers() {
            lock (serverMap) {
                // return a clone. Otherwise NREs and IOEs happen at exit
                List<T> result = new List<T>();
                result.AddRange(serverMap.Values);
                return result;
            }
        }

        public ICollection<T> getAllEnabledServers() {
            ICollection<T> servers = getAllServers();
            return servers.Where(server => server.Enabled).ToList();
        }

        public void load() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(StoreType);
            
            string defaultGuidString = store.loadParameter(DEFAULT_SERVER_GUID, null);
            defaultServerGuid = defaultGuidString != null ? new Guid(defaultGuidString) : Guid.Empty;

            int count = store.loadParameter(SERVER_COUNT, 0);
            try {
                if (count > 0) {
                    for (int i = 1; i <= count; ++i) {
                        string guidStr = store.loadParameter(SERVER_GUID + i, null);
                        Guid guid = new Guid(guidStr);
                        string type = store.loadParameter(SERVER_TYPE + guidStr, null);
                        if (String.IsNullOrEmpty(type) || !SupportedServerType.Equals(new Guid(type))) {
                            // hmm. Is it right? Maybe throw an exception?
                            continue;
                        }

                        string sName = store.loadParameter(SERVER_NAME + "_" + guidStr, null);
                        string url = store.loadParameter(SERVER_URL + "_" + guidStr, null);

                        T server = createServer(
                            guid, sName, url, null, null, 
                            store.loadParameter(SERVER_DONT_USE_PROXY + "_" + guidStr, 0) > 0, 
                            store.loadParameter(SERVER_ENABLED + "_" + guidStr, 1) > 0);
                        
                        server.IsShared = false;

                        server.UserName = CredentialsVault.Instance.getUserName(server);
                        server.Password = CredentialsVault.Instance.getPassword(server);

                        loadCustomServerParameters(store, server);

                        addServer(server, true);
                    }
                }

                RegistryKey key = getSharedServersRegistryKey();
                if (key == null) return;
                using (key) {
                    foreach (var subKeyName in key.GetSubKeyNames()) {
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName)) {
                            if (subKey == null) continue;
                            T server = createServer(
                                new Guid(subKeyName),
                                (string)subKey.GetValue(SERVER_NAME),
                                (string)subKey.GetValue(SERVER_URL),
                                null, null,
                                (int)subKey.GetValue(SERVER_DONT_USE_PROXY, 0) > 0,
                                // enabled/disabled state is always per solution, even for shared servers
                                store.loadParameter(SERVER_ENABLED + subKeyName, 1) > 0);

                            server.UserName = CredentialsVault.Instance.getUserName(server);
                            server.Password = CredentialsVault.Instance.getPassword(server);

                            loadCustomServerParameters(subKey, server);

                            server.IsShared = true;

                            addServer(server, true);
                        }
                    }
                }
            } catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        public void save() {
            try {
                ParameterStore store = ParameterStoreManager.Instance.getStoreFor(StoreType);

                store.storeParameter(DEFAULT_SERVER_GUID, defaultServerGuid.ToString());
                RegistryKey sharedServersRegistryKey = getSharedServersRegistryKey();

                using (sharedServersRegistryKey) {
                    int i = 0;
                    foreach (T s in getAllServers()) {
                        if (!s.IsShared) {
                            ++i;
                            string var = SERVER_GUID + i;
                            store.storeParameter(var, s.GUID.ToString());
                            var = SERVER_NAME + "_" + s.GUID;
                            store.storeParameter(var, s.Name);
                            var = SERVER_URL + "_" + s.GUID;
                            store.storeParameter(var, s.Url);
                            var = SERVER_TYPE + s.GUID;
                            store.storeParameter(var, s.Type.ToString());
                            var = SERVER_DONT_USE_PROXY + "_" + s.GUID;
                            store.storeParameter(var, s.NoProxy ? 1 : 0);
                            var = SERVER_ENABLED + "_" + s.GUID;
                            store.storeParameter(var, s.Enabled ? 1 : 0);

                            saveCustomServerParameters(store, s);
                            
                            // just in case
                            if (sharedServersRegistryKey != null) {
                                try {
                                    sharedServersRegistryKey.DeleteSubKey(s.GUID.ToString());
// ReSharper disable EmptyGeneralCatchClause
                                } catch {
// ReSharper restore EmptyGeneralCatchClause
                                }
                            }
                        } else {
                            if (sharedServersRegistryKey == null) continue;
                            using (RegistryKey subKey = sharedServersRegistryKey.CreateSubKey(s.GUID.ToString())) {
                                if (subKey == null) continue;
                                
                                subKey.SetValue(SERVER_NAME, s.Name);
                                subKey.SetValue(SERVER_URL, s.Url);
                                subKey.SetValue(SERVER_DONT_USE_PROXY, s.NoProxy ? 1 : 0);
                                // enabled/disabled state is always per solution, even for shared servers
                                store.storeParameter(SERVER_ENABLED + s.GUID, s.Enabled ? 1 : 0);

                                saveCustomServerParameters(subKey, s);
                            }
                        }
                        CredentialsVault.Instance.saveCredentials(s);
                    }

                    store.storeParameter(SERVER_COUNT, i);
                }


            } catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        public void setShared(T server, bool shared) {
            lock(serverMap) {
                if (serverMap.ContainsKey(server.GUID)) {
                    serverMap[server.GUID].IsShared = shared;
                }
            }
        }

        public void addServer(T server) {
            addServer(server, false);
        }

        public void addServer(T server, bool noThrow) {
            lock (serverMap) {
                if (serverMap.ContainsKey(server.GUID)) {
                    if (noThrow) return;
                    throw new ModelException("Server exists");
                }
                serverMap.Add(server.GUID, server);
                save();
            }
        }

        public T getServer(Guid guid) {
            lock (serverMap) {
                return serverMap.ContainsKey(guid) ? serverMap[guid] : null;
            }
        }

        public void removeServer(Guid guid) {
            T s = getServer(guid);
            if (s == null) return;
            removeServer(s, false);
            CredentialsVault.Instance.deleteCredentials(s);
        }

        private void removeServer(T server, bool nothrow) {
            lock (serverMap) {
                if (serverMap.ContainsKey(server.GUID)) {
                    serverMap.Remove(server.GUID);
                    if (server.IsShared) {
                        using (RegistryKey sharedServersRegistryKey = getSharedServersRegistryKey()) {
                            if (sharedServersRegistryKey != null) {
                                try {
                                    sharedServersRegistryKey.DeleteSubKey(server.GUID.ToString());
                                } catch (Exception e) {
                                    Debug.WriteLine("AbstractServerModel.removeServer() - exception: " + e);
                                }
                            }
                        }
                    }
                    save();
                } else if (!nothrow) {
                    throw new ModelException("No such server");
                }
            }
        }

        public void clear() {
            lock (serverMap) {
                serverMap.Clear();
            }
        }

        private RegistryKey getSharedServersRegistryKey() {
            return Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY + @"\Shared Servers\" + SupportedServerType);
        }
    }
}
