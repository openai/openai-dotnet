import "../../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

namespace OpenAI;

// Extensible enum unions for anonymous inline status types in the Responses API.

@access(Access.public)
@usage(Usage.input | Usage.output)
union ResponseStatus {
  string,
  completed: "completed",
  failed: "failed",
  in_progress: "in_progress",
  cancelled: "cancelled",
  queued: "queued",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union MessageStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union FunctionCallStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union FileSearchCallStatus {
  string,
  in_progress: "in_progress",
  searching: "searching",
  completed: "completed",
  incomplete: "incomplete",
  failed: "failed",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union WebSearchCallStatus {
  string,
  in_progress: "in_progress",
  searching: "searching",
  completed: "completed",
  failed: "failed",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union CodeInterpreterCallStatus {
  string,
  in_progress: "in_progress",
  interpreting: "interpreting",
  completed: "completed",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union ImageGenerationCallStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  generating: "generating",
  failed: "failed",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union ReasoningStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union ComputerCallActionMouseButton {
  string,
  left: "left",
  right: "right",
  wheel: "wheel",
  back: "back",
  forward: "forward",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union ComputerCallOutputStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union ComputerCallStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union FunctionCallOutputStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}
