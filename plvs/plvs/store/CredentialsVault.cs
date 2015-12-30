using System;
using Atlassian.plvs.api;
using Atlassian.plvs.util;
using Microsoft.Win32;
using System.Diagnostics;

namespace Atlassian.plvs.store {
    internal class CredentialsVault {
        private static readonly CredentialsVault INSTANCE = new CredentialsVault();

        public static CredentialsVault Instance {
            get { return INSTANCE; }
        }

        private const string PAZU_KEY = "Credentials";
        private const string USER_NAME = "UserName_";
        private const string USER_PASSWORD = "UserPassword_";

        private CredentialsVault() {}

        public string getUserName(Server server) {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Constants.PAZU_REG_KEY + "\\" + PAZU_KEY);
                if (key != null) return (string) key.GetValue(USER_NAME + server.GUID, "");
            } catch (Exception e) {
                Debug.WriteLine("CredentialsVault.getUserName() - exception: " + e.Message);
            }
            return "";
        }

        public string getPassword(Server server) {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Constants.PAZU_REG_KEY + "\\" + PAZU_KEY);
                if (key != null) {
                    string saltAndPassword = DPApi.decrypt((string) key.GetValue(USER_PASSWORD + server.GUID, ""), server.GUID.ToString());
                    if (saltAndPassword.StartsWith(server.GUID.ToString())) {
                        // skip salt
                        return saltAndPassword.Substring(server.GUID.ToString().Length);
                    }
                    // non-salted password detected. Let's update registry info with salted version
                    Debug.WriteLine("CredentialsVault.getPassword() - unsalted password read from registry. Updating with salted version");
                    server.Password = saltAndPassword;
                    saveCredentials(server);
                    return saltAndPassword;
                }
            } catch (Exception e) {
                Debug.WriteLine("CredentialsVault.getPassword() - exception: " + e.Message);
            }
            return "";
        }

        public void saveCredentials(Server server) {
            RegistryKey atlKey = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
            if (atlKey == null) return;
            RegistryKey key = atlKey.CreateSubKey(PAZU_KEY);
            if (key == null) return;
            key.SetValue(USER_NAME + server.GUID, server.UserName);
            key.SetValue(USER_PASSWORD + server.GUID, DPApi.encrypt(server.GUID + server.Password, server.GUID.ToString()));
        }

        public void deleteCredentials(Server server) {
            RegistryKey atlKey = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
            if (atlKey == null) return;
            RegistryKey key = atlKey.CreateSubKey(PAZU_KEY);
            if (key == null) return;
            key.DeleteValue(USER_NAME + server.GUID, false);
            key.DeleteValue(USER_PASSWORD + server.GUID, false);
        }
    }
}