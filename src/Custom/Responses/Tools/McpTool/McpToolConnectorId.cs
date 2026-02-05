using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPToolConnectorId")]
public readonly partial struct McpToolConnectorId
{
    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorDropbox")]
    public static McpToolConnectorId Dropbox { get; } = new McpToolConnectorId(ConnectorDropboxValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGmail")]
    public static McpToolConnectorId Gmail { get; } = new McpToolConnectorId(ConnectorGmailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGooglecalendar")]
    public static McpToolConnectorId GoogleCalendar { get; } = new McpToolConnectorId(ConnectorGooglecalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorGoogledrive")]
    public static McpToolConnectorId GoogleDrive { get; } = new McpToolConnectorId(ConnectorGoogledriveValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorMicrosoftteams")]
    public static McpToolConnectorId MicrosoftTeams { get; } = new McpToolConnectorId(ConnectorMicrosoftteamsValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookcalendar")]
    public static McpToolConnectorId OutlookCalendar { get; } = new McpToolConnectorId(ConnectorOutlookcalendarValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorOutlookemail")]
    public static McpToolConnectorId OutlookEmail { get; } = new McpToolConnectorId(ConnectorOutlookemailValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ConnectorSharepoint")]
    public static McpToolConnectorId SharePoint { get; } = new McpToolConnectorId(ConnectorSharepointValue);
}