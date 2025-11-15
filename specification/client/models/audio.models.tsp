import "../../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

@usage(Usage.output)
model DotNetCombinedJsonTranscriptionResponse {
  ...CreateTranscriptionResponseVerboseJson;
  logprobs?: LogProbProperties[];
}

@usage(Usage.output)
model DotNetTranscriptTextUsage {
  type: DotNetTranscriptTextUsageType;
}

union DotNetTranscriptTextUsageType {
  tokens: "tokens",
  duration: "duration",
  string
}