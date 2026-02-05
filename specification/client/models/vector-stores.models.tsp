import "../../base/typespec/vector-stores/main.tsp";
import "./common.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

// This artificial type facilitates the merged representation of input/output chunking strategy values.

@discriminator("type")
model DotNetCombinedChunkingStrategyParam {
  type: "auto" | "static" | "other";
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCombinedAutoChunkingStrategyParam
  extends DotNetCombinedChunkingStrategyParam {
  ...AutoChunkingStrategyRequestParam;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCombinedStaticChunkingStrategyParam
  extends DotNetCombinedChunkingStrategyParam {
  ...StaticChunkingStrategyResponseParam;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCombinedOtherChunkingStrategyParam
  extends DotNetCombinedChunkingStrategyParam {
  ...OtherChunkingStrategyResponseParam;
}

@access(Access.public)
@usage(Usage.input)
model VectorStoreCollectionOptions {
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...PageOrderQueryParameter,
}

alias VectorStoreFileCollectionOrderQueryParameter = {
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: VectorStoreFileCollectionOrder;
};

union VectorStoreFileCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model VectorStoreFileCollectionOptions {
  ...DotNetCollectionAfterQueryParameter,
  ...DotNetCollectionBeforeQueryParameter,
  ...DotNetCollectionLimitQueryParameter,
  ...VectorStoreFileCollectionOrderQueryParameter,

  @query filter?: ListVectorStoreFilesFilter;
}