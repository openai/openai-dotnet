import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// ------------ Environment common ------------

/** The order in which paginated environment templates and files are returned. */
union EnvironmentCollectionOrder {
  string,

  /** Returns resources in ascending order. */
  asc: "asc",

  /** Returns resources in descending order. */
  desc: "desc",
}

/** The kind of execution environment. */
union EnvironmentTypeResource {
  string,

  /** An environment hosted by OpenAI. */
  openai_hosted: "openai_hosted",

  /** An environment hosted by the application. */
  self_hosted: "self_hosted",
}

/** The public lifecycle status of an execution environment. */
union EnvironmentStatusResource {
  string,

  /** The environment is being prepared. */
  pending: "pending",

  /** The environment is connected. */
  connected: "connected",

  /** The environment is disconnected. */
  disconnected: "disconnected",

  /** The environment has expired. */
  expired: "expired",

  /** The environment failed. */
  failed: "failed",
}

// ------------ Packages ------------

/** Packages to install in an OpenAI-hosted environment. */
model EnvironmentPackagesParam {
  /** Python packages to install. Defaults to an empty list. */
  @maxItems(16384)
  python?: string[] | null;

  /** System packages to install. Defaults to an empty list. */
  @maxItems(16384)
  system?: string[] | null;

  /** npm packages to install globally. Defaults to an empty list. */
  @maxItems(16384)
  npm?: string[] | null;
}

/** Packages installed in an OpenAI-hosted environment. */
model EnvironmentPackagesResource {
  /** Python packages installed in the environment. */
  @maxItems(16384)
  python: string[];

  /** System packages installed in the environment. */
  @maxItems(16384)
  system: string[];

  /** npm packages installed globally in the environment. */
  @maxItems(16384)
  npm: string[];
}

// ------------ Setup commands ------------

/** A confidential setup command executed before the hosted agent starts. */
model SetupCommandParam {
  /** The shell command to execute. */
  @maxLength(65536)
  command: string;

  /** The absolute working directory. Defaults to `/workspace`. */
  @maxLength(4096)
  cwd?: string | null;
}

// ------------ Network policy ------------

/** The network access mode for an OpenAI-hosted environment. */
union NetworkAccessParam {
  string,

  /** Allows unrestricted network access. */
  enabled: "enabled",

  /** Disables network access. */
  disabled: "disabled",

  /** Allows access only to configured domains. */
  restricted: "restricted",
}

/** Network access for an OpenAI-hosted environment. */
model NetworkPolicyParam {
  /** The environment's network access mode. */
  access: NetworkAccessParam;

  /** Domains the environment may access when network access is restricted. */
  @maxItems(16384)
  allowed_domains?: string[] | null;
}

/** The network access mode for an OpenAI-hosted environment. */
union NetworkAccessResource {
  string,

  /** Allows unrestricted network access. */
  enabled: "enabled",

  /** Disables network access. */
  disabled: "disabled",

  /** Allows access only to configured domains. */
  restricted: "restricted",
}

/** Network access for an OpenAI-hosted environment. */
model NetworkPolicyResource {
  /** The environment's network access mode. */
  access: NetworkAccessResource;

  /** Domains the environment may access when network access is restricted. */
  @maxItems(2000)
  allowed_domains: string[];
}

// ------------ Inline capability source ------------

/** The type of an inline capability archive. */
union InlineCapabilitySourceType {
  string,
  base64: "base64",
}

@discriminator("type")
model InlineCapabilitySourceParam {
  type: InlineCapabilitySourceType;
}

/** Provides ZIP bytes encoded with standard base64. */
model InlineCapabilitySourceParamBase64 extends InlineCapabilitySourceParam {
  /** The type of the object. Always `base64`. */
  type: InlineCapabilitySourceType.base64;

  /** The archive media type, always `application/zip`. */
  media_type: "application/zip";

  /** Standard-base64 encoded ZIP archive bytes. */
  @minLength(1)
  @maxLength(70254592)
  data: string;
}

// ------------ Hosted skills ------------

/** How a skill is supplied to an OpenAI-hosted environment. */
union HostedSkillType {
  string,
  skill_reference: "skill_reference",
  inline: "inline",
}

@discriminator("type")
model HostedSkillParam {
  type: HostedSkillType;
}

/** References a skill uploaded through the Skills API. */
model HostedSkillParamSkillReference extends HostedSkillParam {
  /** The type of the object. Always `skill_reference`. */
  type: HostedSkillType.skill_reference;

  /** The ID of the skill created through `/v1/skills`. */
  @minLength(1)
  @maxLength(64)
  skill_id: string;

  /** The skill version, a positive integer or `latest`; omission selects the default. */
  version?: string | null;
}

/** Supplies a skill ZIP directly in the session request. */
model HostedSkillParamInline extends HostedSkillParam {
  /** The type of the object. Always `inline`. */
  type: HostedSkillType.inline;

  /** The skill name declared in `SKILL.md`. */
  @minLength(1)
  @maxLength(64)
  name: string;

  /** The skill description declared in `SKILL.md`. */
  @maxLength(1048576)
  description: string;

  /** The inline ZIP archive. */
  source: InlineCapabilitySourceParam;
}

@discriminator("type")
model HostedSkillResource {
  type: HostedSkillType;
}

/** A skill installed from the Skills API. */
model HostedSkillResourceSkillReference extends HostedSkillResource {
  /** The type of the object. Always `skill_reference`. */
  type: HostedSkillType.skill_reference;

  /** The referenced skill ID. */
  skill_id: string;

  /** The concrete skill version installed for this session. */
  version: string;

  /** The installed skill name. */
  name: string;

  /** The installed skill description. */
  description: string;
}

/** A skill installed from an inline ZIP archive. */
model HostedSkillResourceInline extends HostedSkillResource {
  /** The type of the object. Always `inline`. */
  type: HostedSkillType.inline;

  /** The installed skill name. */
  name: string;

  /** The installed skill description. */
  description: string;
}

// ------------ Hosted plugins ------------

/** How a plugin is supplied to an OpenAI-hosted environment. */
union HostedPluginType {
  string,
  inline: "inline",
}

@discriminator("type")
model HostedPluginParam {
  type: HostedPluginType;
}

/** Supplies a plugin ZIP directly in the session request. */
model HostedPluginParamInline extends HostedPluginParam {
  /** The type of the object. Always `inline`. */
  type: HostedPluginType.inline;

  /** The plugin name declared in `.codex-plugin/plugin.json`. */
  @minLength(1)
  @maxLength(64)
  name: string;

  /** The plugin description declared in `.codex-plugin/plugin.json`. */
  @maxLength(1048576)
  description: string;

  /** The inline ZIP archive. */
  source: InlineCapabilitySourceParam;
}

@discriminator("type")
model HostedPluginResource {
  type: HostedPluginType;
}

/** A plugin installed from an inline ZIP archive. */
model HostedPluginResourceInline extends HostedPluginResource {
  /** The type of the object. Always `inline`. */
  type: HostedPluginType.inline;

  /** The installed plugin name. */
  name: string;

  /** The installed plugin description. */
  description: string;
}

// ------------ Hosted environment files ------------

/** How a file is supplied to an OpenAI-hosted environment. */
union HostedEnvironmentFileType {
  string,
  file_id: "file_id",
  inline: "inline",
}

@discriminator("type")
model HostedEnvironmentFileParam {
  type: HostedEnvironmentFileType;
}

/** A file previously uploaded through the OpenAI Files API. */
model HostedEnvironmentFileParamFileId extends HostedEnvironmentFileParam {
  /** The type of the object. Always `file_id`. */
  type: HostedEnvironmentFileType.file_id;

  /** The ID of the uploaded file. */
  @minLength(1)
  @maxLength(256)
  file_id: string;

  /** The absolute destination path inside `/workspace`. */
  @minLength(1)
  @maxLength(4096)
  path: string;
}

/** A file supplied directly as standard-base64 data. */
model HostedEnvironmentFileParamInline extends HostedEnvironmentFileParam {
  /** The type of the object. Always `inline`. */
  type: HostedEnvironmentFileType.inline;

  /** The standard-base64-encoded file contents. */
  @maxLength(6990508)
  data: string;

  /** The absolute destination path inside `/workspace`. */
  @minLength(1)
  @maxLength(4096)
  path: string;
}

@discriminator("type")
model HostedEnvironmentFileResource {
  type: HostedEnvironmentFileType;
}

/** A file copied from the OpenAI Files API. */
model HostedEnvironmentFileResourceFileId extends HostedEnvironmentFileResource {
  /** The type of the object. Always `file_id`. */
  type: HostedEnvironmentFileType.file_id;

  /** The session-scoped ID of the file in the execution environment. */
  id: string;

  /** The ID of the uploaded file. */
  file_id: string;

  /** The file's absolute path inside the environment. */
  path: string;

  /** The decoded file size in bytes. */
  @minValue(0)
  size_bytes: int64;
}

/** A file supplied inline when the session was created. */
model HostedEnvironmentFileResourceInline extends HostedEnvironmentFileResource {
  /** The type of the object. Always `inline`. */
  type: HostedEnvironmentFileType.inline;

  /** The session-scoped ID of the file in the execution environment. */
  id: string;

  /** The file's absolute path inside the environment. */
  path: string;

  /** The decoded file size in bytes. */
  @minValue(0)
  size_bytes: int64;
}

// ------------ Hosted template skills and files ------------

/** How a template skill is resolved for each new session. */
union HostedTemplateSkillType {
  string,
  skill_reference: "skill_reference",
  inline: "inline",
}

@discriminator("type")
model HostedTemplateSkillResource {
  type: HostedTemplateSkillType;
}

/** A skill resolved afresh from the Skills API whenever a session starts. */
model HostedTemplateSkillResourceSkillReference
  extends HostedTemplateSkillResource {
  /** The type of the object. Always `skill_reference`. */
  type: HostedTemplateSkillType.skill_reference;

  /** The referenced skill ID. */
  skill_id: string;

  /** The requested version selector, including `latest`. */
  version: string | null;
}

/** Safe metadata for an inline skill archive. */
model HostedTemplateSkillResourceInline extends HostedTemplateSkillResource {
  /** The type of the object. Always `inline`. */
  type: HostedTemplateSkillType.inline;

  /** The skill name declared in `SKILL.md`. */
  name: string;

  /** The skill description declared in `SKILL.md`. */
  description: string;
}

/** How a template file is resolved for each new session. */
union HostedTemplateFileType {
  string,
  file_id: "file_id",
  inline: "inline",
}

@discriminator("type")
model HostedTemplateFileResource {
  type: HostedTemplateFileType;
}

/** A project-scoped Files API reference resolved separately for each session. */
model HostedTemplateFileResourceFileId extends HostedTemplateFileResource {
  /** The type of the object. Always `file_id`. */
  type: HostedTemplateFileType.file_id;

  /** The ID of the uploaded file. */
  file_id: string;

  /** The file's absolute path inside the environment. */
  path: string;
}

/** Metadata for confidential inline file contents. */
model HostedTemplateFileResourceInline extends HostedTemplateFileResource {
  /** The type of the object. Always `inline`. */
  type: HostedTemplateFileType.inline;

  /** The file's absolute path inside the environment. */
  path: string;

  /** The decoded size of the inline file in bytes. */
  @minValue(0)
  size_bytes: int64;
}

// ------------ Environment (session-embedded) ------------

/** The kind of execution environment configured for a session. */
union EnvironmentType {
  string,
  none: "none",
  openai_hosted: "openai_hosted",
  self_hosted: "self_hosted",
}

@discriminator("type")
model EnvironmentParam {
  type: EnvironmentType;
}

/** Runs the agent without an execution environment. */
model EnvironmentParamNone extends EnvironmentParam {
  /** The type of the object. Always `none`. */
  type: EnvironmentType.none;
}

/** An OpenAI-hosted environment, optionally based on a reusable template. */
model EnvironmentParamOpenaiHosted extends EnvironmentParam {
  /** The type of the object. Always `openai_hosted`. */
  type: EnvironmentType.openai_hosted;

  /** Packages to install in the environment. Defaults to empty package lists. */
  packages?: EnvironmentPackagesParam | null;

  /** Ordered, confidential setup commands. Command bodies are never returned. */
  @maxItems(16)
  setup_commands?: SetupCommandParam[] | null;

  /** Network access policy for the environment. */
  network?: NetworkPolicyParam | null;

  /** Environment variables made available to the agent. */
  @extension("x-oaiTypeLabel", "map")
  env?: Record<string> | null;

  /** Directories that contain capabilities exposed to the agent. Defaults to an empty list. */
  @maxItems(16384)
  capability_directories?: string[] | null;

  /** Skills referenced by ID or provided as inline ZIP archives. Defaults to an empty list. */
  @maxItems(200)
  skills?: HostedSkillParam[] | null;

  /** Plugins provided as inline ZIP archives. Defaults to an empty list. */
  @maxItems(32)
  plugins?: HostedPluginParam[] | null;

  /** Files available before the agent starts. Defaults to an empty list. */
  @maxItems(50)
  files?: HostedEnvironmentFileParam[] | null;

  /** A reusable hosted template applied before inline session configuration. */
  @maxLength(64)
  environment_template_id?: string;
}

/** An application-hosted environment configured inline. */
model EnvironmentParamSelfHosted extends EnvironmentParam {
  /** The type of the object. Always `self_hosted`. */
  type: EnvironmentType.self_hosted;

  /** Absolute project directory inside the self-hosted environment. */
  @maxLength(1048576)
  workspace_directory: string;

  /** Directories that contain capabilities exposed to the agent. Defaults to an empty list. */
  @maxItems(16384)
  capability_directories?: string[] | null;
}

@discriminator("type")
model EnvironmentResource {
  type: EnvironmentType;
}

/** The session talks to CCA without selecting or provisioning an execution environment. */
model EnvironmentResourceNone extends EnvironmentResource {
  /** The type of the object. Always `none`. */
  type: EnvironmentType.none;
}

/** An environment hosted by OpenAI. */
model EnvironmentResourceOpenaiHosted extends EnvironmentResource {
  /** The type of the object. Always `openai_hosted`. */
  type: EnvironmentType.openai_hosted;

  /** The public ID of the environment. */
  id: string;

  /** Packages installed in the environment. */
  packages: EnvironmentPackagesResource;

  /** The effective network access policy for the environment. */
  network: NetworkPolicyResource;

  /** Directories that contain capabilities exposed to the agent. */
  @maxItems(2000)
  capability_directories: string[];

  /** Skills installed in the environment, excluding their archive contents. */
  @maxItems(2000)
  skills: HostedSkillResource[];

  /** Plugins installed in the environment, excluding their archive contents. */
  @maxItems(2000)
  plugins: HostedPluginResource[];

  /** Files available in the environment, excluding their contents. */
  @maxItems(50)
  files: HostedEnvironmentFileResource[];
}

/** An environment hosted by the application. */
model EnvironmentResourceSelfHosted extends EnvironmentResource {
  /** The type of the object. Always `self_hosted`. */
  type: EnvironmentType.self_hosted;

  /** Pass this URL unchanged to `codex exec-server --remote` when connecting this environment. */
  remote_url: string;

  /** The public ID of the environment. */
  id: string;

  /** The absolute project directory inside the environment. Defaults to `/workspace`. */
  workspace_directory: string;

  /** Directories that contain capabilities exposed to the agent. */
  @maxItems(2000)
  capability_directories: string[];
}

// ------------ First-class environment resource ------------

/** Safe metadata for a first-class execution environment. */
model PublicEnvironmentResource {
  /** The ID of the environment. */
  id: string;

  /** The object type. Always `agent.environment`. */
  object: "agent.environment";

  /** Whether the environment is hosted by OpenAI or by the application. */
  type: EnvironmentTypeResource;

  /** The current environment connection status. */
  status: EnvironmentStatusResource;

  /** Files installed in the environment, without their contents. */
  @maxItems(2000)
  files: HostedEnvironmentFileResource[];

  /** Skills installed in the environment, without their archive contents. */
  @maxItems(2000)
  skills: HostedSkillResource[];

  /** Plugins installed in the environment, without their archive contents. */
  @maxItems(2000)
  plugins: HostedPluginResource[];
}

// ------------ Environment files ------------

/** A live file in an execution environment. */
model EnvironmentFileResource {
  /** The object type. Always `agent.environment.file`. */
  object: "agent.environment.file";

  /** The ID of the environment containing this file. */
  environment_id: string;

  /** The absolute file path inside the environment's workspace. */
  path: string;

  /** The file size in bytes. */
  @minValue(0)
  size_bytes: int64;
}

/** A paginated list of live execution environment files. */
model EnvironmentFileListResource {
  /** The object type. Always `page`. */
  object: "page";

  /** Files available on the current page. */
  @maxItems(2000)
  @pageItems
  data: EnvironmentFileResource[];

  /** The opaque cursor to use when requesting the next page, if any. */
  @continuationToken
  next: string | null;

  /** Whether more files follow this page. */
  has_more: boolean;
}

@access(Access.public)
@usage(Usage.input)
model EnvironmentFileCollectionOptions {
  /** The ID of the environment. */
  @path
  environment_id: string;

  /** Restrict the listing to this absolute workspace directory. */
  @query
  path?: string | null;

  /** The maximum number of files to return, between 1 and 100. */
  @query
  limit?: integer;

  /** Sort by case-sensitive path components. Defaults to descending. */
  @query
  order?: EnvironmentCollectionOrder = EnvironmentCollectionOrder.desc;

  /** The opaque token from the previous page. Keep the same path, order, and limit. */
  @query
  @continuationToken
  page?: string;
}

// ------------ Environment templates ------------

/** Reusable configuration that provisions a fresh OpenAI-hosted environment for each session. */
model EnvironmentTemplateResource {
  /** The ID of the reusable environment template. */
  id: string;

  /** An optional human-readable display name for the template. */
  name: string | null;

  /** The object type. Always `agent.environment.template`. */
  object: "agent.environment.template";

  /** The Unix timestamp, in seconds, when the template was created. */
  created_at: int64;

  /** The Unix timestamp, in seconds, when the template was last updated. */
  updated_at: int64;

  /** Packages installed in each fresh OpenAI-hosted environment. */
  packages: EnvironmentPackagesResource;

  /** Runtime network access for each OpenAI-hosted environment. */
  network: NetworkPolicyResource;

  /** Directories that expose capabilities to the agent. */
  @maxItems(2000)
  capability_directories: string[];

  /** Safe skill metadata, preserving unresolved version selectors. */
  @maxItems(200)
  skills: HostedTemplateSkillResource[];

  /** Safe plugin metadata, excluding inline archive contents. */
  @maxItems(32)
  plugins: HostedPluginResource[];

  /** Safe file metadata, excluding contents and session-scoped file IDs. */
  @maxItems(50)
  files: HostedTemplateFileResource[];
}

/** A page of reusable environment templates. */
model EnvironmentTemplateListResource {
  /** The object type, which is always `list`. */
  object: "list";

  /** The resources returned in this page, in the requested sort order. */
  @maxItems(2000)
  @pageItems
  data: EnvironmentTemplateResource[];

  /** The ID of the first resource in `data`, or `null` if the page is empty. */
  first_id: string | null;

  /** The ID of the last resource in `data`, or `null` if the page is empty. */
  @continuationToken
  last_id: string | null;

  /** Whether there are more resources to retrieve after this page. */
  has_more: boolean;
}

/** Parameters for creating a reusable, project-scoped OpenAI-hosted environment template. */
model CreateEnvironmentTemplateParams {
  /** Packages to install in the environment. Defaults to empty package lists. */
  packages?: EnvironmentPackagesParam | null;

  /** Ordered, confidential setup commands. Command bodies are never returned. */
  @maxItems(16)
  setup_commands?: SetupCommandParam[] | null;

  /** Network access policy for the environment. */
  network?: NetworkPolicyParam | null;

  /** Environment variables made available to the agent. */
  @extension("x-oaiTypeLabel", "map")
  env?: Record<string> | null;

  /** Directories that contain capabilities exposed to the agent. Defaults to an empty list. */
  @maxItems(16384)
  capability_directories?: string[] | null;

  /** Skills referenced by ID or provided as inline ZIP archives. Defaults to an empty list. */
  @maxItems(200)
  skills?: HostedSkillParam[] | null;

  /** Plugins provided as inline ZIP archives. Defaults to an empty list. */
  @maxItems(32)
  plugins?: HostedPluginParam[] | null;

  /** Files available before the agent starts. Defaults to an empty list. */
  @maxItems(50)
  files?: HostedEnvironmentFileParam[] | null;

  /** An optional human-readable display name for the template. */
  @minLength(1)
  @maxLength(256)
  name?: string | null;
}

/** Fields to replace on an existing reusable OpenAI-hosted environment template. */
model UpdateEnvironmentTemplateParams {
  /** A replacement human-readable display name, or `null` to clear the name. */
  @minLength(1)
  @maxLength(256)
  name?: string | null;

  /** Packages installed before the runtime network policy applies. */
  packages?: EnvironmentPackagesParam | null;

  /** Replacement confidential setup commands, never included in returned resources. */
  @maxItems(16)
  setup_commands?: SetupCommandParam[] | null;

  /** Network access available after setup completes. */
  network?: NetworkPolicyParam | null;

  /** Replacement confidential environment values. */
  @extension("x-oaiTypeLabel", "map")
  env?: Record<string> | null;

  /** Directories that expose capabilities to the agent. */
  @maxItems(16384)
  capability_directories?: string[] | null;

  /** Replacement skill configuration installed for each new session. */
  @maxItems(200)
  skills?: HostedSkillParam[] | null;

  /** Replacement plugin configuration installed for each new session. */
  @maxItems(32)
  plugins?: HostedPluginParam[] | null;

  /** Replacement file configuration materialized for each new session. */
  @maxItems(50)
  files?: HostedEnvironmentFileParam[] | null;
}

/** A deleted reusable environment template. */
model DeletedEnvironmentTemplateResource {
  /** The ID of the deleted environment template. */
  id: string;

  /** The object type. Always `agent.environment.template.deleted`. */
  object: "agent.environment.template.deleted";

  /** Whether the environment template was deleted. Always `true`. */
  deleted: boolean;
}

@access(Access.public)
@usage(Usage.input)
model EnvironmentTemplateCollectionOptions {
  /** The maximum number of resources to return, between 1 and 100. Defaults to 20. */
  @query
  limit?: integer;

  /** The order in which resources are returned. Defaults to `desc`. */
  @query
  order?: EnvironmentCollectionOrder = EnvironmentCollectionOrder.desc;

  /** Return resources after this resource ID in the selected order. */
  @query
  @continuationToken
  after?: string;
}
