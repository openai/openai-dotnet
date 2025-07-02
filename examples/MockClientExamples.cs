using System.ClientModel.Primitives;
using System.ClientModel;
using Moq;
using NUnit.Framework;
using OpenAI.Audio;

namespace OpenAI.Examples.Miscellaneous;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class MockClientExamples
{
    [Test]
    public void MockClient()
    {
        // Instantiate mocks and the AudioTranscription object.

        Mock<AudioClient> mockClient = new();
        Mock<ClientResult<AudioTranscription>> mockResult = new(null, Mock.Of<PipelineResponse>());
        AudioTranscription transcription = OpenAIAudioModelFactory.AudioTranscription(text: "I swear I saw an apple flying yesterday!");

        // Set up mocks' properties and methods.

        mockResult
            .SetupGet(result => result.Value)
            .Returns(transcription);

        mockClient.Setup(client => client.TranscribeAudio(
                It.IsAny<string>(),
                It.IsAny<AudioTranscriptionOptions>()))
            .Returns(mockResult.Object);

        // Perform validation.

        AudioClient client = mockClient.Object;
        bool containsSecretWord = ContainsSecretWord(client, "<audioFilePath>", "apple");

        Assert.That(containsSecretWord, Is.True);
    }

    public bool ContainsSecretWord(AudioClient client, string audioFilePath, string secretWord)
    {
        AudioTranscription transcription = client.TranscribeAudio(audioFilePath);
        return transcription.Text.Contains(secretWord);
    }
}

#pragma warning restore OPENAI001