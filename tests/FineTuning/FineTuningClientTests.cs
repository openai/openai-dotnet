using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.FineTuning;

public static class Extensions
{
    public static bool IsAsync(this Method method) => method == Method.Async;
}

public enum Method
{
    Sync,
    Async
}


[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
[Category("FineTuning")]
public class FineTuningClientTests
{


    FineTuningClient client;
    OpenAIFileClient fileClient;

    string samplePath;
    string validationPath;

    OpenAIFile sampleFile;
    OpenAIFile validationFile;

    [OneTimeSetUp]
    public void Setup()
    {
        client = GetTestClient();
        fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);

        samplePath = Path.Combine("Assets", "fine_tuning_sample.jsonl");
        validationPath = Path.Combine("Assets", "fine_tuning_sample_validation.jsonl");

        sampleFile = fileClient.UploadFile(samplePath, FileUploadPurpose.FineTune);
        validationFile = fileClient.UploadFile(validationPath, FileUploadPurpose.FineTune);

    }

    [OneTimeTearDown]
    public void TearDown()
    {
        fileClient.DeleteFile(sampleFile.Id);
        fileClient.DeleteFile(validationFile.Id);
    }



    [Test]
    [Parallelizable(ParallelScope.All)]
    public async Task MinimalRequiredParams([Values(Method.Sync, Method.Async)] Method method)
    {
        FineTuningJob ft = method.IsAsync()
            ? await client.FineTuneAsync("gpt-3.5-turbo", sampleFile.Id, false)
            : client.FineTune("gpt-3.5-turbo", sampleFile.Id, false);

        // Assert.AreEqual(0, ft.Hyperparameters.CycleCount);
        Assert.True(ft.Status.InProgress);
        Assert.False(ft.HasCompleted);

        _ = method.IsAsync()
            ? await ft.CancelAndUpdateAsync()
            : ft.CancelAndUpdate();

        Assert.AreEqual(FineTuningStatus.Cancelled, ft.Status);
        Assert.False(ft.Status.InProgress);
        Assert.True(ft.HasCompleted);
    }



    [Test]
    [Parallelizable(ParallelScope.All)]
    public async Task AllParameters([Values(Method.Sync, Method.Async)] Method method)
    {
        // This test does not check for Integrations because it requires a valid API key

        var options = new FineTuningOptions()
        {
            TrainingMethod = FineTuningTrainingMethod.CreateSupervised(
                epochCount: 1,
                batchSize: 2,
                learningRate: 3),
            Suffix = "TestFTJob",
            ValidationFile = validationFile.Id,
            Seed = 1234567
        };


        FineTuningJob ft = method.IsAsync()
            ? await client.FineTuneAsync("gpt-3.5-turbo", sampleFile.Id, false, options)
            : client.FineTune("gpt-3.5-turbo", sampleFile.Id, false, options);

        ft.CancelAndUpdate();

#pragma warning disable CS0618
        Assert.AreEqual(1, ft.Hyperparameters.EpochCount);
        Assert.AreEqual(2, ft.Hyperparameters.BatchSize);
        Assert.AreEqual(3, ft.Hyperparameters.LearningRateMultiplier);
#pragma warning restore

        if (ft.MethodHyperparameters is HyperparametersForSupervised hp)
        {
            Assert.AreEqual(1, hp.EpochCount);
            Assert.AreEqual(2, hp.BatchSize);
            Assert.AreEqual(3, hp.LearningRateMultiplier);
        }
        else
        {
            Assert.Fail($"Expected HyperparametersForSupervised, got {ft.MethodHyperparameters?.GetType().ToString() ?? "null"}");
        }

        Assert.AreEqual(ft.UserProvidedSuffix, "TestFTJob");
        Assert.AreEqual(1234567, ft.Seed);
        Assert.AreEqual(validationFile.Id, ft.ValidationFileId);
    }

    [Test]
    [Parallelizable]
    [Explicit("This test requires wandb.ai account and api key integration.")]
    public void WandBIntegrations()
    {
        FineTuningJob job = client.FineTune(
            "gpt-3.5-turbo",
            sampleFile.Id,
            false, options: new()
            {
                Integrations = { new WeightsAndBiasesIntegration("ft-tests") },
            }
        );
        job.CancelAndUpdate();
    }

    [Test]
    [Parallelizable]
    public void ExceptionThrownOnInvalidFileName()
    {
        Assert.Throws<ClientResultException>(() =>
            client.FineTune(baseModel: "gpt-3.5-turbo", trainingFileId: "Invalid File Name", waitUntilCompleted: false)
        );
    }

    [Test]
    [Parallelizable]
    public void ExceptionThrownOnInvalidModelName()
    {
        Assert.Throws<ClientResultException>(() =>
            client.FineTune(baseModel: "gpt-nonexistent", trainingFileId: sampleFile.Id, waitUntilCompleted: false)
        );
    }

    [Test]
    [Parallelizable]
    public void ExceptionThrownOnInvalidValidationId()
    {
        Assert.Throws<ClientResultException>(() =>
        {
            client.FineTune(
                "gpt-3.5-turbo",
                sampleFile.Id,
                false, new() { ValidationFile = "7" }
            );
        });
    }

    [Test]
    [Parallelizable]
    public void ExceptionThrownOnInvalidValidationIdAsync()
    {
        Assert.ThrowsAsync<ClientResultException>(async () =>
        {
            await client.FineTuneAsync(
                "gpt-3.5-turbo",
                sampleFile.Id,
                false, new() { ValidationFile = "7" }
            );
        });
    }

    [Test]
    [Parallelizable(ParallelScope.All)]
    public void GetJobs([Values(Method.Sync, Method.Async)] Method method)
    {
        // Arrange
        Console.WriteLine("Getting jobs");
        var jobs = method.IsAsync()
            ? client.GetJobsAsync().Take(10).ToBlockingEnumerable()
            : client.GetJobs().Take(10);

        Console.WriteLine("Got jobs");

        // Act
        var counter = 0;
        foreach (var job in jobs)  // Network call will happen here on first iteration.
        {
            Console.WriteLine($"{counter} jobs");
            Console.WriteLine($"Job: {job.JobId}");
            Assert.IsTrue(job.JobId.StartsWith("ftjob"));
            counter++;
        }
        Console.WriteLine($"Got {counter} jobs");

        // Assert
        Assert.Greater(counter, 0);
        Assert.LessOrEqual(counter, 10);
    }

    [Test]
    [Parallelizable]
    public void GetJobsWithAfter()
    {
        var firstJob = client.GetJobs().First();

        if (firstJob is null)
        {
            Assert.Fail("No jobs found. At least 2 jobs have to be found to run this test.");
        }
        var secondJob = client.GetJobs(new() { AfterJobId = firstJob.JobId }).First();

        Assert.AreNotEqual(firstJob.JobId, secondJob.JobId);
        // Can't assert that one was created after the next because they might be created at the same second.
        // Assert.Greater(secondJob.CreatedAt, firstJob.CreatedAt, $"{firstJob}, {secondJob}");
    }

    /// Manual experiments show that there are always at least 2 events:
    /// First one is that the job is created
    /// Second one is "validating training file"
    /// If this test starts failing because of the wrong count, please first check if the above is still true
    [Test]
    [Parallelizable(ParallelScope.All)]
    public void GetJobEvents([Values(Method.Sync, Method.Async)] Method method)
    {
        // Arrange
        FineTuningJob job = client.FineTune("gpt-3.5-turbo", sampleFile.Id, false);

        GetEventsOptions options = new()
        {
            PageSize = 1
        };
        job.CancelAndUpdate();

        // Act
        var events = method.IsAsync()
            ? job.GetEventsAsync(options).ToBlockingEnumerable()
            : job.GetEvents(options);

        var first = events.FirstOrDefault();

        // Assert
        if (first is null)
        {
            Assert.Fail("No events found.");
        }
    }

    [Test]
    [Parallelizable(ParallelScope.All)]
    public void GetCheckpoints([Values(Method.Sync, Method.Async)] Method method)
    {
        // Arrange
        // TODO: When `status` option becomes available, use it to get a succeeded job
        FineTuningJob job = client.GetJobs(new() { PageSize = 100 })
                                  .Where((job) => job.Status == "succeeded")
                                  .First();


        // Act
        var checkpoints = method.IsAsync()
            ? job.GetCheckpointsAsync().ToBlockingEnumerable()
            : job.GetCheckpoints();
        FineTuningCheckpoint first = checkpoints.FirstOrDefault();

        // Assert
        if (first is null)
        {
            Assert.Fail("No checkpoints found.");
        }

        FineTuningCheckpointMetrics metrics = first.Metrics;
        Assert.NotNull(metrics);
        Assert.Greater(metrics.StepNumber, 0);
    }

    private static FineTuningClient GetTestClient() => GetTestClient<FineTuningClient>(TestScenario.FineTuning);
}