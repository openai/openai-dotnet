using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolGAConnectorId")]
public readonly partial struct RealtimeMcpToolConnectorId
{
    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorDropbox")]
    public static RealtimeMcpToolConnectorId Dropbox { get; } = new RealtimeMcpToolConnectorId(ConnectorDropboxValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGmail")]
    public static RealtimeMcpToolConnectorId Gmail { get; } = new RealtimeMcpToolConnectorId(ConnectorGmailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGooglecalendar")]
    public static RealtimeMcpToolConnectorId GoogleCalendar { get; } = new RealtimeMcpToolConnectorId(ConnectorGooglecalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGoogledrive")]
    public static RealtimeMcpToolConnectorId GoogleDrive { get; } = new RealtimeMcpToolConnectorId(ConnectorGoogledriveValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorMicrosoftteams")]
    public static RealtimeMcpToolConnectorId MicrosoftTeams { get; } = new RealtimeMcpToolConnectorId(ConnectorMicrosoftteamsValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookcalendar")]
    public static RealtimeMcpToolConnectorId OutlookCalendar { get; } = new RealtimeMcpToolConnectorId(ConnectorOutlookcalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookemail")]
    public static RealtimeMcpToolConnectorId OutlookEmail { get; } = new RealtimeMcpToolConnectorId(ConnectorOutlookemailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorSharepoint")]
    public static RealtimeMcpToolConnectorId SharePoint { get; } = new RealtimeMcpToolConnectorId(ConnectorSharepointValue);
}