using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

public partial class FineTuningExamples
{
    [Test]
    public async Task Example01_CreateFineTuningJobAsync()
    {
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        FineTuningClient fineTuningClient = openAIClient.GetFineTuningClient();

        using Stream trainingData = BinaryData.FromString(TrainingData).ToStream();
        OpenAIFile trainingFile = await fileClient.UploadFileAsync(
            trainingData,
            "company-tone.jsonl",
            FileUploadPurpose.FineTune);

        FineTuningJob job = await fineTuningClient.FineTuneAsync(
            baseModel: "gpt-4o-mini-2024-07-18",
            trainingFileId: trainingFile.Id,
            waitUntilCompleted: false,
            options: new FineTuningOptions { Suffix = "company-tone" });

        Console.WriteLine($"Started fine-tuning job {job.JobId} with status {job.Status}.");
    }
}

#pragma warning restore OPENAI001
