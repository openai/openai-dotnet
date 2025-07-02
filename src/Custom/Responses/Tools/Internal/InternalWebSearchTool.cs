namespace OpenAI.Responses;

[CodeGenType("WebSearchPreviewTool")]
internal partial class InternalWebSearchTool
{
    // CUSTOM: Use scenario-specific type copy
    [CodeGenMember("UserLocation")]
    internal WebSearchUserLocation UserLocation { get; set; }

    // CUSTOM: Apply use of a scenario-specific type copy.
    [CodeGenMember("SearchContextSize")]
    internal WebSearchContextSize? SearchContextSize { get; set; }
}