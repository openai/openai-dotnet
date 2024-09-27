using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatResponseFormat>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatResponseFormat : IJsonModel<ChatResponseFormat>
{
    void IJsonModel<ChatResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(ChatResponseFormat instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal virtual void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => throw new InvalidOperationException($"The {nameof(WriteCore)} method should be invoked on an overriding type derived from {nameof(ChatResponseFormat)}.");
}
