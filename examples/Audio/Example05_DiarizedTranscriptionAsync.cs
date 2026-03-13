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
            ResponseFormat = AudioTranscriptionFormat.Diarized,
            KnownSpeakerNames = { "agent" },
            KnownSpeakerReferenceUris = { new Uri($"data:audio/wav;base64,{speakerRefBase64}") },
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
        if (transcription.Usage is AudioTranscriptionTokenUsage tokenUsage)
        {
            Console.WriteLine($"  Input tokens:  {tokenUsage.InputTokenCount}");
            Console.WriteLine($"  Output tokens: {tokenUsage.OutputTokenCount}");
            Console.WriteLine($"  Total tokens:  {tokenUsage.TotalTokenCount}");
            if (tokenUsage.InputTokenDetails is not null)
            {
                Console.WriteLine($"  Input token details:");
                Console.WriteLine($"    Text tokens:  {tokenUsage.InputTokenDetails.TextTokenCount}");
                Console.WriteLine($"    Audio tokens: {tokenUsage.InputTokenDetails.AudioTokenCount}");
            }
        }
        else if (transcription.Usage is AudioTranscriptionDurationUsage durationUsage)
        {
            Console.WriteLine($"  Duration: {durationUsage.Duration.TotalSeconds:0.00}s");
        }
    }
}
