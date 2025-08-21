import "../../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

alias ChatCompletionMessageCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ChatCompletionMessageCollectionOrder;
};

union ChatCompletionMessageCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ChatCompletionMessageCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionLimitQueryParameter,
  ...ChatCompletionMessageCollectionOrderQueryParameter,
}