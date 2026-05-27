import "@azure-tools/typespec-client-generator-core";
import "@typespec/openapi";
import "@typespec/http";

using TypeSpec.OpenAPI;
using TypeSpec.Http;

namespace OpenAI.Audio;

/** Token usage statistics for the request. */
@discriminator("type")
model CreateTranscriptionResponseJsonUsage {
  type: CreateTranscriptionResponseJsonUsageType;
}

/** Usage statistics for models billed by token usage. */
@summary("TranscriptTextUsageTokens")
model TranscriptTextUsageTokens extends CreateTranscriptionResponseJsonUsage {
  /** The type of the usage object. Always `tokens` for this variant. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "tokens";

  /** Number of input tokens billed for this request. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_tokens: integer;

  /** Details about the input tokens billed for this request. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_token_details?: TranscriptTextUsageTokensInputTokenDetails;

  /** Number of output tokens generated. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  output_tokens: integer;

  /** Total number of tokens used (input + output). */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  total_tokens: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptTextUsageTokensInputTokenDetails {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  text_tokens?: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  audio_tokens?: integer;
}

/** Usage statistics for models billed by audio input duration. */
@summary("TranscriptTextUsageDuration")
model TranscriptTextUsageDuration extends CreateTranscriptionResponseJsonUsage {
  /** The type of the usage object. Always `duration` for this variant. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "duration";

  /** Duration of the input audio in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  seconds: duration;
}

union CreateTranscriptionResponseJsonUsageType {
  string,
  "tokens",
  "duration",
}
