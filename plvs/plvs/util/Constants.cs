using System.Text.RegularExpressions;

namespace Atlassian.plvs.util {
    public static class Constants {
        public const string INFO_CAPTION = "Atlassian Connector for Visual Studio - Information";
        public const string ERROR_CAPTION = "Atlassian Connector for Visual Studio - Error";
        public const string QUESTION_CAPTION = "Atlassian Connector for Visual Studio - Question";
        public const string USER_NOT_VALUDATED = "Warning! This field is not validated prior to sending to JIRA";

        public const string TIME_TRACKING_SYNTAX = "The format of this is '*w *d *h *m' (weeks, days, hours and minutes)";
        public const string TIME_TRACKING_REGEX = @"^\s*((\d+)w\s*)?((\d+)d\s*)?((\d+)h\s*)?((\d+)m\s*)?$";

        public const string INTEGRATE_WITH_ANKH = "integrate.with.ankh";

        public const string PAZU_REG_KEY = @"Software\Atlassian\Atlassian Connector for Visual Studio";
        public const string USER_AGENT = "Atlassian-VS-Connector";
    }
}
