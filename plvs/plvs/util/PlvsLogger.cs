using System;
using System.IO;
using Microsoft.Win32;

namespace Atlassian.plvs.util {
    public class PlvsLogger {

        private static readonly object lockObject = new object();
        
        public static void log(string txt) {
            lock (lockObject) {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(Constants.PAZU_REG_KEY);
                if (rk == null) {
                    return;
                }
                string fileName = rk.GetValue("LogFileName", null) as string;
                if (fileName == null) {
                    rk.Close();
                    return;
                }
                rk.Close();
                try {
                    FileInfo f = new FileInfo(fileName);
                    StreamWriter w = f.AppendText();
                    w.WriteLine(DateTime.Now + ": " + txt);
                    w.Close();
                } catch (Exception e) {
                    // oh well
                }
            }
        }
    }
}
