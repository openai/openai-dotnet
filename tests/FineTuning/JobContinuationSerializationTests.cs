using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.IO;
using System.Text;
using System.Text.Json;

namespace OpenAI.Tests.FineTuning;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class JobContinuationSerializationTests
{
    [TestCase(false, null)]
    [TestCase(true, null)]
    [TestCase(false, 1)]
    [TestCase(true, 1)]
    [TestCase(false, 100)]
    [TestCase(true, 100)]
    public void JobContinuationCanBeSerializedWithoutLosingItsCursor(bool asynchronous, int? pageSize)
    {
        FineTuningClient client = new("synthetic-key");
        FineTuningJobCollectionOptions options = new() { PageSize = pageSize };
        MockPipelineResponse response = new(200, "OK");
        response.ContentStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"has_more\":true,\"data\":[{\"id\":\"ftjob-first\"},{\"id\":\"ftjob-last\"}]}"));
        ClientResult page = ClientResult.FromResponse(response);
        ContinuationToken token = asynchronous
            ? client.GetJobsAsync(options).GetContinuationToken(page)
            : client.GetJobs(options).GetContinuationToken(page);
        Assert.That(token, Is.Not.Null);
        BinaryData serialized = token.ToBytes();
        using JsonDocument document = JsonDocument.Parse(serialized);
        Assert.That(document.RootElement.GetProperty("after").GetString(), Is.EqualTo("ftjob-last"));
        if (pageSize.HasValue)
            Assert.That(document.RootElement.GetProperty("limit").GetInt32(), Is.EqualTo(pageSize.Value));
        else
            Assert.That(document.RootElement.TryGetProperty("limit", out _), Is.False);
        Assert.That(token.ToBytes().ToString(), Is.EqualTo(serialized.ToString()));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TheFinalPageHasNoContinuationToken(bool asynchronous)
    {
        FineTuningClient client = new("synthetic-key");
        MockPipelineResponse response = new(200, "OK");
        response.ContentStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"has_more\":false,\"data\":[{\"id\":\"ftjob-last\"}]}"));
        ClientResult page = ClientResult.FromResponse(response);
        ContinuationToken token = asynchronous
            ? client.GetJobsAsync().GetContinuationToken(page)
            : client.GetJobs().GetContinuationToken(page);
        Assert.That(token, Is.Null);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void EmptyFinalPagesHaveNoContinuationToken(bool asynchronous)
    {
        FineTuningClient client = new("synthetic-key");
        MockPipelineResponse response = new(200, "OK");
        response.ContentStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"has_more\":false,\"data\":[]}"));
        ClientResult page = ClientResult.FromResponse(response);
        ContinuationToken token = asynchronous
            ? client.GetJobsAsync().GetContinuationToken(page)
            : client.GetJobs().GetContinuationToken(page);
        Assert.That(token, Is.Null);
    }
}
