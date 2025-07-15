import "../../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

@access(Access.public)
@usage(Usage.output)
model DotNetCombinedJsonTranscriptionResponse {
  ...CreateTranscriptionResponseVerboseJson;
  logprobs?: LogProbProperties[];
}
