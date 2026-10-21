using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.FineTuning;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.All)]
[Category("FineTuning")]
[Category("Smoke")]
public class FineTuningJobMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    private const string RunningJob = """
        {"id":"ftjob-abc","object":"fine_tuning.job","model":"gpt-4o-mini","created_at":1,"status":"running","fine_tuned_model":null,"training_file":"file-abc","organization_id":"org-abc","hyperparameters":{"n_epochs":1,"batch_size":1,"learning_rate_multiplier":1.0},"result_files":[],"integrations":[],"seed":1,"metadata":{}}
        """;

    private const string SucceededJob = """
        {"id":"ftjob-abc","object":"fine_tuning.job","model":"gpt-4o-mini","created_at":1,"finished_at":2,"status":"succeeded","fine_tuned_model":"ft:gpt-4o-mini:org::abc","training_file":"file-abc","organization_id":"org-abc","hyperparameters":{"n_epochs":1,"batch_size":1,"learning_rate_multiplier":1.0},"result_files":["file-result"],"integrations":[],"seed":1,"metadata":{}}
        """;

    public FineTuningJobMockTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task UpdateStatusWithRequestOptionsRefreshesTheJob()
    {
        FineTuningClient client = CreateClient(new MockPipelineResponse(200).WithContent(RunningJob), new MockPipelineResponse(200).WithContent(SucceededJob));
        FineTuningJob job = IsAsync
            ? await FineTuningJob.RehydrateAsync(client, "ftjob-abc")
            : FineTuningJob.Rehydrate(client, "ftjob-abc");

        RequestOptions options = new();
        if (IsAsync)
        {
            await job.UpdateStatusAsync(options);
        }
        else
        {
            job.UpdateStatus(options);
        }

        Assert.That(job.HasCompleted, Is.True);
        Assert.That(job.Status, Is.EqualTo(FineTuningStatus.Succeeded));
        Assert.That(job.Value, Is.EqualTo("ft:gpt-4o-mini:org::abc"));
        Assert.That(job.ResultFileIds, Is.EqualTo(new[] { "file-result" }));
    }

    [Test]
    public async Task UpdateStatusWithNoThrowKeepsTheJobWhenTheServiceReturnsAnError()
    {
        FineTuningClient client = CreateClient(new MockPipelineResponse(200).WithContent(RunningJob), new MockPipelineResponse(404).WithContent("""
            {"error":{"message":"No fine-tuning job found with id ftjob-abc","type":"invalid_request_error"}}
            """));
        FineTuningJob job = IsAsync
            ? await FineTuningJob.RehydrateAsync(client, "ftjob-abc")
            : FineTuningJob.Rehydrate(client, "ftjob-abc");

        RequestOptions options = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };
        ClientResult result = IsAsync
            ? await job.UpdateStatusAsync(options)
            : job.UpdateStatus(options);

        Assert.That(result.GetRawResponse().Status, Is.EqualTo(404));
        Assert.That(job.GetRawResponse().Status, Is.EqualTo(404));
        Assert.That(job.JobId, Is.EqualTo("ftjob-abc"));
        Assert.That(job.Status, Is.EqualTo(FineTuningStatus.Running));
        Assert.That(job.HasCompleted, Is.False);
    }

    private FineTuningClient CreateClient(params MockPipelineResponse[] responses)
    {
        Queue<MockPipelineResponse> remaining = new(responses);
        OpenAIClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ => remaining.Dequeue())
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        return new FineTuningClient(s_fakeCredential, clientOptions);
    }
}
