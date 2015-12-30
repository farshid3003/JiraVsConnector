using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace proxytest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            ProxyListener.Instance.init();
            navigateWithProxy(webBrowser1, "https://studio.atlassian.com");
        }

        private struct StructInternetProxyInfo {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        } ;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private const int INTERNET_OPTION_PROXY = 38;
        private const int INTERNET_OPEN_TYPE_PROXY = 3;

        public static void navigateWithProxy(WebBrowser browser, string address) {
            ProxyListener.Instance.navigate(browser, address);
        }

    }
}
