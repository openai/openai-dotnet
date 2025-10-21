using System.ClientModel.Primitives;
using OpenAI.Chat;
using OpenAI.Internal;

namespace OpenAI
{
    internal partial class ResponseFormatJsonSchema : ChatResponseFormat
    {
        internal ResponseFormatJsonSchema(ResponseFormatJsonSchemaJsonSchema jsonSchema) : base(InternalResponseFormatType.JsonSchema)
        {
            Argument.AssertNotNull(jsonSchema, nameof(jsonSchema));

            JsonSchema = jsonSchema;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseFormatJsonSchema(ResponseFormatType2 kind, in JsonPatch patch, ResponseFormatJsonSchemaJsonSchema jsonSchema) : base(kind, patch)
        {
            JsonSchema = jsonSchema;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        internal ResponseFormatJsonSchemaJsonSchema JsonSchema { get; set; }
    }
}
