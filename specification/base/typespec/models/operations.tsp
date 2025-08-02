import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/models")
interface Models {
  @get
  @operationId("listModels")
  @tag("Models")
  @summary("""
    Lists the currently available models, and provides basic information about each one such as the
    owner and availability.
    """)
  listModels(
    @header accept: "application/json",
  ): ListModelsResponse | ErrorResponse;

  @route("{model}")
  @get
  @operationId("retrieveModel")
  @tag("Models")
  @summary("""
    Retrieves a model instance, providing basic information about the model such as the owner and
    permissioning.
    """)
  retrieveModel(
    @header accept: "application/json",

    /** The ID of the model to use for this request. */
    @path `model`: string,
  ): Model | ErrorResponse;

  @route("{model}")
  @delete
  @operationId("deleteModel")
  @tag("Models")
  @summary("""
    Delete a fine-tuned model. You must have the Owner role in your organization to delete a model.
    """)
  deleteModel(
    @header accept: "application/json",

    /** The model to delete */
    @path `model`: string,
  ): DeleteModelResponse | ErrorResponse;
}
