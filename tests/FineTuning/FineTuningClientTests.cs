using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.FineTuning;

[Category("FineTuning")]
public class FineTuningClientTests : OpenAIRecordedTestBase
{
    OpenAIFileClient fileClient;

    string samplePath;
    string validationPath;

    OpenAIFile sampleFile;
    OpenAIFile validationFile;

    string sampleFileId;
    string validationFileId;

    public FineTuningClientTests(bool isAsync) : base(isAsync)
    {
    }

    [OneTimeSetUp]
    public void Setup()
    {
        if (Mode == RecordedTestMode.Playback)
        {
            return;
        }
        fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);

        samplePath = Path.Combine("Assets", "fine_tuning_sample.jsonl");
        validationPath = Path.Combine("Assets", "fine_tuning_sample_validation.jsonl");

        sampleFile = fileClient.UploadFile(samplePath, FileUploadPurpose.FineTune);
        sampleFileId = sampleFile.Id;
        validationFile = fileClient.UploadFile(validationPath, FileUploadPurpose.FineTune);
        validationFileId = validationFile.Id;

    }

    [SetUp]
    public void PerTestSetUp()
    {
        if (Mode == RecordedTestMode.Record)
        {
            Recording.SetVariable("SAMPLE_FILE_ID", sampleFileId);
            Recording.SetVariable("VALIDATION_FILE_ID", validationFileId);
            _ = Recording.Now; // To save the date time to the recording file
        }
        if (Mode == RecordedTestMode.Playback)
        {
            var sampleFileId = Recording.GetVariable("SAMPLE_FILE_ID", null);
            var validationFileId = Recording.GetVariable("VALIDATION_FILE_ID", null);

            sampleFile = OpenAIFilesModelFactory.OpenAIFileInfo(
                id: sampleFileId,
                sizeInBytes: 123,
                createdAt: Recording.Now,
                filename: samplePath,
                purpose: FilePurpose.FineTune);

            validationFile = OpenAIFilesModelFactory.OpenAIFileInfo(
                id: validationFileId,
                sizeInBytes: 123,
                createdAt: Recording.Now,
                filename: validationPath,
                purpose: FilePurpose.FineTune);
        }
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (Mode == RecordedTestMode.Playback)
        {
            return;
        }
        fileClient.DeleteFile(sampleFile.Id);
        fileClient.DeleteFile(validationFile.Id);
    }

    [RecordedTest]
    public async Task MinimalRequiredParams()
    {
        FineTuningClient client = GetTestClient();
        FineTuningJob ft = await client.FineTuneAsync("gpt-3.5-turbo", sampleFile.Id, false);

        // Assert.AreEqual(0, ft.Hyperparameters.CycleCount);
        Assert.That(ft.Status.InProgress);
        Assert.That(ft.HasCompleted, Is.False);

        await ft.CancelAndUpdateAsync();

        Assert.That(ft.Status, Is.EqualTo(FineTuningStatus.Cancelled));
        Assert.That(ft.Status.InProgress, Is.False);
        Assert.That(ft.HasCompleted);
    }

    [RecordedTest]
    public async Task AllParameters()
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

        FineTuningClient client = GetTestClient();
        FineTuningJob ft = await client.FineTuneAsync("gpt-3.5-turbo", sampleFile.Id, false, options);

        ft.CancelAndUpdate();

#pragma warning disable CS0618
        Assert.That(ft.Hyperparameters.EpochCount, Is.EqualTo(1));
        Assert.That(ft.Hyperparameters.BatchSize, Is.EqualTo(2));
        Assert.That(ft.Hyperparameters.LearningRateMultiplier, Is.EqualTo(3));
#pragma warning restore

        if (ft.MethodHyperparameters is HyperparametersForSupervised hp)
        {
            Assert.That(hp.EpochCount, Is.EqualTo(1));
            Assert.That(hp.BatchSize, Is.EqualTo(2));
            Assert.That(hp.LearningRateMultiplier, Is.EqualTo(3));
        }
        else
        {
            Assert.Fail($"Expected HyperparametersForSupervised, got {ft.MethodHyperparameters?.GetType().ToString() ?? "null"}");
        }

        Assert.That(ft.UserProvidedSuffix, Is.EqualTo("TestFTJob"));
        Assert.That(ft.Seed, Is.EqualTo(1234567));
        Assert.That(ft.ValidationFileId, Is.EqualTo(validationFile.Id));
    }

    [RecordedTest]
    [Explicit("This test requires wandb.ai account and api key integration.")]
    public void WandBIntegrations()
    {
        FineTuningClient client = GetTestClient();
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

    [RecordedTest]
    public void ExceptionThrownOnInvalidFileName()
    {
        FineTuningClient client = GetTestClient();
        Assert.ThrowsAsync<ClientResultException>(async () =>
            await client.FineTuneAsync(baseModel: "gpt-3.5-turbo", trainingFileId: "Invalid File Name", waitUntilCompleted: false)
        );
    }

    [RecordedTest]
    public void ExceptionThrownOnInvalidModelName()
    {
        FineTuningClient client = GetTestClient();
        Assert.ThrowsAsync<ClientResultException>(async () =>
            await client.FineTuneAsync(baseModel: "gpt-nonexistent", trainingFileId: sampleFile.Id, waitUntilCompleted: false)
        );
    }

    [RecordedTest]
    public void ExceptionThrownOnInvalidValidationIdAsync()
    {
        FineTuningClient client = GetTestClient();
        Assert.ThrowsAsync<ClientResultException>(async () =>
        {
            await client.FineTuneAsync(
                "gpt-3.5-turbo",
                sampleFile.Id,
                false, new() { ValidationFile = "7" }
            );
        });
    }

    [RecordedTest]
    public void GetJobs()
    {
        FineTuningClient client = GetTestClient();

        // Arrange
        Console.WriteLine("Getting jobs");
        var jobs = client.GetJobsAsync().Take(10).ToBlockingEnumerable();

        Console.WriteLine("Got jobs");

        // Act
        var counter = 0;
        foreach (var job in jobs)  // Network call will happen here on first iteration.
        {
            Console.WriteLine($"{counter} jobs");
            Console.WriteLine($"Job: {job.JobId}");
            Assert.That(job.JobId.StartsWith("ftjob"));
            counter++;
        }
        Console.WriteLine($"Got {counter} jobs");

        // Assert
        Assert.That(counter, Is.GreaterThan(0));
        Assert.That(counter, Is.LessThanOrEqualTo(10));
    }

    [RecordedTest]
    public async Task GetJobsWithAfter()
    {
        FineTuningClient client = GetTestClient();
        var firstJob = await client.GetJobsAsync().FirstAsync();

        if (firstJob is null)
        {
            Assert.Fail("No jobs found. At least 2 jobs have to be found to run this test.");
        }
        var secondJob = await client.GetJobsAsync(new() { AfterJobId = firstJob.JobId }).FirstAsync();

        Assert.That(secondJob.JobId, Is.Not.EqualTo(firstJob.JobId));
        // Can't assert that one was created after the next because they might be created at the same second.
        // Assert.Greater(secondJob.CreatedAt, firstJob.CreatedAt, $"{firstJob}, {secondJob}");
    }

    /// Manual experiments show that there are always at least 2 events:
    /// First one is that the job is created
    /// Second one is "validating training file"
    /// If this test starts failing because of the wrong count, please first check if the above is still true
    [RecordedTest]
    public async Task GetJobEvents()
    {
        FineTuningClient client = GetTestClient();
        // Arrange
        FineTuningJob job = await client.FineTuneAsync("gpt-3.5-turbo", sampleFile.Id, false);

        GetEventsOptions options = new()
        {
            PageSize = 1
        };
        job.CancelAndUpdate();

        // Act
        var events = IsAsync
            ? job.GetEventsAsync(options).ToBlockingEnumerable()
            : job.GetEvents(options);

        var first = events.FirstOrDefault();

        // Assert
        if (first is null)
        {
            Assert.Fail("No events found.");
        }
    }

    [RecordedTest]
    public async Task GetCheckpoints()
    {
        FineTuningClient client = GetTestClient();
        // Arrange
        // TODO: When `status` option becomes available, use it to get a succeeded job
        FineTuningJob job = await client.GetJobsAsync(new() { PageSize = 100 })
                                  .Where((job) => job.Status == "succeeded").FirstAsync();


        // Act
        var checkpoints = IsAsync
            ? job.GetCheckpointsAsync().ToBlockingEnumerable()
            : job.GetCheckpoints();
        FineTuningCheckpoint first = checkpoints.FirstOrDefault();

        // Assert
        if (first is null)
        {
            Assert.Fail("No checkpoints found.");
        }

        FineTuningCheckpointMetrics metrics = first.Metrics;
        Assert.That(metrics, Is.Not.Null);
        Assert.That(metrics.StepNumber, Is.GreaterThan(0));
    }

    private FineTuningClient GetTestClient() => GetProxiedOpenAIClient<FineTuningClient>(TestScenario.FineTuning);
}