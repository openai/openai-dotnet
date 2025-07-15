using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeServerEvent")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RealtimeUpdate
{
    public BinaryData GetRawContent() => ModelReaderWriter.Write(this);

    internal static RealtimeUpdate FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeRealtimeUpdate(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}