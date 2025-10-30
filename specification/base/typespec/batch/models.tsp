/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// Tool generated type. Extracts from Batch.errors.data
alias BatchErrorsData = {
  /** An error code identifying the error type. */
  code?: string;

  /** A human-readable message providing more details about the error. */
  message?: string;

  /** The name of the parameter that caused the error, if applicable. */
  param?: string | null;

  /** The line number of the input file where the error occurred, if applicable. */
  line?: int32 | null;
};

model Batch {
  id: string;

  @doc("""
    The object type, which is always `batch`.
    """)
  object: "batch";

  /** The OpenAI API endpoint used by the batch. */
  endpoint: string;

  errors?: {
    // Tool customization: add a clear enum enforcement of constrained 'object' label
    @doc("""
      The object type, which is always `list`.
      """)
    object?: "list";

    data?: BatchErrorsData[];
  };

  /** The ID of the input file for the batch. */
  input_file_id: string;

  /** The time frame within which the batch should be processed. */
  completion_window: string;

  /** The current status of the batch. */
  status:
    | "validating"
    | "failed"
    | "in_progress"
    | "finalizing"
    | "completed"
    | "expired"
    | "cancelling"
    | "cancelled";

  /** The ID of the file containing the outputs of successfully executed requests. */
  output_file_id?: string;

  /** The ID of the file containing the outputs of requests with errors. */
  error_file_id?: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch started processing. */
  @encode("unixTimestamp", int32)
  in_progress_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch will expire. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch started finalizing. */
  @encode("unixTimestamp", int32)
  finalizing_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch was completed. */
  @encode("unixTimestamp", int32)
  completed_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch failed. */
  @encode("unixTimestamp", int32)
  failed_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch expired. */
  @encode("unixTimestamp", int32)
  expired_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch started cancelling. */
  @encode("unixTimestamp", int32)
  cancelling_at?: utcDateTime;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the batch was cancelled. */
  @encode("unixTimestamp", int32)
  cancelled_at?: utcDateTime;

  /** The request counts for different statuses within the batch. */
  request_counts?: {
    /** Total number of requests in the batch. */
    total: int32;

    /** Number of requests that have been completed successfully. */
    completed: int32;

    /** Number of requests that have failed. */
    failed: int32;
  };

  ...MetadataPropertyForResponse;
}

/** The per-line object of the batch input file */
model BatchRequestInput {
  /** A developer-provided per-request id that will be used to match outputs to inputs. Must be unique for each request in a batch. */
  custom_id?: string;

  @doc("""
    The HTTP method to be used for the request. Currently only `POST` is supported.
    """)
  method?: "POST";

  // Tool customization: url uses the url type
  @doc("""
    The OpenAI API relative URL to be used for the request. Currently `/v1/chat/completions`, `/v1/embeddings`, and `/v1/completions` are supported.
    """)
  url?: url;
}

/** The per-line object of the batch output and error files */
model BatchRequestOutput {
  id?: string;

  /** A developer-provided per-request id that will be used to match outputs to inputs. */
  custom_id?: string;

  response?: {
    /** The HTTP status code of the response */
    status_code?: int32;

    /** An unique identifier for the OpenAI API request. Please include this request ID when contacting support. */
    request_id?: string;

    /** The JSON body of the response */
    @extension("x-oaiTypeLabel", "map")
    body?: Record<unknown>;
  } | null;

  /** For requests that failed with a non-HTTP error, this will contain more information on the cause of the failure. */
  error?: {
    /** A machine-readable error code. */
    code?: string;

    /** A human-readable error message. */
    message?: string;
  } | null;
}

model ListBatchesResponse {
  @pageItems data: Batch[];
  first_id?: string;
  @continuationToken last_id?: string;
  has_more: boolean;
  object: "list";
}
