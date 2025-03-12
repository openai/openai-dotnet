using OpenAI.Internal;
using System;

namespace OpenAI.Chat;

[CodeGenType("CreateChatCompletionRequestWebSearchOptions")]
public partial class ChatWebSearchOptions
{
    [CodeGenMember("UserLocation")]
    internal InternalCreateChatCompletionRequestWebSearchOptionsUserLocation1 UserLocation { get; set; }

    [CodeGenMember("SearchContextSize")]
    internal InternalWebSearchContextSize? SearchContextSize { get; set; }
}