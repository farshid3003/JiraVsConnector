using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml.XPath;

namespace JiraStackHashAnalyzer {
    internal class RssClient : JiraAuthenticatedClient {
        private readonly JiraServer server;

        public RssClient(JiraServer server)
            : base(server.Url, server.UserName, server.Password) {
            this.server = server;
        }

        public List<JiraIssue> getSavedFilterIssues(int filterId, string sortBy, string sortOrder, int start, int max) {
            StringBuilder url = new StringBuilder(BaseUrl + "/sr/jira.issueviews:searchrequest-xml/");
            url.Append(filterId).Append("/SearchRequest-").Append(filterId).Append(".xml");
            url.Append("?sorter/field=" + sortBy);
            url.Append("&sorter/order=" + sortOrder);
            url.Append("&pager/start=" + start);
            url.Append("&tempMax=" + max);

            url.Append(appendAuthentication(false));

            try {
                using (Stream stream = getRssQueryResultStream(url)) {
                    return createIssueList(stream);
                }
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public List<JiraIssue> getCustomFilterIssues(string queryString, string sortBy, string sortOrder, int start,
                                                     int max) {
            StringBuilder url =
                new StringBuilder(BaseUrl + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?" +
                                  queryString);
            url.Append("&sorter/field=" + sortBy);
            url.Append("&sorter/order=" + sortOrder);
            url.Append("&pager/start=" + start);
            url.Append("&tempMax=" + max);

            url.Append(appendAuthentication(false));

            try {
                using (Stream stream = getRssQueryResultStream(url)) {
                    return createIssueList(stream);
                }
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public JiraIssue getIssue(string key) {
            StringBuilder url = new StringBuilder(BaseUrl + "/si/jira.issueviews:issue-xml/");
            url.Append(key).Append("/").Append(key).Append(".xml");

            url.Append(appendAuthentication(true));

            try {
                using(Stream stream = getRssQueryResultStream(url)) {
                    List<JiraIssue> list = createIssueList(stream);
                    if (list.Count != 1) {
                        throw new ArgumentException("No such issue");
                    }
                    return list[0];
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        private Stream getRssQueryResultStream(StringBuilder url) {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url.ToString());
            req.Proxy = null;
            req.Credentials = new NetworkCredential(UserName, Password);
            req.Timeout = 20000;
            req.ReadWriteTimeout = 40000;
            HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
            return resp.GetResponseStream();
        }

        private List<JiraIssue> createIssueList(Stream s) {
            XPathDocument doc = XPathUtils.getXmlDocument(s);

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/rss/channel/item");
            XPathNodeIterator it = nav.Select(expr);

            List<JiraIssue> list = new List<JiraIssue>();
            while (it.MoveNext()) {
                list.Add(new JiraIssue(server, it.Current));
            }

            return list;
        }
    }
}