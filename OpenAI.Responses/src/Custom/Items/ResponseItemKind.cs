using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ItemType")]
[CodeGenVisibility(nameof(Compaction), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(CompactionTrigger), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(CustomToolCall), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(CustomToolCallOutput), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(LocalShellCall), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(LocalShellCallOutput), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(ShellCall), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(ShellCallOutput), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(ToolSearchCall), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(ToolSearchOutput), CodeGenVisibility.Internal)]
public readonly partial struct ResponseItemKind
{
}
