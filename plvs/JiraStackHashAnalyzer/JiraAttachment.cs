using System;

namespace JiraStackHashAnalyzer {
    public class JiraAttachment {

        public JiraAttachment(int id, string name, DateTime created, string author, int size) {
            Id = id;
            Name = name;
            Created = created;
            Author = author;
            Size = size;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime Created { get; private set; }
        public string Author { get; private set; }
        public int Size { get; private set; }

        public string RelativeUrl { get { return "secure/attachment/" + Id + "/" + Name; } }

        public bool Equals(JiraAttachment other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return other.Id == Id 
                && Equals(other.Name, Name) 
                && other.Created.Equals(Created) 
                && Equals(other.Author, Author) 
                && other.Size == Size;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof (JiraAttachment)) {
                return false;
            }
            return Equals((JiraAttachment) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int result = Id;
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Created.GetHashCode();
                result = (result*397) ^ (Author != null ? Author.GetHashCode() : 0);
                result = (result*397) ^ Size;
                return result;
            }
        }
    }
}
