using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.FineTuning;
using OpenAI.Tests.Utility;
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
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.CreateJobAsync(BinaryContent.Create(data)));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.CreateJob(BinaryContent.Create(data)));
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

        await foreach (ClientResult result in client.GetJobsAsync(after: null, limit: null, options: null))
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

        foreach (ClientResult result in client.GetJobs(after: null, limit: null, options: null))
        {
            BinaryData response = result.GetRawResponse().Content;
            JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement jsonRoot = jsonDocument.RootElement;

            Assert.That(jsonRoot.TryGetProperty("data", out _), Is.True);
            Assert.That(jsonRoot.TryGetProperty("has_more", out _), Is.True);
        }
    }

    [Test]
    public void CancelJobCanParseServiceError()
    {
        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.CancelJobAsync("fakeJobId", options: null));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.CancelJob("fakeJobId", options: null));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void GetJobEventsAsyncCanParseServiceError()
    {
        AssertAsyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        IAsyncEnumerable<ClientResult> enumerable = client.GetJobEventsAsync("fakeJobId", after: null, limit: null, options: null);
        IAsyncEnumerator<ClientResult> enumerator = enumerable.GetAsyncEnumerator();

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await enumerator.MoveNextAsync());

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void GetJobEventsCanParseServiceError()
    {
        AssertSyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        IEnumerable<ClientResult> enumerable = client.GetJobEvents("fakeJobId", after: null, limit: null, options: null);
        IEnumerator<ClientResult> enumerator = enumerable.GetEnumerator();

        ClientResultException ex = Assert.Throws<ClientResultException>(() => enumerator.MoveNext());

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void GetJobCheckpointsAsyncCanParseServiceError()
    {
        AssertAsyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        IAsyncEnumerable<ClientResult> enumerable = client.GetJobCheckpointsAsync("fakeJobId", after: null, limit: null, options: null);
        IAsyncEnumerator<ClientResult> enumerator = enumerable.GetAsyncEnumerator();

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await enumerator.MoveNextAsync());

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void GetJobCheckpointsCanParseServiceError()
    {
        AssertSyncOnly();

        FineTuningClient client = GetTestClient<FineTuningClient>(TestScenario.FineTuning);
        IEnumerable<ClientResult> enumerable = client.GetJobCheckpoints("fakeJobId", after: null, limit: null, options: null);
        IEnumerator<ClientResult> enumerator = enumerable.GetEnumerator();

        ClientResultException ex = Assert.Throws<ClientResultException>(() => enumerator.MoveNext());

        Assert.That(ex.Status, Is.EqualTo(404));
    }
}
