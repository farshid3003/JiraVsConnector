using System.IO;
using System.Xml;

namespace soapconnecttest {
    public class XmlWriterSpy : XmlWriter {
        private XmlWriter _me;
        private XmlTextWriter _bu;
        private StringWriter _sw;

        public XmlWriterSpy(XmlWriter implementation) {
            _me = implementation;
            _sw = new StringWriter();
            _bu = new XmlTextWriter(_sw);
            _bu.Formatting = Formatting.Indented;
        }

        public override void Flush() {
            _me.Flush();
            _bu.Flush();
            _sw.Flush();
        }
        public string Xml { get { return (_sw == null ? null : _sw.ToString()); } }

        public override void Close() { _me.Close(); _bu.Close(); }
        public override string LookupPrefix(string ns) { return _me.LookupPrefix(ns); }
        public override void WriteBase64(byte[] buffer, int index, int count) { _me.WriteBase64(buffer, index, count); _bu.WriteBase64(buffer, index, count); }
        public override void WriteCData(string text) { _me.WriteCData(text); _bu.WriteCData(text); }
        public override void WriteCharEntity(char ch) { _me.WriteCharEntity(ch); _bu.WriteCharEntity(ch); }
        public override void WriteChars(char[] buffer, int index, int count) { _me.WriteChars(buffer, index, count); _bu.WriteChars(buffer, index, count); }
        public override void WriteComment(string text) { _me.WriteComment(text); _bu.WriteComment(text); }
        public override void WriteDocType(string name, string pubid, string sysid, string subset) { _me.WriteDocType(name, pubid, sysid, subset); _bu.WriteDocType(name, pubid, sysid, subset); }
        public override void WriteEndAttribute() { _me.WriteEndAttribute(); _bu.WriteEndAttribute(); }
        public override void WriteEndDocument() { _me.WriteEndDocument(); _bu.WriteEndDocument(); }
        public override void WriteEndElement() { _me.WriteEndElement(); _bu.WriteEndElement(); }
        public override void WriteEntityRef(string name) { _me.WriteEntityRef(name); _bu.WriteEntityRef(name); }
        public override void WriteFullEndElement() { _me.WriteFullEndElement(); _bu.WriteFullEndElement(); }
        public override void WriteProcessingInstruction(string name, string text) { _me.WriteProcessingInstruction(name, text); _bu.WriteProcessingInstruction(name, text); }
        public override void WriteRaw(string data) { _me.WriteRaw(data); _bu.WriteRaw(data); }
        public override void WriteRaw(char[] buffer, int index, int count) { _me.WriteRaw(buffer, index, count); _bu.WriteRaw(buffer, index, count); }
        public override void WriteStartAttribute(string prefix, string localName, string ns) { _me.WriteStartAttribute(prefix, localName, ns); _bu.WriteStartAttribute(prefix, localName, ns); }
        public override void WriteStartDocument(bool standalone) { _me.WriteStartDocument(standalone); _bu.WriteStartDocument(standalone); }
        public override void WriteStartDocument() { _me.WriteStartDocument(); _bu.WriteStartDocument(); }
        public override void WriteStartElement(string prefix, string localName, string ns) { _me.WriteStartElement(prefix, localName, ns); _bu.WriteStartElement(prefix, localName, ns); }
        public override WriteState WriteState { get { return _me.WriteState; } }
        public override void WriteString(string text) { _me.WriteString(text); _bu.WriteString(text); }
        public override void WriteSurrogateCharEntity(char lowChar, char highChar) { _me.WriteSurrogateCharEntity(lowChar, highChar); _bu.WriteSurrogateCharEntity(lowChar, highChar); }
        public override void WriteWhitespace(string ws) { _me.WriteWhitespace(ws); _bu.WriteWhitespace(ws); }

    }
}
