using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Replace the generated writer so external subclasses serialize patches as well as their discriminator.
[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Decisions.DecisionsContentPart>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class DecisionsContentPart : IJsonModel<DecisionsContentPart>
{
    void IJsonModel<DecisionsContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }

        writer.WriteStartObject();
        JsonModelWriteCore(writer, options);
        // CUSTOM: Avoid duplicating patches already written by generated derived cores.
        if (this is not DecisionsInputTextContentPart and not DecisionsInputImageContentPart and not UnknownDecisionsContentPart)
        {
            Patch.WriteTo(writer);
        }
        writer.WriteEndObject();
    }
}
