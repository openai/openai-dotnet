using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FileSearchToolCallItemResource")]
[CodeGenSuppress("FileSearchCallResponseItem")]
public partial class FileSearchCallResponseItem
{
    public FileSearchCallResponseItem() : this(ResponseItemKind.FileSearchCall, null, default, default, null, null)
    {
    }
}