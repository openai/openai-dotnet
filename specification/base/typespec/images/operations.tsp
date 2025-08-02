import "@typespec/http";
import "@typespec/openapi";

import "../common/models.tsp";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/images")
interface Images {
  @route("generations")
  @post
  @operationId("createImage")
  @tag("Images")
  @summary("Creates an image given a prompt")
  createImage(
    @header accept: "application/json",
    @body requestBody: CreateImageRequest,
  ): ImagesResponse | ErrorResponse;

  @route("edits")
  @post
  @operationId("createImageEdit")
  @tag("Images")
  @summary("Creates an edited or extended image given an original image and a prompt.")
  createImageEdit(
    @header accept: "application/json",
    @header contentType: "multipart/form-data",
    @multipartBody requestBody: CreateImageEditRequest,
  ): ImagesResponse | ErrorResponse;

  @route("variations")
  @post
  @operationId("createImageVariation")
  @tag("Images")
  @summary("Creates an edited or extended image given an original image and a prompt.")
  createImageVariation(
    @header accept: "application/json",
    @header contentType: "multipart/form-data",
    @multipartBody requestBody: CreateImageVariationRequest,
  ): ImagesResponse | ErrorResponse;
}
