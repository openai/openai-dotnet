import "../../base/typespec/chat/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;


alias ChatCompletionCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ChatCompletionCollectionOrder;
};

union ChatCompletionCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ChatCompletionCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionLimitQueryParameter,
  ...ChatCompletionCollectionOrderQueryParameter,
  @query metadata?: Record<string>,
  @query `model`?: string,
}

alias ChatCompletionMessageCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ChatCompletionCollectionOrder;
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


