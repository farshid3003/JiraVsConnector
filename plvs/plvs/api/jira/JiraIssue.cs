﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraIssue {
        public const int UNKNOWN = -1;

        public class Comment {
            public string Body { get; internal set; }
            public string Created { get; internal set; }
            public string Author { get; internal set; }

            public bool Equals(Comment other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Body, Body) && Equals(other.Created, Created) && Equals(other.Author, Author);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof (Comment) && Equals((Comment) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    int result = (Body != null ? Body.GetHashCode() : 0);
                    result = (result*397) ^ (Created != null ? Created.GetHashCode() : 0);
                    result = (result*397) ^ (Author != null ? Author.GetHashCode() : 0);
                    return result;
                }
            }
        }

        public string ServerLanguage { get; private set; }

        private readonly List<Comment> comments = new List<Comment>();

        private List<string> versions = new List<string>();

        private List<string> fixVersions = new List<string>();

        private List<string> components = new List<string>();

        private readonly List<JiraAttachment> attachments = new List<JiraAttachment>();

        private readonly List<IssueLinkType> issueLinks = new List<IssueLinkType>();

//        public JToken RestIssue { get; private set; }

        public JiraIssue() {
            ServerLanguage = null;
        }

        public JiraIssue(JiraServer server, JToken issue) {
            try {
                Server = server;

                Key = issue["key"].Value<string>();
                Id = issue["id"].Value<int>();
                ProjectKey = Key.Substring(0, Key.LastIndexOf('-'));

                var fields = issue["fields"];

                ParentKey = fields["parent"] != null && fields["parent"].HasValues ? fields["parent"]["key"].Value<string>() : null;
                getSubtasks(fields["subtasks"]);
                Summary = fields["summary"].Value<string>();
                getAttachments(fields["attachment"]);
                Status = fields["status"]["name"].Value<string>();
                StatusIconUrl = fields["status"]["iconUrl"].Value<string>();
                StatusId = fields["status"]["id"].Value<int>();
                var prio = fields["priority"];
                if (prio != null && prio.HasValues) {
                    Priority = prio["name"].Value<string>();
                    PriorityIconUrl = fields["priority"]["iconUrl"].Value<string>();
                    PriorityId = fields["priority"]["id"].Value<int>();
                }

                var renderedDescription = issue["renderedFields"]["description"];
                Description = renderedDescription != null ? renderedDescription.Value<string>() : fields["description"].Value<string>();

                IssueType = fields["issuetype"]["name"].Value<string>();
                IssueTypeIconUrl = fields["issuetype"]["iconUrl"].Value<string>();
                IssueTypeId = fields["issuetype"]["id"].Value<int>();
                if (fields["assignee"] != null && fields["assignee"].HasValues) {
                    Assignee = fields["assignee"]["name"].Value<string>();
                    JiraServerCache.Instance.getUsers(server).putUser(new JiraUser(Assignee, fields["assignee"]["displayName"].Value<string>()));
                } else {
                    Assignee = "Unknown";
                }
                if (fields["reporter"] != null && fields["reporter"].HasValues) {
                    Reporter = fields["reporter"]["name"].Value<string>();
                    JiraServerCache.Instance.getUsers(server).putUser(new JiraUser(Reporter, fields["reporter"]["displayName"].Value<string>()));
                } else {
                    Reporter = "Unknown";
                }
                CreationDate = DateTime.Parse(fields["created"].Value<string>(), CultureInfo.InvariantCulture);
                UpdateDate = DateTime.Parse(fields["updated"].Value<string>(), CultureInfo.InvariantCulture);
                Resolution = fields["resolution"] != null && fields["resolution"].HasValues ? fields["resolution"]["name"].Value<string>() : null;
                ResolutionId = Resolution != null ? fields["resolution"]["id"].Value<int>() : UNKNOWN;

                getTimeFieldsFromJson(issue);

                if (fields["versions"] != null) {
                    foreach (var v in fields["versions"]) {
                        Versions.Add(v["name"].Value<string>());
                    }
                }
                if (fields["fixVersions"] != null) {
                    foreach (var v in fields["fixVersions"]) {
                        FixVersions.Add(v["name"].Value<string>());
                    }
                }
                if (fields["components"] != null) {
                    foreach (var v in fields["components"]) {
                        Components.Add(v["name"].Value<string>());
                    }
                }
                var env = fields["environment"];
                Environment = env != null ? env.Value<string>() : null;
                getComments(fields["comment"], issue["renderedFields"]["comment"]);
                getIssueLinks(fields["issuelinks"]);

//                RestIssue = issue;
            } catch (Exception e) {
                throw new InvalidOperationException("Unable to parse issue JSON object: " + issue, e);
            }
        }

        public JiraIssue(JiraServer server, string serverLanguage, XPathNavigator nav) {
            Server = server;
            ServerLanguage = serverLanguage;

            nav.MoveToFirstChild();
            do {
                switch (nav.Name) {
                    case "key":
                        Key = nav.Value;
                        Id = XPathUtils.getAttributeSafely(nav, "id", UNKNOWN);
                        ProjectKey = Key.Substring(0, Key.LastIndexOf('-'));
                        break;
                    case "parent":
                        ParentKey = nav.Value;
                        break;
                    case "subtasks":
                        getSubtasks(nav);
                        break;
                    case "summary":
                        Summary = nav.Value;
                        break;
                    case "attachments":
                        getAttachments(nav);
                        break;
                    case "status":
                        Status = nav.Value;
                        StatusIconUrl = XPathUtils.getAttributeSafely(nav, "iconUrl", null);
                        StatusId = XPathUtils.getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "priority":
                        Priority = nav.Value;
                        PriorityIconUrl = XPathUtils.getAttributeSafely(nav, "iconUrl", null);
                        PriorityId = XPathUtils.getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "description":
                        Description = nav.Value;
                        break;
                    case "type":
                        IssueType = nav.Value;
                        IssueTypeIconUrl = XPathUtils.getAttributeSafely(nav, "iconUrl", null);
                        IssueTypeId = XPathUtils.getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "assignee":
                        Assignee = XPathUtils.getAttributeSafely(nav, "username", "Unknown");
                        string assigneName = nav.Value;
                        JiraServerCache.Instance.getUsers(server).putUser(new JiraUser(Assignee, assigneName));
                        break;
                    case "reporter":
                        Reporter = XPathUtils.getAttributeSafely(nav, "username", "Unknown");
                        string reporterName = nav.Value;
                        JiraServerCache.Instance.getUsers(server).putUser(new JiraUser(Reporter, reporterName));
                        break;
                    case "created":
                        CreationDate = JiraIssueUtils.getDateTimeFromJiraTimeString(serverLanguage, nav.Value);
                        break;
                    case "updated":
                        UpdateDate = JiraIssueUtils.getDateTimeFromJiraTimeString(serverLanguage, nav.Value);
                        break;
                    case "resolution":
                        Resolution = nav.Value;
                        ResolutionId = XPathUtils.getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "timeestimate":
                        RemainingEstimate = nav.Value;
                        RemainingEstimateInSeconds = XPathUtils.getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "timeoriginalestimate":
                        OriginalEstimate = nav.Value;
                        OriginalEstimateInSeconds = XPathUtils.getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "timespent":
                        TimeSpent = nav.Value;
                        TimeSpentInSeconds = XPathUtils.getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "version":
                        versions.Add(nav.Value);
                        break;
                    case "fixVersion":
                        fixVersions.Add(nav.Value);
                        break;
                    case "component":
                        components.Add(nav.Value);
                        break;
                    case "comments":
                        getComments(nav);
                        break;
                    case "environment":
                        Environment = nav.Value;
                        break;
                    case "issuelinks":
                        getIssueLinks(nav);
                        break;
                    default:
                        break;
                }
            } while (nav.MoveToNext());
            if (Key == null || Summary == null) {
                throw new InvalidDataException();
            }
        }

        private void getAttachments(XPathNavigator nav) {
            XPathExpression expr = nav.Compile("attachment");
            XPathNodeIterator it = nav.Select(expr);

            if (!nav.MoveToFirstChild()) return;
            while (it.MoveNext()) {
                JiraAttachment a = new JiraAttachment(
                    int.Parse(XPathUtils.getAttributeSafely(it.Current, "id", "0")),
                    XPathUtils.getAttributeSafely(it.Current, "name", "none"),
                    JiraIssueUtils.getDateTimeFromJiraTimeString(ServerLanguage, XPathUtils.getAttributeSafely(it.Current, "created", "none")),
                    XPathUtils.getAttributeSafely(it.Current, "author", "none"),
                    int.Parse(XPathUtils.getAttributeSafely(it.Current, "size", "0")));
                attachments.Add(a);
            }
            nav.MoveToParent();
        }

        private void getAttachments(JToken atts) {
            if (atts == null) return;
            foreach (var att in atts) {
                attachments.Add(new JiraAttachment(att));
            }
        }

        private void getIssueLinks(JToken links) {
            if (links == null) return;
            var d = new Dictionary<int, IssueLinkType>();
            foreach (var link in links) {
                var type = link["type"];
                var id = link["id"].Value<int>();
                var ilt = new IssueLinkType(id, type["name"].Value<string>(), type["outward"].Value<string>(), type["inward"].Value<string>());
                d[id] = ilt;
            }
            foreach (var link in links) {
                var id = link["id"].Value<int>();
                if (link["inwardIssue"] != null && link["inwardIssue"].HasValues) {
                    d[id].InwardLinks.Add(link["inwardIssue"]["key"].Value<string>());
                }
                if (link["outwardIssue"] != null && link["outwardIssue"].HasValues) {
                    d[id].OutwardLinks.Add(link["outwardIssue"]["key"].Value<string>());
                }
            }
            foreach (var link in d.Values) {
                if (link.InwardLinks.Count == 0) link.InwardLinksName = null;
                if (link.OutwardLinks.Count == 0) link.OutwardLinksName = null;
            }

            issueLinks.AddRange(d.Values);
        }

        private void getIssueLinks(XPathNavigator nav) {
            XPathExpression expr = nav.Compile("issuelinktype");
            XPathNodeIterator it = nav.Select(expr);

            if (!nav.MoveToFirstChild()) return;
            while (it.MoveNext()) {
                int id = XPathUtils.getAttributeSafely(it.Current, "id", 0);
                
                string linkTypeName = null;
                string outwardLinksName = null;
                string inwardLinksName = null;
                List<JiraNamedEntity> outwardIdsAndKeys = null;
                List<JiraNamedEntity> inwardIdsAndKeys = null;

                if (it.Current.MoveToFirstChild()) {
                    do {
                        switch (it.Current.Name) {
                            case "name":
                                linkTypeName = it.Current.Value;
                                break;
                            case "outwardlinks":
                                outwardLinksName = XPathUtils.getAttributeSafely(it.Current, "description", null);
                                outwardIdsAndKeys = getLinks(it.Current);
                                break;
                            case "inwardlinks":
                                inwardLinksName = XPathUtils.getAttributeSafely(it.Current, "description", null);
                                inwardIdsAndKeys = getLinks(it.Current);
                                break;
                        }
                    } while (it.Current.MoveToNext());

                    if (id == 0 || linkTypeName == null) continue;

                    IssueLinkType ilt = new IssueLinkType(id, linkTypeName, outwardLinksName, inwardLinksName);
                    if (outwardIdsAndKeys != null) {
                        foreach (JiraNamedEntity entity in outwardIdsAndKeys) {
                            ilt.OutwardLinks.Add(entity.Name);
                        }
                    }
                    if (inwardIdsAndKeys != null) {
                        foreach (JiraNamedEntity entity in inwardIdsAndKeys) {
                            ilt.InwardLinks.Add(entity.Name);
                        }
                    }
                    issueLinks.Add(ilt);
                }
                it.Current.MoveToParent();
            }
            nav.MoveToParent();
        }

        private static List<JiraNamedEntity> getLinks(XPathNavigator nav) {
            XPathExpression expr = nav.Compile("issuelink/issuekey");
            XPathNodeIterator it = nav.Select(expr);

            List<JiraNamedEntity> result = new List<JiraNamedEntity>();
            if (nav.MoveToFirstChild()) {
                while (it.MoveNext()) {
                    int id = XPathUtils.getAttributeSafely(it.Current, "id", 0);
                    string key = it.Current.Value;
                    result.Add(new JiraNamedEntity(id, key, null));
                }
                nav.MoveToParent();
            }
            return result;
        }

        private void getComments(XPathNavigator nav) {
            XPathExpression expr = nav.Compile("comment");
            XPathNodeIterator it = nav.Select(expr);

            if (!nav.MoveToFirstChild()) return;
            while (it.MoveNext()) {
                Comment c = new Comment
                            {
                                Body = it.Current.Value,
                                Author = XPathUtils.getAttributeSafely(it.Current, "author", "Unknown"),
                                Created = XPathUtils.getAttributeSafely(it.Current, "created", "Unknown")
                            };
                comments.Add(c);
            }
            nav.MoveToParent();
        }

        private void getComments(JToken cmts, JToken rendered) {
            if (cmts == null) return;
            var list = rendered != null ? rendered["comments"] : cmts["comments"];
            var cmtTimes = new Dictionary<int, DateTime>();
            foreach (var cmt in cmts["comments"]) {
                cmtTimes[cmt["id"].Value<int>()] = DateTime.Parse(cmt["created"].Value<string>(), CultureInfo.InvariantCulture);
            }
            foreach (var comment in list) {
                var id = comment["id"].Value<int>();
                comments.Add(new Comment {
                                               Body = comment["body"].Value<string>(),
                                               Author = comment["author"] != null && comment["author"]["displayName"] != null 
                                                    ? comment["author"]["displayName"].Value<string>() 
                                                    : "Unknown",
                                               Created = JiraIssueUtils.getJiraFormattedTimeString(cmtTimes[id])
                                           });
            }
        }

        private void getSubtasks(XPathNavigator nav) {
            var expr = nav.Compile("subtask");
            var it = nav.Select(expr);

            if (!nav.MoveToFirstChild()) return;
            while (it.MoveNext()) {
                var subKey = it.Current.Value;
                SubtaskKeys.Add(subKey);
            }
            nav.MoveToParent();
        }

        private void getSubtasks(JToken subs) {
            if (subs == null) return;
            foreach (var sub in subs) {
                SubtaskKeys.Add(sub["key"].Value<string>());
            }
        }

        private void getTimeFieldsFromJson(JToken issue) {
            JToken r = issue["renderedFields"];
            JToken tt = issue["fields"]["timetracking"];
            if (r == null) return;
            OriginalEstimate = getValSafely<string>(r["timeoriginalestimate"], null);
            RemainingEstimate = getValSafely<string>(r["timeestimate"], null);
            TimeSpent = getValSafely<string>(r["timespent"], null);
            OriginalEstimateInSeconds = getValSafely(tt != null ? tt["originalEstimateSeconds"] : null, 0);
            RemainingEstimateInSeconds = getValSafely(tt != null ? tt["remainingEstimateSeconds"] : null, 0);
            TimeSpentInSeconds = getValSafely(tt != null ? tt["timeSpentSeconds"] : null, 0);
        }

        private static T getValSafely<T>(JToken jToken, T defVal) {
            return jToken == null ? defVal : jToken.Value<T>();
        }

        public JiraServer Server { get; private set; }

        public string IssueType { get; private set; }

        public int IssueTypeId { get; set; }

        public string ParentKey { get; set; }

        public bool IsSubtask { get { return ParentKey != null; } }

        public bool HasSubtasks { get { return subtaskKeys.Count > 0; } }

        public List<string> SubtaskKeys { get { return subtaskKeys; } }

        public List<JiraAttachment> Attachments { get { return attachments; } }

        private readonly List<string> subtaskKeys = new List<string>();

        public bool HasLinks { get { return issueLinks.Count > 0; } }

        public List<IssueLinkType> IssueLinks { get { return issueLinks; } }

        public string IssueTypeIconUrl { get; private set; }

        public string Description { get; set; }

        public int Id { get; private set; }

        public string Key { get; private set; }

        public string Summary { get; set; }

        public string Status { get; private set; }

        public int StatusId { get; private set; }

        public string StatusIconUrl { get; private set; }

        public string Priority { get; private set; }

        public string PriorityIconUrl { get; private set; }

        public int PriorityId { get; set; }

        public string Resolution { get; private set; }

        public int ResolutionId { get; private set; }

        public string Reporter { get; private set; }

        public string Assignee { get; set; }

        public DateTime CreationDate { get; private set; }

        public DateTime UpdateDate { get; private set; }

        public string ProjectKey { get; set; }

        public string Environment { get; private set; }

        public string OriginalEstimate { get; private set; }

        public int OriginalEstimateInSeconds { get; private set; }

        public string RemainingEstimate { get; set; }

        public int RemainingEstimateInSeconds { get; set; }

        public string TimeSpent { get; set; }

        public int TimeSpentInSeconds { get; set; }

        public List<Comment> Comments {
            get { return comments; }
        }

        public List<string> Versions {
            get { return versions; }
            set { versions = value; }
        }

        public List<string> FixVersions {
            get { return fixVersions; }
            set { fixVersions = value; }
        }

        public List<string> Components {
            get { return components; }
            set { components = value; }
        }

        public JiraNamedEntity SecurityLevel { get; set; }

        public bool HasAttachments { get { return Attachments != null && Attachments.Count > 0; } }

        public string ToolTipText {
            get {
                return
                    Key + "\r\n\r\n"
                    + "Type:\t\t" + IssueType + "\r\n"
                    + "Summary:\t" + (Summary.Length < 100 ? Summary : Summary.Substring(0, 100) + "...") + "\r\n"
                    + "Status:\t\t" + Status + "\r\n"
                    + "Priority:\t\t" + (Priority ?? "None") + "\r\n"
                    + "Reporter:\t" + JiraServerCache.Instance.getUsers(Server).getUser(Reporter) + "\r\n"
                    + "Assignee:\t" + JiraServerCache.Instance.getUsers(Server).getUser(Assignee) + "\r\n"
                    + "Resolution:\t" + (Resolution ?? "None") + "\r\n"
                    + "Created:\t\t" + JiraIssueUtils.getTimeStringFromIssueDateTime(CreationDate) + "\r\n"
                    + "Last Updated:\t" + JiraIssueUtils.getTimeStringFromIssueDateTime(UpdateDate);
            }
        }

        public bool Equals(JiraIssue other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            bool eq = true;
            eq &= other.Server.GUID.Equals(Server.GUID);
            eq &= other.IssueType.Equals(IssueType);
            eq &= other.IssueTypeId.Equals(IssueTypeId);
            eq &= other.IssueTypeIconUrl.Equals(IssueTypeIconUrl);
            eq &= string.Equals(other.Description, Description);
            eq &= other.Id == Id;
            eq &= other.Key.Equals(Key);
            eq &= string.Equals(other.Summary, Summary);
            eq &= other.Status.Equals(Status);
            eq &= other.StatusIconUrl.Equals(StatusIconUrl);
            eq &= Equals(other.Priority, Priority);
            eq &= string.Equals(other.Resolution, Resolution);
            eq &= other.Reporter.Equals(Reporter);
            eq &= string.Equals(other.Assignee, Assignee);
            eq &= other.CreationDate.Equals(CreationDate);
            eq &= other.UpdateDate.Equals(UpdateDate);
            eq &= other.ProjectKey.Equals(ProjectKey);
            eq &= string.Equals(other.Environment, Environment);
            eq &= string.Equals(other.OriginalEstimate, OriginalEstimate);
            eq &= string.Equals(other.RemainingEstimate, RemainingEstimate);
            eq &= string.Equals(other.TimeSpent, TimeSpent);
            eq &= string.Equals(other.ParentKey, ParentKey);
            eq &= Equals(other.PriorityIconUrl, PriorityIconUrl);
            eq &= other.StatusId == StatusId;
            eq &= other.PriorityId == PriorityId;
            eq &= PlvsUtils.compareLists(other.comments, comments);
            eq &= PlvsUtils.compareLists(other.versions, versions);
            eq &= PlvsUtils.compareLists(other.fixVersions, fixVersions);
            eq &= PlvsUtils.compareLists(other.components, components);
            eq &= PlvsUtils.compareLists(other.SubtaskKeys, SubtaskKeys);
            eq &= PlvsUtils.compareLists(other.Attachments, Attachments);
            eq &= PlvsUtils.compareLists(other.IssueLinks, IssueLinks);

            return eq;
        }

        public override int GetHashCode() {
            unchecked {
                int result = (comments != null ? comments.GetHashCode() : 0);
                result = (result*397) ^ (versions != null ? versions.GetHashCode() : 0);
                result = (result*397) ^ (fixVersions != null ? fixVersions.GetHashCode() : 0);
                result = (result*397) ^ (SubtaskKeys.GetHashCode());
                result = (result*397) ^ (IssueLinks.GetHashCode());
                result = (result*397) ^ (components != null ? components.GetHashCode() : 0);
                result = (result*397) ^ (attachments != null ? attachments.GetHashCode() : 0);
                result = (result*397) ^ (Server != null ? Server.GUID.GetHashCode() : 0);
                result = (result*397) ^ (IssueType != null ? IssueType.GetHashCode() : 0);
                result = (result*397) ^ IssueTypeId;
                result = (result*397) ^ (IssueTypeIconUrl != null ? IssueTypeIconUrl.GetHashCode() : 0);
                result = (result*397) ^ (Description != null ? Description.GetHashCode() : 0);
                result = (result*397) ^ Id;
                result = (result*397) ^ (Key != null ? Key.GetHashCode() : 0);
                result = (result*397) ^ (Summary != null ? Summary.GetHashCode() : 0);
                result = (result*397) ^ (Status != null ? Status.GetHashCode() : 0);
                result = (result*397) ^ (StatusIconUrl != null ? StatusIconUrl.GetHashCode() : 0);
                result = (result*397) ^ (Priority != null ? Priority.GetHashCode() : 0);
                result = (result*397) ^ (Resolution != null ? Resolution.GetHashCode() : 0);
                result = (result*397) ^ (Reporter != null ? Reporter.GetHashCode() : 0);
                result = (result*397) ^ (Assignee != null ? Assignee.GetHashCode() : 0);
                result = (result*397) ^ CreationDate.GetHashCode();
                result = (result*397) ^ UpdateDate.GetHashCode();
                result = (result*397) ^ (ProjectKey != null ? ProjectKey.GetHashCode() : 0);
                result = (result*397) ^ (Environment != null ? Environment.GetHashCode() : 0);
                result = (result*397) ^ (OriginalEstimate != null ? OriginalEstimate.GetHashCode() : 0);
                result = (result*397) ^ (RemainingEstimate != null ? RemainingEstimate.GetHashCode() : 0);
                result = (result*397) ^ (TimeSpent != null ? TimeSpent.GetHashCode() : 0);
                result = (result*397) ^ (ParentKey != null ? ParentKey.GetHashCode() : 0);
                result = (result*397) ^ (PriorityIconUrl != null ? PriorityIconUrl.GetHashCode() : 0);
                result = (result*397) ^ StatusId;
                result = (result*397) ^ PriorityId;
                return result;
            }
        }
    }
}