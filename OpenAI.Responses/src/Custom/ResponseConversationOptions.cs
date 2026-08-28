using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ConversationParam2")]
[CodeGenSuppress("ResponseConversationOptions")]
public partial class ResponseConversationOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ConversationId { get; set; }
}