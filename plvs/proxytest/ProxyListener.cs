using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace proxytest {
    internal class ProxyListener {
        public const string PROXY_ADDRESS = "127.0.0.1:12345";

        private readonly HttpListener listener;

        private static readonly ProxyListener instance = new ProxyListener();

        public static ProxyListener Instance { get { return instance; } }

        public bool HasListener { get { return listenerThread != null; } }

        private Thread listenerThread;

        private ProxyListener() {
            listener = new HttpListener();

            try {
                listener.Prefixes.Add("http://" + PROXY_ADDRESS + "/");
                //                listener.Prefixes.Add("https://" + PROXY_ADDRESS + "/");
            } catch (Exception e) {
                Debug.WriteLine("ProxyListener - ctor() - exception: " + e);
                listener = null;
            }
        }

        private AutoResetEvent ev = new AutoResetEvent(false);
        private string TARGET = "/?target=";

        public void init() {
            if (listener == null) return;
            listenerThread = new Thread(listenerRunner);
            listenerThread.Start();

            ev.WaitOne();
        }

        private void listenerRunner() {
            listener.Start();
            ev.Set();

            while (true) {
                try {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;

                    string url = request.RawUrl;

                    StringBuilder sb = new StringBuilder();

                    if (url.StartsWith(TARGET)) {
                        string targetUrl = HttpUtility.UrlDecode(url.Substring(TARGET.Length));
                        if (targetUrl != null) {
                            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(targetUrl);
//                            foreach (var header in request.Headers.AllKeys) {
//                                r.Headers[header] = request.Headers[header];
//                            }
//                            CookieContainer cc = new CookieContainer();
//                            cc.Add(request.Cookies);
//                            r.CookieContainer = cc;
                            r.Method = "GET";
                            using (WebResponse rsp = r.GetResponse()) {
                                using (StreamReader sr = new StreamReader(rsp.GetResponseStream())) {
                                    sb.Append(sr.ReadToEnd());
                                }
                            }
                        }
                    } else {
                        sb.Append("<html><body><h1>" + context.Request.HttpMethod + " " + context.Request.Url + "</h1>");
                        dumpRequest(request, sb);
                        sb.Append("</body></html>");
                    }

                    HttpListenerResponse response = context.Response;
                    byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                } catch (ThreadAbortException) {
                    Debug.WriteLine("ProxyListener.listenerRunner() - thread aborted, stopping listener");
                    listener.Stop();
                    break;
                } catch (Exception e) {
                    Debug.WriteLine("ProxyListener.listenerRunner() - exception: " + e);   
                }
            }
        }

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
 
        public void Dispose() {
            if (listenerThread == null) return;
            listenerThread.Abort();
            listenerThread.Join();
        }

        public void navigate(WebBrowser browser, string address) {
            browser.Navigate("http://" + PROXY_ADDRESS + TARGET + HttpUtility.UrlEncode(address));
        }
    }
}
