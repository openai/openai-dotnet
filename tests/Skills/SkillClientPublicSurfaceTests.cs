#pragma warning disable OPENAI001

using NUnit.Framework;
using OpenAI.Skills;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;

namespace OpenAI.Tests.Skills;

[TestFixture]
public class SkillClientPublicSurfaceTests
{
    [TestCase(nameof(SkillClient.UploadSkill), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.UploadSkillAsync), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.DownloadSkill), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.DownloadSkillAsync), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.UploadSkillVersion), typeof(string), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.UploadSkillVersionAsync), typeof(string), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.DownloadSkillVersion), typeof(string), typeof(string), typeof(RequestOptions))]
    [TestCase(nameof(SkillClient.DownloadSkillVersionAsync), typeof(string), typeof(string), typeof(RequestOptions))]
    public void SkillClientExposesRenamedOperations(string methodName, params Type[] parameterTypes)
    {
        MethodInfo method = typeof(SkillClient).GetMethod(methodName, parameterTypes);

        Assert.That(method, Is.Not.Null);
        Assert.That(method.IsPublic, Is.True);
    }

    [TestCase("CreateSkill", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase("CreateSkillAsync", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase("GetSkillContent", typeof(string), typeof(RequestOptions))]
    [TestCase("GetSkillContentAsync", typeof(string), typeof(RequestOptions))]
    [TestCase("CreateSkillVersion", typeof(string), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase("CreateSkillVersionAsync", typeof(string), typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
    [TestCase("DownloadSkillVersionContent", typeof(string), typeof(string), typeof(RequestOptions))]
    [TestCase("DownloadSkillVersionContentAsync", typeof(string), typeof(string), typeof(RequestOptions))]
    public void SkillClientDoesNotExposeOldOperationNames(string methodName, params Type[] parameterTypes)
    {
        MethodInfo method = typeof(SkillClient).GetMethod(methodName, parameterTypes);

        Assert.That(method, Is.Null);
    }
}
