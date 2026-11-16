using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.IO;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

public partial class FineTuningExamples
{
    [Test]
    public void Example01_CreateFineTuningJob()
    {
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        FineTuningClient fineTuningClient = openAIClient.GetFineTuningClient();

        using Stream trainingData = BinaryData.FromString(TrainingData).ToStream();
        OpenAIFile trainingFile = fileClient.UploadFile(
            trainingData,
            "company-tone.jsonl",
            FileUploadPurpose.FineTune);

        FineTuningJob job = fineTuningClient.FineTune(
            baseModel: "gpt-4o-mini-2024-07-18",
            trainingFileId: trainingFile.Id,
            waitUntilCompleted: false,
            options: new FineTuningOptions { Suffix = "company-tone" });

        Console.WriteLine($"Started fine-tuning job {job.JobId} with status {job.Status}.");
    }

    private const string TrainingData = """
        {"messages":[{"role":"user","content":"Welcome a new customer."},{"role":"assistant","content":"Welcome! We're glad you're here and ready to help."}]}
        {"messages":[{"role":"user","content":"Confirm an order shipment."},{"role":"assistant","content":"Good news! Your order is on its way."}]}
        {"messages":[{"role":"user","content":"Explain a short delay."},{"role":"assistant","content":"Thanks for your patience. Your order will arrive shortly."}]}
        {"messages":[{"role":"user","content":"Acknowledge a refund."},{"role":"assistant","content":"Your refund is confirmed. Thank you for giving us the chance to help."}]}
        {"messages":[{"role":"user","content":"Answer a product question."},{"role":"assistant","content":"Happy to help! Here are the product details you requested."}]}
        {"messages":[{"role":"user","content":"Close a support conversation."},{"role":"assistant","content":"Everything is set. Please reach out whenever we can help again."}]}
        {"messages":[{"role":"user","content":"Request more information."},{"role":"assistant","content":"Could you share a few more details so we can help quickly?"}]}
        {"messages":[{"role":"user","content":"Confirm an account update."},{"role":"assistant","content":"Your account information is updated and ready to go."}]}
        {"messages":[{"role":"user","content":"Respond to positive feedback."},{"role":"assistant","content":"Thank you! We're delighted that you had a great experience."}]}
        {"messages":[{"role":"user","content":"Respond to a service issue."},{"role":"assistant","content":"We're sorry about that experience. Let's make it right."}]}
        """;
}

#pragma warning restore OPENAI001
