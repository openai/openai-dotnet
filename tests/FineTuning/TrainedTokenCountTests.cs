using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;

#pragma warning disable OPENAI001

[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public class TrainedTokenCountTests
{
    private static string Job(int? tokens, string status = "succeeded") => JsonSerializer.Serialize(new
    {
        id = "ftjob-synthetic", @object = "fine_tuning.job", model = "synthetic-model",
        created_at = 1, finished_at = 2, organization_id = "org-synthetic",
        fine_tuned_model = "ft:synthetic", result_files = Array.Empty<string>(),
        status, trained_tokens = tokens, training_file = "file-synthetic", validation_file = (string)null,
        hyperparameters = new { n_epochs = 1, batch_size = 1, learning_rate_multiplier = 1.0 },
        integrations = Array.Empty<object>(), seed = 1, metadata = new Dictionary<string, string>()
    });

    private static FineTuningClient Client(bool asynchronous, params string[] responses)
    {
        Queue<string> remaining = new(responses);
        MockPipelineTransport transport = new(_ => {
            Assert.That(remaining, Is.Not.Empty, "Unexpected request from synthetic client");
            return new MockPipelineResponse(200).WithContent(remaining.Dequeue());
        }) { ExpectSyncPipeline = !asynchronous };
        return new FineTuningClient(new ApiKeyCredential("synthetic-key"), new OpenAIClientOptions { Transport = transport });
    }

    [TestCase(false, null)]
    [TestCase(true, null)]
    [TestCase(false, 0)]
    [TestCase(true, 0)]
    [TestCase(false, 123456)]
    [TestCase(true, 123456)]
    [TestCase(false, int.MaxValue)]
    [TestCase(true, int.MaxValue)]
    public async Task RehydrationPreservesReportedTrainedTokens(bool asynchronous, int? tokens)
    {
        FineTuningClient client = Client(asynchronous, Job(tokens));
        FineTuningJob job = asynchronous
            ? await FineTuningJob.RehydrateAsync(client, "ftjob-synthetic")
            : FineTuningJob.Rehydrate(client, "ftjob-synthetic");
        Assert.That(job.BillableTrainedTokenCount, Is.EqualTo(tokens ?? 0));
        Assert.That(job.JobId, Is.EqualTo("ftjob-synthetic"));
        Assert.That(job.HasCompleted, Is.True);
    }

    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task StatusRefreshAndCancellationCopyUpdatedUsage(bool asynchronous, bool cancel)
    {
        FineTuningClient client = Client(asynchronous, Job(null, "running"), Job(654321, cancel ? "cancelled" : "succeeded"));
        FineTuningJob job = asynchronous
            ? await FineTuningJob.RehydrateAsync(client, "ftjob-synthetic")
            : FineTuningJob.Rehydrate(client, "ftjob-synthetic");
        Assert.That(job.BillableTrainedTokenCount, Is.Zero);
        Assert.That(job.HasCompleted, Is.False);
        if (asynchronous)
        {
            if (cancel) await job.CancelAndUpdateAsync();
            else await job.UpdateStatusAsync();
        }
        else
        {
            if (cancel) job.CancelAndUpdate();
            else job.UpdateStatus();
        }
        Assert.That(job.BillableTrainedTokenCount, Is.EqualTo(654321));
        Assert.That(job.HasCompleted, Is.True);
        Assert.That(job.Status, Is.EqualTo(cancel ? FineTuningStatus.Cancelled : FineTuningStatus.Succeeded));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ListedJobsPreserveTheirTokenCount(bool asynchronous)
    {
        string page = "{\"object\":\"list\",\"has_more\":false,\"last_id\":\"ftjob-synthetic\",\"data\":[" + Job(777) + "]}";
        FineTuningClient client = Client(asynchronous, page);
        List<FineTuningJob> jobs = [];
        if (asynchronous)
        {
            await foreach (FineTuningJob job in client.GetJobsAsync(new FineTuningJobCollectionOptions())) jobs.Add(job);
        }
        else jobs.AddRange(client.GetJobs(new FineTuningJobCollectionOptions()));
        Assert.That(jobs, Has.Count.EqualTo(1));
        Assert.That(jobs.Single().BillableTrainedTokenCount, Is.EqualTo(777));
    }
}
