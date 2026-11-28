import "../base/typespec/skills/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI.Skills;

@@clientName(OpenAI.Skills, "SkillsClient", "csharp");
@@clientNamespace(OpenAI.Skills, "OpenAI.Skills", "csharp");
@@dynamicModel(OpenAI.Skills, "csharp");

@@alternateType(ListSkills::parameters.limit, int32);
@@alternateType(ListSkillVersions::parameters.limit, int32);

// Move operations into their corresponding interface
@@clientLocation(ListSkills, "Skills");
@@clientLocation(CreateSkillMultipart, "Skills");
@@clientLocation(CreateSkillJson, "Skills");
@@clientLocation(DeleteSkill, "Skills");
@@clientLocation(GetSkill, "Skills");
@@clientLocation(UpdateSkillDefaultVersion, "Skills");
@@clientLocation(GetSkillContent, "Skills");
@@clientLocation(ListSkillVersions, "Skills");
@@clientLocation(CreateSkillVersionMultipart, "Skills");
@@clientLocation(CreateSkillVersionJson, "Skills");
@@clientLocation(DeleteSkillVersion, "Skills");
@@clientLocation(GetSkillVersion, "Skills");
@@clientLocation(GetSkillVersionContent, "Skills");

// Rename operation for consistency with other APIs
@@clientName(CreateSkillMultipart, "UploadSkill");
@@clientName(UpdateSkillDefaultVersion, "UpdateSkill");
@@clientName(GetSkillContent, "DownloadSkill");
@@clientName(CreateSkillVersionMultipart, "UploadSkillVersion");
@@clientName(GetSkillVersionContent, "DownloadSkillVersion");

// Remove JSON variants from C# generation (only multipart is usable)
@@scope(CreateSkillJson, "!csharp");
@@scope(CreateSkillVersionJson, "!csharp");

// Do not generate convenience methods - protocol only
@@convenientAPI(ListSkills, false, "csharp");
@@convenientAPI(CreateSkillMultipart, false, "csharp");
@@convenientAPI(CreateSkillJson, false, "csharp");
@@convenientAPI(DeleteSkill, false, "csharp");
@@convenientAPI(GetSkill, false, "csharp");
@@convenientAPI(UpdateSkillDefaultVersion, false, "csharp");
@@convenientAPI(GetSkillContent, false, "csharp");
@@convenientAPI(ListSkillVersions, false, "csharp");
@@convenientAPI(CreateSkillVersionMultipart, false, "csharp");
@@convenientAPI(CreateSkillVersionJson, false, "csharp");
@@convenientAPI(DeleteSkillVersion, false, "csharp");
@@convenientAPI(GetSkillVersion, false, "csharp");
@@convenientAPI(GetSkillVersionContent, false, "csharp");
