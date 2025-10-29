import "../../base/typespec/containers/main.tsp";
import "./common.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

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
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
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
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...ContainerFileCollectionOrderQueryParameter,
}