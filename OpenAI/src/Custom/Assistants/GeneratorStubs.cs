using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants;

/*
 * This file stubs and performs minimal customization to generated public types for the OpenAI.Assistants namespace
 * that are not otherwise attributed elsewhere.
 */

[CodeGenType("AssistantCollectionOptions")]
public partial class AssistantCollectionOptions { }

[CodeGenType("AssistantCollectionOrder")]
public readonly partial struct AssistantCollectionOrder { }

[CodeGenType("AssistantToolsCode")]
public partial class CodeInterpreterToolDefinition { }

[CodeGenType("MessageCollectionOptions")]
public partial class MessageCollectionOptions { }

[CodeGenType("MessageCollectionOrder")]
public readonly partial struct MessageCollectionOrder { }

[CodeGenType("MessageObjectStatus")]
public readonly partial struct MessageStatus { }

[CodeGenType("MessageObjectIncompleteDetails1")]
public partial class MessageFailureDetails { }

[CodeGenType("MessageObjectIncompleteDetailsReason")]
public readonly partial struct MessageFailureReason { }

[CodeGenType("RunCollectionOptions")]
public partial class RunCollectionOptions { }

[CodeGenType("RunCollectionOrder")]
public readonly partial struct RunCollectionOrder { }

[CodeGenType("RunObjectLastError1")]
public partial class RunError { }

[CodeGenType("RunObjectLastErrorCode")]
public readonly partial struct RunErrorCode { }

[CodeGenType("RunObjectIncompleteDetails1")]
public partial class RunIncompleteDetails { }

[CodeGenType("RunStepCollectionOptions")]
public partial class RunStepCollectionOptions { }

[CodeGenType("RunStepCollectionOrder")]
public readonly partial struct RunStepCollectionOrder { }

[CodeGenType("RunStepObjectStatus")]
public readonly partial struct RunStepStatus { }

[CodeGenType("RunStepObjectLastError1")]
public partial class RunStepError { }

[CodeGenType("RunStepObjectLastErrorCode")]
public readonly partial struct RunStepErrorCode { }

[CodeGenType("RunStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject")]
public partial class RunStepCodeInterpreterOutput { }