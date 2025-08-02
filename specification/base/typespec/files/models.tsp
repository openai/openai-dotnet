/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model CreateFileRequest {
  // Tool customization: Apply HttpPart for property in multipart request model
  /** The File object (not file name) to be uploaded. */
  file: HttpPart<bytes>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    The intended purpose of the uploaded file. One of: - `assistants`: Used in the Assistants API - `batch`: Used in the Batch API - `fine-tune`: Used for fine-tuning - `vision`: Images used for vision fine-tuning - `user_data`: Flexible file type for any purpose - `evals`: Used for eval data sets
    """)
  purpose: HttpPart<
    | "assistants"
    | "batch"
    | "fine-tune"
    | "vision"
    | "user_data"
    | "evals">;
}

model ListFilesResponse {
  // Tool customization: apply the strict 'list' object value instead of open 'string'
  object: "list";

  data: OpenAIFile[];
  first_id: string;
  last_id: string;
  has_more: boolean;
}

model DeleteFileResponse {
  id: string;
  object: "file";
  deleted: boolean;
}

@doc("""
  The `File` object represents a document that has been uploaded to OpenAI.
  """)
model OpenAIFile {
  /** The file identifier, which can be referenced in the API endpoints. */
  id: string;

  // Tool customization: file response values can have null byte counts
  /** The size of the file, in bytes. */
  bytes: int64 | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the file was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the file will expire. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime;

  /** The name of the file. */
  filename: string;

  @doc("""
    The object type, which is always `file`.
    """)
  object: "file";

  // Tool customization: Add missing user_data and evals purposes
  @doc("""
    The intended purpose of the file. Supported values are `assistants`, `assistants_output`, `batch`, `batch_output`, `fine-tune`, `fine-tune-results` and `vision`.
    """)
  purpose:
    | "assistants"
    | "assistants_output"
    | "batch"
    | "batch_output"
    | "fine-tune"
    | "fine-tune-results"
    | "vision"
    | "user_data"
    | "evals";

  #suppress "deprecated" "Warning unnecessary for spec description of deprecation"
  #deprecated "This field is marked as deprecated."
  @doc("""
    Deprecated. The current status of the file, which can be either `uploaded`, `processed`, or `error`.
    """)
  status: "uploaded" | "processed" | "error";

  #suppress "deprecated" "Warning unnecessary for spec description of deprecation"
  #deprecated "This field is marked as deprecated."
  @doc("""
    Deprecated. For details on why a fine-tuning training file failed validation, see the `error` field on `fine_tuning.job`.
    """)
  status_details?: string;
}
