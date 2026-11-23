using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants;

[CodeGenType("AssistantToolsFileSearch")]
public partial class FileSearchToolDefinition : ToolDefinition
{
    public int? MaxResults
    {
        get => FileSearch?.InternalMaxNumResults;
        set => (FileSearch ??= new()).InternalMaxNumResults = value;
    }

    // CUSTOM: "file_search" is optional on the wire (e.g., in message attachments), so FileSearch can be null.
    public FileSearchRankingOptions RankingOptions
    {
        get => FileSearch?.RankingOptions;
        set => (FileSearch ??= new()).RankingOptions = value;
    }

    // CUSTOM: Ensure default constructor applies discriminator value and initializes inner object.
    public FileSearchToolDefinition() : this(kind: InternalAssistantToolDefinitionType.FileSearch, additionalBinaryDataProperties: null, fileSearch: new())
    { }
}
