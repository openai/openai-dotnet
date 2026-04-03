import "../containers/models.tsp";
import "@typespec/openapi";
import "@typespec/http";
using TypeSpec.OpenAPI;
using TypeSpec.Http;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SkillReferenceParam extends ContainerSkill {
  /** References a skill created with the /v1/skills endpoint. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "skill_reference" = "skill_reference";

  /** The ID of the referenced skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxLength(64)
  @minLength(1)
  skill_id: string;

  /** Optional skill version. Use a positive integer or 'latest'. Omit for default. */
  version?: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model InlineSkillParam extends ContainerSkill {
  /** Defines an inline skill for this request. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "inline" = "inline";

  /** The name of the skill. */
  name: string;

  /** The description of the skill. */
  description: string;

  /** Inline skill payload */
  source: InlineSkillSourceParam;
}

/** Inline skill payload */
model InlineSkillSourceParam {
  /** The type of the inline skill source. Must be `base64`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "base64" = "base64";

  /** The media type of the inline skill payload. Must be `application/zip`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  media_type: "application/zip" = "application/zip";

  /** Base64-encoded skill zip bundle. */
  @maxLength(70254592)
  @minLength(1)
  data: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model LocalSkillParam {
  /** The name of the skill. */
  name: string;

  /** The description of the skill. */
  description: string;

  /** The path to the directory containing the skill. */
  path: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SkillResource {
  /** Unique identifier for the skill. */
  id: string;

  /** The object type, which is `skill`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "skill" = "skill";

  /** Name of the skill. */
  name: string;

  /** Description of the skill. */
  description: string;

  /** Unix timestamp (seconds) for when the skill was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** Default version for the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  default_version: string;

  /** Latest version for the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  latest_version: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SkillListResource {
  /** The type of object returned, must be `list`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "list" = "list";

  /** A list of items */
  data: SkillResource[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  last_id: string | null;

  /** Whether there are more items available. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

/** Uploads a skill either as a directory (multipart `files[]`) or as a single zip file. */
@summary("Create skill request")
model CreateSkillBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  files: HttpPart<bytes[] | bytes>;
}

/** Updates the default version pointer for a skill. */
@summary("Update skill request")
model SetDefaultSkillVersionBody {
  /** The skill version number to set as default. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  default_version: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeletedSkillResource {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "skill.deleted" = "skill.deleted";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SkillVersionResource {
  /** The object type, which is `skill.version`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "skill.version" = "skill.version";

  /** Unique identifier for the skill version. */
  id: string;

  /** Identifier of the skill for this version. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  skill_id: string;

  /** Version number for this skill. */
  version: string;

  /** Unix timestamp (seconds) for when the version was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** Name of the skill version. */
  name: string;

  /** Description of the skill version. */
  description: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SkillVersionListResource {
  /** The type of object returned, must be `list`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "list" = "list";

  /** A list of items */
  data: SkillVersionResource[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  last_id: string | null;

  /** Whether there are more items available. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;
}

/** Uploads a new immutable version of a skill. */
@summary("Create skill version request")
model CreateSkillVersionBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  files: HttpPart<bytes[] | bytes>;

  /** Whether to set this version as the default. */
  default?: HttpPart<boolean>;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeletedSkillVersionResource {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "skill.version.deleted" = "skill.version.deleted";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;

  /** The deleted skill version. */
  version: string;
}
