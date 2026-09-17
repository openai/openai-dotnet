using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ResponseSteerAcceptedEvent")]
public partial class StreamingResponseSteerAcceptedUpdate { }

[CodeGenType("ResponseSteerPendingEvent")]
public partial class StreamingResponseSteerPendingUpdate { }

[CodeGenType("ResponseSteerFailedEvent")]
public partial class StreamingResponseSteerFailedUpdate { }

[CodeGenType("ResponseSteerSubmission")]
public partial class ResponseSteerSubmission { }

[CodeGenType("ResponseSteerError")]
public partial class ResponseSteerError { }

[CodeGenType("ResponseSteerFailedSubmission")]
public partial class ResponseSteerFailedSubmission { }

[CodeGenType("ResponseSteerRequiredInput")]
public partial class ResponseSteerRequiredInput
{
    [CodeGenMember("Type")]
    public ResponseSteerRequiredInputKind Kind { get; }
}

[CodeGenType("ResponseSteerFunctionCallOutput")]
public partial class ResponseSteerFunctionCallOutput { }

[CodeGenType("ResponseSteerCustomToolCallOutput")]
public partial class ResponseSteerCustomToolCallOutput { }

[CodeGenType("ResponseSteerComputerCallOutput")]
public partial class ResponseSteerComputerCallOutput { }

[CodeGenType("ResponseSteerShellCallOutput")]
public partial class ResponseSteerShellCallOutput { }

[CodeGenType("ResponseSteerApplyPatchCallOutput")]
public partial class ResponseSteerApplyPatchCallOutput { }

[CodeGenType("ResponseSteerToolSearchOutput")]
public partial class ResponseSteerToolSearchOutput { }

[CodeGenType("ResponseSteerMcpApprovalResponse")]
public partial class ResponseSteerMcpApprovalResponse { }

[CodeGenType("ResponseSteerPendingReason")]
public readonly partial struct ResponseSteerPendingReason { }

[CodeGenType("ResponseSteerErrorCode")]
public readonly partial struct ResponseSteerErrorCode { }

[CodeGenType("ResponseSteerRequiredInputKind")]
public readonly partial struct ResponseSteerRequiredInputKind { }

[CodeGenType("UnknownResponseSteerRequiredInput")]
internal partial class InternalUnknownResponseSteerRequiredInput { }
