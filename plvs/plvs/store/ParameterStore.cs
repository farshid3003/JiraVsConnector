using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Atlassian.plvs.store {
    public class ParameterStore {
        private readonly Dictionary<string, string> parameterMap = new Dictionary<string, string>();

        public void storeParameter(string name, int value) {
            parameterMap[name] = value.ToString();
        }

        public void storeParameter(string name, string value) {
            parameterMap[name] = value;
        }

        public string loadParameter(string name, string defaultValue) {
            return parameterMap.ContainsKey(name) ? parameterMap[name] : defaultValue;
        }

        public int loadParameter(string name, int defaultValue) {
            string val = loadParameter(name, null);
            return val == null ? defaultValue : int.Parse(val);
        }

        public void readOptions(Stream stream) {
            using (BinaryReader bReader = new BinaryReader(stream)) {
                string allParameters = bReader.ReadString();
                string[] splitParameters = allParameters.Split('\n');
                foreach (string[] strings in splitParameters.Select(splitParameter => splitParameter.Split('=')).Where(strings => strings.Length == 2)) {
                    parameterMap[strings[0]] = strings[1];
                }
            }
        }

        public void writeOptions(Stream stream) {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in parameterMap.Where(pair => pair.Value != null)) {
                sb.Append(pair.Key).Append("=").Append(pair.Value).Append('\n');
            }
            using (BinaryWriter bw = new BinaryWriter(stream)) {
                bw.Write(sb.ToString());
            }
        }

        public void clear() {
            parameterMap.Clear();
        }
    }
}