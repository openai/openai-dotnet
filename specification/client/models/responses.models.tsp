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
  ...CollectionAfterQueryParameter,
  ...CollectionBeforeQueryParameter,
  ...CollectionLimitQueryParameter,
  ...ResponseItemCollectionOrderQueryParameter,
}
