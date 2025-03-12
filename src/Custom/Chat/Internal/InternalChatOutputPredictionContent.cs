using System.ClientModel.Primitives;
using System.Data;
using System.Text.Json;
using System;
using System.Collections;

namespace OpenAI.Chat;

[CodeGenType("ChatOutputPredictionContent")]
internal partial class InternalChatOutputPredictionContent
{
    // CUSTOM: Assign type to a collection of content parts
    [CodeGenMember("Content")]
    public ChatMessageContent Content { get; set; } = new();
}