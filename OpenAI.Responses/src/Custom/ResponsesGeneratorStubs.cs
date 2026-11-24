using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("DotNetResponseDefaultToolChoice")] public readonly partial struct ResponseToolChoiceEnum  {}
[CodeGenType("ToolChoiceAllowedMode")] public readonly partial struct ResponseAllowedToolsChoiceMode  {}
[CodeGenType("ToolChoiceObject")] public partial class ResponseToolChoiceObject  {}
[CodeGenType("ToolChoiceObjectFunction")] public partial class ResponseFunctionToolChoice  {}
[CodeGenType("ToolChoiceObjectComputer")] public partial class ResponseComputerToolChoice  {}
[CodeGenType("ToolChoiceObjectComputerUsePreview")] public partial class ResponseComputerUsePreviewToolChoice  {}
[CodeGenType("ToolChoiceObjectFileSearch")] public partial class ResponseFileSearchToolChoice  {}
[CodeGenType("ToolChoiceObjectType")] public readonly partial struct ResponseToolChoiceKind {}
[CodeGenType("ToolChoiceObjectWebSearch")] public partial class ResponseWebSearchToolChoice  {}
[CodeGenType("ToolChoiceObjectWebSearchPreview")] public partial class ResponseWebSearchPreviewToolChoice  {}
[CodeGenType("ToolChoiceObjectImageGen")] public partial class ResponseImageGenerationToolChoice  {}
[CodeGenType("ToolChoiceObjectCodeInterpreter")] public partial class ResponseCodeInterpreterToolChoice  {}
[CodeGenType("ToolChoiceObjectMCP")] public partial class ResponseMcpToolChoice  {}
[CodeGenType("ToolChoiceObjectCustom")] public partial class ResponseCustomToolChoice  {}
[CodeGenType("ToolChoiceObjectProgrammaticToolCalling")] public partial class ResponseProgrammaticToolCallingToolChoice  {}
[CodeGenType("ToolChoiceObjectApplyPatch")] public partial class ResponseApplyPatchToolChoice  {}
[CodeGenType("ToolChoiceObjectShell")] public partial class ResponseShellToolChoice  {}
[CodeGenType("ToolChoiceObjectAllowedTools")] public partial class ResponseAllowedToolsChoice  { }