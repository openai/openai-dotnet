using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.ToolDefinition>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public abstract partial class ToolDefinition : IJsonModel<ToolDefinition>
{
    void IJsonModel<ToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(ToolDefinition instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected abstract void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
}
