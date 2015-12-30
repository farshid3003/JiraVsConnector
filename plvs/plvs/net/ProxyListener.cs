using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.net {
    public class ProxyListener : IDisposable {

        private const ushort PORT_MIN = 51111;
        private const ushort PORT_MAX = 59999;

        public const string PROXY_ADDRESS = "127.0.0.1:";

        public ushort Port { get; private set; }

        public const string TARGET_PARAMETER = "/?target=";

        private readonly HttpListener listener;

        private static readonly ProxyListener instance = new ProxyListener();

        public static ProxyListener Instance { get { return instance; } }

        public bool HasListener { get { return listenerThread != null; } }

        private Thread listenerThread;

        private ProxyListener() {
            bool haveFreePort = false;
            for (Port = PORT_MIN; Port <= PORT_MAX; ++Port) {
                try {
                    new TcpClient("127.0.0.1", Port);
                } catch (SocketException e) {
                    if (e.SocketErrorCode.Equals(SocketError.ConnectionRefused)) {
                        haveFreePort = true;
                        break;
                    }
                }
            }

            if (!haveFreePort) {
                Debug.WriteLine("ProxyListener - ctor() - no free ports, is this system nuts?");
                return;
            }

            listener = new HttpListener();

            try {
                listener.Prefixes.Add("http://" + PROXY_ADDRESS + Port + "/");
            } catch (Exception e) {
                Debug.WriteLine("ProxyListener - ctor() - exception: " + e);
                listener = null;
            }
        }

        private readonly AutoResetEvent ev = new AutoResetEvent(false);

        public void init() {
            lock (this) {
                if (listener == null) return;
                if (listenerThread != null) return;
                listenerThread = PlvsUtils.createThread(listenerRunner);
                listenerThread.Start();
                ev.WaitOne();
            }
        }

        private void listenerRunner() {
            try {
                listener.Start();
            } catch (Exception e) {
                Debug.WriteLine("ProxyListener.listenerRunner() - caught exception " + e.Message 
                    + ", unfortunately I am unable to set the proxy for self-signed SSL servers. You are out of luck. Disable UAC and you will be ok");
                listenerThread = null;
                ev.Set();
                return;
            }

            ev.Set();
            while (true) {
                try {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    if (request.RemoteEndPoint.Address.Equals(IPAddress.Loopback)) {
                        string url = request.RawUrl;

                        if (url.StartsWith(TARGET_PARAMETER)) {
                            string targetUrl = HttpUtility.UrlDecode(url.Substring(TARGET_PARAMETER.Length));
                            if (targetUrl != null) {
                                HttpWebRequest r = (HttpWebRequest)WebRequest.Create(targetUrl);
//                                foreach (var header in request.Headers.AllKeys) {
//                                    r.Headers[header] = request.Headers[header];
//                                }
//                                CookieContainer cc = new CookieContainer();
//                                cc.Add(request.Cookies);
//                                r.CookieContainer = cc;
                                r.Method = "GET";

                                using (WebResponse rsp = r.GetResponse()) {
                                    Stream output = response.OutputStream;
                                    byte[] buffer = new byte[1024];
                                    using (Stream stream = rsp.GetResponseStream()) {
                                        MemoryStream ms = new MemoryStream();
                                        int read;
                                        do {
                                            read = stream.Read(buffer, 0, 1024);
                                            if (read <= 0) continue;
                                            ms.Write(buffer, 0, read);
                                        } while (read > 0);

                                        response.ContentLength64 = ms.Length;
                                        output.Write(ms.ToArray(), 0, (int) ms.Length);

                                    }
                                    output.Close();
                                }
                            }
                        }
                    }
                } catch (ThreadAbortException) {
                    Debug.WriteLine("ProxyListener.listenerRunner() - thread aborted, stopping listener");
                    listener.Stop();
                    break;
                } catch (Exception e) {
                    Debug.WriteLine("ProxyListener.listenerRunner() - exception: " + e);
                }
            }
        }

#if false
        private void dumpRequest(HttpListenerRequest request, StringBuilder sb) {
            dumpObject(request, sb);
        }

        private void dumpObject(object o, StringBuilder sb) {
            dumpObject(o, sb, true);
        }

        private void dumpObject(object o, StringBuilder sb, bool ulli) {
            if (ulli)
                sb.Append("<ul>");

            if (o is string || o is int || o is long || o is double) {
                if (ulli)
                    sb.Append("<li>");

                sb.Append(o.ToString());

                if (ulli)
                    sb.Append("</li>");
            } else {
                Type t = o.GetType();
                foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                    sb.Append("<li><b>" + p.Name + ":</b> ");
                    object val = null;

                    try {
                        val = p.GetValue(o, null);
                    } catch {}

                    if (val is string || val is int || val is long || val is double)
                        sb.Append(val);
                    else if (val != null) {
                        Array arr = val as Array;
                        if (arr == null) {
                            NameValueCollection nv = val as NameValueCollection;
                            if (nv == null) {
                                IEnumerable ie = val as IEnumerable;
                                if (ie == null)
                                    sb.Append(val.ToString());
                                else
                                    foreach (object oo in ie)
                                        dumpObject(oo, sb);
                            } else {
                                sb.Append("<ul>");
                                foreach (string key in nv.AllKeys) {
                                    sb.AppendFormat("<li>{0} = ", key);
                                    dumpObject(nv[key], sb, false);
                                    sb.Append("</li>");
                                }
                                sb.Append("</ul>");
                            }
                        } else
                            foreach (object oo in arr)
                                dumpObject(oo, sb);
                    } else {
                        sb.Append("<i>null</i>");
                    }
                    sb.Append("</li>");
                }
            }
            if (ulli)
                sb.Append("</ul>");
        }
#endif
 
        public void Dispose() {
            if (listenerThread == null) return;
            listenerThread.Abort();
            listenerThread.Join();
        }
    }

    public static class ProxySetter {
        public static void navigateWithProxy(this WebBrowser browser, string address) {
            if (ProxyListener.Instance.HasListener) {
                browser.Navigate("http://" + ProxyListener.PROXY_ADDRESS + ProxyListener.Instance.Port + ProxyListener.TARGET_PARAMETER + HttpUtility.UrlEncode(address));
            } else {
                browser.Navigate(address);
            }
        }
    }
}
