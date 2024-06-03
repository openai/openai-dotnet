using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public void Example04_SimpleTranslation()
    {
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_french.wav");

        AudioTranslation translation = client.TranslateAudio(audioFilePath);

        Console.WriteLine($"{translation.Text}");
    }
}
