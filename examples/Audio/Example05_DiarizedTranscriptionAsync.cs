using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public async Task Example05_DiarizedTranscriptionAsync()
    {
        AudioClient client = new("gpt-4o-transcribe-diarize", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_meeting.wav");
        string speakerRefPath = Path.Combine("Assets", "audio_agent_reference.wav");

        byte[] speakerRefBytes = File.ReadAllBytes(speakerRefPath);
        string speakerRefBase64 = Convert.ToBase64String(speakerRefBytes);

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
            KnownSpeakerNames = { "agent" },
            KnownSpeakerReferences = { $"data:audio/wav;base64,{speakerRefBase64}" },
        };

        DiarizedAudioTranscription transcription = await client.TranscribeAudioDiarizedAsync(audioFilePath, options);

        Console.WriteLine("Transcription:");
        Console.WriteLine($"{transcription.Text}");

        Console.WriteLine();
        Console.WriteLine($"Duration: {transcription.Duration.TotalSeconds:0.00}s");

        Console.WriteLine();
        Console.WriteLine($"Segments:");
        foreach (DiarizedTranscriptionSegment segment in transcription.Segments)
        {
            Console.WriteLine($"  [{segment.Speaker}] {segment.Text,90} : {segment.StartTime.TotalMilliseconds,5:0} - {segment.EndTime.TotalMilliseconds,5:0}");
        }

        Console.WriteLine();
        Console.WriteLine($"Usage:");
        if (transcription.Usage is TranscriptionTokenUsage tokenUsage)
        {
            Console.WriteLine($"  Input tokens:  {tokenUsage.InputTokens}");
            Console.WriteLine($"  Output tokens: {tokenUsage.OutputTokens}");
            Console.WriteLine($"  Total tokens:  {tokenUsage.TotalTokens}");
            if (tokenUsage.InputTokenDetails is not null)
            {
                Console.WriteLine($"  Input token details:");
                Console.WriteLine($"    Text tokens:  {tokenUsage.InputTokenDetails.TextTokens}");
                Console.WriteLine($"    Audio tokens: {tokenUsage.InputTokenDetails.AudioTokens}");
            }
        }
        else if (transcription.Usage is TranscriptionDurationUsage durationUsage)
        {
            Console.WriteLine($"  Duration: {durationUsage.Seconds.TotalSeconds:0.00}s");
        }
    }
}
