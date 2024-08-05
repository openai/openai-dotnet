using NUnit.Framework;
using OpenAI.Files;
using OpenAI.FineTuning;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.FineTuning;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.Fixtures)]
public partial class FineTuningTests
{
    [Test]
    public void BasicFineTuningOperationsWork()
    {
        // Upload training file first
        FileClient fileClient = GetTestClient<FileClient>(TestScenario.Files);
        string filename = "toy_chat.jsonl";
        BinaryData fileContent = BinaryData.FromString("""
            {"messages": [{"role": "user", "content": "I lost my book today."}, {"role": "assistant", "content": "You can read everything on ebooks these days!"}]}
            {"messages": [{"role": "system", "content": "You are a happy assistant that puts a positive spin on everything."}, {"role": "assistant", "content": "You're great!"}]}
            """);
        OpenAIFileInfo uploadedFile = fileClient.UploadFile(fileContent, filename, FileUploadPurpose.FineTune);
        Assert.That(uploadedFile?.Filename, Is.EqualTo(filename));

        // Submit fine-tuning job
        FineTuningClient client = GetTestClient();

        string json = $"{{\"training_file\":\"{uploadedFile.Id}\",\"model\":\"gpt-3.5-turbo\"}}";
        BinaryData input = BinaryData.FromString(json);
        using BinaryContent content = BinaryContent.Create(input);

        FineTuningOperation operation = client.CreateJob(ReturnWhen.Started, content);
    }

    [OneTimeTearDown]
    protected void Cleanup()
    {
        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPEN_API_KEY")))
        {
            return;
        }

        FileClient fileClient = new();
        RequestOptions requestOptions = new()
        {
            ErrorOptions = ClientErrorBehaviors.NoThrow,
        };
        foreach (OpenAIFileInfo file in _filesToDelete)
        {
            Console.WriteLine($"Cleanup: {file.Id} -> {fileClient.DeleteFile(file.Id, requestOptions)?.GetRawResponse().Status}");
        }
        _filesToDelete.Clear();
    }

    /// <summary>
    /// Performs basic, invariant validation of a target that was just instantiated from its corresponding origination
    /// mechanism. If applicable, the instance is recorded into the test run for cleanup of persistent resources.
    /// </summary>
    /// <typeparam name="T"> Instance type being validated. </typeparam>
    /// <param name="target"> The instance to validate. </param>
    /// <exception cref="NotImplementedException"> The provided instance type isn't supported. </exception>
    private void Validate<T>(T target)
    {
        if (target is OpenAIFileInfo file)
        {
            Assert.That(file?.Id, Is.Not.Null);
            _filesToDelete.Add(file);
        }
        else
        {
            throw new NotImplementedException($"{nameof(Validate)} helper not implemented for: {typeof(T)}");
        }
    }

    private readonly List<OpenAIFileInfo> _filesToDelete = [];

    private static FineTuningClient GetTestClient() => GetTestClient<FineTuningClient>(TestScenario.FineTuning);

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly string s_cleanupMetadataKey = $"test_metadata_cleanup_eligible";
}

#pragma warning restore OPENAI001
