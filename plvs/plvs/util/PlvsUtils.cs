using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
#if VS2010
using System.Windows.Media.Imaging;
#endif
using Atlassian.plvs.api;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.attributes;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.windows;
using EnvDTE;
using Process = System.Diagnostics.Process;
using Timer = System.Windows.Forms.Timer;

namespace Atlassian.plvs.util {
    public static class PlvsUtils {
        public static string GetStringValue(this Enum value) {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if (attribs == null) return null;
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static string GetColorValue(this Enum value) {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            ColorValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(ColorValueAttribute), false) as ColorValueAttribute[];
            if (attribs == null) return "#000000";
            return attribs.Length > 0 ? attribs[0].ColorValue : "#000000";
        }

        public static bool compareLists<T>(IList<T> lhs, IList<T> rhs) {
            if (lhs == null && rhs == null) return true;
            if (lhs == null || rhs == null) return false;

            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; ++i) {
                if (!lhs[i].Equals(rhs[i])) return false;
            }
            return true;
        }

        // ok, this method officially sucks. I am only updating bindings on toolwindow creation.
        // Also, command names are hardcoded.
        // 
        // If anybody can tell me how to get notified about key bindings change, please let me know
        public static void updateKeyBindingsInformation(DTE dte, IDictionary<string, ToolStripItem> buttons) {
            if (dte == null) return;
            IEnumerator enumerator = dte.Commands.GetEnumerator();
            while (enumerator.MoveNext()) {
                Command c = (Command)enumerator.Current;
                if (buttons.ContainsKey(c.Name)) {
                    addBindingToButton(buttons[c.Name], c.Bindings as object[]);
                }
            }
        }

        public static string getAssemblyBasedLocalFilePath(string fileName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.EscapedCodeBase;

            if (name != null) {
                name = name.Substring(0, name.LastIndexOf("/"));
                return name + "/" + fileName;
            }
            throw new InvalidOperationException("Unable to retrieve assembly location");
        }

        private static void addBindingToButton(ToolStripItem button, object[] bindings) {
            if (bindings == null || bindings.Length == 0) return;

            string bindingText = bindings[0].ToString();
            string text = button.Text.Contains(" (") ? button.Text.Substring(0, button.Text.IndexOf(" (")) : button.Text;
            button.Text = text + " (" + bindingText.Substring("Global::".Length) + ")";
        }

        public static void showErrors(string msg, ICollection<Exception> exceptions) {
            if (exceptions.Count == 1) {
                foreach (Exception e in exceptions) {
                    FiveOhThreeJiraException e503 = e as FiveOhThreeJiraException;
                    XPathUtils.InvalidXmlDocumentException eXml = e as XPathUtils.InvalidXmlDocumentException;
                    SoapException eSoap = e as SoapException;
                    if (e503 != null) {
                        show503Error(e503, msg);
                    } else if (eXml != null) {
                        showXmlDocumentError(eXml, msg);
                    } else if (eSoap != null) {
                        showSoapError(eSoap, msg);
                    } else {
                        showNonJira503Errors(exceptions, msg);
                    }
                    break;
                }
            } else {
                showNonJira503Errors(exceptions, msg);
            }
        }

        private static string getJira503Description(JiraServer server) {
            return string.Format(
                "Your JIRA server \"<a href={1}>{0}</a>\" has returned error 503 (Service unavailable). \r\nThis usually means that the server is "
                + "not configured to accept remote API calls. \r\n<p>You can configure remote client access to your server \r\n"
                + "<a href={1}/secure/admin/jira/EditApplicationProperties!default.jspa>here</a><br>(look for \"Accept remote API calls\").\r\n", 
                server.Name, server.Url);
        }

        private static void showNonJira503Errors(IEnumerable<Exception> exceptions, string msg) {
            StringBuilder sb = new StringBuilder();
            List<Exception> exList = new List<Exception>(exceptions);
            int i = 0;
            foreach (Exception exception in exList) {
                sb.Append(getExceptionMessage(exception)).Append(getExceptionDetailsLink("" + i));
                ++i;
                if (i < exList.Count) {
                    sb.Append("<br>\r\n");
                }
            }
            MessageBoxWithHtml.showError(Constants.ERROR_CAPTION, (msg != null ? msg + "<br>\r\n<br>\r\n" : "") + sb,
                delegate {
                    sb = new StringBuilder();
                    foreach (var exception in exList) {
                        sb.Append(getFullExceptionTextDetails(msg, exception));
                        sb.Append("\r\n");
                    }
                    Clipboard.SetText(sb.ToString());
                },
                delegate(string tag) {
                    int idx = int.Parse(tag);
                    new ExceptionViewer(null, exList[idx]).ShowDialog();
                });
        }

        public static void showError(string msg, Exception e) {
            FiveOhThreeJiraException e503 = e as FiveOhThreeJiraException;
            XPathUtils.InvalidXmlDocumentException eXml = e as XPathUtils.InvalidXmlDocumentException;
            if (e503 != null) {
                show503Error(e503, msg);
            } else if (eXml != null) {
                showXmlDocumentError(eXml, msg);
            } else {
                string exceptionMessage = getExceptionMessage(e) + getExceptionDetailsLink("ex");
                MessageBoxWithHtml.showError(
                    Constants.ERROR_CAPTION, 
                    (msg != null ? msg + "<br>\r\n<br>\r\n" : "") + exceptionMessage,
                    () => Clipboard.SetText(getFullExceptionTextDetails(msg, e)),
                    delegate { new ExceptionViewer(msg, e).ShowDialog(); });
            }
        }

        private static void show503Error(FiveOhThreeJiraException e503, string msg) {
            MessageBoxWithHtml.showError(
                Constants.ERROR_CAPTION, 
                getJira503Description(e503.Server),
                () => Clipboard.SetText(getFullExceptionTextDetails(msg, e503)), 
                null);
        }

        private static void showXmlDocumentError(XPathUtils.InvalidXmlDocumentException e, string msg) {
            MessageBoxWithHtml.showError(
                Constants.ERROR_CAPTION,
                e.Message + getExceptionDetailsLink("eXml"),
                () => Clipboard.SetText((msg != null ? msg + "\r\n\r\n" : "") + e.SourceDoc),
                delegate { new ExceptionViewer(e.SourceDoc, e).ShowDialog(); });
        }

        private static void showSoapError(SoapException e, string msg) {
            MessageBoxWithHtml.showError(
                Constants.ERROR_CAPTION,
                e.Message + getExceptionDetailsLink("eSoap"),
                () => Clipboard.SetText(e.Detail.Name + ": " + e.Detail.Value + "\r\n\r\n" + getFullExceptionTextDetails(msg, e)),
                delegate { new ExceptionViewer(e.Detail.Name + ": " + e.Detail.Value, e).ShowDialog(); });
        }

        private static string getExceptionDetailsLink(string tag) {
            return "<br><a href=\"" + MessageBoxWithHtml.EXCEPTION_LINK_TAG + tag + "\">exception details</a>";                
        }

        private const string RAE = "com.atlassian.jira.rpc.exception.RemoteAuthenticationException: ";
        private const string RVE = "com.atlassian.jira.rpc.exception.RemoteValidationException: ";

        private static string getExceptionMessage(Exception e) {
            if (e == null) {
                return "";
            }
            if (e.InnerException != null) {
                string message = e.InnerException.Message;
                if (message.Contains(RAE)) {
                    return message.Substring(message.LastIndexOf(RAE) + RAE.Length);
                }
                return message;
            }
            if (e.Message.Contains(RVE)) {
                return getJiraValidationErrorMessage(e);
            }
            return e.Message;
        }

        private static readonly Regex errorRegex = new Regex(@"Errors: \{(.+)\}");
        private static readonly Regex errorMsgRegex = new Regex(@"Error Messages: \[(.+)\]");

        private static string getJiraValidationErrorMessage(Exception e) {
            if (errorRegex.IsMatch(e.Message)) {
                return errorRegex.Match(e.Message).Groups[1].Value;
            }
            if (errorMsgRegex.IsMatch(e.Message)) {
                return errorMsgRegex.Match(e.Message).Groups[1].Value;
            }
            return e.Message;
        }

        public static string getTextDocument(Stream stream) {
            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];

            int count;

            do {
                count = stream.Read(buf, 0, buf.Length);
                if (count == 0) continue;
                string tempString = Encoding.ASCII.GetString(buf, 0, count);
                sb.Append(tempString);
            } while (count > 0);

            return sb.ToString();
        }

        public static byte[] getBytesFromStream(Stream stream) {
            MemoryStream ms = new MemoryStream();
            byte[] buf = new byte[8192];

            int count;

            do {
                count = stream.Read(buf, 0, buf.Length);
                if (count == 0) continue;
                ms.Write(buf, 0, count);
            } while (count > 0);

            ms.Close();

            return ms.GetBuffer();
        }

        /// <summary>
        /// This method fixes PLVS-109 - for some weird reason if you only add one menu item 
        /// to a drop down during DropDownOpened event, the menu shows up at coordinates (0, 0)
        /// instead of below the drop down button. Probably some Windows Forms bug 
        /// - I am able to reproduce this on a plain Forms project
        /// </summary>
        public static void addPhonyMenuItemFixingPlvs109(ToolStripDropDownButton menu) {
            ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            menu.DropDownItems.Add(toolStripSeparator);
            // yes, just one microsecond is enough to make it work. There probably is a better way, 
            // but frankly, I cannot be bothered to figure out what it could be
            Timer t = new Timer { Interval = 1 };
            t.Tick += (a, b) => { t.Stop(); menu.DropDownItems.Remove(toolStripSeparator); };
            t.Start();
        }

        ///  FUNCTION Enquote Public Domain 2002 JSON.org 
        ///  @author JSON.org 
        ///  @version 0.1 
        ///  Ported to C# by Are Bjolseth, teleplan.no 
        public static string JsonEncode(string s) {
            if (string.IsNullOrEmpty(s)) {
                return "\"\"";
            }
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            sb.Append('"');
            for (i = 0; i < len; i += 1) {
                char c = s[i];
                if ((c == '\\') || (c == '"') || (c == '>')) {
                    sb.Append('\\');
                    sb.Append(c);
                } else if (c == '\b')
                    sb.Append("\\b");
                else if (c == '\t')
                    sb.Append("\\t");
                else if (c == '\n')
                    sb.Append("\\n");
                else if (c == '\f')
                    sb.Append("\\f");
                else if (c == '\r')
                    sb.Append("\\r");
                else {
                    if (c < ' ') {
                        //t = "000" + Integer.toHexString(c); 
                        string tmp = new string(c, 1);
                        t = "000" + int.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                        sb.Append("\\u" + t.Substring(t.Length - 4));
                    } else {
                        sb.Append(c);
                    }
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

#if VS2010
        // from http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/16bce8a4-1ee7-4be9-bd7f-0cc2b0f80cf0/
        public static BitmapSource bitmapSourceFromPngImage(System.Drawing.Image img) {
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
            PngBitmapDecoder decoder = new PngBitmapDecoder(memStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
#endif

        public static void installSslCertificateHandler() {
            ServicePointManager.ServerCertificateValidationCallback = certValidationCallback;
        }

        private static readonly List<string> allowedCerts = new List<string>();
        private static readonly List<string> rejectedCerts = new List<string>();

        private static bool certValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
            if (errors.Equals(SslPolicyErrors.None)) return true;
            lock(rejectedCerts) {
                if (rejectedCerts.Contains(certificate.GetCertHashString())) {
                    return false;
                }
                if (allowedCerts.Contains(certificate.GetCertHashString())) {
                    return true;
                }

                // PLVS-117: search for installed certs. If found, allow it, whether issuer is kosher or not 
                // (this lets self-signed certs be treated as good enough)
                X509Store store = new X509Store();
                try {
                    store.Open(OpenFlags.ReadOnly);
                    if (store.Certificates.Cast<X509Certificate2>().Any(cert => cert.Equals(certificate))) {
                        X509Certificate2 c2 = new X509Certificate2(certificate);
                        if (DateTime.Now.CompareTo(c2.NotBefore) >= 0 && DateTime.Now.CompareTo(c2.NotAfter) <= 0) {
                            allowedCerts.Add(certificate.GetCertHashString());
                            return true;
                        }
                    }
                } finally {
                    store.Close();
                }

                BadCertificateDialog dlg = new BadCertificateDialog(sender, new X509Certificate2(certificate));
                bool result = false;
                AtlassianPanel.Instance.safeInvoke(new MethodInvoker(() => {
                                                                         if (dlg.ShowDialog(AtlassianPanel.Instance) != DialogResult.Yes) {
                                                                             rejectedCerts.Add(certificate.GetCertHashString());
                                                                             result = false;
                                                                         } else {
                                                                             allowedCerts.Add(certificate.GetCertHashString());
                                                                             result = true;
                                                                         }
                                                                     }));
                return result;
            }
        }

        public static void safeInvoke(this Control control, Delegate action) {
            try {
                control.Invoke(action);
            } catch (InvalidOperationException e) {
                Debug.WriteLine("PlvsUtils.safeInvoke() - exception: " + e.Message);
#if VS2010
            } catch (CultureNotFoundException e) {
                // PLVS-323
                Debug.WriteLine("PlvsUtils.safeInvoke() - exception: " + e.Message);
#endif
            } catch(Exception e) {
                // PLVS-321 - no idea. We are getting COMException. Can't reproduce, but let's just catch this here and see what happens
                Debug.WriteLine("PlvsUtils.safeInvoke() - exception: " + e.Message);
            }
        }

        public static bool IsNullOrEmpty(this ICollection c) {
            return (c == null || c.Count == 0);
        }

        public static string getFullExceptionTextDetails(string message, Exception exception) {
            if (message == null && exception == null) {
                return "";
            }
            string innerExceptionDetails = exception != null ? getFullExceptionTextDetails(null, exception.InnerException) : null;
            return 
                (message != null ? message + "\r\n\r\n" : "") 
                + (exception != null
                    ? (exception.Message + "\r\n\r\n" 
                        + exception.GetType() + "\r\n\r\n" 
                        + (string.IsNullOrEmpty(innerExceptionDetails) ? "" : innerExceptionDetails + "\r\n\r\n")
                        +  exception.StackTrace)
                    : "");
        }

        public static string getServerNodeName<T>(AbstractServerModel<T> model, T server) where T : Server {
            return server.Name + (model.DefaultServerGuid.Equals(server.GUID) ? " (default)" : "");
        }

        public static System.Threading.Thread createThread(ThreadStart threadStart) {
            ThreadStart ts = delegate {
                                 try {
                                     threadStart.Invoke();
                                 } catch (Exception e) {
                                     if (isConnectorException(e)) {
                                         UnhandledExceptionDialog dlg = new UnhandledExceptionDialog(e);
                                         dlg.ShowDialog();
                                     } else {
                                         throw;
                                     }
                                 }
                             };
            System.Threading.Thread t = new System.Threading.Thread(ts);
            return t;
        }

        public static string unescape(this string s) {
            return s.Replace("\\\"", "\"");
        }

        public static DTE Dte { get; set; }

        public static string getThroberPath() {
            string throbberPath = null;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.EscapedCodeBase;

            if (name != null) {
                name = name.Substring(0, name.LastIndexOf("/"));
                throbberPath = name + "/ajax-loader.gif";
            }

            return throbberPath;
        }

        public static string getThrobberHtml(string throbberPath, string text) {
            if (throbberPath == null) {
                return "<html><head>" + Resources.summary_and_description_css + "</head><body class=\"summary\">" + text + "</body></html>";
            }
            return string.Format(Resources.throbber_html, throbberPath);
        }

        // PLVS-217
        public static bool isConnectorException(Exception e) {
            return e.StackTrace != null && e.StackTrace.Split(new[] { '\n' }).Any(line => line.Contains("Atlassian.plvs."));
        }

        public static void runBrowser(string url) {
            try {
                // see http://stackoverflow.com/questions/12206368/process-starturl-broken-on-windows-8-chrome-are-there-alternatives
                Process.Start(url);
            } catch (Win32Exception e) {
                try {
                    Debug.WriteLine(e.Message);
                    var startInfo = new ProcessStartInfo("iexplore.exe", url);
                    Process.Start(startInfo);
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}

// from http://www.rhyous.com/2011/01/24/how-read-the-64-bit-registry-from-a-32-bit-application-or-vice-versa/
namespace Read64bitRegistryFrom32bitApp {
    public enum RegSAM {
        QueryValue = 0x0001,
        SetValue = 0x0002,
        CreateSubKey = 0x0004,
        EnumerateSubKeys = 0x0008,
        Notify = 0x0010,
        CreateLink = 0x0020,
        WOW64_32Key = 0x0200,
        WOW64_64Key = 0x0100,
        WOW64_Res = 0x0300,
        Read = 0x00020019,
        Write = 0x00020006,
        Execute = 0x00020019,
        AllAccess = 0x000f003f
    }

    public static class RegHive {
        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
    }

    public static class RegistryWOW6432 {
        #region Member Variables
        #region Read 64bit Reg from 32bit app
        [DllImport("Advapi32.dll")]
        static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string lpSubKey,
            uint ulOptions,
            int samDesired,
            out int phkResult);

        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);

        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        public static extern int RegQueryValueEx(
            int hKey, string lpValueName,
            int lpReserved,
            ref uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);
        #endregion
        #endregion

        #region Functions
        static public string GetRegKey64(UIntPtr inHive, String inKeyName, String inPropertyName) {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_64Key, inPropertyName);
        }

        static public string GetRegKey32(UIntPtr inHive, String inKeyName, String inPropertyName) {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_32Key, inPropertyName);
        }

        static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName) {
            //UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
            int hkey = 0;

            try {
                uint lResult = RegOpenKeyEx(RegHive.HKEY_LOCAL_MACHINE, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
                if (0 != lResult) return null;
                uint lpType = 0;
                uint lpcbData = 1024;
                StringBuilder AgeBuffer = new StringBuilder(1024);
                RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, AgeBuffer, ref lpcbData);
                string Age = AgeBuffer.ToString();
                return Age;
            } finally {
                if (0 != hkey) RegCloseKey(hkey);
            }
        }
        #endregion

        #region Enums
        #endregion
    }
}
