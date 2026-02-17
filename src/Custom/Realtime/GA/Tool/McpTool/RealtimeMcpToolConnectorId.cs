using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolGAConnectorId")]
public readonly partial struct GARealtimeMcpToolConnectorId
{
    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorDropbox")]
    public static GARealtimeMcpToolConnectorId Dropbox { get; } = new GARealtimeMcpToolConnectorId(ConnectorDropboxValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGmail")]
    public static GARealtimeMcpToolConnectorId Gmail { get; } = new GARealtimeMcpToolConnectorId(ConnectorGmailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGooglecalendar")]
    public static GARealtimeMcpToolConnectorId GoogleCalendar { get; } = new GARealtimeMcpToolConnectorId(ConnectorGooglecalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGoogledrive")]
    public static GARealtimeMcpToolConnectorId GoogleDrive { get; } = new GARealtimeMcpToolConnectorId(ConnectorGoogledriveValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorMicrosoftteams")]
    public static GARealtimeMcpToolConnectorId MicrosoftTeams { get; } = new GARealtimeMcpToolConnectorId(ConnectorMicrosoftteamsValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookcalendar")]
    public static GARealtimeMcpToolConnectorId OutlookCalendar { get; } = new GARealtimeMcpToolConnectorId(ConnectorOutlookcalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookemail")]
    public static GARealtimeMcpToolConnectorId OutlookEmail { get; } = new GARealtimeMcpToolConnectorId(ConnectorOutlookemailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorSharepoint")]
    public static GARealtimeMcpToolConnectorId SharePoint { get; } = new GARealtimeMcpToolConnectorId(ConnectorSharepointValue);
}