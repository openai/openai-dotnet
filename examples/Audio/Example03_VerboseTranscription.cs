using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public void Example03_VerboseTranscription()
    {
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_houseplant_care.mp3");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };

        AudioTranscription transcription = client.TranscribeAudio(audioFilePath, options);

        Console.WriteLine("Transcription:");
        Console.WriteLine($"{transcription.Text}");

        Console.WriteLine();
        Console.WriteLine($"Words:");
        foreach (TranscribedWord word in transcription.Words)
        {
            Console.WriteLine($"  {word.Word,15} : {word.StartTime.TotalMilliseconds,5:0} - {word.EndTime.TotalMilliseconds,5:0}");
        }

        Console.WriteLine();
        Console.WriteLine($"Segments:");
        foreach (TranscribedSegment segment in transcription.Segments)
        {
            Console.WriteLine($"  {segment.Text,90} : {segment.StartTime.TotalMilliseconds,5:0} - {segment.EndTime.TotalMilliseconds,5:0}");
        }
    }
}
