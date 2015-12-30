using System;
using System.Collections.Generic;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    public class IssueLinkType {
        public IssueLinkType(int id, string name, string outwardLinksName, string inwardLinksName) {
            Id = id;
            Name = name;
            OutwardLinksName = outwardLinksName;
            InwardLinksName = inwardLinksName;
            OutwardLinks = new List<string>();
            InwardLinks = new List<string>();
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string OutwardLinksName { get; set; }
        public string InwardLinksName { get; set; }
        public List<string> OutwardLinks { get; private set; }
        public List<string> InwardLinks { get; private set; }

        public bool Equals(IssueLinkType other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id 
                && Equals(other.Name, Name) 
                && Equals(other.OutwardLinksName, OutwardLinksName) 
                && Equals(other.InwardLinksName, InwardLinksName)
                && PlvsUtils.compareLists(other.OutwardLinks, OutwardLinks) 
                && PlvsUtils.compareLists(other.InwardLinks, InwardLinks);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return 
                obj.GetType() == typeof (IssueLinkType) 
                && Equals((IssueLinkType) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int result = Id;
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (OutwardLinksName != null ? OutwardLinksName.GetHashCode() : 0);
                result = (result*397) ^ (InwardLinksName != null ? InwardLinksName.GetHashCode() : 0);
                result = (result*397) ^ (OutwardLinks != null ? OutwardLinks.GetHashCode() : 0);
                result = (result*397) ^ (InwardLinks != null ? InwardLinks.GetHashCode() : 0);
                return result;
            }
        }
    }
}
