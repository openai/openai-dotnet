import "../../base/typespec/assistants/main.tsp";
import "./common.models.tsp";
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
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
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
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...MessageCollectionOrderQueryParameter,
}

@access(Access.public)
@usage(Usage.input)
model RunCollectionOptions {
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...RunCollectionOrderQueryParameter,
}

alias RunCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunCollectionOrder;
};

union RunCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model RunStepCollectionOptions {
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...RunStepCollectionOrderQueryParameter,
}

alias RunStepCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunStepCollectionOrder;
};

union RunStepCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}