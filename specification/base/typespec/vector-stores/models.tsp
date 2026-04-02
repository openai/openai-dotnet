import "@typespec/openapi";
import "../common/models.tsp";
import "../specialized-types/models.tsp";
using TypeSpec.OpenAPI;
namespace OpenAI;
/** The default strategy. This strategy currently uses a `max_chunk_size_tokens` of `800` and `chunk_overlap_tokens` of `400`. */
@summary("Auto Chunking Strategy")
model AutoChunkingStrategyRequestParam extends ChunkingStrategyRequestParam {
  /** Always `auto`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "auto";
}

/** Customize your own chunking strategy by setting chunk size and chunk overlap. */
@summary("Static Chunking Strategy")
model StaticChunkingStrategyRequestParam extends ChunkingStrategyRequestParam {
  /** Always `static`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "static";

  static: StaticChunkingStrategy;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model StaticChunkingStrategy {
  /** The maximum number of tokens in each chunk. The default value is `800`. The minimum value is `100` and the maximum value is `4096`. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(4096)
  @minValue(100)
  max_chunk_size_tokens: integer;

  /**The number of tokens that overlap between chunks. The default value is `400`.
  Note that the overlap must not exceed half of `max_chunk_size_tokens`.*/
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunk_overlap_tokens: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateVectorStoreFileBatchRequest {
  /** A list of [File](https://platform.openai.com/docs/api-reference/files) IDs that the vector store should use. Useful for tools like `file_search` that can access files.  If `attributes` or `chunking_strategy` are provided, they will be  applied to all files in the batch. Mutually exclusive with `files`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxItems(500)
  @minItems(1)
  file_ids?: string[];

  /** A list of objects that each include a `file_id` plus optional `attributes` or `chunking_strategy`. Use this when you need to override metadata for specific files. The global `attributes` or `chunking_strategy` will be ignored and must be specified for each file. Mutually exclusive with `file_ids`. */
  @maxItems(500)
  @minItems(1)
  files?: CreateVectorStoreFileRequest[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunking_strategy?: ChunkingStrategyRequestParam;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  attributes?: VectorStoreFileAttributes | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateVectorStoreFileRequest {
  /** A [File](https://platform.openai.com/docs/api-reference/files) ID that the vector store should use. Useful for tools like `file_search` that can access files. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  file_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunking_strategy?: ChunkingStrategyRequestParam;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  attributes?: VectorStoreFileAttributes | null;
}

/** The chunking strategy used to chunk the file(s). If not set, will use the `auto` strategy. Only applicable if `file_ids` is non-empty. */
@discriminator("type")
model ChunkingStrategyRequestParam {
  type: ChunkingStrategyRequestParamType;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateVectorStoreRequest {
  /** A list of [File](https://platform.openai.com/docs/api-reference/files) IDs that the vector store should use. Useful for tools like `file_search` that can access files. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxItems(500)
  file_ids?: string[];

  /** The name of the vector store. */
  name?: string;

  /** A description for the vector store. Can be used to describe the vector store's purpose. */
  description?: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  expires_after?: VectorStoreExpirationAfter;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunking_strategy?: ChunkingStrategyRequestParam;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  metadata?: Metadata | null;
}

/** The expiration policy for a vector store. */
@summary("Vector store expiration policy")
model VectorStoreExpirationAfter {
  /** Anchor timestamp after which the expiration policy applies. Supported anchors: `last_active_at`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  anchor: "last_active_at";

  /** The number of days after the anchor time that the vector store will expire. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(365)
  @minValue(1)
  days: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeleteVectorStoreFileResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.file.deleted";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeleteVectorStoreResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.deleted";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ListVectorStoreFilesResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  object: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @pageItems
  @extension("x-ms-list-page-items", true)
  data: VectorStoreFileObject[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @continuationToken
  @extension("x-ms-list-continuation-token", true)
  last_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

/** A list of files attached to a vector store. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Vector store files")
@extension(
  "x-oaiMeta",
  #{
    name: "The vector store file object",
    beta: true,
    example: """
      {
        "id": "file-abc123",
        "object": "vector_store.file",
        "usage_bytes": 1234,
        "created_at": 1698107661,
        "vector_store_id": "vs_abc123",
        "status": "completed",
        "last_error": null,
        "chunking_strategy": {
          "type": "static",
          "static": {
            "max_chunk_size_tokens": 800,
            "chunk_overlap_tokens": 400
          }
        }
      }
      
      """,
  }
)
model VectorStoreFileObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  /** The object type, which is always `vector_store.file`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.file";

  /** The total vector store usage in bytes. Note that this may be different from the original file size. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  usage_bytes: integer;

  /** The Unix timestamp (in seconds) for when the vector store file was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** The ID of the [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) that the [File](https://platform.openai.com/docs/api-reference/files) is attached to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  vector_store_id: string;

  /** The status of the vector store file, which can be either `in_progress`, `completed`, `cancelled`, or `failed`. The status `completed` indicates that the vector store file is ready for use. */
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  status: "in_progress" | "completed" | "cancelled" | "failed";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  last_error: VectorStoreFileObjectLastError | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunking_strategy?: ChunkingStrategyResponse;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  attributes?: VectorStoreFileAttributes | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VectorStoreFileObjectLastError {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  code: "server_error" | "unsupported_file" | "invalid_file";
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  message: string;
}

/** The strategy used to chunk the file. */
@discriminator("type")
model ChunkingStrategyResponse {
  type: ChunkingStrategyResponseType;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@summary("Static Chunking Strategy")
model StaticChunkingStrategyResponseParam extends ChunkingStrategyResponse {
  /** Always `static`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "static";

  static: StaticChunkingStrategy;
}

/** This is returned when the chunking strategy is unknown. Typically, this is because the file was indexed before the `chunking_strategy` concept was introduced in the API. */
@summary("Other Chunking Strategy")
model OtherChunkingStrategyResponseParam extends ChunkingStrategyResponse {
  /** Always `other`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "other";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ListVectorStoresResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  object: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @pageItems
  @extension("x-ms-list-page-items", true)
  data: VectorStoreObject[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @continuationToken
  @extension("x-ms-list-continuation-token", true)
  last_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

/** A vector store is a collection of processed files can be used by the `file_search` tool. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Vector store")
@extension(
  "x-oaiMeta",
  #{
    name: "The vector store object",
    example: """
      {
        "id": "vs_123",
        "object": "vector_store",
        "created_at": 1698107661,
        "usage_bytes": 123456,
        "last_active_at": 1698107661,
        "name": "my_vector_store",
        "status": "completed",
        "file_counts": {
          "in_progress": 0,
          "completed": 100,
          "cancelled": 0,
          "failed": 0,
          "total": 100
        },
        "last_used_at": 1698107661
      }
      
      """,
  }
)
model VectorStoreObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  /** The object type, which is always `vector_store`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store";

  /** The Unix timestamp (in seconds) for when the vector store was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** The name of the vector store. */
  name: string;

  /** The total number of bytes used by the files in the vector store. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  usage_bytes: integer;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  file_counts: VectorStoreObjectFileCounts;

  /** The status of the vector store, which can be either `expired`, `in_progress`, or `completed`. A status of `completed` indicates that the vector store is ready for use. */
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  status: "expired" | "in_progress" | "completed";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  expires_after?: VectorStoreExpirationAfter;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  expires_at?: utcDateTime | null;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  last_active_at: utcDateTime | null;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  metadata: Metadata | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VectorStoreObjectFileCounts {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  in_progress: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  completed: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  failed: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  cancelled: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  total: integer;
}

#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{ name: "Update vector store file attributes request" }
)
model UpdateVectorStoreFileAttributesRequest {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  attributes: VectorStoreFileAttributes | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model UpdateVectorStoreRequest {
  /** The name of the vector store. */
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  name?: string | null;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  expires_after?: VectorStoreExpirationAfter;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  metadata?: Metadata | null;
}

/** A batch of files attached to a vector store. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Vector store file batch")
@extension(
  "x-oaiMeta",
  #{
    name: "The vector store files batch object",
    beta: true,
    example: """
      {
        "id": "vsfb_123",
        "object": "vector_store.files_batch",
        "created_at": 1698107661,
        "vector_store_id": "vs_abc123",
        "status": "completed",
        "file_counts": {
          "in_progress": 0,
          "completed": 100,
          "failed": 0,
          "cancelled": 0,
          "total": 100
        }
      }
      
      """,
  }
)
model VectorStoreFileBatchObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  /** The object type, which is always `vector_store.file_batch`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.files_batch";

  /** The Unix timestamp (in seconds) for when the vector store files batch was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** The ID of the [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) that the [File](https://platform.openai.com/docs/api-reference/files) is attached to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  vector_store_id: string;

  /** The status of the vector store files batch, which can be either `in_progress`, `completed`, `cancelled` or `failed`. */
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  status: "in_progress" | "completed" | "cancelled" | "failed";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  file_counts: VectorStoreFileBatchObjectFileCounts;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VectorStoreFileBatchObjectFileCounts {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  in_progress: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  completed: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  failed: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  cancelled: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  total: integer;
}

/** Represents the parsed content of a vector store file. */
model VectorStoreFileContentResponse {
  /** The object type, which is always `vector_store.file_content.page` */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.file_content.page";

  /** Parsed content of the file. */
  data: VectorStoreFileContentResponseData[];

  /** Indicates if there are more content pages to fetch. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  next_page: string | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VectorStoreFileContentResponseData {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  type?: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  text?: string;
}

#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiMeta", #{ name: "Vector store search request" })
model VectorStoreSearchRequest {
  /** A query string for a search */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  query: string | string[];

  /** Whether to rewrite the natural language query for vector search. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  rewrite_query?: boolean;

  /** The maximum number of results to return. This number should be between 1 and 50 inclusive. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(50)
  @minValue(1)
  max_num_results?: integer = 10;

  /** A filter to apply based on file attributes. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  filters?: ComparisonFilter | CompoundFilter;

  /** Ranking options for search. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  ranking_options?: VectorStoreSearchRequestRankingOptions;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VectorStoreSearchRequestRankingOptions {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  ranker?: "none" | "auto" | "default-2024-11-15" = "auto";

  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(1)
  @minValue(0)
  score_threshold?: numeric;
}

#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiMeta", #{ name: "Vector store search result content object" })
model VectorStoreSearchResultContentObject {
  /** The type of content. */
  type: "text";

  /** The text content returned from search. */
  text: string;
}

#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiMeta", #{ name: "Vector store search result item" })
model VectorStoreSearchResultItem {
  /** The ID of the vector store file. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  file_id: string;

  /** The name of the vector store file. */
  filename: string;

  /** The similarity score for the result. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(1)
  @minValue(0)
  score: numeric;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  attributes: VectorStoreFileAttributes | null;

  /** Content chunks from the file. */
  content: VectorStoreSearchResultContentObject[];
}

#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiMeta", #{ name: "Vector store search results page" })
model VectorStoreSearchResultsPage {
  /** The object type, which is always `vector_store.search_results.page` */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "vector_store.search_results.page";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  search_query: string[];

  /** The list of search result items. */
  data: VectorStoreSearchResultItem[];

  /** Indicates if there are more results to fetch. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  next_page: string | null;
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum ListVectorStoreFilesFilter {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  in_progress,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  completed,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  failed,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  cancelled,
}

union ChunkingStrategyRequestParamType {
  string,
  "auto",
  "static",
}

union ChunkingStrategyResponseType {
  string,
  "static",
  "other",
}
