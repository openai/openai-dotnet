import "@typespec/openapi";
using TypeSpec.OpenAPI;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeleteModelResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  object: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ListModelsResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "list";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  data: Model[];
}

/** Describes an OpenAI model offering that can be used with the API. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Model")
@extension(
  "x-oaiMeta",
  #{
    name: "The model object",
    example: """
      {
        "id": "VAR_chat_model_id",
        "object": "model",
        "created": 1686935002,
        "owned_by": "openai"
      }
      
      """,
  }
)
model Model {
  /** The model identifier, which can be referenced in the API endpoints. */
  id: string;

  /** The Unix timestamp (in seconds) when the model was created. */
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created: utcDateTime;

  /** The object type, which is always "model". */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "model";

  /** The organization that owns the model. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  owned_by: string;
}