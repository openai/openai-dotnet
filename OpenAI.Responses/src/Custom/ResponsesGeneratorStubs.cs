using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("DotNetResponseDefaultToolChoice")] public readonly partial struct ResponseDefaultToolChoice {}
[CodeGenType("ToolChoiceAllowedMode")] public readonly partial struct ResponseToolChoiceAllowedMode {}
[CodeGenType("ToolChoiceObject")] public partial class ResponseCustomToolChoice {}
[CodeGenType("ToolChoiceObjectFunction")] public partial class ResponseCustomFunctionToolChoice {}
[CodeGenType("ToolChoiceObjectComputer")] public partial class ResponseCustomComputerToolChoice {}
[CodeGenType("ToolChoiceObjectComputerUsePreview")] public partial class ResponseCustomComputerUsePreviewToolChoice {}
[CodeGenType("ToolChoiceObjectFileSearch")] public partial class ResponseCustomFileSearchToolChoice {}
[CodeGenType("ToolChoiceObjectType")] public readonly partial struct ResponseCustomToolChoiceKind {}
[CodeGenType("ToolChoiceObjectWebSearch")] public partial class ResponseCustomWebSearchToolChoice {}
[CodeGenType("ToolChoiceObjectWebSearchPreview")] public partial class ResponseCustomWebSearchPreviewToolChoice {}
[CodeGenType("ToolChoiceObjectImageGen")] public partial class ResponseCustomImageGenerationToolChoice {}
[CodeGenType("ToolChoiceObjectCodeInterpreter")] public partial class ResponseCustomCodeInterpreterToolChoice {}
[CodeGenType("ToolChoiceObjectMCP")] public partial class ResponseCustomMcpToolChoice {}
[CodeGenType("ToolChoiceObjectCustom")] public partial class ResponseCustomCustomToolChoice {}
[CodeGenType("ToolChoiceObjectProgrammaticToolCalling")] public partial class ResponseCustomProgrammaticToolCallingToolChoice {}
[CodeGenType("ToolChoiceObjectApplyPatch")] public partial class ResponseCustomApplyPatchToolChoice {}
[CodeGenType("ToolChoiceObjectShell")] public partial class ResponseCustomShellToolChoice {}
[CodeGenType("ToolChoiceObjectAllowedTools")] public partial class ResponseCustomAllowedToolsToolChoice { }