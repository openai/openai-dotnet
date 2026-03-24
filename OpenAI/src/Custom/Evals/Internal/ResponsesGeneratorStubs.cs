using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Evals;

// CUSTOM: Generator stubs for responses types that leak into the OpenAI project
// through evals/models.tsp importing responses/models.tsp.
[CodeGenType("Tool")] internal partial class InternalTool {}
[CodeGenType("ToolType")] internal readonly partial struct InternalToolType {}
[CodeGenType("UnknownTool")] internal partial class InternalUnknownTool {}
[CodeGenType("ResponseTextFormatConfiguration")] internal partial class InternalResponseTextFormatConfiguration {}
[CodeGenType("ResponseTextFormatConfigurationType")] internal readonly partial struct InternalResponseTextFormatConfigurationType {}
[CodeGenType("ResponseTextFormatConfigurationText")] internal partial class InternalResponseTextFormatConfigurationText {}
[CodeGenType("ResponseTextFormatConfigurationJsonObject")] internal partial class InternalResponseTextFormatConfigurationJsonObject {}
[CodeGenType("ResponseTextFormatConfigurationJsonSchema")] internal partial class InternalResponseTextFormatConfigurationJsonSchema {}
[CodeGenType("UnknownResponseTextFormatConfiguration")] internal partial class InternalUnknownResponseTextFormatConfiguration {}
[CodeGenType("CodeInterpreterContainerConfiguration")] internal partial class InternalCodeInterpreterContainerConfiguration {}
[CodeGenType("CodeInterpreterContainerConfigurationType")] internal readonly partial struct InternalCodeInterpreterContainerConfigurationType {}
[CodeGenType("UnknownCodeInterpreterContainerConfiguration")] internal partial class InternalUnknownCodeInterpreterContainerConfiguration {}
[CodeGenType("DotNetResponseReasoningEffortLevel")] internal readonly partial struct InternalDotNetResponseReasoningEffortLevel {}
