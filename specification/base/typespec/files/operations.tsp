import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("files")
@tag("Files")
interface Files {
  @get
  @operationId("listFiles")
  @summary("Returns a list of files that belong to the user's organization.")
  listFiles(
    @header accept: "application/json",

    // TODO: This is just a string in the OpenAPI spec.
    /** Only return files with the given purpose. */
    @query purpose?: string,
  ): ListFilesResponse | ErrorResponse;

  @post
  @operationId("createFile")
  @summary("""
    Upload a file that can be used across various endpoints. The size of all the files uploaded by
    one organization can be up to 100 GB.
    
    The size of individual files can be a maximum of 512 MB or 2 million tokens for Assistants. See
    the [Assistants Tools guide](/docs/assistants/tools) to learn more about the types of files
    supported. The Fine-tuning API only supports `.jsonl` files.
    
    Please [contact us](https://help.openai.com/) if you need to increase these storage limits.
    """)
  createFile(
    @header accept: "application/json",
    @header contentType: "multipart/form-data",
    @multipartBody requestBody: CreateFileRequest,
  ): OpenAIFile | ErrorResponse;

  @route("{file_id}")
  @delete
  @operationId("deleteFile")
  @summary("Delete a file")
  deleteFile(
    @header accept: "application/json",

    /** The ID of the file to use for this request. */
    @path file_id: string,
  ): DeleteFileResponse | ErrorResponse;

  @route("{file_id}")
  @get
  @operationId("retrieveFile")
  @summary("Returns information about a specific file.")
  retrieveFile(
    @header accept: "application/json",

    /** The ID of the file to use for this request. */
    @path file_id: string,
  ): OpenAIFile | ErrorResponse;

  @route("{file_id}/content")
  @get
  @operationId("downloadFile")
  @summary("Returns the contents of the specified file.")
  // TODO: The OpenAPI spec says this returns a plain string but in reality it is returning
  // the file content as bytes.
  downloadFile(
    @header accept: "application/json",

    /** The ID of the file to use for this request. */
    @path file_id: string,
  ): bytes | ErrorResponse;
}
