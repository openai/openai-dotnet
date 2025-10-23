using System.ClientModel.Primitives;

namespace OpenAI.Chat
{
    public partial class ResponseFormatJsonSchema : ResponseFormat
    {
        public ResponseFormatJsonSchema(ResponseFormatJsonSchemaJsonSchema jsonSchema) : base(ResponseFormatType.JsonSchema)
        {
            Argument.AssertNotNull(jsonSchema, nameof(jsonSchema));

            JsonSchema = jsonSchema;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        public ResponseFormatJsonSchema(ResponseFormatType kind, in JsonPatch patch, ResponseFormatJsonSchemaJsonSchema jsonSchema) : base(kind, patch)
        {
            JsonSchema = jsonSchema;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        public ResponseFormatJsonSchemaJsonSchema JsonSchema { get; set; }
    }
}
