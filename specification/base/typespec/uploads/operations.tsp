import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/uploads")
interface Uploads {
  @post
  @operationId("createUpload")
  @tag("Uploads")
  @summary("""
    Creates an intermediate [Upload](/docs/api-reference/uploads/object) object that you can add [Parts](/docs/api-reference/uploads/part-object) to. Currently, an Upload can accept at most 8 GB in total and expires after an hour after you create it.
    
    Once you complete the Upload, we will create a [File](/docs/api-reference/files/object) object that contains all the parts you uploaded. This File is usable in the rest of our platform as a regular File object.
    
    For certain `purpose`s, the correct `mime_type` must be specified. Please refer to documentation for the supported MIME types for your use case:
    - [Assistants](/docs/assistants/tools/file-search/supported-files)
    
    For guidance on the proper filename extensions for each purpose, please follow the documentation on [creating a File](/docs/api-reference/files/create).
    """)
  createUpload(
    @header accept: "application/json",
    @body requestBody: CreateUploadRequest,
  ): Upload | ErrorResponse;

  @route("{upload_id}/parts")
  @post
  @operationId("addUploadPart")
  @tag("Uploads")
  @summary("""
    Adds a [Part](/docs/api-reference/uploads/part-object) to an [Upload](/docs/api-reference/uploads/object) object. A Part represents a chunk of bytes from the file you are trying to upload. 
    
    Each Part can be at most 64 MB, and you can add Parts until you hit the Upload maximum of 8 GB.
    
    It is possible to add multiple Parts in parallel. You can decide the intended order of the Parts when you [complete the Upload](/docs/api-reference/uploads/complete).
    """)
  addUploadPart(
    @header accept: "application/json",
    @header contentType: "multipart/form-data",
    @path upload_id: string,
    @multipartBody requestBody: AddUploadPartRequest,
  ): UploadPart | ErrorResponse;

  @route("{upload_id}/complete")
  @post
  @operationId("completeUpload")
  @tag("Uploads")
  @summary("""
    Completes the [Upload](/docs/api-reference/uploads/object). 
    
    Within the returned Upload object, there is a nested [File](/docs/api-reference/files/object) object that is ready to use in the rest of the platform.
    
    You can specify the order of the Parts by passing in an ordered list of the Part IDs.
    
    The number of bytes uploaded upon completion must match the number of bytes initially specified when creating the Upload object. No Parts may be added after an Upload is completed.
    """)
  completeUpload(
    @header accept: "application/json",
    @path upload_id: string,
    @body requestBody: CompleteUploadRequest,
  ): Upload | ErrorResponse;

  @route("{upload_id}/cancel")
  @post
  @operationId("cancelUpload")
  @tag("Uploads")
  @summary("Cancels the Upload. No Parts may be added after an Upload is cancelled.")
  cancelUpload(
    @header accept: "application/json",
    @path upload_id: string,
  ): Upload | ErrorResponse;
}
