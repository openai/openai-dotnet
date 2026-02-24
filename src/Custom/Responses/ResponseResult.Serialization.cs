using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Responses
{
    public partial class ResponseResult : IJsonModel<ResponseResult>
    {
        private static void DeserializeInstructions(JsonProperty property, ref IReadOnlyList<ResponseItem> instructions)
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
                        items.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), ModelSerializationExtensions.WireOptions));
                    }
                }
                instructions = items;
            }
        }
    }
}
