/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// Tool generated type. Extracts from VectorStoreFileContentResponse.data
alias VectorStoreFileContentResponseData = {
  @doc("""
    The content type (currently only `"text"`)
    """)
  type?: string;

  /** The text content */
  text?: string;
};

/** The expiration policy for a vector store. */
model VectorStoreExpirationAfter {
  // Tool customization: Use enum instead of string constants to maintain backwards compatibility
  @doc("""
    Anchor timestamp after which the expiration policy applies. Supported anchors: `last_active_at`.
    """)
  anchor: VectorStoreExpirationAnchor.last_active_at;

  /** The number of days after the anchor time that the vector store will expire. */
  @minValue(1)
  @maxValue(365)
  days: int32;
}

@doc("""
  A vector store is a collection of processed files can be used by the `file_search` tool.
  """)
model VectorStoreObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `vector_store`.
    """)
  object: "vector_store";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the vector store was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The name of the vector store. */
  name: string;

  /** The total number of bytes used by the files in the vector store. */
  usage_bytes: int32;

  file_counts: {
    /** The number of files that are currently being processed. */
    in_progress: int32;

    /** The number of files that have been successfully processed. */
    completed: int32;

    /** The number of files that have failed to process. */
    failed: int32;

    /** The number of files that were cancelled. */
    cancelled: int32;

    /** The total number of files. */
    total: int32;
  };

  @doc("""
    The status of the vector store, which can be either `expired`, `in_progress`, or `completed`. A status of `completed` indicates that the vector store is ready for use.
    """)
  status: "expired" | "in_progress" | "completed";

  expires_after?: VectorStoreExpirationAfter;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the vector store will expire. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the vector store was last active. */
  @encode("unixTimestamp", int32)
  last_active_at: utcDateTime | null;

  ...MetadataPropertyForResponse;
}

model CreateVectorStoreRequest {
  @doc("""
    A list of [File](/docs/api-reference/files) IDs that the vector store should use. Useful for tools like `file_search` that can access files.
    """)
  @maxItems(500)
  file_ids?: string[];

  /** The name of the vector store. */
  name?: string;

  expires_after?: VectorStoreExpirationAfter;

  @doc("""
    The chunking strategy used to chunk the file(s). If not set, will use the `auto` strategy. Only applicable if `file_ids` is non-empty.
    """)
  chunking_strategy?: AutoChunkingStrategyRequestParam | StaticChunkingStrategyRequestParam;

  ...MetadataPropertyForRequest;
}

model UpdateVectorStoreRequest {
  /** The name of the vector store. */
  name?: string | null;

  expires_after?: VectorStoreExpirationAfter | null;
  ...MetadataPropertyForRequest;
}

model ListVectorStoresResponse {
  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "list";

  data: VectorStoreObject[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model DeleteVectorStoreResponse {
  id: string;
  deleted: boolean;
  object: "vector_store.deleted";
}

/** A list of files attached to a vector store. */
model VectorStoreFileObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `vector_store.file`.
    """)
  object: "vector_store.file";

  /** The total vector store usage in bytes. Note that this may be different from the original file size. */
  usage_bytes: int32;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the vector store file was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The ID of the [vector store](/docs/api-reference/vector-stores/object) that the [File](/docs/api-reference/files) is attached to. */
  vector_store_id: string;

  @doc("""
    The status of the vector store file, which can be either `in_progress`, `completed`, `cancelled`, or `failed`. The status `completed` indicates that the vector store file is ready for use.
    """)
  status: "in_progress" | "completed" | "cancelled" | "failed";

  @doc("""
    The last error associated with this vector store file. Will be `null` if there are no errors.
    """)
  last_error: {
    @doc("""
      One of `server_error` or `rate_limit_exceeded`.
      """)
    code: "server_error" | "unsupported_file" | "invalid_file";

    /** A human-readable description of the error. */
    message: string;
  } | null;

  // Tool customization: Use customized common base for chunking strategy response
  /** The strategy used to chunk the file. */
  chunking_strategy?: ChunkingStrategyResponseParam;

  attributes?: VectorStoreFileAttributes | null;
}

model CreateVectorStoreFileRequest {
  @doc("""
    A [File](/docs/api-reference/files) ID that the vector store should use. Useful for tools like `file_search` that can access files.
    """)
  file_id: string;

  chunking_strategy?: ChunkingStrategyRequestParam;
  attributes?: VectorStoreFileAttributes | null;
}

model ListVectorStoreFilesResponse {
  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "list";

  data: VectorStoreFileObject[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model DeleteVectorStoreFileResponse {
  id: string;
  deleted: boolean;
  object: "vector_store.file.deleted";
}

/** A batch of files attached to a vector store. */
model VectorStoreFileBatchObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `vector_store.file_batch`.
    """)
  object: "vector_store.files_batch";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the vector store files batch was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The ID of the [vector store](/docs/api-reference/vector-stores/object) that the [File](/docs/api-reference/files) is attached to. */
  vector_store_id: string;

  @doc("""
    The status of the vector store files batch, which can be either `in_progress`, `completed`, `cancelled` or `failed`.
    """)
  status: "in_progress" | "completed" | "cancelled" | "failed";

  file_counts: {
    /** The number of files that are currently being processed. */
    in_progress: int32;

    /** The number of files that have been processed. */
    completed: int32;

    /** The number of files that have failed to process. */
    failed: int32;

    /** The number of files that where cancelled. */
    cancelled: int32;

    /** The total number of files. */
    total: int32;
  };
}

model CreateVectorStoreFileBatchRequest {
  @doc("""
    A list of [File](/docs/api-reference/files) IDs that the vector store should use. Useful for tools like `file_search` that can access files.
    """)
  @minItems(1)
  @maxItems(500)
  file_ids: string[];

  chunking_strategy?: ChunkingStrategyRequestParam;
  attributes?: VectorStoreFileAttributes | null;
}

// Tool customization (apply_discriminator): Apply customized common base for chunking strategy response
@doc("""
  This is returned when the chunking strategy is unknown. Typically, this is because the file was indexed before the `chunking_strategy` concept was introduced in the API.
  """)
model OtherChunkingStrategyResponseParam extends ChunkingStrategyResponseParam {
  @doc("""
    Always `other`.
    """)
  type: "other";
}

// Tool customization (apply_discriminator): Apply customized common base for chunking strategy response
model StaticChunkingStrategyResponseParam
  extends ChunkingStrategyResponseParam {
  @doc("""
    Always `static`.
    """)
  type: "static";

  static: StaticChunkingStrategy;
}

model StaticChunkingStrategy {
  @doc("""
    The maximum number of tokens in each chunk. The default value is `800`. The minimum value is `100` and the maximum value is `4096`.
    """)
  @minValue(100)
  @maxValue(4096)
  max_chunk_size_tokens: int32;

  @doc("""
    The number of tokens that overlap between chunks. The default value is `400`.
    
    Note that the overlap must not exceed half of `max_chunk_size_tokens`.
    """)
  chunk_overlap_tokens: int32;
}

// Tool customization: Qualify union as discriminated base
@doc("""
  The chunking strategy used to chunk the file(s). If not set, will use the `auto` strategy.
  """)
@discriminator("type")
model ChunkingStrategyRequestParam {
  /** The type of chunking strategy. */
  type: "auto" | "static";
}

// Tool customization (apply_discriminator): apply custom common base
@doc("""
  The default strategy. This strategy currently uses a `max_chunk_size_tokens` of `800` and `chunk_overlap_tokens` of `400`.
  """)
model AutoChunkingStrategyRequestParam extends ChunkingStrategyRequestParam {
  @doc("""
    Always `auto`.
    """)
  type: "auto";
}

// Tool customization (apply_discriminator): apply custom common base
/** Customize your own chunking strategy by setting chunk size and chunk overlap. */
model StaticChunkingStrategyRequestParam extends ChunkingStrategyRequestParam {
  @doc("""
    Always `static`.
    """)
  type: "static";

  static: StaticChunkingStrategy;
}

model UpdateVectorStoreFileAttributesRequest {
  attributes: VectorStoreFileAttributes | null;
}

/** Represents the parsed content of a vector store file. */
model VectorStoreFileContentResponse {
  @doc("""
    The object type, which is always `vector_store.file_content.page`
    """)
  object: "vector_store.file_content.page";

  /** Parsed content of the file. */
  data: VectorStoreFileContentResponseData[];

  /** Indicates if there are more content pages to fetch. */
  has_more: boolean;

  /** The token for the next page, if any. */
  next_page: string | null;
}

model VectorStoreSearchRequest {
  /** A query string for a search */
  query: string | string[];

  /** Whether to rewrite the natural language query for vector search. */
  rewrite_query?: boolean = false;

  /** The maximum number of results to return. This number should be between 1 and 50 inclusive. */
  @minValue(1)
  @maxValue(50)
  max_num_results?: int32 = 10;

  /** A filter to apply based on file attributes. */
  filters?: ComparisonFilter | CompoundFilter;

  /** Ranking options for search. */
  ranking_options?: {
    ranker?: "auto" | "default-2024-11-15" = "auto";

    @minValue(0)
    @maxValue(1)
    score_threshold?: float32 = 0;
  };
}

model VectorStoreSearchResultContentObject {
  /** The type of content. */
  type: "text";

  /** The text content returned from search. */
  text: string;
}

model VectorStoreSearchResultItem {
  /** The ID of the vector store file. */
  file_id: string;

  /** The name of the vector store file. */
  filename: string;

  /** The similarity score for the result. */
  @minValue(0)
  @maxValue(1)
  score: float32;

  attributes: VectorStoreFileAttributes | null;

  /** Content chunks from the file. */
  content: VectorStoreSearchResultContentObject[];
}

model VectorStoreSearchResultsPage {
  @doc("""
    The object type, which is always `vector_store.search_results.page`
    """)
  object: "vector_store.search_results.page";

  search_query: string[];

  /** The list of search result items. */
  data: VectorStoreSearchResultItem[];

  /** Indicates if there are more results to fetch. */
  has_more: boolean;

  /** The token for the next page, if any. */
  next_page: string | null;
}
