using TypeSpec.OpenAPI;

namespace OpenAI;

// This customization allows us to concretely specify that the file_search object must provide
// either ID references --or-- in-line creation helpers, but not both.

alias CreateThreadRequestToolResourcesFileSearch = CreateThreadRequestToolResourcesFileSearchVectorStoreIdReferences | CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpers;

model CreateThreadRequestToolResourcesFileSearchVectorStoreIdReferences {
  ...CreateThreadRequestToolResourcesFileSearchBase;

  /**
   * The [vector store](/docs/api-reference/vector-stores/object) attached to this thread.
   * There can be a maximum of 1 vector store attached to the thread.
   */
  @maxItems(1)
  vector_store_ids?: string[];
}

model CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpers {
  ...CreateThreadRequestToolResourcesFileSearchBase;

  /**
   * A helper to create a [vector store](/docs/api-reference/vector-stores/object) with
   * file_ids and attach it to this thread. There can be a maximum of 1 vector store attached
   * to the thread.
   */
  @maxItems(1)
  vector_stores?: CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelper[];
}

model CreateThreadRequestToolResourcesFileSearchBase {
  // Common fields (currently none)
}

alias CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelper = {
  /**
   * A list of [file](/docs/api-reference/files) IDs to add to the vector store. There can be
   * a maximum of 10000 files in a vector store.
   */
  @maxItems(10000)
  file_ids?: string[];

  ...MetadataPropertyForRequest;
};
