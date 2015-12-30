using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Atlassian.plvs.util;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.store {
    public class ParameterStoreManager : IVsPersistSolutionOpts {
        public enum StoreType {
            JIRA_SERVERS = 1,
            SETTINGS = 2,
            BAMBOO_SERVERS = 3,
            ACTIVE_ISSUES = 4
        }

        private readonly Dictionary<StoreType, ParameterStore> stores = new Dictionary<StoreType, ParameterStore>();

        private ParameterStoreManager() {
            stores[StoreType.JIRA_SERVERS] = new ParameterStore();
            stores[StoreType.BAMBOO_SERVERS] = new ParameterStore();
            stores[StoreType.SETTINGS] = new ParameterStore();
            stores[StoreType.ACTIVE_ISSUES] = new ParameterStore();
        }

        private static readonly ParameterStoreManager INSTANCE = new ParameterStoreManager();

        public static ParameterStoreManager Instance {
            get { return INSTANCE; }
        }

        public ParameterStore getStoreFor(StoreType type) {
            return stores[type];
        }

        public void clear() {
            foreach (ParameterStore store in stores.Values) {
                store.clear();
            }
        }

        public int SaveUserOptions(IVsSolutionPersistence pPersistence) {
            try {
                foreach (StoreType type in Enum.GetValues(typeof (StoreType))) {
                    pPersistence.SavePackageUserOpts(this, getKey(type));
                }
            }
            finally {
                Marshal.ReleaseComObject(pPersistence);
            }
            return VSConstants.S_OK;
        }

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts) {
            try {
                foreach (StoreType type in Enum.GetValues(typeof (StoreType))) {
                    pPersistence.LoadPackageUserOpts(this, getKey(type));
                }
            }
            finally {
                Marshal.ReleaseComObject(pPersistence);
            }
            return VSConstants.S_OK;
        }

        public int WriteUserOptions(IStream pOptionsStream, string pszKey) {
            try {
                using (StreamEater wrapper = new StreamEater(pOptionsStream)) {
                    try {
                        StoreType type = getTypeFromKey(pszKey);
                        stores[type].writeOptions(wrapper);
// ReSharper disable EmptyGeneralCatchClause
                    } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
                    }
                }

                return VSConstants.S_OK;
            }
            finally {
                Marshal.ReleaseComObject(pOptionsStream);
            }
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey) {
            try {
                using (StreamEater wrapper = new StreamEater(pOptionsStream)) {
                    try {
                        StoreType type = getTypeFromKey(pszKey);
                        stores[type].readOptions(wrapper);
// ReSharper disable EmptyGeneralCatchClause
                    } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
                    }
                }
                return VSConstants.S_OK;
            }
            finally {
                Marshal.ReleaseComObject(pOptionsStream);
            }
        }

        private static string getKey(StoreType type) {
            return type.ToString();
        }

        private static StoreType getTypeFromKey(string key) {
            return (StoreType) Enum.Parse(typeof (StoreType), key);
        }
    }
}