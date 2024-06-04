using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public async Task Example04_SimpleTranslationAsync()
    {
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_french.wav");

        AudioTranslation translation = await client.TranslateAudioAsync(audioFilePath);

        Console.WriteLine($"{translation.Text}");
    }
}
