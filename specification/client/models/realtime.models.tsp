import "../../base/typespec/realtime/main.tsp";
import "./common.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetRealtimeError_GA {
  type?: string;
  code?: string | null;
  message?: string;
  param?: string | null;
  event_id?: string | null;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetRealtimeSessionCreateRequestGADefaultTracing_GA {
  string,
  auto: "auto",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetRealtimeTracing_GA {
  default_tracing?: DotNetRealtimeSessionCreateRequestGADefaultTracing_GA;
  custom_tracing?: RealtimeSessionCreateRequestGATracing_GA;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetRealtimeDefaultTruncation_GA {
  string,
  auto: "auto",
  disabled: "disabled",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetRealtimeTruncation_GA {
  default_truncation?: DotNetRealtimeDefaultTruncation_GA;
  custom_truncation?: RealtimeTruncationBase_GA;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetRealtimeDefaultToolChoice_GA {
  string,
  none: "none",
  auto: "auto",
  required: "required",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetRealtimeToolChoice_GA {
  default_tool_choice?: DotNetRealtimeDefaultToolChoice_GA;
  custom_tool_choice?: ToolChoiceBase_GA;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetRealtimeDefaultToolCallApprovalPolicy_GA {
  string,
  always_require_approval: "always",
  never_require_approval: "never",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetRealtimeToolCallApprovalPolicy_GA {
  default_policy?: DotNetRealtimeDefaultToolCallApprovalPolicy_GA;
  custom_policy?: MCPToolRequireApproval_GA;
}