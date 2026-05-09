import "@typespec/http";
import "@typespec/openapi";

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
    ...ContainerCollectionOptions,
  ): ContainerListResource | ContainersErrorResponse;

  @post
  @operationId("CreateContainer")
  createContainer(
    @body
    body: CreateContainerBody,
  ): ContainerResource | ContainersErrorResponse;

  @get
  @route("{container_id}")
  @operationId("RetrieveContainer")
  retrieveContainer(
    @path
    container_id: string,
  ): ContainerResource | ContainersErrorResponse;

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
  } | ContainersErrorResponse;

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
  ): ContainerFileResource | ContainersErrorResponse;

  @get
  @route("{container_id}/files")
  @operationId("ListContainerFiles")
  @list
  listContainerFiles(
    @path
    container_id: string,

    ...ContainerFileCollectionOptions,
  ): ContainerFileListResource | ContainersErrorResponse;

  @get
  @route("{container_id}/files/{file_id}")
  @operationId("RetrieveContainerFile")
  retrieveContainerFile(
    @path
    container_id: string,

    @path
    file_id: string,
  ): ContainerFileResource | ContainersErrorResponse;

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
  } | ContainersErrorResponse;

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
  } | ContainersErrorResponse;
}
