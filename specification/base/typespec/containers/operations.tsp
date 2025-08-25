import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/containers")
@tag("Containers")
interface Containers {
  @get
  @operationId("ListContainers")
  @list
  listContainers(
    ...PageLimitQueryParameter,
    ...PageOrderQueryParameter,
    ...PageAfterQueryParameter,
  ): ContainerListResource | ErrorResponse;

  @post
  @operationId("CreateContainer")
  createContainer(
    @body
    body: CreateContainerBody,
  ): ContainerResource | ErrorResponse;

  @get
  @route("{container_id}")
  @operationId("RetrieveContainer")
  retrieveContainer(
    @path
    container_id: string,
  ): ContainerResource | ErrorResponse;

  @delete
  @route("{container_id}")
  @operationId("DeleteContainer")
  deleteContainer(
    @path
    container_id: string,
  ): {
    id: string;
    object: "container.deleted";
    deleted: true;
  } | ErrorResponse;

  @post
  @route("{container_id}/files")
  @operationId("CreateContainerFile")
  createContainerFile(
    @path
    container_id: string,

    @header
    contentType: "multipart/form-data",

    @multipartBody
    body: CreateContainerFileBody,
  ): ContainerFileResource | ErrorResponse;

  @get
  @route("{container_id}/files")
  @operationId("ListContainerFiles")
  @list
  listContainerFiles(
    @path
    container_id: string,

    ...PageLimitQueryParameter,
    ...PageOrderQueryParameter,
    ...PageAfterQueryParameter,
  ): ContainerFileListResource | ErrorResponse;

  @get
  @route("{container_id}/files/{file_id}")
  @operationId("RetrieveContainerFile")
  retrieveContainerFile(
    @path
    container_id: string,

    @path
    file_id: string,
  ): ContainerFileResource | ErrorResponse;

  @delete
  @route("{container_id}/files/{file_id}")
  @operationId("DeleteContainerFile")
  deleteContainerFile(
    @path
    container_id: string,

    @path
    file_id: string,
  ): {
    id: string;
    object: "container.file.deleted";
    deleted: true;
  } | ErrorResponse;

  @get
  @route("{container_id}/files/{file_id}/content")
  @operationId("RetrieveContainerFileContent")
  retrieveContainerFileContent(
    @path
    container_id: string,

    @path
    file_id: string,
  ): {
    @encode("base64") @body file_content: bytes;
  } | ErrorResponse;
}
