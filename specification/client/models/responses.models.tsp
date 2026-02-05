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

alias DotNetResponseItemCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: DotNetResponseItemCollectionOrder;
};

union DotNetResponseItemCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model DotNetResponseItemCollectionOptions {
  @path response_id: string;
  ...DotNetCollectionAfterQueryParameter;
  ...DotNetCollectionBeforeQueryParameter;
  ...DotNetCollectionLimitQueryParameter;
  ...DotNetResponseItemCollectionOrderQueryParameter;
}

@access(Access.public)
@usage(Usage.input)
model DotNetGetResponseOptions {
  @path response_id: string;
  @query startingAfter?: int32;
  @query includeObfuscation?: boolean;
  @query includedProperties?: Includable[] | null;
  @query streamingEnabled?: boolean;
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