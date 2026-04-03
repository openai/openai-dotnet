using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Skills;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Skills;

[Category("Skills")]
public partial class SkillTests : OpenAIRecordedTestBase
{
    public SkillTests(bool isAsync) : base(isAsync)
    {
    }

    [RecordedTest]
    public async Task BasicSkillWorks()
    {
        SkillClient client = GetProxiedOpenAIClient<SkillClient>();

        // Create a skill using multipart directory upload
        byte[] skillMd = await File.ReadAllBytesAsync(Path.Combine("Assets", "skills_basic_math_SKILL.md"));

        using MultiPartFormDataBinaryContent createContent = new();
        createContent.Add(skillMd, "files[]", "basic_math/SKILL.md", "text/markdown");

        ClientResult createResult = await client.CreateSkillMultipartAsync(createContent, createContent.ContentType);

        BinaryData createResponseData = createResult.GetRawResponse().Content;
        using JsonDocument createJson = JsonDocument.Parse(createResponseData);
        JsonElement createRoot = createJson.RootElement;

        Assert.That(createRoot.GetProperty("object").GetString(), Is.EqualTo("skill"));

        string skillId = createRoot.GetProperty("id").GetString();
        Assert.That(skillId, Is.Not.Null.And.Not.Empty);
        Assert.That(createRoot.GetProperty("created_at").GetInt64(), Is.GreaterThan(0));

        // Get the skill
        ClientResult getResult = await client.GetSkillAsync(skillId);

        BinaryData getResponseData = getResult.GetRawResponse().Content;
        using JsonDocument getJson = JsonDocument.Parse(getResponseData);
        JsonElement getRoot = getJson.RootElement;

        Assert.That(getRoot.GetProperty("id").GetString(), Is.EqualTo(skillId));
        Assert.That(getRoot.GetProperty("object").GetString(), Is.EqualTo("skill"));

        // List skills and verify the created skill appears
        ClientResult listResult = await client.GetSkillsAsync();

        BinaryData listResponseData = listResult.GetRawResponse().Content;
        using JsonDocument listJson = JsonDocument.Parse(listResponseData);
        JsonElement listRoot = listJson.RootElement;

        Assert.That(listRoot.GetProperty("object").GetString(), Is.EqualTo("list"));
        Assert.That(listRoot.TryGetProperty("data", out JsonElement dataElement), Is.True);
        Assert.That(dataElement.ValueKind, Is.EqualTo(JsonValueKind.Array));

        // Delete the skill
        ClientResult deleteResult = await client.DeleteSkillAsync(skillId);

        BinaryData deleteResponseData = deleteResult.GetRawResponse().Content;
        using JsonDocument deleteJson = JsonDocument.Parse(deleteResponseData);
        JsonElement deleteRoot = deleteJson.RootElement;

        Assert.That(deleteRoot.GetProperty("id").GetString(), Is.EqualTo(skillId));
        Assert.That(deleteRoot.GetProperty("object").GetString(), Is.EqualTo("skill.deleted"));
        Assert.That(deleteRoot.GetProperty("deleted").GetBoolean(), Is.True);
    }

    [RecordedTest]
    public async Task BasicSkillVersionWorks()
    {
        SkillClient client = GetProxiedOpenAIClient<SkillClient>();

        // Create a skill — this also creates version 1
        byte[] skillMd = await File.ReadAllBytesAsync(Path.Combine("Assets", "skills_basic_math_SKILL.md"));

        using MultiPartFormDataBinaryContent createContent = new();
        createContent.Add(skillMd, "files[]", "basic_math/SKILL.md", "text/markdown");

        ClientResult createResult = await client.CreateSkillMultipartAsync(createContent, createContent.ContentType);

        BinaryData createResponseData = createResult.GetRawResponse().Content;
        using JsonDocument createJson = JsonDocument.Parse(createResponseData);
        JsonElement createRoot = createJson.RootElement;
        string skillId = createRoot.GetProperty("id").GetString();

        Assert.That(createRoot.GetProperty("default_version").GetString(), Is.EqualTo("1"));
        Assert.That(createRoot.GetProperty("latest_version").GetString(), Is.EqualTo("1"));

        // Create a new version
        using MultiPartFormDataBinaryContent versionContent = new();
        versionContent.Add(skillMd, "files[]", "basic_math/SKILL.md", "text/markdown");

        ClientResult versionResult = await client.CreateSkillVersionMultipartAsync(skillId, versionContent, versionContent.ContentType);

        BinaryData versionResponseData = versionResult.GetRawResponse().Content;
        using JsonDocument versionJson = JsonDocument.Parse(versionResponseData);
        JsonElement versionRoot = versionJson.RootElement;

        Assert.That(versionRoot.GetProperty("object").GetString(), Is.EqualTo("skill.version"));
        Assert.That(versionRoot.GetProperty("skill_id").GetString(), Is.EqualTo(skillId));
        Assert.That(versionRoot.GetProperty("version").GetString(), Is.EqualTo("2"));

        // Verify the skill now reflects the new version
        ClientResult getSkillResult = await client.GetSkillAsync(skillId);

        BinaryData getSkillData = getSkillResult.GetRawResponse().Content;
        using JsonDocument getSkillJson = JsonDocument.Parse(getSkillData);
        JsonElement getSkillRoot = getSkillJson.RootElement;

        Assert.That(getSkillRoot.GetProperty("latest_version").GetString(), Is.EqualTo("2"));

        // Clean up
        await client.DeleteSkillAsync(skillId);
    }
}
