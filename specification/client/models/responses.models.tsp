import "../../base/typespec/responses/main.tsp";
import "./common.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetItemReferenceItemResource extends ItemResource {
  type: ItemType.item_reference;
  ...ItemReferenceItemBase;
}

alias ResponseItemCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ResponseItemCollectionOrder;
};

union ResponseItemCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ResponseItemCollectionOptions {
  ...DotNetCollectionAfterQueryParameter;
  ...DotNetCollectionBeforeQueryParameter;
  ...DotNetCollectionLimitQueryParameter;
  ...ResponseItemCollectionOrderQueryParameter;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetGlobalToolCallApprovalPolicy {
  string,
  AlwaysRequireApproval: "always",
  NeverRequireApproval: "never",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCustomToolCallApprovalPolicy {
  /** A list of tools that always require approval. */
  always?: MCPToolFilter;

  /** A list of tools that never require approval. */
  `never`?: MCPToolFilter;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetToolCallApprovalPolicy {
  global_policy?: DotNetGlobalToolCallApprovalPolicy;
  custom_policy?: DotNetCustomToolCallApprovalPolicy;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCodeInterpreterToolContainer {
  container_id?: string;
  container?: CodeInterpreterContainerConfiguration;
}

@usage(Usage.input| Usage.json)
model DotNetCreateResponse {
    ...CreateResponse;
}
