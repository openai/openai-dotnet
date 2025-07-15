using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("AssistantToolsFileSearch")]
public partial class FileSearchToolDefinition : ToolDefinition
{
    public int? MaxResults
    {
        get => FileSearch?.InternalMaxNumResults;
        set => FileSearch.InternalMaxNumResults = value;
    }

    public FileSearchRankingOptions RankingOptions
    {
        get => FileSearch.RankingOptions;
        set => FileSearch.RankingOptions = value;
    }

    // CUSTOM: Ensure default constructor applies discriminator value and initializes inner object.
    public FileSearchToolDefinition() : this(kind: InternalAssistantToolDefinitionType.FileSearch, additionalBinaryDataProperties: null, fileSearch: new())
    { }
}
