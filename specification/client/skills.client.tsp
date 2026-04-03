import "../base/typespec/skills/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

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

// Do not generate convenience methods - protocol only
@@convenientAPI(ListSkills, false);
@@convenientAPI(CreateSkillMultipart, false);
@@convenientAPI(CreateSkillJson, false);
@@convenientAPI(DeleteSkill, false);
@@convenientAPI(GetSkill, false);
@@convenientAPI(UpdateSkillDefaultVersion, false);
@@convenientAPI(GetSkillContent, false);
@@convenientAPI(ListSkillVersions, false);
@@convenientAPI(CreateSkillVersionMultipart, false);
@@convenientAPI(CreateSkillVersionJson, false);
@@convenientAPI(DeleteSkillVersion, false);
@@convenientAPI(GetSkillVersion, false);
@@convenientAPI(GetSkillVersionContent, false);
