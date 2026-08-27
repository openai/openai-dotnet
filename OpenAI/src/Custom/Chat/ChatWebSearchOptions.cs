using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("CreateChatCompletionRequestWebSearchOptions")]
public partial class ChatWebSearchOptions
{
    [CodeGenMember("UserLocation")]
    internal InternalCreateChatCompletionRequestWebSearchOptionsUserLocation1 UserLocation { get; set; }
}