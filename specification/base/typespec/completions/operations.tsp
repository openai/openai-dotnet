import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/completions")
interface Completions {
  @post
  @operationId("createCompletion")
  @tag("Completions")
  @summary("Creates a completion for the provided prompt and parameters.")
  createCompletion(
    @header accept: "application/json",
    @body requestBody: CreateCompletionRequest,
  ): CreateCompletionResponse | ErrorResponse;
}
