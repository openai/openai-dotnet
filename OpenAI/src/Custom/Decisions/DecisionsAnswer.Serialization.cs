using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Decisions.DecisionsAnswer>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class DecisionsAnswer : IJsonModel<DecisionsAnswer>
{
    void IJsonModel<DecisionsAnswer>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }

        writer.WriteStartObject();
        JsonModelWriteCore(writer, options);
        // Generated derived cores already flush the patch; external subclasses inherit the base core, which does not.
        if (this is not DecisionsChoiceAnswer and not DecisionsPredicateAnswer and not DecisionsRefusalAnswer and not DecisionsScoreAnswer and not UnknownDecisionsAnswer)
        {
            Patch.WriteTo(writer);
        }
        writer.WriteEndObject();
    }
}
