import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI.Skills;

@tag("Skills")
@summary("List all skills for the current project.")
@get
@route("/skills")
op ListSkills(
  /** Number of items to retrieve */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @maxValue(100)
  @minValue(0)
  limit?: integer,

  /** Sort order of results by timestamp. Use `asc` for ascending order or `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: SkillCollectionOrder,

  /** Identifier for the last item from the previous pagination request */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  after?: string,
): SkillListResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Create a new skill.")
@post
@route("/skills")
@sharedRoute
op CreateSkillMultipart(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateSkillBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Create a new skill.")
@post
@route("/skills")
@sharedRoute
op CreateSkillJson(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateSkillBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Delete a skill by its ID.")
@delete
@route("/skills/{skill_id}")
op DeleteSkill(
  /** The identifier of the skill to delete. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): DeletedSkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Get a skill by its ID.")
@get
@route("/skills/{skill_id}")
op GetSkill(
  /** The identifier of the skill to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Update the default version pointer for a skill.")
@post
@route("/skills/{skill_id}")
op UpdateSkillDefaultVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "application/json" | "application/x-www-form-urlencoded",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: SetDefaultSkillVersionBody,
): SkillResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Download a skill zip bundle by its ID.")
@get
@route("/skills/{skill_id}/content")
op GetSkillContent(
  /** The identifier of the skill to download. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,
): {
  @header
  contentType: "application/zip";

  @body
  body: bytes;
} | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("List skill versions for a skill.")
@get
@route("/skills/{skill_id}/versions")
op ListSkillVersions(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** Number of versions to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @maxValue(100)
  @minValue(0)
  limit?: integer,

  /** Sort order of results by version number. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: SkillVersionCollectionOrder,

  /** The skill version ID to start after. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  after?: string,
): SkillVersionListResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Create a new immutable skill version.")
@post
@route("/skills/{skill_id}/versions")
@sharedRoute
op CreateSkillVersionMultipart(
  /** The identifier of the skill to version. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateSkillVersionBody,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Create a new immutable skill version.")
@post
@route("/skills/{skill_id}/versions")
@sharedRoute
op CreateSkillVersionJson(
  /** The identifier of the skill to version. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateSkillVersionBody,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Delete a skill version.")
@delete
@route("/skills/{skill_id}/versions/{version}")
op DeleteSkillVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The skill version number. */
  @path
  version: string,
): DeletedSkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Get a specific skill version.")
@get
@route("/skills/{skill_id}/versions/{version}")
op GetSkillVersion(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The version number to retrieve. */
  @path
  version: string,
): SkillVersionResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};

@tag("Skills")
@summary("Download a skill version zip bundle.")
@get
@route("/skills/{skill_id}/versions/{version}/content")
op GetSkillVersionContent(
  /** The identifier of the skill. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  skill_id: string,

  /** The skill version number. */
  @path
  version: string,
): {
  @header
  contentType: "application/zip";

  @body
  body: bytes;
} | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: SkillsErrorResponse;
};
