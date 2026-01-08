using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.Threading;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
internal class GenerateSpeechMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public GenerateSpeechMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public void GenerateSpeechRespectsTheCancellationToken()
    {
        AudioClient client = CreateProxyFromClient(new AudioClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateSpeechAsync("text", GeneratedSpeechVoice.Echo, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }
}
