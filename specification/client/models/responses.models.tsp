import "../../base/typespec/responses/main.tsp";
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
  ...CollectionAfterQueryParameter;
  ...CollectionBeforeQueryParameter;
  ...CollectionLimitQueryParameter;
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
  always?: {
    /** List of tools that require approval. */
    tool_names?: string[];
  };

  /** A list of tools that never require approval. */
  `never`?: {
    /** List of tools that do not require approval. */
    tool_names?: string[];
  };
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetToolCallApprovalPolicy {
  global_policy?: DotNetGlobalToolCallApprovalPolicy;
  custom_policy?: DotNetCustomToolCallApprovalPolicy;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCodeInterpreterContainer {
  container_id?: string;
  container?: CodeInterpreterContainerConfiguration;
}
