using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public async Task Example01_SimpleTextToSpeechAsync()
    {
        AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string input = "Overwatering is a common issue for those taking care of houseplants. To prevent it, it is"
            + " crucial to allow the soil to dry out between waterings. Instead of watering on a fixed schedule,"
            + " consider using a moisture meter to accurately gauge the soil’s wetness. Should the soil retain"
            + " moisture, it is wise to postpone watering for a couple more days. When in doubt, it is often safer"
            + " to water sparingly and maintain a less-is-more approach.";

        BinaryData speech = await client.GenerateSpeechAsync(input, GeneratedSpeechVoice.Alloy);

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.mp3");
        speech.ToStream().CopyTo(stream);
    }
}
