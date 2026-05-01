/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */
import "@azure-tools/typespec-client-generator-core";


using Http;
using Azure.ClientGenerator.Core;

namespace OpenAI;

model ContainersError {
  code: string | null;
  message: string;
  param: string | null;
  type: string;
}

@error
model ContainersErrorResponse {
  error: ContainersError;
}

model ContainerFileListResource {
  /** The type of object returned, must be 'list'. */
  object: "list";

  /** A list of container files. */
  @pageItems data: ContainerFileResource[];

  /** The ID of the first file in the list. */
  first_id: string;

  /** The ID of the last file in the list. */
  @continuationToken last_id: string;

  /** Whether there are more files available. */
  has_more: boolean;
}

model ContainerFileResource {
  /** Unique identifier for the file. */
  id: string;

  @doc("""
    The type of this object (`container.file`).
    """)
  object: string;

  /** The container this file belongs to. */
  container_id: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** Unix timestamp (in seconds) when the file was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** Size of the file in bytes. */
  bytes: int32;

  /** Path of the file in the container. */
  path: string;

  @doc("""
    Source of the file (e.g., `user`, `assistant`).
    """)
  source: string;
}

model ContainerListResource {
  /** The type of object returned, must be 'list'. */
  object: "list";

  /** A list of containers. */
  @pageItems data: ContainerResource[];

  /** The ID of the first container in the list. */
  first_id: string;

  /** The ID of the last container in the list. */
  @continuationToken last_id: string;

  /** Whether there are more containers available. */
  has_more: boolean;
}

model ContainerResource {
  /** Unique identifier for the container. */
  id: string;

  /** The type of this object. */
  object: string;

  /** Name of the container. */
  name: string;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** Unix timestamp (in seconds) when the container was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** Status of the container (e.g., active, deleted). */
  status: string;

  /**
   * The container will expire after this time period.
   * The anchor is the reference point for the expiration.
   * The minutes is the number of minutes after the anchor before the container expires.
   */
  expires_after?: {
    /** The reference point for the expiration. */
    anchor?: "last_active_at";

    /** The number of minutes after the anchor before the container expires. */
    minutes?: int32;
  };
}

model CreateContainerBody {
  /** Name of the container to create. */
  name: string;

  /** IDs of files to copy to the container. */
  file_ids?: string[];

  /** Container expiration time in seconds relative to the 'anchor' time. */
  expires_after?: {
    /** Time anchor for the expiration time. Currently only 'last_active_at' is supported. */
    anchor: "last_active_at";

    minutes: int32;
  };
}

model CreateContainerFileBody {
  // Tool customization: Apply HttpPart for property in multipart request model
  /** Name of the file to create. */
  file_id?: HttpPart<string>;

  // Tool customization: Apply HttpPart for property in multipart request model
  /** The File object (not file name) to be uploaded. */
  file?: HttpPart<bytes>;
}

union ContainerCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerCollectionOptions {
  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20;

  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ContainerCollectionOrder;

  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken after?: string;
}

union ContainerFileCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerFileCollectionOptions {
  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20;

  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: ContainerFileCollectionOrder;

  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken after?: string;
}