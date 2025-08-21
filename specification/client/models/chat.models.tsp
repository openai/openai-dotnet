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

alias ContainerCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ContainerCollectionOrder;
};

union ContainerCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionLimitQueryParameter,
  ...ContainerCollectionOrderQueryParameter,
}

alias ContainerFileCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ContainerCollectionOrder;
};

union ContainerFileCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerFileCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionLimitQueryParameter,
  ...ContainerFileCollectionOrderQueryParameter,
}

