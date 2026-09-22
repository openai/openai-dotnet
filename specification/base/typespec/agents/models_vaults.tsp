import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// ------------ Vault common ------------

/** Whether a vault or credential is active or archived. */
union VaultStatusParam {
  /** The vault or credential is active. */
  active: "active",

  /** The vault or credential is archived. */
  archived: "archived",
}

/** The order in which paginated vaults and credentials are returned. */
union VaultCollectionOrder {
  string,

  /** Returns resources in ascending order. */
  asc: "asc",

  /** Returns resources in descending order. */
  desc: "desc",
}

// ------------ Vaults ------------

/** The object type. Always `vault`. */
union VaultObject {
  string,
  vault: "vault",
}

/** A collection of credentials for MCP servers and OpenAI-hosted environments. */
model VaultResource {
  /** The ID of the vault. */
  id: string;

  /** The object type. Always `vault`. */
  object: VaultObject;

  /** The human-readable name of the vault, if set. */
  name: string | null;

  /** Custom string key-value pairs attached to the vault. */
  @extension("x-oaiTypeLabel", "map")
  metadata: Record<string>;

  /** The Unix timestamp, in seconds, when the vault was created. */
  created_at: int64;
}

/** A page of vaults. */
model VaultListResource {
  /** The object type, which is always `list`. */
  object: "list";

  /** The vaults returned in this page, in the requested sort order. */
  @maxItems(2000)
  @pageItems
  data: VaultResource[];

  /** The ID of the first vault in `data`, or `null` if the page is empty. */
  first_id: string | null;

  /** The ID of the last vault in `data`, or `null` if the page is empty. */
  @continuationToken
  last_id: string | null;

  /** Whether there are more vaults to retrieve after this page. */
  has_more: boolean;
}

/** Parameters for creating a vault to store credentials used by agent tools. */
model CreateVaultParams {
  /** The vault name, trimmed before storage; 1 to 256 UTF-8 bytes after trimming. */
  @minLength(1)
  @maxLength(1048576)
  name: string;

  /** Up to 1024 string key-value pairs. Omission or null defaults to an empty map. */
  @extension("x-oaiTypeLabel", "map")
  metadata?: Record<string> | null;
}

/** The object type. Always `vault.deleted`. */
union DeletedVaultObject {
  string,
  vault_deleted: "vault.deleted",
}

/** Confirmation that a vault was deleted. */
model DeletedVaultResource {
  /** The ID of the deleted vault. */
  id: string;

  /** The object type. Always `vault.deleted`. */
  object: DeletedVaultObject;

  /** Whether the vault was deleted. Always `true`. */
  deleted: boolean;
}

@access(Access.public)
@usage(Usage.input)
model VaultCollectionOptions {
  /** The order in which vaults are returned. Defaults to `desc`. */
  @query
  order?: VaultCollectionOrder = VaultCollectionOrder.desc;

  /** The maximum number of vaults to return, clamped between 1 and 100. */
  @query
  limit?: integer;

  /** Filter by one or more lifecycle statuses. Both statuses are included by default. */
  @query
  status?: VaultStatusParam[];

  /** Return vaults after this vault ID in the selected order. */
  @query
  @continuationToken
  after?: string;
}

// ------------ Credential auth (resource) ------------

/** The client authentication method used for OAuth token refresh. */
union McpOauthTokenEndpointAuthType {
  string,
  none: "none",
  client_secret_basic: "client_secret_basic",
  client_secret_post: "client_secret_post",
}

@discriminator("type")
model McpOauthTokenEndpointAuthResource {
  type: McpOauthTokenEndpointAuthType;
}

/** Sends the client ID without a client secret. */
model McpOauthTokenEndpointAuthResourceNone
  extends McpOauthTokenEndpointAuthResource {
  /** The type of the object. Always `none`. */
  type: McpOauthTokenEndpointAuthType.none;
}

/** Sends the client ID and secret using HTTP Basic authentication. */
model McpOauthTokenEndpointAuthResourceClientSecretBasic
  extends McpOauthTokenEndpointAuthResource {
  /** The type of the object. Always `client_secret_basic`. */
  type: McpOauthTokenEndpointAuthType.client_secret_basic;
}

/** Sends the client ID and secret in the token request body. */
model McpOauthTokenEndpointAuthResourceClientSecretPost
  extends McpOauthTokenEndpointAuthResource {
  /** The type of the object. Always `client_secret_post`. */
  type: McpOauthTokenEndpointAuthType.client_secret_post;
}

/** Configuration used to refresh an MCP OAuth access token, excluding secret values. */
model McpOauthRefreshResource {
  /** The HTTPS OAuth token endpoint used for refresh. */
  token_endpoint: string;

  /** The OAuth client ID used when requesting a new access token. */
  client_id: string;

  /** The resource URI sent to the OAuth token endpoint during refresh, if configured. */
  resource: string | null;

  /** Space-separated OAuth scopes requested during refresh, if configured. */
  scope: string | null;

  /** How the OAuth client authenticates to the token endpoint, excluding its client secret. */
  token_endpoint_auth: McpOauthTokenEndpointAuthResource;
}

/** Destination permissions for an environment-variable credential. */
union VaultCredentialNetworkingType {
  string,
  unrestricted: "unrestricted",
  limited: "limited",
}

@discriminator("type")
model VaultCredentialNetworkingResource {
  type: VaultCredentialNetworkingType;
}

/** Allows substitution for destinations permitted by the environment network policy. */
model VaultCredentialNetworkingResourceUnrestricted
  extends VaultCredentialNetworkingResource {
  /** The type of the object. Always `unrestricted`. */
  type: VaultCredentialNetworkingType.unrestricted;
}

/** Allows substitution only for the listed hosts. */
model VaultCredentialNetworkingResourceLimited
  extends VaultCredentialNetworkingResource {
  /** The type of the object. Always `limited`. */
  type: VaultCredentialNetworkingType.limited;

  /** The 1 to 16 distinct allowed hostnames or IPv4 addresses, normalized to lowercase. */
  @minItems(1)
  @maxItems(16)
  allowed_hosts: string[];
}

/** The authentication method used by a vault credential. */
union VaultCredentialAuthType {
  string,
  mcp_oauth: "mcp_oauth",
  static_bearer: "static_bearer",
  environment_variable: "environment_variable",
}

@discriminator("type")
model VaultCredentialAuthResource {
  type: VaultCredentialAuthType;
}

/** Public metadata for an OAuth credential; tokens and client secrets are never returned. */
model VaultCredentialAuthResourceMcpOauth extends VaultCredentialAuthResource {
  /** The type of the object. Always `mcp_oauth`. */
  type: VaultCredentialAuthType.mcp_oauth;

  /** The HTTPS MCP server URL authorized by this credential. */
  mcp_server_url: string;

  /** When the OAuth access token expires, as an RFC 3339 timestamp, if known. */
  expires_at: string | null;

  /** Public refresh metadata without refresh tokens or OAuth client secrets. */
  refresh: McpOauthRefreshResource | null;
}

/** Metadata for a bearer-token credential, without automatic OAuth refresh. */
model VaultCredentialAuthResourceStaticBearer
  extends VaultCredentialAuthResource {
  /** The type of the object. Always `static_bearer`. */
  type: VaultCredentialAuthType.static_bearer;

  /** The HTTPS MCP server URL authorized by this credential. */
  mcp_server_url: string;
}

/** An HTTP credential for OpenAI-hosted environments only. */
model VaultCredentialAuthResourceEnvironmentVariable
  extends VaultCredentialAuthResource {
  /** The type of the object. Always `environment_variable`. */
  type: VaultCredentialAuthType.environment_variable;

  /** The environment variable name that receives the placeholder in the sandbox. */
  secret_name: string;

  /** The destinations where the proxy can substitute the secret. */
  networking: VaultCredentialNetworkingResource;
}

// ------------ Credentials ------------

/** The object type. Always `vault.credential`. */
union VaultCredentialObject {
  string,
  vault_credential: "vault.credential",
}

/** Metadata for a stored credential. Secret values are never returned. */
model VaultCredentialResource {
  /** The ID of the credential. */
  id: string;

  /** The object type. Always `vault.credential`. */
  object: VaultCredentialObject;

  /** The ID of the vault containing this credential. */
  vault_id: string;

  /** The human-readable name of the credential. */
  name: string;

  /** The authentication method and non-secret configuration of the credential. */
  auth: VaultCredentialAuthResource;

  /** The Unix timestamp, in seconds, when the credential was created. */
  created_at: int64;

  /** The Unix timestamp, in seconds, when the credential was last updated. */
  updated_at: int64;
}

/** A page of vault credentials. */
model VaultCredentialListResource {
  /** The object type, which is always `list`. */
  object: "list";

  /** The credentials returned in this page, in the requested sort order. */
  @maxItems(2000)
  @pageItems
  data: VaultCredentialResource[];

  /** The ID of the first credential in `data`, or `null` if the page is empty. */
  first_id: string | null;

  /** The ID of the last credential in `data`, or `null` if the page is empty. */
  @continuationToken
  last_id: string | null;

  /** Whether there are more credentials to retrieve after this page. */
  has_more: boolean;
}

/** The object type. Always `vault.credential.deleted`. */
union DeletedVaultCredentialObject {
  string,
  vault_credential_deleted: "vault.credential.deleted",
}

/** Confirmation that a vault credential was deleted. */
model DeletedVaultCredentialResource {
  /** The ID of the deleted credential. */
  id: string;

  /** The object type. Always `vault.credential.deleted`. */
  object: DeletedVaultCredentialObject;

  /** Whether the credential was deleted. Always `true`. */
  deleted: boolean;
}

@access(Access.public)
@usage(Usage.input)
model VaultCredentialCollectionOptions {
  /** The ID of the vault. */
  @path
  vault_id: string;

  /** The order in which credentials are returned. Defaults to `desc`. */
  @query
  order?: VaultCollectionOrder = VaultCollectionOrder.desc;

  /** The maximum number of credentials to return, clamped between 1 and 100. */
  @query
  limit?: integer;

  /** Filter by one or more lifecycle statuses. Both statuses are included by default. */
  @query
  status?: VaultStatusParam[];

  /** Return credentials after this credential ID in the selected order. */
  @query
  @continuationToken
  after?: string;
}

// ------------ Credential auth (create) ------------

@discriminator("type")
model CreateMcpOauthTokenEndpointAuthParam {
  type: McpOauthTokenEndpointAuthType;
}

/** Sends the client ID without a client secret. */
model CreateMcpOauthTokenEndpointAuthParamNone
  extends CreateMcpOauthTokenEndpointAuthParam {
  /** The type of the object. Always `none`. */
  type: McpOauthTokenEndpointAuthType.none;
}

/** Sends the client ID and secret using HTTP Basic authentication. */
model CreateMcpOauthTokenEndpointAuthParamClientSecretBasic
  extends CreateMcpOauthTokenEndpointAuthParam {
  /** The type of the object. Always `client_secret_basic`. */
  type: McpOauthTokenEndpointAuthType.client_secret_basic;

  /** The OAuth client secret to store. Never returned in credential resources. */
  @maxLength(1048576)
  client_secret: string;
}

/** Sends the client ID and secret in the token request body. */
model CreateMcpOauthTokenEndpointAuthParamClientSecretPost
  extends CreateMcpOauthTokenEndpointAuthParam {
  /** The type of the object. Always `client_secret_post`. */
  type: McpOauthTokenEndpointAuthType.client_secret_post;

  /** The OAuth client secret to store. Never returned in credential resources. */
  @maxLength(1048576)
  client_secret: string;
}

/** Configuration for refreshing the access token of an MCP OAuth credential. */
model CreateMcpOauthRefreshParam {
  /** The HTTPS OAuth token endpoint used to exchange the refresh token for a new access token. */
  @maxLength(1048576)
  token_endpoint: string;

  /** The OAuth client ID used when requesting a new access token. */
  @maxLength(1048576)
  client_id: string;

  /** The resource URI to send to the OAuth token endpoint during refresh, if required. */
  @maxLength(1048576)
  resource?: string | null;

  /** Space-separated OAuth scopes to request during refresh, if required. */
  @maxLength(1048576)
  scope?: string | null;

  /** The refresh token to store. This secret is never returned in credential resources. */
  @maxLength(1048576)
  refresh_token: string;

  /** How the OAuth client authenticates to the token endpoint. */
  token_endpoint_auth: CreateMcpOauthTokenEndpointAuthParam;
}

@discriminator("type")
model VaultCredentialNetworkingParam {
  type: VaultCredentialNetworkingType;
}

/** Allows substitution for destinations permitted by the environment network policy. */
model VaultCredentialNetworkingParamUnrestricted
  extends VaultCredentialNetworkingParam {
  /** The type of the object. Always `unrestricted`. */
  type: VaultCredentialNetworkingType.unrestricted;
}

/** Allows substitution only for the listed hosts. */
model VaultCredentialNetworkingParamLimited
  extends VaultCredentialNetworkingParam {
  /** The type of the object. Always `limited`. */
  type: VaultCredentialNetworkingType.limited;

  /** The 1 to 16 distinct allowed hostnames or IPv4 addresses, normalized to lowercase. */
  @minItems(1)
  @maxItems(16)
  allowed_hosts: string[];
}

@discriminator("type")
model CreateVaultCredentialAuthParam {
  type: VaultCredentialAuthType;
}

/** An OAuth credential for an HTTPS MCP destination. */
model CreateVaultCredentialAuthParamMcpOauth
  extends CreateVaultCredentialAuthParam {
  /** The type of the object. Always `mcp_oauth`. */
  type: VaultCredentialAuthType.mcp_oauth;

  /** The HTTPS MCP server URL authorized by this credential. */
  @maxLength(1048576)
  mcp_server_url: string;

  /** A write-only OAuth access token; never returned by credential resources. */
  @maxLength(1048576)
  access_token: string;

  /** When the OAuth access token expires, as an RFC 3339 timestamp, if known. */
  @maxLength(1048576)
  expires_at?: string | null;

  /** Optional refresh configuration for an HTTPS OAuth token endpoint. */
  refresh?: CreateMcpOauthRefreshParam | null;
}

/** A bearer token for an MCP server, without automatic OAuth refresh. */
model CreateVaultCredentialAuthParamStaticBearer
  extends CreateVaultCredentialAuthParam {
  /** The type of the object. Always `static_bearer`. */
  type: VaultCredentialAuthType.static_bearer;

  /** The HTTPS MCP server URL authorized by this credential. */
  @maxLength(1048576)
  mcp_server_url: string;

  /** The bearer token to store. This secret is never returned in credential resources. */
  @maxLength(1048576)
  token: string;
}

/** An HTTP credential for OpenAI-hosted environments only. */
model CreateVaultCredentialAuthParamEnvironmentVariable
  extends CreateVaultCredentialAuthParam {
  /** The type of the object. Always `environment_variable`. */
  type: VaultCredentialAuthType.environment_variable;

  /** The environment variable name that receives the placeholder, such as `SERVICE_API_KEY`. */
  @minLength(1)
  @maxLength(1048576)
  secret_name: string;

  /** The write-only secret to store. Never returned in credential resources. */
  @minLength(1)
  @maxLength(1048576)
  secret_value: string;

  /** The destinations where the proxy can substitute this secret. */
  networking: VaultCredentialNetworkingParam;
}

/** Parameters for storing a credential for an MCP server or an OpenAI-hosted environment. */
model CreateVaultCredentialParams {
  /** The credential name, trimmed before storage; 1 to 256 UTF-8 bytes after trimming. */
  @minLength(1)
  @maxLength(1048576)
  name: string;

  /** The authentication method and write-only secret values to store. */
  auth: CreateVaultCredentialAuthParam;
}

// ------------ Credential auth (rotate) ------------

@discriminator("type")
model RotateMcpOauthTokenEndpointAuthParam {
  type: McpOauthTokenEndpointAuthType;
}

/** Updates credentials sent using HTTP Basic authentication. */
model RotateMcpOauthTokenEndpointAuthParamClientSecretBasic
  extends RotateMcpOauthTokenEndpointAuthParam {
  /** The type of the object. Always `client_secret_basic`. */
  type: McpOauthTokenEndpointAuthType.client_secret_basic;

  /** The replacement OAuth client secret. Omit or pass `null` to keep the stored secret. */
  @maxLength(1048576)
  client_secret?: string | null;
}

/** Updates credentials sent in the token request body. */
model RotateMcpOauthTokenEndpointAuthParamClientSecretPost
  extends RotateMcpOauthTokenEndpointAuthParam {
  /** The type of the object. Always `client_secret_post`. */
  type: McpOauthTokenEndpointAuthType.client_secret_post;

  /** The replacement OAuth client secret. Omit or pass `null` to keep the stored secret. */
  @maxLength(1048576)
  client_secret?: string | null;
}

/** Updates to an MCP credential's existing OAuth refresh configuration. */
model RotateMcpOauthRefreshParam {
  /** The replacement refresh token. Omit or pass `null` to keep the stored token. */
  @maxLength(1048576)
  refresh_token?: string | null;

  /** Replacement space-separated OAuth scopes for refresh requests. */
  @maxLength(1048576)
  scope?: string | null;

  /** Client-secret updates for the existing token endpoint authentication method. */
  token_endpoint_auth?: RotateMcpOauthTokenEndpointAuthParam | null;
}

@discriminator("type")
model RotateVaultCredentialAuthParam {
  type: VaultCredentialAuthType;
}

/** Rotate an OAuth credential for an HTTPS MCP destination. */
model RotateVaultCredentialAuthParamMcpOauth
  extends RotateVaultCredentialAuthParam {
  /** The type of the object. Always `mcp_oauth`. */
  type: VaultCredentialAuthType.mcp_oauth;

  /** A write-only replacement OAuth access token. */
  @maxLength(1048576)
  access_token?: string | null;

  /** The replacement expiry as an RFC 3339 timestamp, or `null` to clear it. */
  @maxLength(1048576)
  expires_at?: string | null;

  /** Optional write-only refresh-token and client-secret updates. */
  refresh?: RotateMcpOauthRefreshParam | null;
}

/** Replace the bearer token for the credential's MCP server. */
model RotateVaultCredentialAuthParamStaticBearer
  extends RotateVaultCredentialAuthParam {
  /** The type of the object. Always `static_bearer`. */
  type: VaultCredentialAuthType.static_bearer;

  /** The replacement bearer token. This secret is never returned in credential resources. */
  @maxLength(1048576)
  token: string;
}

/** Replace the secret for an OpenAI-hosted environment credential. */
model RotateVaultCredentialAuthParamEnvironmentVariable
  extends RotateVaultCredentialAuthParam {
  /** The type of the object. Always `environment_variable`. */
  type: VaultCredentialAuthType.environment_variable;

  /** The write-only replacement secret. Never returned in credential resources. */
  @minLength(1)
  @maxLength(1048576)
  secret_value: string;
}

/** Secret, expiry, and OAuth refresh scope updates for an existing vault credential. */
model RotateVaultCredentialParams {
  /** Replacement values for the credential's existing authentication method. */
  auth: RotateVaultCredentialAuthParam;
}
