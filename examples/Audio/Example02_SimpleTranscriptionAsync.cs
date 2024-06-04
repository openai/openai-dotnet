using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public async Task Example02_SimpleTranscriptionAsync()
    {
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_houseplant_care.mp3");

        AudioTranscription transcription = await client.TranscribeAudioAsync(audioFilePath);

        Console.WriteLine($"{transcription.Text}");
    }
}
