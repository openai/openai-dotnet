using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;

[Category("Smoke")]
public class ExpiredEstimateTests
{
    private static FineTuningJob CreateJob(string status, DateTimeOffset? estimate)
    {
        BinaryData content = BinaryData.FromString(JsonSerializer.Serialize(new
        {
            id = "ftjob-synthetic", @object = "fine_tuning.job", model = "synthetic-model",
            created_at = 0, finished_at = (long?)null, fine_tuned_model = (string)null,
            organization_id = "synthetic-org", result_files = Array.Empty<string>(),
            status, training_file = "file-synthetic", validation_file = (string)null,
            trained_tokens = (int?)null, error = (object)null,
            hyperparameters = new { n_epochs = "auto", batch_size = "auto", learning_rate_multiplier = "auto" },
            estimated_finish = estimate?.ToUnixTimeSeconds(), seed = 1,
        }));
        OpenAIClientOptions options = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent(content.ToString())),
        };
        FineTuningClient client = new(new ApiKeyCredential("synthetic-test-key"), options);
        return FineTuningJob.Rehydrate(client, "ftjob-synthetic");
    }

    [TestCase("running", -120)]
    [TestCase("queued", -120)]
    [TestCase("running", null)]
    [TestCase("queued", null)]
    [TestCase("running", 120)]
    [TestCase("queued", 120)]
    public void PollingDelayRemainsPositiveAfterEstimateExpires(string status, int? offset)
    {
        DateTimeOffset? estimate = offset.HasValue ? DateTimeOffset.UtcNow.AddSeconds(offset.Value) : null;
        FineTuningJob job = CreateJob(status, estimate);
        MethodInfo method = typeof(FineTuningJob).GetMethod("GetDelay", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);
        TimeSpan delay = (TimeSpan)method.Invoke(job, null);
        Assert.That(delay, Is.GreaterThan(TimeSpan.Zero));
        if (offset.GetValueOrDefault() <= 0)
            Assert.That(delay, Is.EqualTo(TimeSpan.FromSeconds(status == "running" ? 30 : 1)));
        else
            Assert.That(delay.TotalSeconds, Is.InRange(117, 120));
    }

    [TestCase(false, "running", -120)]
    [TestCase(true, "running", -120)]
    [TestCase(false, "queued", -120)]
    [TestCase(true, "queued", -120)]
    [TestCase(false, "running", null)]
    [TestCase(true, "running", null)]
    [TestCase(false, "running", 120)]
    [TestCase(true, "running", 120)]
    public async Task CancellationIsNotReplacedByInvalidDelay(bool asynchronous, string status, int? offset)
    {
        FineTuningJob job = CreateJob(status, offset.HasValue ? DateTimeOffset.UtcNow.AddSeconds(offset.Value) : null);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        if (asynchronous)
        {
            Assert.CatchAsync<OperationCanceledException>(async () => await job.WaitForCompletionAsync(cancellation.Token));
        }
        else
        {
            Assert.Catch<OperationCanceledException>(() => job.WaitForCompletion(cancellation.Token));
        }
        await Task.CompletedTask;
    }

    [TestCase("succeeded")]
    [TestCase("failed")]
    [TestCase("cancelled")]
    public async Task CompletedJobsDoNotWaitForAnExpiredEstimate(string status)
    {
        FineTuningJob job = CreateJob(status, DateTimeOffset.UtcNow.AddDays(-1));
        Assert.That(job.HasCompleted, Is.True);
        Assert.DoesNotThrow(() => job.WaitForCompletion());
        await job.WaitForCompletionAsync();
    }
}
