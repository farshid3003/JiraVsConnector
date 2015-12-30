using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs.api.jira {
    public class JiraAuthenticatedClient : IDisposable {
        private readonly bool dontUseProxy;

        protected string UserName { get; private set; }
        protected string Password { get; private set; }
        protected string BaseUrl { get; private set; }

        private const string NO_SESSION_COOKIE = "No session cookies found in response";

        private const string JSESSIONID = "JSESSIONID=";
        private const string STUDIO_CROWD_TOKEN = "studio.crowd.tokenkey=";
        private const string REQUEST_SOURCE = "vs-ide-connector";

        public IDictionary<string, string> SessionTokens { get; set; }

        public JiraAuthenticatedClient(string url, string userName, string password, bool dontUseProxy) {
            this.dontUseProxy = dontUseProxy;
            BaseUrl = url;
            UserName = userName;
            Password = password;
        }

        public IDictionary<string, string> login() {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(BaseUrl + "/Dashboard.jspa");
            req.Proxy = dontUseProxy ? null : GlobalSettings.Proxy;
            req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(BaseUrl, UserName, Password);
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.Method = "POST";

            req.ContentType = "application/x-www-form-urlencoded";
            string pars = getLoginPostData(UserName, Password);
            req.ContentLength = pars.Length;
            using (StreamWriter outStream = new StreamWriter(req.GetRequestStream(), Encoding.ASCII)) {
                outStream.Write(pars);
                outStream.Flush();
                outStream.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (resp.GetResponseStream()) {
                    if (!resp.StatusCode.Equals(HttpStatusCode.OK)) {
                        throw new LoginException(new Exception(resp.StatusDescription));
                    }

                    string cookies = resp.Headers["Set-Cookie"];
                    if (cookies == null) {
                        throw new LoginException(new Exception(NO_SESSION_COOKIE));
                    }
                    string jSessionId = getSessionToken(cookies, JSESSIONID);
                    string studioCrowdToken = getSessionToken(cookies, STUDIO_CROWD_TOKEN);
                    if (jSessionId == null && studioCrowdToken == null) {
                        throw new LoginException(new Exception(NO_SESSION_COOKIE));
                    }
                    Dictionary<string, string> tokens = new Dictionary<string, string>();
                    if (jSessionId != null) {
                        tokens[JSESSIONID] = jSessionId;
                    }
                    if (studioCrowdToken != null) {
                        tokens[STUDIO_CROWD_TOKEN] = studioCrowdToken;
                    }
                    SessionTokens = tokens;
                    return SessionTokens;
                }
            }
        }

        protected static string getSessionToken(string cookies, string tokenName) {
            int idxStart = cookies.LastIndexOf(tokenName);
            if (idxStart == -1) {
                return null;
            }
            int idxEnd = cookies.IndexOf(";", idxStart);
            return idxEnd == -1 ? null : cookies.Substring(idxStart + tokenName.Length, idxEnd - idxStart - tokenName.Length);
        }

        protected string appendAuthentication(string url) {
            if (UserName != null) {
                return (url.Contains("?") ? "&" : "?") + CredentialUtils.getOsAuthString(UserName, Password);
            }
            return "";
        }

        protected string appendRequestSource(string url)
        {
            return (url.ToString().Contains("?") ? "&" : "?") + "requestSource=" + REQUEST_SOURCE;
        }

        protected void setSessionCookie(HttpWebRequest req) {
            if (SessionTokens != null) {
                req.Headers["Cookie"] = getSessionCookieString(SessionTokens);
            }
        }

        public static void setSessionCookie(WebHeaderCollection headers, IDictionary<string, string> tokens) {
            headers["Cookie"] = getSessionCookieString(tokens);
        }

        public static string getSessionCookieString(IDictionary<string, string> tokens) {
            if (tokens == null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (var key in tokens.Keys) {
                sb.Append(key).Append(tokens[key]).Append(';');
            }
            return sb.Length > 0 ? sb.ToString() : null;
        }

        public static string getLoginPostData(string userName, string password) {
            return string.Format("os_username={0}&os_password={1}&os_cookie=true",
                HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(userName)),
                HttpUtility.UrlEncode(password));
        }

        public void Dispose() {
        }

    }
}
