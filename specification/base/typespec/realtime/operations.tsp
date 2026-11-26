import "@typespec/http";
import "./models_GA.tsp";

using TypeSpec.Http;

namespace OpenAI.Realtime;

/** Create a Realtime client secret with an associated session configuration. */
@tag("Realtime")
@summary("Create client secret")
@post
@route("/realtime/client_secrets")
op createRealtimeClientSecret(
  /** Create a client secret with the given session configuration. */
  @body
  options: RealtimeCreateClientSecretRequest_GA,
): RealtimeCreateClientSecretResponse_GA | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: RealtimeErrorResponse_GA;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: RealtimeErrorResponse_GA;
};
