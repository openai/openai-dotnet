using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FileSearchTool")]
[CodeGenSuppress("FileSearchTool")]
public partial class FileSearchTool
{
    public FileSearchTool() : this(ResponseToolKind.FileSearch, default, null, default, null, null)
    {
    }
}