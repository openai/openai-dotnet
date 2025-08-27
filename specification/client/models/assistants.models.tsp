import "../../base/typespec/assistants/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

alias AssistantCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: AssistantCollectionOrder;
};

union AssistantCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model AssistantCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionBeforeQueryParameter,
  ...CollectionLimitQueryParameter,
  ...AssistantCollectionOrderQueryParameter,
}



alias MessageCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: MessageCollectionOrder;
};

union MessageCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model MessageCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionBeforeQueryParameter,
  ...CollectionLimitQueryParameter,
  ...MessageCollectionOrderQueryParameter,
}