using System;
using System.Net;

namespace soapconnecttest {
    public static class CredentialUtils {
        public static CredentialCache getCredentialsForUserAndPassword(string url, string userName, string password) {
            CredentialCache credsCache = new CredentialCache();
            NetworkCredential creds = new NetworkCredential(getUserNameWithoutDomain(userName), password, getUserDomain(userName));
            credsCache.Add(new Uri(url), "Negotiate", creds);
            credsCache.Add(new Uri(url), "Ntlm", creds);
            credsCache.Add(new Uri(url), "Digest", creds);
            credsCache.Add(new Uri(url), "Basic", creds);
            return credsCache;
        }

//        public static NetworkCredential getCredentialsForUserAndPassword(string url, string userName, string password) {
//            CredentialCache credsCache = new CredentialCache();
//            displayRegisteredModules();
//            NetworkCredential creds = new NetworkCredential(getUserNameWithoutDomain(userName), password, getUserDomain(userName));
//            return creds;
            //            credsCache.Add(new Uri(url), "Negotiate", creds);
            //            credsCache.Add(new Uri(url), "Ntlm", creds);
            //            credsCache.Add(new Uri(url), "Basic", creds);
//            return credsCache;
//        }

#if false
        private static void displayRegisteredModules() {

            IEnumerator registeredModules = AuthenticationManager.RegisteredModules;

            Debug.WriteLine("\r\nThe following authentication modules are now registered with the system:");

            while (registeredModules.MoveNext()) {

                Debug.WriteLine(string.Format("\r \n Module : {0}", registeredModules.Current));

                IAuthenticationModule currentAuthenticationModule = (IAuthenticationModule)registeredModules.Current;

                Debug.WriteLine(string.Format("\t  CanPreAuthenticate : {0}", currentAuthenticationModule.CanPreAuthenticate));

            }

        }
#endif

        public static string getUserNameWithoutDomain(string userName) {
            string userWithoutDomain = userName.Contains("\\")
                                           ? userName.Substring(userName.IndexOf("\\") + 1)
                                           : userName;
            return userWithoutDomain;
        }

        public static string getUserDomain(string userName) {
            string domain = userName.Contains("\\")
                                ? userName.Substring(0, userName.IndexOf("\\"))
                                : null;
            return domain;
        }

    }
}