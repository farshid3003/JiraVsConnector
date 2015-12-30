using System.Web;

namespace JiraStackHashAnalyzer {
    internal class JiraAuthenticatedClient {

        protected string UserName { get; private set; }
        protected string Password { get; private set; }
        protected string BaseUrl { get; private set; }

        public JiraAuthenticatedClient(string url, string userName, string password) {
            BaseUrl = url;
            UserName = userName;
            Password = password;
        }

        protected string appendAuthentication(bool first) {
            if (UserName != null) {
                return (first ? "?" : "&") + getOsAuthString(UserName, Password);
            }
            return "";
        }

        public static string getOsAuthString(string userName, string password) {
            return
                "os_username=" + HttpUtility.UrlEncode(userName)
                + "&os_password=" + HttpUtility.UrlEncode(password);
        }
    }
}
