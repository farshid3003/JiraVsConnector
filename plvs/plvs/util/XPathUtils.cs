using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Atlassian.plvs.util {
    public static class XPathUtils {

        public static string getAttributeSafely(XPathNavigator nav, string name, string defaultValue) {
            if (nav.HasAttributes && nav.MoveToFirstAttribute()) {
                do {
                    if (!nav.Name.Equals(name)) continue;
                    string val = nav.Value;
                    nav.MoveToParent();
                    return val;
                } while (nav.MoveToNextAttribute());
                nav.MoveToParent();
            }
            return defaultValue;
        }

        public static int getAttributeSafely(XPathNavigator nav, string name, int defaultValue) {
            if (nav.HasAttributes && nav.MoveToFirstAttribute()) {
                do {
                    if (!nav.Name.Equals(name)) continue;
                    int val = nav.ValueAsInt;
                    nav.MoveToParent();
                    return val;
                } while (nav.MoveToNextAttribute());
                nav.MoveToParent();
            }
            return defaultValue;
        }

        public static XPathDocument getXmlDocument(Stream stream) {
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            int count;

            do {
                // fill the buffer with data
                count = stream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count == 0) continue;
                // translate from bytes to ASCII text
//                string tempString = Encoding.ASCII.GetString(buf, 0, count);
                string tempString = Encoding.UTF8.GetString(buf, 0, count);

                // continue building the string
                sb.Append(tempString);
            }
            while (count > 0); // any more data to read?

            try {
                XPathDocument doc = new XPathDocument(new StringReader(sb.ToString()));
                return doc;
            } catch (Exception e) {
                throw new InvalidXmlDocumentException(sb.ToString(), e);
            }
        }

        public class InvalidXmlDocumentException : Exception {
            public string SourceDoc { get; private set; }

            public InvalidXmlDocumentException(string source, Exception e) : base("Failed to parse XML document", e) {
                SourceDoc = source;
            }
        }
    }
}
