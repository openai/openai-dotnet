import "../../base/typespec/batch/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

@access(Access.public)
@usage(Usage.input)
model BatchCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionLimitQueryParameter,
}


