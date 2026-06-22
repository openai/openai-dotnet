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

union ContainerMemoryLimit {
  string,
  `1g`: "1g",
  `4g`: "4g",
  `16g`: "16g",
  `64g`: "64g",
}

union ContainerExpiresAfterAnchor {
  string,
  last_active_at: "last_active_at"
}

model ContainerExpiresAfter {
  anchor?: ContainerExpiresAfterAnchor;
  minutes?: integer;
}

union ContainerNetworkPolicyType {
  string,
  disabled: "disabled",
  allowlist: "allowlist",
}

/** Network access policy for the container. */
@discriminator("type")
model ContainerNetworkPolicy {
  type: ContainerNetworkPolicyType;
}

model ContainerNetworkPolicyDisabled extends ContainerNetworkPolicy {
  /** Disable outbound network access. Always `disabled`. */
  type: ContainerNetworkPolicyType.disabled;
}

model ContainerNetworkPolicyAllowlist extends ContainerNetworkPolicy {
  /** Allow outbound network access only to specified domains. Always `allowlist`. */
  type: ContainerNetworkPolicyType.allowlist;

  /** A list of allowed domains when type is `allowlist`. */
  @minItems(1)
  allowed_domains: string[];

  /** Optional domain-scoped secrets for allowlisted domains. */
  @minItems(1)
  @visibility(Lifecycle.Create)
  domain_secrets?: ContainerNetworkPolicyDomainSecret[];
}

model ContainerNetworkPolicyDomainSecret {
  /** The domain associated with the secret. */
  @minLength(1)
  domain: string;

  /** The name of the secret to inject for the domain. */
  @minLength(1)
  name: string;

  /** The secret value to inject for the domain. */
  @maxLength(10485760)
  @minLength(1)
  value: string;
}

model ContainerResource {
  /** Unique identifier for the container. */
  id: string;

  /** The type of this object. */
  object: "container";

  /** Name of the container. */
  name: string;

  /** Unix timestamp (in seconds) when the container was created. */
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** Status of the container (e.g., active, deleted). */
  status: string;

  /** Unix timestamp (in seconds) when the container was last active. */
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  last_active_at?: utcDateTime;

  /** The container will expire after this time period.
   * The anchor is the reference point for the expiration.
   * The minutes is the number of minutes after the anchor before the container expires.
   */
  expires_after?: ContainerExpiresAfter;

  /** The memory limit configured for the container. */
  memory_limit?: ContainerMemoryLimit;

  network_policy?: ContainerNetworkPolicy;
}

model CreateContainerOptions {
  /** Name of the container to create. */
  name: string;

  /** IDs of files to copy to the container. */
  file_ids?: string[];

  /** Container expiration time in seconds relative to the 'anchor' time. */
  expires_after?: ContainerExpiresAfter;

  // TODO: Add support for skills.
  /** An optional list of skills referenced by id or inline data. */
  // skills?: ContainerSkill[];

  /** Optional memory limit for the container. Defaults to "1g". */
  memory_limit?: ContainerMemoryLimit;

  network_policy?: ContainerNetworkPolicy;
}

model ContainerFileResource {
  /** Unique identifier for the file. */
  id: string;

  /** The type of this object (`container.file`). */
  object: "container.file";

  /** The container this file belongs to. */
  container_id: string;

  /** Unix timestamp (in seconds) when the file was created. */
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** Size of the file in bytes. */
  bytes: integer;

  /** Path of the file in the container. */
  path: string;

  /** Source of the file (e.g., `user`, `assistant`). */
  source: string;
}

model CreateContainerFileOptions {
  /** Name of the file to create. */
  file_id?: HttpPart<string>;

  /** The File object (not file name) to be uploaded. */
  file?: HttpPart<bytes>;
}

model ContainerDeletionResult {
  id: string;
  object: "container.deleted";
  deleted: boolean;
}

model ContainerFileDeletionResult {
  id: string;
  object: "container.file.deleted";
  deleted: true;
}

union ContainerCollectionOrder {
  string,
  asc: "asc",
  desc: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerCollectionOptions {
  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  @query
  limit?: integer = 20;

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  @query
  order?: ContainerCollectionOrder = ContainerCollectionOrder.desc;

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  @query
  @continuationToken
  after?: string;

  /** Filter results by container name. */
  @query
  name?: string;
}

model ContainerCollection {
  /** The type of object returned, must be 'list'. */
  object: "list";

  /** A list of containers. */
  @pageItems
  data: ContainerResource[];

  /** The ID of the first container in the list. */
  first_id: string;

  /** The ID of the last container in the list. */
  @continuationToken
  last_id: string;

  /** Whether there are more containers available. */
  has_more: boolean;
}

union ContainerFileCollectionOrder {
  string,
  asc: "asc",
  desc: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ContainerFileCollectionOptions {
  @path
  container_id: string;

  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  @query
  limit?: integer = 20;

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  @query
  order?: ContainerFileCollectionOrder = ContainerFileCollectionOrder.desc;

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  @query
  @continuationToken
  after?: string;
}

model ContainerFileCollection {
  /** The type of object returned, must be 'list'. */
  object: "list";

  /** A list of container files. */
  @pageItems
  data: ContainerFileResource[];

  /** The ID of the first file in the list. */
  first_id: string;

  /** The ID of the last file in the list. */
  @continuationToken
  last_id: string;

  /** Whether there are more files available. */
  has_more: boolean;
}
