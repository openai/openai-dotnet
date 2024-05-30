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
        public void Sample01_SimpleTextToSpeech()
        {
            AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string input = "The sun rises in the east and sets in the west. This simple fact has been"
                + " observed by humans for thousands of years.";

            BinaryData speech = client.GenerateSpeechFromText(input, GeneratedSpeechVoice.Alloy);

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.mp3");
            speech.ToStream().CopyTo(stream);
        }
    }
}
