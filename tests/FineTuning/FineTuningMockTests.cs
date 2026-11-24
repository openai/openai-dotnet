using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;

[Parallelizable(ParallelScope.All)]
[Category("FineTuning")]
[Category("Smoke")]
public class FineTuningMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public FineTuningMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public void WaitForCompletionKeepsWaitingWhenTheEstimatedFinishHasPassed()
    {
        // A running job whose estimated finish is already in the past, as when a job overruns its estimate.
        long estimatedFinish = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();
        int requestCount = 0;
        FineTuningClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ =>
            {
                Interlocked.Increment(ref requestCount);
                return new MockPipelineResponse(200).WithContent($$"""
                    {
                        "object": "fine_tuning.job",
                        "id": "ftjob-abc123",
                        "model": "gpt-4o-mini-2024-07-18",
                        "created_at": {{estimatedFinish - 600}},
                        "finished_at": null,
                        "fine_tuned_model": null,
                        "organization_id": "org-123",
                        "result_files": [],
                        "status": "running",
                        "validation_file": null,
                        "training_file": "file-abc123",
                        "hyperparameters": {
                            "n_epochs": "auto",
                            "batch_size": "auto",
                            "learning_rate_multiplier": "auto"
                        },
                        "trained_tokens": null,
                        "error": null,
                        "seed": 42,
                        "estimated_finish": {{estimatedFinish}}
                    }
                    """);
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        FineTuningClient client = CreateProxyFromClient(new FineTuningClient(s_fakeCredential, clientOptions));
        using CancellationTokenSource cancellationSource = new(TimeSpan.FromMilliseconds(500));

        // The wait must poll on its regular interval until cancelled, not fail on a negative delay.
        Assert.That(
            async () => await client.FineTuneAsync("gpt-4o-mini-2024-07-18", "file-abc123", waitUntilCompleted: true, cancellationToken: cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
        Assert.That(requestCount, Is.EqualTo(1));
    }
}
