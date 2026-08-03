import "../../base/typespec/vector-stores/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

namespace OpenAI;

@access(Access.public)
@usage(Usage.input | Usage.output)
union VectorStoreStatus {
  string,
  expired: "expired",
  in_progress: "in_progress",
  completed: "completed",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union VectorStoreFileStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  cancelled: "cancelled",
  failed: "failed",
}
