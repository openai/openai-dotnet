/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../files";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model CreateUploadRequest {
  /** The name of the file to upload. */
  filename: string;

  /**
   * The intended purpose of the uploaded file.
   *
   * See the [documentation on File purposes](/docs/api-reference/files/create#files-create-purpose).
   */
  purpose: "assistants" | "batch" | "fine-tune" | "vision";

  /** The number of bytes in the file you are uploading. */
  bytes: int32;

  /**
   * The MIME type of the file.
   *
   * This must fall within the supported MIME types for your file purpose. See the supported MIME types for assistants and vision.
   */
  mime_type: string;
}

model AddUploadPartRequest {
  // Tool customization: Apply HttpPart for property in multipart request model
  /** The chunk of bytes for this Part. */
  data: HttpPart<bytes>;
}

model CompleteUploadRequest {
  /** The ordered list of Part IDs. */
  part_ids: string[];

  /** The optional md5 checksum for the file contents to verify if the bytes uploaded matches what you expect. */
  md5?: string;
}

/** The Upload object can accept byte chunks in the form of Parts. */
model Upload {
  /** The Upload unique identifier, which can be referenced in API endpoints. */
  id: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the Upload was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The name of the file to be uploaded. */
  filename: string;

  /** The intended number of bytes to be uploaded. */
  bytes: int32;

  /** The intended purpose of the file. [Please refer here](/docs/api-reference/files/object#files/object-purpose) for acceptable values. */
  purpose: string;

  /** The status of the Upload. */
  status: "pending" | "completed" | "cancelled" | "expired";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the Upload will expire. */
  @encode("unixTimestamp", int32)
  expires_at: utcDateTime;

  /** The object type, which is always "upload". */
  object?: "upload";

  file?: OpenAIFile | null;
}

/** The upload Part represents a chunk of bytes we can add to an Upload object. */
model UploadPart {
  /** The upload Part unique identifier, which can be referenced in API endpoints. */
  id: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the Part was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The ID of the Upload object that this Part was added to. */
  upload_id: string;

  @doc("""
    The object type, which is always `upload.part`.
    """)
  object: "upload.part";
}
