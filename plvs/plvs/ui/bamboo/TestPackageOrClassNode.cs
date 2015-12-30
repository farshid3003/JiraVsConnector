using System.Drawing;

namespace Atlassian.plvs.ui.bamboo {
    internal class TestPackageOrClassNode {
        private readonly bool isPackage;
        public string Name { get; private set; }
        public Image Icon { get; private set; }

        public TestPackageOrClassNode(string name, bool isPackage) {
            this.isPackage = isPackage;
            Name = name;
            Icon = isPackage ? Resources.VSObject_Namespace : Resources.VSObject_Class;
        }

        public bool Equals(TestPackageOrClassNode other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.isPackage.Equals(isPackage) && Equals(other.Name, Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (TestPackageOrClassNode) && Equals((TestPackageOrClassNode) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (isPackage.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public override string ToString() {
            return Name;
        }
    }
}
