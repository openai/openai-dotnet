import "../../base/typespec/vector-stores/main.tsp";
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
  ...CollectionAfterQueryParameter,
  ...CollectionBeforeQueryParameter,
  ...CollectionLimitQueryParameter,
  ...PageOrderQueryParameter,
}

@access(Access.public)
@usage(Usage.input)
model VectorStoreFileAssociationCollectionOptions {
  ...CollectionAfterQueryParameter,
  ...CollectionBeforeQueryParameter,
  ...CollectionLimitQueryParameter,
  ...PageOrderQueryParameter,

  @query filter?: ListVectorStoreFilesFilter;
}