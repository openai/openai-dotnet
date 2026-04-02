import "../common";
import "./models_GA.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("realtime")
@tag("Realtime")
interface Realtime {
  /** Create a Realtime client secret with an associated session configuration. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @summary("Create client secret")
  @post
  @route("/client_secrets")
  @tag("Realtime")
  createRealtimeClientSecret(
    /** Create a client secret with the given session configuration. */
    @body
    options: RealtimeCreateClientSecretRequest_GA,
  ): RealtimeCreateClientSecretResponse_GA | {
    @maxValue(499)
    @minValue(400)
    @statusCode
    statusCode: int32;

    @body
    body: Error;
  } | {
    @maxValue(599)
    @minValue(500)
    @statusCode
    statusCode: int32;

    @body
    body: Error;
  };
}