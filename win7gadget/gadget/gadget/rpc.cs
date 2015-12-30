using System.Runtime.CompilerServices;

namespace gadget {

    public delegate void LoginCallback(string token);
    public delegate void GetProjectsCallback(object result);
    public delegate void CallbackError(string error);

    [Imported]
    [IgnoreNamespace]
    public class rpc {
        public static void login(string url, string login, string password, LoginCallback cb, CallbackError errorCb) { }
        public static void getprojects(string url, string token, GetProjectsCallback cb, CallbackError errorCb) { }
    }
}
