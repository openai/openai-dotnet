import "../../base/typespec/runs/main.tsp";
import "../../base/typespec/messages/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

namespace OpenAI;

// Extensible enum unions for anonymous inline types in the Assistants/Runs API.

@access(Access.public)
@usage(Usage.input | Usage.output)
union RunStepKind {
  string,
  message_creation: "message_creation",
  tool_calls: "tool_calls",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union AssistantMessageRole {
  string,
  user: "user",
  assistant: "assistant",
}
