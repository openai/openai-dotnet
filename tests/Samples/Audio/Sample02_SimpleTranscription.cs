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
        public void Sample02_SimpleTranscription()
        {
            AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string audioFilename = "speed-talking.wav";
            string audioPath = Path.Combine("Assets", audioFilename);
            using Stream audio = File.OpenRead(audioPath);

            AudioTranscription transcription = client.TranscribeAudio(audio, audioFilename);

            Console.WriteLine($"{transcription.Text}");
        }
    }
}
