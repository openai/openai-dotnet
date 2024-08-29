using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("ToolResourcesFileSearch")]
[CodeGenSerialization(nameof(NewVectorStores), "vector_stores", SerializationValueHook = nameof(SerializeNewVectorStores))]
public partial class FileSearchToolResources
{
    private ChangeTrackingList<string> _vectorStoreIds = new();

    [CodeGenMember("VectorStoreIds")]
    public IList<string> VectorStoreIds
    {
        get => _vectorStoreIds;
        set
        {
            _vectorStoreIds = new ChangeTrackingList<string>();
            foreach (string item in value)
            {
                _vectorStoreIds.Add(item);
            }
        }
    }

    [CodeGenMember("VectorStores")]
    public IList<VectorStoreCreationHelper> NewVectorStores { get; } = new ChangeTrackingList<VectorStoreCreationHelper>();

    public FileSearchToolResources()
    { }

    private void SerializeNewVectorStores(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteObjectValue(NewVectorStores, options);
}

