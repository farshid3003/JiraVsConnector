// Guids.cs
// MUST match guids.h
using System;

namespace Atlassian.plvs {
    internal static class GuidList {
        public const string guidplvsPkgString = "36fa5f7f-2b5d-4cec-8c06-10c483683a16";
        public const string guidplvsCmdSetString = "85a7fbbb-c60c-4329-9f0f-2fdf1fa122e6";
        public const string guidToolWindowPersistanceString = "06c81945-10ef-4d72-8daf-32d29f7e9573";

        public static readonly Guid guidplvsCmdSet = new Guid(guidplvsCmdSetString);

        public const string GUID_JIRA_LINK_MARKER_SERVICE_STRING = "34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A";

        public const string JIRA_LINK_MARGIN_MARKER_STRING = "658DDF58-FC14-4db9-8110-B52A6845B6CF";

        public const string JIRA_LINK_TEXT_MARKER = "D7F03136-206D-4674-ADC7-DA0E9EE38869";

        public static readonly Guid GuidFontsAndColorsTextEditor = new Guid("a27b4e24-a735-4d1d-b8e7-9716e1e3d8e0");

        public static readonly Guid JiraLinkMarginMarker = new Guid(JIRA_LINK_MARGIN_MARKER_STRING);

        public static readonly Guid JiraLinkTextMarker = new Guid(JIRA_LINK_TEXT_MARKER);

        public static readonly Guid CSHARP_LANGUAGE_GUID = new Guid("694DD9B6-B865-4C5B-AD85-86356E9C88DC");
        public static readonly Guid C_AND_CPP_LANGUAGE_GUID = new Guid("B2F072B0-ABC1-11D0-9D62-00C04FD9DFD9");
        public static readonly Guid VB_LANGUAGE_GUID = new Guid("e34acdc0-baae-11d0-88bf-00a0c9110049");
    } ;
}