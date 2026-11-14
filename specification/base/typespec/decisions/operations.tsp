import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI.Decisions;

@tag("Decisions")
@route("/decisions")
@post
@summary("Create a decision")
op createDecision(@body body: createDecision_request_object): createDecision_response_200_object;