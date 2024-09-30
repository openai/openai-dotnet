﻿using System.Collections.Generic;

namespace OpenAI.Chat;

[CodeGenModel("ChatCompletionStreamResponseDelta")]
[CodeGenSuppress("InternalChatCompletionStreamResponseDelta")]
[CodeGenSerialization(nameof(Content), SerializationValueHook = nameof(SerializeContentValue), DeserializationValueHook = nameof(DeserializeContentValue))]
internal partial class InternalChatCompletionStreamResponseDelta
{
    // CUSTOM: Changed type from string.
    /// <summary> The role of the author of this message. </summary>
    [CodeGenMember("Role")]
    public ChatMessageRole? Role { get; }

    // CUSTOM: Changed type from string.
    /// <summary> The contents of the message. </summary>
    [CodeGenMember("Content")]
    public ChatMessageContent Content { get; }
}
