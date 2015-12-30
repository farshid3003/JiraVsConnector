using System;

namespace Atlassian.plvs.attributes {

    public class StringValueAttribute : Attribute {

        public string StringValue { get; protected set; }

        public StringValueAttribute(string value) {
            StringValue = value;
        }
    }

    public class ColorValueAttribute : Attribute {

        public string ColorValue { get; protected set; }

        public ColorValueAttribute(string htmlColor) {
            ColorValue = htmlColor;
        }
    }

}