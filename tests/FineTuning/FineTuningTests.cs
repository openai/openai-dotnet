using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.FineTuning;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("FineTuning")]
internal class FineTuningTests : SyncAsyncTestBase
{
    public FineTuningTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public void CreateJobCanParseServiceError()
    {
        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        BinaryData data = BinaryData.FromString($$"""
        {
            "model": "gpt-3.5-turbo"
        }
        """);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.CreateFineTuningJobAsync(BinaryContent.Create(data), waitUntilCompleted: false));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.CreateFineTuningJob(BinaryContent.Create(data), waitUntilCompleted: false));
        }

        Assert.That(ex.Status, Is.EqualTo(400));
        Assert.That(ex.Message, Contains.Substring("training_file"));
    }

    [Test]
    public void GetJobCanParseServiceError()
    {
        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GetJobAsync("fakeJobId", options: null));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GetJob("fakeJobId", options: null));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task GetJobsAsyncWorks()
    {
        AssertAsyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);

        await foreach (ClientResult result in client.GetJobsAsync(after: null, limit: null, options: null).GetRawPagesAsync())
        {
            BinaryData response = result.GetRawResponse().Content;
            JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement jsonRoot = jsonDocument.RootElement;

            Assert.That(jsonRoot.TryGetProperty("data", out _), Is.True);
            Assert.That(jsonRoot.TryGetProperty("has_more", out _), Is.True);
        }
    }

    [Test]
    public void GetJobsWorks()
    {
        AssertSyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);

        foreach (ClientResult result in client.GetJobs(after: null, limit: null, options: null).GetRawPages())
        {
            BinaryData response = result.GetRawResponse().Content;
            JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement jsonRoot = jsonDocument.RootElement;

            Assert.That(jsonRoot.TryGetProperty("data", out _), Is.True);
            Assert.That(jsonRoot.TryGetProperty("has_more", out _), Is.True);
        }
    }

    // We need to add this test back once we have access to the test resources
    //
    //[Test]
    //public void BasicFineTuningJobOperationsWork()
    //{
    //    // Upload training file first
    //    FileClient fileClient = GetTestClient<FileClient>(TestScenario.Files);
    //    string filename = "toy_chat.jsonl";
    //    BinaryData fileContent = BinaryData.FromString("""
    //        {"messages": [{"role": "user", "content": "I lost my book today."}, {"role": "assistant", "content": "You can read everything on ebooks these days!"}]}
    //        {"messages": [{"role": "system", "content": "You are a happy assistant that puts a positive spin on everything."}, {"role": "assistant", "content": "You're great!"}]}
    //        """);
    //    OpenAIFile uploadedFile = fileClient.UploadFile(fileContent, filename, FileUploadPurpose.FineTune);
    //    Assert.That(uploadedFile?.Filename, Is.EqualTo(filename));

    //    // Submit fine-tuning job
    //    FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);

    //    string json = $"{{\"training_file\":\"{uploadedFile.Id}\",\"model\":\"gpt-3.5-turbo\"}}";
    //    BinaryData input = BinaryData.FromString(json);
    //    using BinaryContent content = BinaryContent.Create(input);

    //    FineTuningJobOperation operation = client.CreateJob(content, waitUntilCompleted: false);
    //}
}
