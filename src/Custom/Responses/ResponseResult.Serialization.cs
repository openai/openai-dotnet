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
                List<ResponseItem> array = new List<ResponseItem>();
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    array.Add(ResponseItem.CreateDeveloperMessageItem(property.Value.GetString()));
                }
                else if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), ModelSerializationExtensions.WireOptions));
                    }
                }
                instructions = array;
            }
        }
    }
}
