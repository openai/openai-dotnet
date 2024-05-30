using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenModel("AssistantObjectToolResources")]
[CodeGenSerialization(nameof(FileSearch), "file_search", SerializationValueHook = nameof(SerializeFileSearch))]
public partial class ToolResources
{
    /// <summary> Gets the code interpreter. </summary>
    public CodeInterpreterToolResources CodeInterpreter { get; init; }
    /// <summary> Gets the file search. </summary>
    public FileSearchToolResources FileSearch { get; init; }

    public ToolResources()
    {}

    private void SerializeFileSearch(Utf8JsonWriter writer)
        => writer.WriteObjectValue(FileSearch, new ModelReaderWriterOptions("J"));
}
