import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/embeddings")
interface Embeddings {
  @post
  @operationId("createEmbedding")
  @tag("Embeddings")
  @summary("Creates an embedding vector representing the input text.")
  createEmbedding(
    @header accept: "application/json",
    @body requestBody: CreateEmbeddingRequest,
  ): CreateEmbeddingResponse | ErrorResponse;
}
