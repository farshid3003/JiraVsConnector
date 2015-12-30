using System;
using System.Threading;
using System.Windows.Forms;
using jiratimeoutbug.com.atlassian.studio;
using Timer = System.Windows.Forms.Timer;

namespace jiratimeoutbug {
    public partial class Form1 : Form {
        private bool inProgress;

        private Timer t;

        public Form1() {
            InitializeComponent();

            t = new Timer();
            t.Interval = 1000;
            t.Tick += t_Tick;
        }

        private void t_Tick(object sender, EventArgs e) {
            t.Stop();
            Thread thr = new Thread(() => doStuff(textUser.Text, textPassword.Text));
            thr.Start();
        }

        private void doStuff(string user, string pwd) {
            string txt = "successfully invoked login/logout at ";

            try {
                JiraSoapServiceService s = new JiraSoapServiceService();
                s.Url = "https://studio.atlassian.com//rpc/soap/jirasoapservice-v2";
                s.Timeout = 2000;
                string token = s.login(user, pwd);
                s.logout(token);
            } catch (Exception e) {
                txt = "caught exception " + e.Message + " at ";
            }

            try {
                Invoke(new MethodInvoker(delegate {
                                             textLog.Text = txt + DateTime.Now.ToLongTimeString() + "\r\n" + textLog.Text;
                    if (inProgress) {
                        t.Start();
                    }
                }));
            } catch {
                
            }
        }

        private void buttonStartStop_Click(object sender, EventArgs e) {
            if (inProgress) {
                buttonStartStop.Text = "Go";
                t.Stop();
            } else {
                buttonStartStop.Text = "Stop";
                t.Start();
            }
            inProgress = !inProgress;
        }
    }
}