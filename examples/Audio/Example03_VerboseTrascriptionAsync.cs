using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AudioExamples
{
    [Test]
    public async Task Example03_VerboseTranscriptionAsync()
    {
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string audioFilePath = Path.Combine("Assets", "audio_houseplant_care.mp3");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            Granularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };

        AudioTranscription transcription = await client.TranscribeAudioAsync(audioFilePath, options);

        Console.WriteLine($"[TRANSCRIPTION]: {transcription.Text}");
        Console.WriteLine($"[WORDS]:");
        foreach (TranscribedWord wordItem in transcription.Words)
        {
            Console.WriteLine($"  {wordItem.Word,10}: {wordItem.Start.TotalMilliseconds,4:0} - {wordItem.End.TotalMilliseconds,4:0}");
        }
        Console.WriteLine($"[SEGMENTS]:");
        foreach (TranscribedSegment segmentItem in transcription.Segments)
        {
            Console.WriteLine($"  {segmentItem.Id,10}: {segmentItem.Text}: "
                + $"{segmentItem.Start.TotalMilliseconds,4:0} - {segmentItem.End.TotalMilliseconds,4:0}");
        }
    }
}
