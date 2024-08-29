using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("AssistantToolsFileSearch")]
[CodeGenSuppress(nameof(FileSearchToolDefinition))]
public partial class FileSearchToolDefinition : ToolDefinition
{
    public int? MaxResults
    {
        get => _fileSearch?.InternalMaxNumResults;
        set => _fileSearch.InternalMaxNumResults = value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FileSearchToolDefinition"/>.
    /// </summary>
    public FileSearchToolDefinition()
        : base("file_search")
    {
        _fileSearch = new InternalAssistantToolsFileSearchFileSearch();
    }

    [CodeGenMember("FileSearch")]
    private InternalAssistantToolsFileSearchFileSearch _fileSearch;
}
