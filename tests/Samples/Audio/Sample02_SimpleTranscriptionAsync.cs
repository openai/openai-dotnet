using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Samples
{
    public partial class AudioSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public async Task Sample02_SimpleTranscriptionAsync()
        {
            AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string audioFilename = "speed-talking.wav";
            string audioPath = Path.Combine("Assets", audioFilename);
            using Stream audio = File.OpenRead(audioPath);

            AudioTranscription transcription = await client.TranscribeAudioAsync(audio, audioFilename);

            Console.WriteLine($"{transcription.Text}");
        }
    }
}
