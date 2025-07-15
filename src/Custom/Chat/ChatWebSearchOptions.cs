using OpenAI.Internal;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("CreateChatCompletionRequestWebSearchOptions")]
public partial class ChatWebSearchOptions
{
    [CodeGenMember("UserLocation")]
    internal InternalCreateChatCompletionRequestWebSearchOptionsUserLocation1 UserLocation { get; set; }

    [CodeGenMember("SearchContextSize")]
    internal InternalWebSearchContextSize? SearchContextSize { get; set; }
}