import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI.Containers;

@tag("Containers")
@operationId("CreateContainer")
@post
@route("/containers")
op createContainer(
  @body
  options: ContainerCreationOptions,
): ContainerResource | ContainersErrorResponse;

@tag("Containers")
@operationId("RetrieveContainer")
@get
@route("/containers/{container_id}")
op retrieveContainer(
  @path
  container_id: string,
): ContainerResource | ContainersErrorResponse;

@tag("Containers")
@operationId("DeleteContainer")
@delete
@route("/containers/{container_id}")
op deleteContainer(
  @path
  container_id: string,
): ContainerDeletionResult | ContainersErrorResponse;

@tag("Containers")
@operationId("ListContainers")
@get
@route("/containers")
@list
op listContainers(
  ...ContainerCollectionOptions,
): ContainerCollection | ContainersErrorResponse;

@tag("Containers")
@operationId("CreateContainerFile")
@post
@route("/containers/{container_id}/files")
op createContainerFile(
  @path
  container_id: string,

  @header
  contentType: "multipart/form-data",

  @multipartBody
  options: ContainerFileUploadOptions,
): ContainerFileResource | ContainersErrorResponse;

@tag("Containers")
@operationId("RetrieveContainerFile")
@get
@route("/containers/{container_id}/files/{file_id}")
op retrieveContainerFile(
  @path
  container_id: string,

  @path
  file_id: string,
): ContainerFileResource | ContainersErrorResponse;

@tag("Containers")
@operationId("DeleteContainerFile")
@delete
@route("/containers/{container_id}/files/{file_id}")
op deleteContainerFile(
  @path
  container_id: string,

  @path
  file_id: string,
): ContainerFileDeletionResult | ContainersErrorResponse;

@tag("Containers")
@operationId("ListContainerFiles")
@get
@route("/containers/{container_id}/files")
@list
op listContainerFiles(
  ...ContainerFileCollectionOptions,
): ContainerFileCollection | ContainersErrorResponse;

@tag("Containers")
@operationId("RetrieveContainerFileContent")
@get
@route("/containers/{container_id}/files/{file_id}/content")
op retrieveContainerFileContent(
  @path
  container_id: string,

  @path
  file_id: string,
): {
  @encode("base64") @body file_content: bytes;
} | ContainersErrorResponse;
