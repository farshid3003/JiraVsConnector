using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace soapconnecttest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void buttonGo_Click(object sender, EventArgs e) {
            if (textUrl.Text.Trim().Length ==  0) return;
            textLog.Text = "";
            buttonGo.Enabled = false;
            Thread t = new Thread(() => worker(textUrl.Text, textLogin.Text, textPassword.Text));
            t.Start();
        }

        private bool doReadStream;
        private bool streamRead = false;
        private string readXml;

        private void webresponseHandler(WebResponse response) {
            if (!doReadStream) return;
            using(Stream stream = response.GetResponseStream()) {
                if (stream == null) return;
                StreamReader sr = new StreamReader(stream);
                readXml = sr.ReadToEnd();
                streamRead = true;
            }
        }

        private void worker(string url, string userName, string password) {
            log("Started...");
            SoapSession s = new SoapSession(url, webresponseHandler);
            streamRead = false;
            try {
                doReadStream = false;
                log("Loggin in...");
                s.login(userName, password);
                doReadStream = true;
                log("Getting priorities...");
                List<JiraNamedEntity> priorities = s.getPriorities();
                foreach (var priority in priorities) {
                    log("    " + priority.Name);
                }
//                log("Getting projects...");
//                List<JiraProject> jiraProjects = s.getProjects();
//                foreach (var project in jiraProjects) {
//                    log("    " + project.Key + " - " + project.Name);
//                }
                log("Finished");
            } catch (Exception e) {
                if (!streamRead) {
                    log(s.getWriterXml());
                    log(e.ToString());
                } else {
                    log("Written-----------:\r\n");    
                    log(s.getWriterXml());
                    log("\r\n\r\nRead-----------:\r\n");
                    log(readXml ?? "Nothing");
                }
            }
            Invoke(new MethodInvoker(delegate { buttonGo.Enabled = true; }));
        }

        private void log(string txt) {
            Invoke(new MethodInvoker(delegate {
                                         textLog.Text = textLog.Text + "\r\n" + txt;
                                         textLog.SelectAll();
                                         textLog.ScrollToCaret();
                                     }));
        }

    }
}
