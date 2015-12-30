using System.Collections;
using System.Html;
using System.Gadgets;

namespace gadget {

    public class FlyoutScriptlet {

        private readonly InputElement buttonClose;

        private FlyoutScriptlet() {
            buttonClose = (InputElement)(Document.GetElementById("buttonClose"));
            buttonClose.AttachEvent("onclick", buttonCloseClick);
        }

        private static void buttonCloseClick() {
            Gadget.Flyout.Show = false;
        }

        public static void setIssueDetailsText(string text) {
            Element element = Gadget.Flyout.Document.GetElementById("issueDetails");
            element.InnerHTML = text;
        }

        public static void setIssueKeyAndType(string text) {
            Element element = Gadget.Flyout.Document.GetElementById("issueKeyLink");
            element.InnerHTML = text;
        }

        public static void Main(Dictionary arguments) {
            FlyoutScriptlet scriptlet = new FlyoutScriptlet();
        }
    }
}
