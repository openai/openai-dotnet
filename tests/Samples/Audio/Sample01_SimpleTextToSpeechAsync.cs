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
        public async Task Sample01_SimpleTextToSpeechAsync()
        {
            AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string input = "The sun rises in the east and sets in the west. This simple fact has been"
                + " observed by humans for thousands of years.";

            BinaryData speech = await client.GenerateSpeechFromTextAsync(input, GeneratedSpeechVoice.Alloy);

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.mp3");
            speech.ToStream().CopyTo(stream);
        }
    }
}
