using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Category("Audio")]
public partial class VoiceTests : OpenAIRecordedTestBase
{
    public VoiceTests(bool isAsync) : base(isAsync)
    {
    }
    
    [Ignore("Voice API requires organization-level access that is not yet available.")]
    [Test]
    public async Task CreateVoiceConsentAndVoiceWorks()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>();

        // Step 1: Create a voice consent recording
        string consentAudioPath = Path.Combine("Assets", "audio_agent_reference.wav");
        byte[] consentAudioBytes = await File.ReadAllBytesAsync(consentAudioPath);

        using MultiPartFormDataBinaryContent consentContent = new();
        consentContent.Add("Test Consent", "name");
        consentContent.Add("en-US", "language");
        consentContent.Add(consentAudioBytes, "recording", "consent_recording.wav", "audio/x-wav");

        ClientResult consentResult = await client.CreateVoiceConsentAsync(consentContent, consentContent.ContentType);

        BinaryData consentResponseData = consentResult.GetRawResponse().Content;
        using JsonDocument consentJson = JsonDocument.Parse(consentResponseData);
        JsonElement consentRoot = consentJson.RootElement;

        Assert.That(consentRoot.GetProperty("object").GetString(), Is.EqualTo("audio.voice_consent"));

        string consentId = consentRoot.GetProperty("id").GetString();
        Assert.That(consentId, Is.Not.Null.And.Not.Empty);
        Assert.That(consentRoot.GetProperty("name").GetString(), Is.EqualTo("Test Consent"));
        Assert.That(consentRoot.GetProperty("language").GetString(), Is.EqualTo("en-US"));
        Assert.That(consentRoot.GetProperty("created_at").GetInt64(), Is.GreaterThan(0));

        // Step 2: Create a custom voice using the consent
        string audioSamplePath = Path.Combine("Assets", "audio_agent_reference.wav");
        byte[] audioSampleBytes = await File.ReadAllBytesAsync(audioSamplePath);

        using MultiPartFormDataBinaryContent voiceContent = new();
        voiceContent.Add("Test Voice", "name");
        voiceContent.Add(consentId, "consent");
        voiceContent.Add(audioSampleBytes, "audio_sample", "audio_sample.wav", "audio/x-wav");

        ClientResult voiceResult = await client.CreateVoiceAsync(voiceContent, voiceContent.ContentType);

        BinaryData voiceResponseData = voiceResult.GetRawResponse().Content;
        using JsonDocument voiceJson = JsonDocument.Parse(voiceResponseData);
        JsonElement voiceRoot = voiceJson.RootElement;

        Assert.That(voiceRoot.GetProperty("object").GetString(), Is.EqualTo("audio.voice"));

        string voiceId = voiceRoot.GetProperty("id").GetString();
        Assert.That(voiceId, Is.Not.Null.And.Not.Empty);
        Assert.That(voiceRoot.GetProperty("name").GetString(), Is.EqualTo("Test Voice"));
        Assert.That(voiceRoot.GetProperty("created_at").GetInt64(), Is.GreaterThan(0));
    }
}
