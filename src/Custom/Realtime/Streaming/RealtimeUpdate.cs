using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeServerEvent")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RealtimeUpdate
{
    public BinaryData GetRawContent() => ModelReaderWriter.Write(this, ModelReaderWriterOptions.Json, OpenAIContext.Default);
}