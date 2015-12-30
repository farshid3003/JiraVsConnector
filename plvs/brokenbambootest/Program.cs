using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace brokenbambootest {
    internal class Program {
        private const string FAVOURITE_PLANS_ACTION = "/rest/api/latest/plan?favourite&expand=plans.plan";

        private static void Main(string[] args) {
            if (args.Length < 4) {
                Debug.WriteLine("usage: [client|request] <url> <username> <password>");
                Console.WriteLine("usage: <url> <username> <password>");
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(certValidationCallback);

            string url = args[1];
            string endpoint = url + FAVOURITE_PLANS_ACTION + "&os_authType=basic";
            string user = args[2];
            string pwd = args[3];

            try {
                switch (args[0]) {
                    case "client":
                        WebClient client = new WebClient {Credentials = new NetworkCredential(user, pwd)};
                        byte[] downloadData = client.DownloadData(endpoint);
                        Console.Write(Encoding.ASCII.GetString(downloadData, 0, downloadData.Length));
                        break;
                    case "request":
                        var req = (HttpWebRequest) WebRequest.Create(endpoint);
                        req.Timeout = 10000;
                        req.ReadWriteTimeout = 20000;
                        req.ContentType = "application/xml";
                        req.Method = "GET";

                        setBasicAuthHeader(req, user, pwd);

                        var resp = (HttpWebResponse) req.GetResponse();

                        StreamReader sr = new StreamReader(resp.GetResponseStream());
                        Console.Write(sr.ReadToEnd());
                        break;
                }
            } catch (Exception e) {
                Console.Write(e.ToString());
            }
            Console.Write("\r\nPress any key to continue...");
            Console.Read();
        }

        private static void setBasicAuthHeader(HttpWebRequest req, string user, string pwd) {
            string authInfo = user + ":" + pwd;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        private static bool certValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
                                                   SslPolicyErrors sslpolicyerrors) {
            return true;
        }
    }
}