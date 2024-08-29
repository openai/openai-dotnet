using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/*
 * This file stubs and performs minimal customization to generated public types for the OpenAI.Assistants namespace
 * that are not otherwise attributed elsewhere.
 */
[Experimental("OPENAI001")]
[CodeGenModel("AssistantToolsCode")]
public partial class CodeInterpreterToolDefinition : ToolDefinition { }

[Experimental("OPENAI001")]
[CodeGenModel("MessageObjectStatus")]
public readonly partial struct MessageStatus { }

[Experimental("OPENAI001")]
[CodeGenModel("MessageObjectIncompleteDetails")]
public partial class MessageFailureDetails { }

[Experimental("OPENAI001")]
[CodeGenModel("MessageObjectIncompleteDetailsReason")]
public readonly partial struct MessageFailureReason { }

[Experimental("OPENAI001")]
[CodeGenModel("RunCompletionUsage")]
public partial class RunTokenUsage { }

[Experimental("OPENAI001")]
[CodeGenModel("RunObjectLastError")]
public partial class RunError { }

[Experimental("OPENAI001")]
[CodeGenModel("RunObjectLastErrorCode")]
public readonly partial struct RunErrorCode { }

[Experimental("OPENAI001")]
[CodeGenModel("RunObjectIncompleteDetails")]
public partial class RunIncompleteDetails { }

[Experimental("OPENAI001")]
[CodeGenModel("RunObjectIncompleteDetailsReason")]
public readonly partial struct RunIncompleteReason { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepObjectType")]
public readonly partial struct RunStepType { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepObjectStatus")]
public readonly partial struct RunStepStatus { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepObjectLastError")]
public partial class RunStepError { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepObjectLastErrorCode")]
public readonly partial struct RunStepErrorCode { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepCompletionUsage")]
public partial class RunStepTokenUsage { }

[Experimental("OPENAI001")]
[CodeGenModel("RunStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject")]
public partial class RunStepCodeInterpreterOutput { }
