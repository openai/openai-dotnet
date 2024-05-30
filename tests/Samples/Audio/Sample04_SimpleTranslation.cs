using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;

namespace OpenAI.Samples
{
    public partial class AudioSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public void Sample03_SimpleTranslation()
        {
            AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string audioFilename = "french.wav";
            string audioPath = Path.Combine("Assets", audioFilename);
            using Stream audio = File.OpenRead(audioPath);

            AudioTranslation translation = client.TranslateAudio(audio, audioFilename);

            Console.WriteLine($"{translation.Text}");
        }
    }
}
