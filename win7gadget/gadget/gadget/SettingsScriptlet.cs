using System;
using System.Collections;
using System.Html;
using System.Gadgets;

namespace gadget {

    public class SettingsScriptlet {

        private static Element dropDownProjects;
        private static InputElement buttonTestConnection;
        private static InputElement buttonGetProjects;
        private static TextElement txtUrl;
        private static TextElement txtLogin;
        private static TextElement txtPassword;
        private static Element labelInfo;
        private static TextElement txtPollingInterval;
        private static CheckBoxElement chkHideResolved;

        private const string FILTERS_SELECT = "filters";
        private const string PROJECTS_SELECT = "projects";

        public const string SETTING_URL = "url";
        public const string SETTING_LOGIN = "login";
        public const string SETTING_PASSWORD = "password";

        public const string SETTING_FILTERNAME = "filterName";
        public const string SETTING_FILTERVALUE = "filterValue";

        public const string SETTING_PROJECTNAME = "projectName";
        public const string SETTING_PROJECTKEY = "projectKey";

        public const string SETTING_POLLING_INTERVAL = "pollingInterval";
        public const string SETTING_HIDE_RESOLVED = "hideResolved";

        private static bool haveProject;

        private SettingsScriptlet() {
            reinit();
        }

        private static void reinit() {
            Gadget.OnSettingsClosing = SettingsClosing;

            dropDownProjects = Document.GetElementById("projects");
            dropDownProjects.Disabled = true;

            txtUrl = (TextElement) Document.GetElementById("url");
            txtUrl.AttachEvent("onchange", txtUrlTextChanged);
            txtLogin = (TextElement) Document.GetElementById("login");
            txtPassword = (TextElement) Document.GetElementById("password");
            txtPollingInterval = (TextElement) Document.GetElementById("pollingInterval");

            buttonTestConnection = (InputElement)Document.GetElementById("testConnection");
            buttonTestConnection.AttachEvent("onclick", buttonTestConnectionClick);

            buttonGetProjects = (InputElement) Document.GetElementById("retrieveProjects");
            buttonGetProjects.AttachEvent("onclick", buttonGetProjectsClick);
            chkHideResolved = (CheckBoxElement)Document.GetElementById("hideresolved");

            labelInfo = Document.GetElementById("info");

            txtUrl.Value = Gadget.Settings.ReadString(SETTING_URL);
            txtLogin.Value = Gadget.Settings.ReadString(SETTING_LOGIN);
            txtPassword.Value = Gadget.Settings.ReadString(SETTING_PASSWORD);
            string interval = Gadget.Settings.ReadString(SETTING_POLLING_INTERVAL);
            txtPollingInterval.Value = string.IsNullOrEmpty(interval) ? "5" : interval;
            string val = Gadget.Settings.ReadString(SETTING_FILTERVALUE);
            if (!string.IsNullOrEmpty(val)) {
                optionreader.setselectedval(FILTERS_SELECT, val);
            }

            val = Gadget.Settings.ReadString(SETTING_HIDE_RESOLVED);
            chkHideResolved.Checked = !string.IsNullOrEmpty(val) && val.CompareTo("1") == 0;

            string projectKey = Gadget.Settings.ReadString(SETTING_PROJECTKEY);
            string projectName = Gadget.Settings.ReadString(SETTING_PROJECTNAME);
            if (!(string.IsNullOrEmpty(projectKey) || string.IsNullOrEmpty(projectName))) {
                optionreader.clearoptions(PROJECTS_SELECT);
                optionreader.addoption(PROJECTS_SELECT, projectKey, projectName);
                haveProject = true;
            }

            updateButtonStates();
        }

        private static void SettingsClosing(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;
            e.Cancel = !saveSettings();
        }

        private static void buttonGetProjectsClick() {
            labelInfo.Style.Color = "#000000";
            labelInfo.InnerHTML = "Retrieving Projects...";
            rpc.login(txtUrl.Value, txtLogin.Value, txtPassword.Value, gotTokenForGetProjects, connectionError);
        }

        private static void gotTokenForGetProjects(string token) {
            rpc.getprojects(txtUrl.Value, token, gotProjects, connectionError);
        }

        private static void gotProjects(object result) {
            labelInfo.Style.Color = "#000000";
            labelInfo.InnerHTML = "Retrieved Projects";

            string curKey = null;

            if (haveProject) {
                curKey = optionreader.getselectedval(PROJECTS_SELECT);
            }

            optionreader.clearoptions(PROJECTS_SELECT);

            Dictionary d = (Dictionary)result;
            foreach (DictionaryEntry entry in d) {
                int nr;
                try {
                    nr = int.Parse(entry.Key);
                } catch (Exception) {
                    continue;
                }
                if (Number.IsNaN(nr)) continue;

                addProject(entry.Value);
            }

            Document.GetElementById(PROJECTS_SELECT).Disabled = false;

            if (haveProject) {
                optionreader.setselectedval(PROJECTS_SELECT, curKey);
            }
            haveProject = true;
        }

        private static void addProject(object projectObject) {
            Dictionary d = (Dictionary)projectObject;
            string key = null;
            string name = null;
            foreach (DictionaryEntry entry in d) {
                if (entry.Key == "key") key = entry.Value.ToString();
                if (entry.Key == "name") name = entry.Value.ToString();
            }
            if (key == null || name == null) return;
            optionreader.addoption(PROJECTS_SELECT, key, name);
        }

        private static void txtUrlTextChanged() {
            updateButtonStates();
        }

        private static void updateButtonStates() {
            string url = txtUrl.Value;
            bool disabled = !isValidUrl(url);
            buttonTestConnection.Disabled = disabled;
            buttonGetProjects.Disabled = disabled;
//            labelInfo.InnerHTML = "val=" + txtUrl.Value;
        }

        private static bool isValidUrl(string url) {
            return !(url.Length == 0 || !(url.StartsWith("https://") || url.StartsWith("http://")) || url.EndsWith("://"));
        }

        private static void buttonTestConnectionClick() {
            labelInfo.Style.Color = "#000000";
            labelInfo.InnerHTML = "Testing Server Connection...";
            rpc.login(txtUrl.Value, txtLogin.Value, txtPassword.Value, gotLoginToken, connectionError);
        }

        private static void connectionError(string error) {
            labelInfo.Style.Color = "#ff0000";
            labelInfo.InnerHTML = "Connection error: " + error;
        }

        private static void gotLoginToken(string token) {
            labelInfo.Style.Color = "#000000";
            labelInfo.InnerHTML = "Connection successful";
        }

        public static void Main(Dictionary arguments) {
#pragma warning disable 168
            SettingsScriptlet scriptlet = new SettingsScriptlet();
#pragma warning restore 168
        }

        private static bool saveSettings() {
            if (!isValidUrl(txtUrl.Value) || !haveProject) return false;
            Gadget.Settings.WriteString(SETTING_URL, txtUrl.Value);
            Gadget.Settings.WriteString(SETTING_LOGIN, txtLogin.Value);
            Gadget.Settings.WriteString(SETTING_PASSWORD, txtPassword.Value);
            Gadget.Settings.WriteString(SETTING_FILTERVALUE, optionreader.getselectedval(FILTERS_SELECT));
            Gadget.Settings.WriteString(SETTING_FILTERNAME, optionreader.getselectedtext(FILTERS_SELECT));
            Gadget.Settings.Write(SETTING_PROJECTKEY, optionreader.getselectedval(PROJECTS_SELECT));
            Gadget.Settings.Write(SETTING_PROJECTNAME, optionreader.getselectedtext(PROJECTS_SELECT));
            if (Number.IsNaN(int.Parse(txtPollingInterval.Value))) {
                return false;
            }
            Gadget.Settings.WriteString(SETTING_POLLING_INTERVAL, txtPollingInterval.Value);
            Gadget.Settings.WriteString(SETTING_HIDE_RESOLVED, chkHideResolved.Checked ? "1" : "0");
            return true;
        }
    }
}
