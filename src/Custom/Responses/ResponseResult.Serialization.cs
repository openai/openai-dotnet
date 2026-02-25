using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Responses
{
    [CodeGenSerialization(nameof(Instructions), DeserializationValueHook = nameof(DeserializeInstructions))]
    public partial class ResponseResult : IJsonModel<ResponseResult>
    {
        // CUSTOM: Support instructions returned as either a string or an array of ResponseItem.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeserializeInstructions(JsonProperty property, ref IList<ResponseItem> instructions, ModelReaderWriterOptions options = null)
        {
            if (property.Value.ValueKind == JsonValueKind.Null)
            {
                instructions = new ChangeTrackingList<ResponseItem>();
            }
            else
            {
                List<ResponseItem> items = new List<ResponseItem>();
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    items.Add(ResponseItem.CreateDeveloperMessageItem(property.Value.GetString()));
                }
                else if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        items.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), options));
                    }
                }
                instructions = items;
            }
        }
    }
}
