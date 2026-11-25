using Moq;
using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class EventContinuationTokenTests
{
    private static async Task<ContinuationToken> GetToken(string json, int? limit, bool asynchronous)
    {
        using HttpClient http = new(new JobHandler());
        FineTuningClient client = new(new ApiKeyCredential("synthetic-key"), new OpenAIClientOptions
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new HttpClientPipelineTransport(http)
        });
        FineTuningJob job = asynchronous ? await client.GetJobAsync("ftjob-fixture") : client.GetJob("ftjob-fixture");
        Mock<PipelineResponse> response = new();
        response.SetupGet(value => value.Status).Returns(200);
        response.SetupGet(value => value.Content).Returns(BinaryData.FromString(json));
        ClientResult page = ClientResult.FromResponse(response.Object);
        return asynchronous
            ? job.GetEventsAsync(new GetEventsOptions { PageSize = limit }).GetContinuationToken(page)
            : job.GetEvents(new GetEventsOptions { PageSize = limit }).GetContinuationToken(page);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task PublicCollectionsUseTheLastEventAsTheCursor(bool asynchronous)
    {
        ContinuationToken token = await GetToken("""{"object":"list","data":[{"id":"event-first"},{"id":"event-last"}],"has_more":true}""", 2, asynchronous);
        Assert.That(token, Is.Not.Null);
        BinaryData bytes = ContinuationToken.FromBytes(token.ToBytes()).ToBytes();
        using JsonDocument serialized = JsonDocument.Parse(bytes);
        Assert.That(serialized.RootElement.GetProperty("jobId").GetString(), Is.EqualTo("ftjob-fixture"));
        Assert.That(serialized.RootElement.GetProperty("after").GetString(), Is.EqualTo("event-last"));
        Assert.That(serialized.RootElement.GetProperty("limit").GetInt32(), Is.EqualTo(2));
    }

    [TestCase("{\"data\":[],\"has_more\":false}")]
    [TestCase("{\"data\":[{\"id\":\"last\"}],\"has_more\":false}")]
    [TestCase("{\"data\":[],\"has_more\":true}")]
    [TestCase("{\"data\":[{}],\"has_more\":true}")]
    [TestCase("{\"data\":[{\"id\":null}],\"has_more\":true}")]
    public async Task PagesWithoutANextCursorReturnNull(string json)
    {
        foreach (bool asynchronous in new[] { false, true })
            Assert.That(await GetToken(json, 2, asynchronous), Is.Null);
    }

    [TestCase(null)]
    [TestCase(1)]
    [TestCase(100)]
    public async Task SingleEventPreservesOptionalPageSize(int? pageSize)
    {
        foreach (bool asynchronous in new[] { false, true })
        {
            ContinuationToken token = await GetToken("""{"data":[{"id":"event-only"}],"has_more":true}""", pageSize, asynchronous);
            using JsonDocument serialized = JsonDocument.Parse(token.ToBytes());
            Assert.That(serialized.RootElement.GetProperty("after").GetString(), Is.EqualTo("event-only"));
            if (pageSize.HasValue)
                Assert.That(serialized.RootElement.GetProperty("limit").GetInt32(), Is.EqualTo(pageSize.Value));
            else Assert.That(serialized.RootElement.TryGetProperty("limit", out _), Is.False);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ExtraLastIdDoesNotChangeTheResult(bool more)
    {
        ContinuationToken token = await GetToken($$"""{"data":[{"id":"event-only"}],"has_more":{{more.ToString().ToLowerInvariant()}},"last_id":"event-only"}""", 2, false);
        if (!more) Assert.That(token, Is.Null);
        else
        {
            using JsonDocument serialized = JsonDocument.Parse(token.ToBytes());
            Assert.That(serialized.RootElement.GetProperty("after").GetString(), Is.EqualTo("event-only"));
        }
    }

    private sealed class JobHandler : HttpMessageHandler
    {
        private static HttpResponseMessage Response(HttpRequestMessage request)
        {
            Assert.That(request.RequestUri.AbsolutePath, Is.EqualTo("/v1/fine_tuning/jobs/ftjob-fixture"));
            return new(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"id":"ftjob-fixture","object":"fine_tuning.job","created_at":0,"model":"synthetic","status":"running","training_file":"file-fixture","result_files":[],"hyperparameters":{},"integrations":[],"metadata":{}}""", Encoding.UTF8, "application/json")
            };
        }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => Response(request);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Response(request));
    }
}
