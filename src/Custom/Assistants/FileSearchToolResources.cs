using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("ToolResourcesFileSearch")]
[CodeGenVisibility(nameof(FileSearchToolResources), CodeGenVisibility.Public)]
[CodeGenSerialization(nameof(NewVectorStores), "vector_stores", SerializationValueHook = nameof(SerializeNewVectorStores))]
public partial class FileSearchToolResources
{
    [CodeGenMember("VectorStores")]
    public IList<VectorStoreCreationHelper> NewVectorStores { get; }

    private void SerializeNewVectorStores(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteObjectValue(NewVectorStores, options);
}

