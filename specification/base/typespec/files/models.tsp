import "@typespec/openapi";
import "@typespec/http";
import "@azure-tools/typespec-client-generator-core";
using TypeSpec.OpenAPI;
using TypeSpec.Http;
using Azure.ClientGenerator.Core;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateFileRequest {
  /** The File object (not file name) to be uploaded. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiMeta", #{ exampleFilePath: "fine-tune.jsonl" })
  file: HttpPart<bytes>;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  purpose: HttpPart<FilePurpose>;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  expires_after?: HttpPart<FileExpirationAfter>;
}

/** The expiration policy for a file. By default, files with `purpose=batch` expire after 30 days and all other files are persisted until they are manually deleted. */
#suppress "@azure-tools/typespec-autorest/unsupported-multipart-type" "Auto-suppressed warnings non-applicable rules during import."
@summary("File expiration policy")
model FileExpirationAfter {
  /** Anchor timestamp after which the expiration policy applies. Supported anchors: `created_at`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  anchor: "created_at";

  /** The number of seconds after the anchor time that the file will expire. Must be between 3600 (1 hour) and 2592000 (30 days). */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(2592000)
  @minValue(3600)
  seconds: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeleteFileResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "file";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ListFilesResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  object: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @pageItems
  data: OpenAIFile[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @continuationToken
  last_id: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

union FileCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model FileCollectionOptions {
  /** Only return files with the given purpose. */
  @query purpose?: UploadFilePurpose;

  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;

  /** A limit on the number of objects to be returned. */
  @query pageSizeLimit?: int32 = 10000;

  /** Sort order by the `created_at` timestamp of the objects. */
  @query order?: FileCollectionOrder;
}

/** The `File` object represents a document that has been uploaded to OpenAI. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("OpenAIFile")
@extension(
  "x-oaiMeta",
  #{
    name: "The file object",
    example: """
      {
        "id": "file-abc123",
        "object": "file",
        "bytes": 120000,
        "created_at": 1677610602,
        "expires_at": 1680202602,
        "filename": "salesOverview.pdf",
        "purpose": "assistants",
      }
      
      """,
  }
)
model OpenAIFile {
  /** The file identifier, which can be referenced in the API endpoints. */
  id: string;

  /** The size of the file, in bytes. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  bytes: integer;

  /** The Unix timestamp (in seconds) for when the file was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** The Unix timestamp (in seconds) for when the file will expire. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  expires_at?: utcDateTime;

  /** The name of the file. */
  filename: string;

  /** The object type, which is always `file`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "file";

  /** The intended purpose of the file. Supported values are `assistants`, `assistants_output`, `batch`, `batch_output`, `fine-tune`, `fine-tune-results`, `vision`, and `user_data`. */
  purpose: UploadFilePurpose;

  /** Deprecated. The current status of the file, which can be either `uploaded`, `processed`, or `error`. */
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #deprecated "deprecated"
  status: "uploaded" | "processed" | "error";

  /** Deprecated. For details on why a fine-tuning training file failed validation, see the `error` field on `fine_tuning.job`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #deprecated "deprecated"
  status_details?: string;
}

/** The intended purpose of a file returned by or used to filter the list-files endpoint. */
#suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
union UploadFilePurpose {
  assistants: "assistants",
  assistants_output: "assistants_output",
  batch: "batch",
  batch_output: "batch_output",
  `fine-tune`: "fine-tune",
  `fine-tune-results`: "fine-tune-results",
  vision: "vision",
  user_data: "user_data",
}

/** The intended purpose of the uploaded file. One of: - `assistants`: Used in the Assistants API - `batch`: Used in the Batch API - `fine-tune`: Used for fine-tuning - `vision`: Images used for vision fine-tuning - `user_data`: Flexible file type for any purpose - `evals`: Used for eval data sets */
union FilePurpose {
  string,
  assistants: "assistants",
  batch: "batch",
  `fine-tune`: "fine-tune",
  vision: "vision",
  user_data: "user_data",
  evals: "evals",
}
