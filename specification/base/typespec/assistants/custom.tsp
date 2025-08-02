import "@azure-tools/typespec-client-generator-core";
import "@typespec/http";
import "../common/models.tsp";
import "../vector-stores/models.tsp";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;
using TypeSpec.Http;

namespace OpenAI;

// This customization allows us to concretely specify that the file_search object must provide
// either ID references --or-- in-line creation helpers, but not both.

model ToolResourcesFileSearch {
  ...ToolResourcesFileSearchIdsOnly;
  ...ToolResourcesFileSearchVectorStoreCreationHelpers;
}

model ToolResourcesFileSearchIdsOnly {
  /**
   * The [vector store](/docs/api-reference/vector-stores/object) attached to this assistant.
   * There can be a maximum of 1 vector store attached to the assistant.
   */
  @maxItems(1)
  vector_store_ids?: string[];
}

model ToolResourcesFileSearchVectorStoreCreationHelpers {
  /**
   * A helper to create a [vector store](/docs/api-reference/vector-stores/object) with
   * file_ids and attach it to this assistant. There can be a maximum of 1 vector store
   * attached to the assistant.
   */
  @maxItems(1)
  vector_stores?: ToolResourcesFileSearchVectorStoreCreationHelper[];
}

alias ToolResourcesFileSearchVectorStoreCreationHelper = {
  /**
   * A list of [file](/docs/api-reference/files) IDs to add to the vector store. There can be
   * a maximum of 10000 files in a vector store.
   */
  @maxItems(10000)
  file_ids?: string[];

  @doc("""
    The chunking strategy used to chunk the file(s). If not set, will use the `auto` strategy. Only applicable if `file_ids` is non-empty.
    """)
  @extension("x-oaiExpandable", true)
  chunking_strategy?: ChunkingStrategyRequestParam;

  ...MetadataPropertyForRequest;
};

union AssistantToolDefinitionType {
  code_interpreter: "code_interpreter",
  file_search: "file_search",
  function: "function",
}

@discriminator("type")
model AssistantToolDefinition {
  type: AssistantToolDefinitionType;
}
