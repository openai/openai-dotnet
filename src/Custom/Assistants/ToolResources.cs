using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("AssistantObjectToolResources")]
[CodeGenSerialization(nameof(FileSearch), "file_search", SerializationValueHook = nameof(SerializeFileSearch))]
public partial class ToolResources
{
    /// <summary> Gets the code interpreter. </summary>
    public CodeInterpreterToolResources CodeInterpreter { get; set; }
    /// <summary> Gets the file search. </summary>
    public FileSearchToolResources FileSearch { get; set; }

    public ToolResources()
    { }

    private void SerializeFileSearch(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteObjectValue(FileSearch, options);
}
