using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[TestFixture(true)]
[TestFixture(false)]
[Category("Assistants")]
public class AssistantsTests : SyncAsyncTestBase
{
    private readonly List<Assistant> _assistantsToDelete = [];
    private readonly List<AssistantThread> _threadsToDelete = [];
    private readonly List<ThreadMessage> _messagesToDelete = [];
    private readonly List<OpenAIFile> _filesToDelete = [];
    private readonly List<string> _vectorStoreIdsToDelete = [];

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly string s_testAssistantName = $".NET SDK Test Assistant - Please Delete Me";
    private static readonly string s_cleanupMetadataKey = $"test_metadata_cleanup_eligible";

    private static AssistantClient GetTestClient() => GetTestClient<AssistantClient>(TestScenario.Assistants);

    public AssistantsTests(bool isAsync)
        : base(isAsync)
    {
    }

    [OneTimeTearDown]
    protected void Cleanup()
    {
        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            return;
        }

        AssistantClient client = GetTestClient<AssistantClient>(TestScenario.Assistants);
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        VectorStoreClient vectorStoreClient = GetTestClient<VectorStoreClient>(TestScenario.VectorStores);
        RequestOptions requestOptions = new()
        {
            ErrorOptions = ClientErrorBehaviors.NoThrow,
        };
        foreach (ThreadMessage message in _messagesToDelete)
        {
            Console.WriteLine($"Cleanup: {message.Id} -> {client.DeleteMessage(message.ThreadId, message.Id, requestOptions)?.GetRawResponse().Status}");
        }
        foreach (Assistant assistant in _assistantsToDelete)
        {
            Console.WriteLine($"Cleanup: {assistant.Id} -> {client.DeleteAssistant(assistant.Id, requestOptions)?.GetRawResponse().Status}");
        }
        foreach (AssistantThread thread in _threadsToDelete)
        {
            Console.WriteLine($"Cleanup: {thread.Id} -> {client.DeleteThread(thread.Id, requestOptions)?.GetRawResponse().Status}");
        }
        foreach (OpenAIFile file in _filesToDelete)
        {
            Console.WriteLine($"Cleanup: {file.Id} -> {fileClient.DeleteFile(file.Id, requestOptions)?.GetRawResponse().Status}");
        }
        foreach (string vectorStoreId in _vectorStoreIdsToDelete)
        {
            Console.WriteLine($"Cleanup: {vectorStoreId} => {vectorStoreClient.DeleteVectorStore(vectorStoreId, requestOptions)?.GetRawResponse().Status}");
        }
        _messagesToDelete.Clear();
        _assistantsToDelete.Clear();
        _threadsToDelete.Clear();
        _filesToDelete.Clear();
        _vectorStoreIdsToDelete.Clear();
    }

    [Test]
    public async Task BasicAssistantOperationsWork()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = IsAsync
            ? await client.CreateAssistantAsync("gpt-4o-mini")
            : client.CreateAssistant("gpt-4o-mini");
        Validate(assistant);
        Assert.That(assistant.Name, Is.Null.Or.Empty);
        AssistantModificationOptions modificationOptions = new AssistantModificationOptions()
        {
            Name = "test assistant name",
        };
        assistant = IsAsync
            ? await client.ModifyAssistantAsync(assistant.Id, modificationOptions)
            : client.ModifyAssistant(assistant.Id, modificationOptions);
        Assert.That(assistant.Name, Is.EqualTo("test assistant name"));
        AssistantDeletionResult deletionResult = IsAsync
            ? await client.DeleteAssistantAsync(assistant.Id)
            : client.DeleteAssistant(assistant.Id);
        Assert.That(deletionResult.AssistantId, Is.EqualTo(assistant.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _assistantsToDelete.Remove(assistant);
        AssistantCreationOptions creationOptions = new AssistantCreationOptions()
        {
            Metadata =
            {
                [s_cleanupMetadataKey] = "hello!"
            },
        };
        assistant = IsAsync
            ? await client.CreateAssistantAsync("gpt-4o-mini", creationOptions)
            : client.CreateAssistant("gpt-4o-mini", creationOptions);
        Validate(assistant);
        Assistant retrievedAssistant = client.GetAssistant(assistant.Id);
        Assert.That(retrievedAssistant.Id, Is.EqualTo(assistant.Id));
        Assert.That(retrievedAssistant.Metadata.TryGetValue(s_cleanupMetadataKey, out string metadataValue) && metadataValue == "hello!");
        modificationOptions = new AssistantModificationOptions()
        {
            Metadata =
            {
                [s_cleanupMetadataKey] = "goodbye!",
            },
        };
        Assistant modifiedAssistant = IsAsync
            ? await client.ModifyAssistantAsync(assistant.Id, modificationOptions)
            : client.ModifyAssistant(assistant.Id, modificationOptions);
        Assert.That(modifiedAssistant.Id, Is.EqualTo(assistant.Id));
        Assistant listedAssistant = null;
        if (IsAsync)
        {
            AsyncCollectionResult<Assistant> recentAssistants = client.GetAssistantsAsync();

            await foreach (Assistant pageItem in recentAssistants)
            {
                if (pageItem.Id == assistant.Id)
                {
                    listedAssistant = pageItem;
                    break;
                }
            }
        }
        else
        {
            CollectionResult<Assistant> recentAssistants = client.GetAssistants();
            listedAssistant = recentAssistants.FirstOrDefault(pageItem => pageItem.Id == assistant.Id);
        }
        Assert.That(listedAssistant, Is.Not.Null);
        Assert.That(listedAssistant.Metadata.TryGetValue(s_cleanupMetadataKey, out string newMetadataValue) && newMetadataValue == "goodbye!");
    }

    [Test]
    public async Task BasicThreadOperationsWork()
    {
        AssistantClient client = GetTestClient();
        AssistantThread thread = IsAsync
            ? await client.CreateThreadAsync()
            : client.CreateThread();
        Validate(thread);
        Assert.That(thread.CreatedAt, Is.GreaterThan(s_2024));
        ThreadDeletionResult deletionResult = IsAsync
            ? await client.DeleteThreadAsync(thread.Id)
            : client.DeleteThread(thread.Id);
        Assert.That(deletionResult.ThreadId, Is.EqualTo(thread.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _threadsToDelete.Remove(thread);

        ThreadCreationOptions options = new()
        {
            Metadata =
            {
                ["threadMetadata"] = "threadMetadataValue",
            }
        };
        thread = IsAsync
            ? await client.CreateThreadAsync(options)
            : client.CreateThread(options);
        Validate(thread);
        Assert.That(thread.Metadata.TryGetValue("threadMetadata", out string threadMetadataValue) && threadMetadataValue == "threadMetadataValue");
        AssistantThread retrievedThread = IsAsync
            ? await client.GetThreadAsync(thread.Id)
            : client.GetThread(thread.Id);
        Assert.That(retrievedThread.Id, Is.EqualTo(thread.Id));
        ThreadModificationOptions modificationOptions = new ThreadModificationOptions()
        {
            Metadata =
            {
                ["threadMetadata"] = "newThreadMetadataValue",
            },
        };
        thread = IsAsync
            ? await client.ModifyThreadAsync(thread.Id, modificationOptions)
            : client.ModifyThread(thread.Id, modificationOptions);
        Assert.That(thread.Metadata.TryGetValue("threadMetadata", out threadMetadataValue) && threadMetadataValue == "newThreadMetadataValue");
    }

    [Test]
    public async Task BasicMessageOperationsWork()
    {
        AssistantClient client = GetTestClient();
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        ThreadMessage message = IsAsync
            ? await client.CreateMessageAsync(thread.Id, MessageRole.User, ["Hello, world!"])
            : client.CreateMessage(thread.Id, MessageRole.User, ["Hello, world!"]);
        Validate(message);
        Assert.That(message.CreatedAt, Is.GreaterThan(s_2024));
        Assert.That(message.Content?.Count, Is.EqualTo(1));
        Assert.That(message.Content[0], Is.Not.Null);
        Assert.That(message.Content[0].Text, Is.EqualTo("Hello, world!"));
        MessageDeletionResult deletionResult = IsAsync
            ? await client.DeleteMessageAsync(message.ThreadId, message.Id)
            : client.DeleteMessage(message.ThreadId, message.Id);
        Assert.That(deletionResult.MessageId, Is.EqualTo(message.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _messagesToDelete.Remove(message);

        MessageCreationOptions creationOptions = new MessageCreationOptions()
        {
            Metadata =
            {
                ["messageMetadata"] = "messageMetadataValue",
            },
        };
        message = IsAsync
            ? await client.CreateMessageAsync(thread.Id, MessageRole.User, ["Goodbye, world!"], creationOptions)
            : client.CreateMessage(thread.Id, MessageRole.User, ["Goodbye, world!"], creationOptions);
        Validate(message);
        Assert.That(message.Metadata.TryGetValue("messageMetadata", out string metadataValue) && metadataValue == "messageMetadataValue");

        ThreadMessage retrievedMessage = IsAsync
            ? await client.GetMessageAsync(message.ThreadId, message.Id)
            : client.GetMessage(message.ThreadId, message.Id);
        Assert.That(retrievedMessage.Id, Is.EqualTo(message.Id));

        MessageModificationOptions modificationOptions = new MessageModificationOptions()
        {
            Metadata =
            {
                ["messageMetadata"] = "newValue",
            }
        };
        message = IsAsync
            ? await client.ModifyMessageAsync(message.ThreadId, message.Id, modificationOptions)
            : client.ModifyMessage(message.ThreadId, message.Id, modificationOptions);
        Assert.That(message.Metadata.TryGetValue("messageMetadata", out metadataValue) && metadataValue == "newValue");

        List<ThreadMessage> messages = IsAsync
            ? await client.GetMessagesAsync(thread.Id).ToListAsync()
            : [.. client.GetMessages(thread.Id)];

        Assert.That(messages.Count, Is.EqualTo(1));
        Assert.That(messages[0].Id, Is.EqualTo(message.Id));
        Assert.That(messages[0].Metadata.TryGetValue("messageMetadata", out metadataValue) && metadataValue == "newValue");
    }

    [Test]
    public async Task ThreadWithInitialMessagesWorks()
    {
        AssistantClient client = GetTestClient();
        ThreadCreationOptions options = new()
        {
            InitialMessages =
            {
                "Hello, world!",
                new(
                    MessageRole.User,
                    [
                        "Can you describe this image for me?",
                        MessageContent.FromImageUri(new Uri("https://test.openai.com/image.png"))
                    ])
                {
                    Metadata =
                    {
                        ["messageMetadata"] = "messageMetadataValue",
                    },
                },
            },
        };
        AssistantThread thread = IsAsync
            ? await client.CreateThreadAsync(options)
            : client.CreateThread(options);
        Validate(thread);
        MessageCollectionOptions collectionOptions = new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending };
        List<ThreadMessage> messages = IsAsync
            ? await client.GetMessagesAsync(thread.Id, collectionOptions).ToListAsync()
            : client.GetMessages(thread.Id, collectionOptions).ToList();
        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[0].Content?.Count, Is.EqualTo(1));
        Assert.That(messages[0].Content[0].Text, Is.EqualTo("Hello, world!"));
        Assert.That(messages[1].Content?.Count, Is.EqualTo(2));
        Assert.That(messages[1].Content[0], Is.Not.Null);
        Assert.That(messages[1].Content[0].Text, Is.EqualTo("Can you describe this image for me?"));
        Assert.That(messages[1].Content[1], Is.Not.Null);
        Assert.That(messages[1].Content[1].ImageUri.AbsoluteUri, Is.EqualTo("https://test.openai.com/image.png"));
    }

    [Test]
    public async Task ThreadWithImageDetailWorks()
    {
        AssistantClient client = GetTestClient();

        ThreadCreationOptions options = new()
        {
            InitialMessages =
        {
            new(
                MessageRole.User,
                [
                    "Describe this image with auto detail:",
                    MessageContent.FromImageUri(
                        new Uri("https://test.openai.com/image.png"),
                        MessageImageDetail.Auto)
                ])
        }
        };

        AssistantThread thread = IsAsync
            ? await client.CreateThreadAsync(options)
            : client.CreateThread(options);

        Validate(thread);

        MessageCollectionOptions collectionOptions = new() { Order = MessageCollectionOrder.Ascending };
        List<ThreadMessage> messages = IsAsync
            ? await client.GetMessagesAsync(thread.Id, collectionOptions).ToListAsync()
            : client.GetMessages(thread.Id, collectionOptions).ToList();

        Assert.That(messages.Count, Is.EqualTo(1));
        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[0].Content?.Count, Is.EqualTo(2));

        Assert.That(messages[0].Content[0].Text, Is.EqualTo("Describe this image with auto detail:"));
        Assert.That(messages[0].Content[1].ImageUri.AbsoluteUri, Is.EqualTo("https://test.openai.com/image.png"));
    }

    [Test]
    public async Task BasicRunOperationsWork()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o-mini");
        Validate(assistant);
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        List<ThreadRun> runs = IsAsync
            ? await client.GetRunsAsync(thread.Id).ToListAsync()
            : client.GetRuns(thread.Id).ToList();
        Assert.That(runs.Count, Is.EqualTo(0));
        ThreadMessage message = client.CreateMessage(thread.Id, MessageRole.User, ["Hello, assistant!"]);
        Validate(message);
        ThreadRun run = IsAsync
            ? await client.CreateRunAsync(thread.Id, assistant.Id)
            : client.CreateRun(thread.Id, assistant.Id);
        Validate(run);
        Assert.That(run.Status, Is.EqualTo(RunStatus.Queued));
        Assert.That(run.CreatedAt, Is.GreaterThan(s_2024));
        ThreadRun retrievedRun = IsAsync
            ? await client.GetRunAsync(run.ThreadId, run.Id)
            : client.GetRun(run.ThreadId, run.Id);
        Assert.That(retrievedRun.Id, Is.EqualTo(run.Id));
        runs = IsAsync
                    ? await client.GetRunsAsync(thread.Id).ToListAsync()
                    : client.GetRuns(thread.Id).ToList();
        Assert.That(runs.Count, Is.EqualTo(1));
        Assert.That(runs[0].Id, Is.EqualTo(run.Id));

        List<ThreadMessage> messages = IsAsync ?
            await client.GetMessagesAsync(thread.Id).ToListAsync() :
            client.GetMessages(thread.Id).ToList();
        Assert.That(messages.Count, Is.GreaterThanOrEqualTo(1));
        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(1000);
            run = IsAsync
                ? await client.GetRunAsync(run.ThreadId, run.Id)
                : client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.CompletedAt, Is.GreaterThan(s_2024));
        Assert.That(run.RequiredActions.Count, Is.EqualTo(0));
        Assert.That(run.AssistantId, Is.EqualTo(assistant.Id));
        Assert.That(run.FailedAt, Is.Null);
        Assert.That(run.IncompleteDetails, Is.Null);

        messages = IsAsync ?
            await client.GetMessagesAsync(thread.Id).ToListAsync() :
            client.GetMessages(thread.Id).ToList();
        Assert.That(messages.Count, Is.EqualTo(2));

        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[1].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[1].Id, Is.EqualTo(message.Id));
    }

    [Test]
    public async Task BasicRunStepFunctionalityWorks()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
        {
            Tools = { new CodeInterpreterToolDefinition() },
            Instructions = "You help the user with mathematical descriptions and visualizations.",
        });
        Validate(assistant);

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile equationFile = fileClient.UploadFile(
            BinaryData.FromString("""
            x,y
            2,5
            7,14,
            8,22
            """).ToStream(),
            "text/csv",
            FileUploadPurpose.Assistants);
        Validate(equationFile);

        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages =
            {
                "Describe the contents of any available tool resource file."
                + " Graph a linear regression and provide the coefficient of correlation."
                + " Explain any code executed to evaluate.",
            },
            ToolResources = new()
            {
                CodeInterpreter = new()
                {
                    FileIds = { equationFile.Id },
                }
            }
        });
        Validate(thread);

        ThreadRun run = client.CreateRun(thread.Id, assistant.Id);
        Validate(run);

        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(1000);
            run = client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.Usage?.TotalTokenCount, Is.GreaterThan(0));

        List<RunStep> runSteps = IsAsync
            ? await client.GetRunStepsAsync(run.ThreadId, run.Id).ToListAsync()
            : client.GetRunSteps(run.ThreadId, run.Id).ToList();
        RunStep firstStep = runSteps[0];
        RunStep secondStep = runSteps[1];

        Assert.That(runSteps.Count, Is.GreaterThan(1));
        Assert.Multiple(() =>
        {
            Assert.That(firstStep.AssistantId, Is.EqualTo(assistant.Id));
            Assert.That(firstStep.ThreadId, Is.EqualTo(thread.Id));
            Assert.That(firstStep.RunId, Is.EqualTo(run.Id));
            Assert.That(firstStep.CreatedAt, Is.GreaterThan(s_2024));
            Assert.That(firstStep.CompletedAt, Is.GreaterThan(s_2024));
        });
        RunStepDetails details = firstStep.Details;
        Assert.That(details?.CreatedMessageId, Is.Not.Null.And.Not.Empty);

        details = secondStep.Details;
        Assert.Multiple(() =>
        {
            Assert.That(details?.ToolCalls.Count, Is.GreaterThan(0));
            Assert.That(details.ToolCalls[0].Kind, Is.EqualTo(RunStepToolCallKind.CodeInterpreter));
            Assert.That(details.ToolCalls[0].Id, Is.Not.Null.And.Not.Empty);
            Assert.That(details.ToolCalls[0].CodeInterpreterInput, Is.Not.Null.And.Not.Empty);
            Assert.That(details.ToolCalls[0].CodeInterpreterOutputs?.Count, Is.GreaterThan(0));
            Assert.That(details.ToolCalls[0].CodeInterpreterOutputs[0].ImageFileId, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public async Task SettingResponseFormatWorks()
    {
        AssistantClient client = GetTestClient();
        AssistantCreationOptions creationOptions = new AssistantCreationOptions()
        {
            ResponseFormat = AssistantResponseFormat.CreateAutoFormat(),
        };
        Assistant assistant = IsAsync
            ? await client.CreateAssistantAsync("gpt-4o-mini", creationOptions)
            : client.CreateAssistant("gpt-4o-mini", creationOptions);
        Validate(assistant);
        Assert.That(assistant.ResponseFormat == "auto");
        AssistantModificationOptions modificationOptions = new AssistantModificationOptions()
        {
            ResponseFormat = AssistantResponseFormat.CreateTextFormat(),
        };
        assistant = IsAsync
            ? await client.ModifyAssistantAsync(assistant.Id, modificationOptions)
            : client.ModifyAssistant(assistant.Id, modificationOptions);
        Assert.That(assistant.ResponseFormat == AssistantResponseFormat.CreateTextFormat());
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        ThreadMessage message = client.CreateMessage(thread.Id, MessageRole.User, ["Write some JSON for me!"]);
        Validate(message);
        RunCreationOptions runCreationOptions = new RunCreationOptions()
        {
            ResponseFormat = AssistantResponseFormat.CreateJsonObjectFormat(),
        };
        ThreadRun run = IsAsync
            ? await client.CreateRunAsync(thread.Id, assistant.Id, runCreationOptions)
            : client.CreateRun(thread.Id, assistant.Id, runCreationOptions);
        Assert.That(run.ResponseFormat == AssistantResponseFormat.CreateJsonObjectFormat());
    }

    [Test]
    public async Task FunctionToolsWork()
    {
        AssistantClient client = GetTestClient();
        AssistantCreationOptions creationOptions = new AssistantCreationOptions()
        {
            Tools =
            {
                new FunctionToolDefinition("get_favorite_food_for_day_of_week")
                {
                    Description = "gets the user's favorite food for a given day of the week, like Tuesday",
                    Parameters = BinaryData.FromObjectAsJson(new
                    {
                        type = "object",
                        properties = new
                        {
                            day_of_week = new
                            {
                                type = "string",
                                description = "a day of the week, like Tuesday or Saturday",
                            }
                        }
                    }),
                },
            },
        };
        Assistant assistant = IsAsync
            ? await client.CreateAssistantAsync("gpt-4o-mini", creationOptions)
            : client.CreateAssistant("gpt-4o-mini", creationOptions);
        Validate(assistant);
        Assert.That(assistant.Tools?.Count, Is.EqualTo(1));

        FunctionToolDefinition responseToolDefinition = assistant.Tools[0] as FunctionToolDefinition;
        Assert.That(responseToolDefinition?.FunctionName, Is.EqualTo("get_favorite_food_for_day_of_week"));
        Assert.That(responseToolDefinition?.Parameters, Is.Not.Null);

        ThreadCreationOptions threadCreationOptions = new ThreadCreationOptions()
        {
            InitialMessages = { "What should I eat on Thursday?" },
        };
        RunCreationOptions runCreationOptions = new RunCreationOptions()
        {
            AdditionalInstructions = "Call provided tools when appropriate.",
        };
        ThreadRun run = IsAsync
            ? await client.CreateThreadAndRunAsync(assistant.Id, threadCreationOptions, runCreationOptions)
            : client.CreateThreadAndRun(assistant.Id, threadCreationOptions, runCreationOptions);
        Validate(run);

        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(1000);
            run = IsAsync
                ? await client.GetRunAsync(run.ThreadId, run.Id)
                : client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.RequiresAction));
        Assert.That(run.RequiredActions?.Count, Is.EqualTo(1));
        Assert.That(run.RequiredActions[0].ToolCallId, Is.Not.Null.And.Not.Empty);
        Assert.That(run.RequiredActions[0].FunctionName, Is.EqualTo("get_favorite_food_for_day_of_week"));
        Assert.That(run.RequiredActions[0].FunctionArguments, Is.Not.Null.And.Not.Empty);

        run = IsAsync
            ? await client.SubmitToolOutputsToRunAsync(run.ThreadId, run.Id, [new(run.RequiredActions[0].ToolCallId, "tacos")])
            : client.SubmitToolOutputsToRun(run.ThreadId, run.Id, [new(run.RequiredActions[0].ToolCallId, "tacos")]);
        Assert.That(run.Status.IsTerminal, Is.False);

        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(1000);
            run = IsAsync
                ? await client.GetRunAsync(run.ThreadId, run.Id)
                : client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));

        List<ThreadMessage> messages = client.GetMessages(run.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Descending }).ToList();
        Assert.That(messages.Count, Is.GreaterThan(1));
        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[0].Content?[0], Is.Not.Null);
        Assert.That(messages[0].Content[0].Text.ToLowerInvariant(), Does.Contain("tacos"));
    }

    [Test]
    public async Task StreamingRunWorksAsync()
    {
        AssertAsyncOnly();

        AssistantClient client = GetTestClient();
        Assistant assistant = await client.CreateAssistantAsync("gpt-4o-mini");
        Validate(assistant);

        AssistantThread thread = await client.CreateThreadAsync(new ThreadCreationOptions()
        {
            InitialMessages = { "Hello there, assistant! How are you today?", },
        });
        Validate(thread);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        AsyncCollectionResult<StreamingUpdate> streamingResult
            = client.CreateRunStreamingAsync(thread.Id, assistant.Id);

        Print(">>> Connected <<<");

        await foreach (StreamingUpdate update in streamingResult)
        {
            string message = $"{update.UpdateKind} ";
            if (update is RunUpdate runUpdate)
            {
                message += $"at {update.UpdateKind switch
                {
                    StreamingUpdateReason.RunCreated => runUpdate.Value.CreatedAt,
                    StreamingUpdateReason.RunQueued => runUpdate.Value.StartedAt,
                    StreamingUpdateReason.RunInProgress => runUpdate.Value.StartedAt,
                    StreamingUpdateReason.RunCompleted => runUpdate.Value.CompletedAt,
                    _ => "???",
                }}";
            }
            if (update is MessageContentUpdate contentUpdate)
            {
                if (contentUpdate.Role.HasValue)
                {
                    message += $"[{contentUpdate.Role}]";
                }
                message += $"[{contentUpdate.MessageIndex}] {contentUpdate.Text}";
            }
            Print(message);
        }
        Print(">>> Done <<<");
    }

    [Test]
    public void StreamingRunWorks()
    {
        AssertSyncOnly();

        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o-mini");
        Validate(assistant);

        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages = { "Hello there, assistant! How are you today?", },
        });
        Validate(thread);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        CollectionResult<StreamingUpdate> streamingResult
            = client.CreateRunStreaming(thread.Id, assistant.Id);

        Print(">>> Connected <<<");

        foreach (StreamingUpdate update in streamingResult)
        {
            string message = $"{update.UpdateKind} ";
            if (update is RunUpdate runUpdate)
            {
                message += $"at {update.UpdateKind switch
                {
                    StreamingUpdateReason.RunCreated => runUpdate.Value.CreatedAt,
                    StreamingUpdateReason.RunQueued => runUpdate.Value.StartedAt,
                    StreamingUpdateReason.RunInProgress => runUpdate.Value.StartedAt,
                    StreamingUpdateReason.RunCompleted => runUpdate.Value.CompletedAt,
                    _ => "???",
                }}";
            }
            if (update is MessageContentUpdate contentUpdate)
            {
                if (contentUpdate.Role.HasValue)
                {
                    message += $"[{contentUpdate.Role}]";
                }
                message += $"[{contentUpdate.MessageIndex}] {contentUpdate.Text}";
            }
            Print(message);
        }
        Print(">>> Done <<<");
    }

    [TestCase]
    public async Task StreamingToolCallAsync()
    {
        AssertAsyncOnly();

        AssistantClient client = GetTestClient();
        FunctionToolDefinition getWeatherTool = new("get_current_weather")
        {
            Description = "Gets the user's current weather",
        };
        Assistant assistant = await client.CreateAssistantAsync("gpt-4o-mini", new()
        {
            Tools = { getWeatherTool }
        });
        Validate(assistant);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        Print(" >>> Beginning call ... ");
        AsyncCollectionResult<StreamingUpdate> asyncResults = client.CreateThreadAndRunStreamingAsync(
            assistant.Id,
            new()
            {
                InitialMessages = { "What should I wear outside right now?", },
            });
        Print(" >>> Starting enumeration ...");

        ThreadRun run = null;

        do
        {
            run = null;
            List<ToolOutput> toolOutputs = [];
            await foreach (StreamingUpdate update in asyncResults)
            {
                string message = update.UpdateKind.ToString();

                if (update is RunUpdate runUpdate)
                {
                    message += $" run_id:{runUpdate.Value.Id}";
                    run = runUpdate.Value;
                }
                if (update is RequiredActionUpdate requiredActionUpdate)
                {
                    Assert.That(requiredActionUpdate.FunctionName, Is.EqualTo(getWeatherTool.FunctionName));
                    Assert.That(requiredActionUpdate.GetThreadRun().Status, Is.EqualTo(RunStatus.RequiresAction));
                    message += $" {requiredActionUpdate.FunctionName}";
                    toolOutputs.Add(new(requiredActionUpdate.ToolCallId, "warm and sunny"));
                }
                if (update is MessageContentUpdate contentUpdate)
                {
                    message += $" {contentUpdate.Text}";
                }
                Print(message);
            }
            if (toolOutputs.Count > 0)
            {
                asyncResults = client.SubmitToolOutputsToRunStreamingAsync(run.ThreadId, run.Id, toolOutputs);
            }
        } while (run?.Status.IsTerminal == false);
    }

    [TestCase]
    public void StreamingToolCall()
    {
        AssertSyncOnly();

        AssistantClient client = GetTestClient();
        FunctionToolDefinition getWeatherTool = new("get_current_weather")
        {
            Description = "Gets the user's current weather",
        };
        Assistant assistant = client.CreateAssistant("gpt-4o-mini", new()
        {
            Tools = { getWeatherTool }
        });
        Validate(assistant);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        Print(" >>> Beginning call ... ");
        CollectionResult<StreamingUpdate> results = client.CreateThreadAndRunStreaming(
            assistant.Id,
            new()
            {
                InitialMessages = { "What should I wear outside right now?", },
            });
        Print(" >>> Starting enumeration ...");

        ThreadRun run = null;

        do
        {
            run = null;
            List<ToolOutput> toolOutputs = [];
            foreach (StreamingUpdate update in results)
            {
                string message = update.UpdateKind.ToString();

                if (update is RunUpdate runUpdate)
                {
                    message += $" run_id:{runUpdate.Value.Id}";
                    run = runUpdate.Value;
                }
                if (update is RequiredActionUpdate requiredActionUpdate)
                {
                    Assert.That(requiredActionUpdate.FunctionName, Is.EqualTo(getWeatherTool.FunctionName));
                    Assert.That(requiredActionUpdate.GetThreadRun().Status, Is.EqualTo(RunStatus.RequiresAction));
                    message += $" {requiredActionUpdate.FunctionName}";
                    toolOutputs.Add(new(requiredActionUpdate.ToolCallId, "warm and sunny"));
                }
                if (update is MessageContentUpdate contentUpdate)
                {
                    message += $" {contentUpdate.Text}";
                }
                Print(message);
            }
            if (toolOutputs.Count > 0)
            {
                results = client.SubmitToolOutputsToRunStreaming(run.ThreadId, run.Id, toolOutputs);
            }
        } while (run?.Status.IsTerminal == false);
    }

    [Test]
    public async Task FileSearchWorks()
    {
        // First, we need to upload a simple test file.
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile testFile = fileClient.UploadFile(
            BinaryData.FromString("""
            This file describes the favorite foods of several people.

            Summanus Ferdinand: tacos
            Tekakwitha Effie: pizza
            Filip Carola: cake
            """).ToStream(),
            "favorite_foods.txt",
            FileUploadPurpose.Assistants);
        Validate(testFile);

        AssistantClient client = GetTestClient();

        FileSearchToolDefinition fileSearchTool = new()
        {
            MaxResults = 2,
            RankingOptions = new(0.5f)
            {
                Ranker = FileSearchRanker.Default20240821
            }
        };

        FileSearchToolResources fileSearchResources = new()
        {
            NewVectorStores =
            {
                new VectorStoreCreationHelper([testFile.Id]),
            },
        };

        AssistantCreationOptions creationOptions = new AssistantCreationOptions()
        {
            Tools = { fileSearchTool },
            ToolResources = new()
            {
                FileSearch = fileSearchResources
            },
        };

        // Create an assistant, using the creation helper to make a new vector store
        Assistant assistant = IsAsync
            ? await client.CreateAssistantAsync("gpt-4o-mini", creationOptions)
            : client.CreateAssistant("gpt-4o-mini", creationOptions);
        Validate(assistant);
        Assert.That(assistant.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        string createdVectorStoreId = assistant.ToolResources.FileSearch.VectorStoreIds[0];
        _vectorStoreIdsToDelete.Add(createdVectorStoreId);

        // Modify an assistant to use the existing vector store
        AssistantModificationOptions modificationOptions = new AssistantModificationOptions()
        {
            ToolResources = new()
            {
                FileSearch = new()
                {
                    VectorStoreIds = { assistant.ToolResources.FileSearch.VectorStoreIds[0] },
                },
            },
        };
        assistant = IsAsync
            ? await client.ModifyAssistantAsync(assistant.Id, modificationOptions)
            : client.ModifyAssistant(assistant.Id, modificationOptions);
        Assert.That(assistant.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        Assert.That(assistant.ToolResources.FileSearch.VectorStoreIds[0], Is.EqualTo(createdVectorStoreId));

        // Create a thread with an override vector store
        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages = { "Using the files you have available, what's Filip's favorite food?" },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([testFile.Id])
                    }
                }
            }
        });
        Validate(thread);
        Assert.That(thread.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        createdVectorStoreId = thread.ToolResources.FileSearch.VectorStoreIds[0];
        _vectorStoreIdsToDelete.Add(createdVectorStoreId);

        // Ensure that modifying the thread with an existing vector store works
        ThreadModificationOptions threadModificationOptions = new ThreadModificationOptions()
        {
            ToolResources = new()
            {
                FileSearch = new()
                {
                    VectorStoreIds = { createdVectorStoreId },
                }
            }
        };
        thread = IsAsync
            ? await client.ModifyThreadAsync(thread.Id, threadModificationOptions)
            : client.ModifyThread(thread.Id, threadModificationOptions);
        Assert.That(thread.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        Assert.That(thread.ToolResources.FileSearch.VectorStoreIds[0], Is.EqualTo(createdVectorStoreId));

        ThreadRun run = IsAsync
            ? await client.CreateRunAsync(thread.Id, assistant.Id)
            : client.CreateRun(thread.Id, assistant.Id);
        Validate(run);
        do
        {
            Thread.Sleep(1000);
            run = IsAsync
                ? await client.GetRunAsync(run.ThreadId, run.Id)
                : client.GetRun(run.ThreadId, run.Id);
        } while (run?.Status.IsTerminal == false);
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));

        CollectionResult<ThreadMessage> messages = client.GetMessages(thread.Id, new() { Order = MessageCollectionOrder.Descending });
        int messageCount = 0;
        bool hasCake = false;
        foreach (ThreadMessage message in messages)
        {
            messageCount++;

            foreach (MessageContent content in message.Content)
            {
                Console.WriteLine(content.Text);
                foreach (TextAnnotation annotation in content.TextAnnotations)
                {
                    Console.WriteLine($"  --> From file: {annotation.InputFileId}, replacement: {annotation.TextToReplace}");
                }

                if (!hasCake)
                {
                    hasCake = content.Text.ToLower().Contains("cake");
                }
            }
        }
        Assert.That(messageCount > 1);
        Assert.That(hasCake, Is.True);

        // Validate GetRunSteps.
        CollectionResult<RunStep> runSteps = client.GetRunSteps(run.ThreadId, run.Id);
        Assert.That(runSteps, Is.Not.Null.And.Not.Empty);

        List<RunStep> toolCallRunSteps = runSteps.Where(runStep => runStep.Kind == RunStepKind.ToolCall).ToList();
        Assert.That(toolCallRunSteps, Is.Not.Null.And.Not.Empty);

        foreach (RunStep toolCallRunStep in toolCallRunSteps)
        {
            Assert.That(toolCallRunStep.Details, Is.Not.Null);
            Assert.That(toolCallRunStep.Details.ToolCalls, Has.Count.GreaterThan(0));

            foreach (RunStepToolCall toolCall in toolCallRunStep.Details.ToolCalls)
            {
                Assert.That(toolCall.Kind == RunStepToolCallKind.FileSearch);
                Assert.That(toolCall, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(toolCall.FileSearchRankingOptions.Ranker, Is.EqualTo(fileSearchTool.RankingOptions.Ranker));
                    Assert.That(toolCall.FileSearchRankingOptions.ScoreThreshold, Is.EqualTo(fileSearchTool.RankingOptions.ScoreThreshold));
                    Assert.That(toolCall.FileSearchResults, Has.Count.GreaterThan(0));
                });

                RunStepFileSearchResult fileSearchResult = toolCall.FileSearchResults[0];
                Assert.Multiple(() =>
                {
                    Assert.That(fileSearchResult.FileId, Is.Not.Null.And.Not.Empty);
                    Assert.That(fileSearchResult.FileName, Is.Not.Null.And.Not.Empty);
                    Assert.That(fileSearchResult.Score, Is.GreaterThan(0));

                    // Confirm that we always get the Content property, since we are always passing the `include[]` query parameter.
                    Assert.That(fileSearchResult.Content, Has.Count.GreaterThan(0));
                });

                RunStepFileSearchResultContent fileSearchResultContent = fileSearchResult.Content[0];
                Assert.Multiple(() =>
                {
                    Assert.That(fileSearchResultContent.Kind, Is.EqualTo(RunStepFileSearchResultContentKind.Text));
                    Assert.That(fileSearchResultContent.Text, Is.Not.Null.And.Not.Empty);
                });
            }
        }

        // Also validate GetRunStep.
        RunStep runStep = client.GetRunStep(run.ThreadId, run.Id, toolCallRunSteps[0].Id);
        Assert.That(runStep, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(runStep.Kind, Is.EqualTo(RunStepKind.ToolCall));
            Assert.That(runStep.Details, Is.Not.Null);
            Assert.That(runStep.Details.ToolCalls, Has.Count.GreaterThan(0));
            Assert.That(runStep.Details.ToolCalls[0].Kind, Is.EqualTo(RunStepToolCallKind.FileSearch));

            // Confirm that we always get the Content property, since we are always passing the `include[]` query parameter.
            Assert.That(runStep.Details.ToolCalls[0].FileSearchResults[0].Content, Has.Count.GreaterThan(0));
        });
    }

    [Test]
    public void FileSearchStreamingWorksSync()
    {
        AssertSyncOnly();

        const string fileContent = """
                The favorite food of several people:
                - Summanus Ferdinand: tacos
                - Tekakwitha Effie: pizza
                - Filip Carola: cake
                """;

        const string fileName = "favorite_foods.txt";

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        AssistantClient client = GetTestClient<AssistantClient>(TestScenario.Assistants);

        // First, upload a simple test file.
        OpenAIFile testFile = fileClient.UploadFile(BinaryData.FromString(fileContent), fileName, FileUploadPurpose.Assistants);
        Validate(testFile);

        // Create an assistant, using the creation helper to make a new vector store.
        AssistantCreationOptions assistantCreationOptions = new()
        {
            Tools = { new FileSearchToolDefinition() },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores = { new VectorStoreCreationHelper([testFile.Id]) }
                }
            }
        };
        Assistant assistant = client.CreateAssistant("gpt-4o-mini", assistantCreationOptions);
        Validate(assistant);

        Assert.That(assistant.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        string vectorStoreId = assistant.ToolResources.FileSearch.VectorStoreIds[0];
        _vectorStoreIdsToDelete.Add(vectorStoreId);

        // Create a thread.
        ThreadCreationOptions threadCreationOptions = new()
        {
            InitialMessages = { "Using the files you have available, what's Filip's favorite food?" }
        };
        AssistantThread thread = client.CreateThread(threadCreationOptions);
        Validate(thread);

        string message = string.Empty;

        // Create run and stream the results.
        CollectionResult<StreamingUpdate> streamingResult = client.CreateRunStreaming(thread.Id, assistant.Id);

        foreach (StreamingUpdate update in streamingResult)
        {
            if (update is MessageContentUpdate contentUpdate)
            {
                message += $"{contentUpdate.Text}";
            }
            else if (update is RunStepDetailsUpdate detailUpdate)
            {
                Assert.That(detailUpdate.FunctionName, Is.Null);
            }
            else if (update is RunStepUpdate runStepUpdate)
            {
                RunStep runStep = runStepUpdate.Value;
                Assert.That(runStep, Is.Not.Null);

                if (runStepUpdate.UpdateKind == StreamingUpdateReason.RunStepCompleted)
                {
                    if (runStep.Kind == RunStepKind.ToolCall)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(runStep.Kind, Is.EqualTo(RunStepKind.ToolCall));
                            Assert.That(runStep.Details, Is.Not.Null);
                            Assert.That(runStep.Details.ToolCalls, Has.Count.GreaterThan(0));
                            Assert.That(runStep.Details.ToolCalls[0].Kind, Is.EqualTo(RunStepToolCallKind.FileSearch));

                            // Confirm that we always get the Content property, since we are always passing the `include[]` query parameter.
                            Assert.That(runStep.Details.ToolCalls[0].FileSearchResults[0].Content, Has.Count.GreaterThan(0));
                        });
                    }
                }
            }
        }


        Assert.That(message, Does.Contain("cake"));
    }

    [Test]
    public async Task FileSearchStreamingWorksAsync()
    {
        AssertAsyncOnly();

        const string fileContent = """
                The favorite food of several people:
                - Summanus Ferdinand: tacos
                - Tekakwitha Effie: pizza
                - Filip Carola: cake
                """;

        const string fileName = "favorite_foods.txt";

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        AssistantClient client = GetTestClient<AssistantClient>(TestScenario.Assistants);

        // First, upload a simple test file.
        OpenAIFile testFile = await fileClient.UploadFileAsync(BinaryData.FromString(fileContent), fileName, FileUploadPurpose.Assistants);
        Validate(testFile);

        // Create an assistant, using the creation helper to make a new vector store.
        AssistantCreationOptions assistantCreationOptions = new()
        {
            Tools = { new FileSearchToolDefinition() },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores = { new VectorStoreCreationHelper([testFile.Id]) }
                }
            }
        };
        Assistant assistant = await client.CreateAssistantAsync("gpt-4o-mini", assistantCreationOptions);
        Validate(assistant);

        Assert.That(assistant.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        string vectorStoreId = assistant.ToolResources.FileSearch.VectorStoreIds[0];
        _vectorStoreIdsToDelete.Add(vectorStoreId);

        // Create a thread.
        ThreadCreationOptions threadCreationOptions = new()
        {
            InitialMessages = { "Using the files you have available, what's Filip's favorite food?" }
        };
        AssistantThread thread = await client.CreateThreadAsync(threadCreationOptions);
        Validate(thread);

        string message = string.Empty;

        // Create run and stream the results.
        AsyncCollectionResult<StreamingUpdate> streamingResult = client.CreateRunStreamingAsync(thread.Id, assistant.Id);

        await foreach (StreamingUpdate update in streamingResult)
        {
            if (update is MessageContentUpdate contentUpdate)
            {
                message += $"{contentUpdate.Text}";
            }
            else if (update is RunStepDetailsUpdate detailUpdate)
            {
                Assert.That(detailUpdate.FunctionName, Is.Null);
            }
            else if (update is RunStepUpdate runStepUpdate)
            {
                if (runStepUpdate.UpdateKind == StreamingUpdateReason.RunStepCompleted)
                {
                    RunStep runStep = runStepUpdate.Value;
                    Assert.That(runStep, Is.Not.Null);

                    if (runStepUpdate.UpdateKind == StreamingUpdateReason.RunStepCompleted)
                    {
                        if (runStep.Kind == RunStepKind.ToolCall)
                        {
                            Assert.Multiple(() =>
                            {
                                Assert.That(runStep.Kind, Is.EqualTo(RunStepKind.ToolCall));
                                Assert.That(runStep.Details, Is.Not.Null);
                                Assert.That(runStep.Details.ToolCalls, Has.Count.GreaterThan(0));
                                Assert.That(runStep.Details.ToolCalls[0].Kind, Is.EqualTo(RunStepToolCallKind.FileSearch));

                                // Confirm that we always get the Content property, since we are always passing the `include[]` query parameter.
                                Assert.That(runStep.Details.ToolCalls[0].FileSearchResults[0].Content, Has.Count.GreaterThan(0));
                            });
                        }
                    }
                }
            }
        }

        Assert.That(message, Does.Contain("cake"));
    }

    [Test]
    public async Task Pagination_CanEnumerateAssistantsAsync()
    {
        AssertAsyncOnly();

        const int TestAssistantCount = 10;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < 10; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}",
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        AsyncCollectionResult<Assistant> assistants = client.GetAssistantsAsync(new AssistantCollectionOptions() { Order = AssistantCollectionOrder.Descending });

        int lastIdSeen = int.MaxValue;

        await foreach (Assistant assistant in assistants)
        {
            Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");
            if (assistant.Name?.StartsWith("Test Assistant ") == true)
            {
                Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                Assert.That(seenId, Is.LessThan(lastIdSeen));
                lastIdSeen = seenId;
            }
            count++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
    }

    [Test]
    public void Pagination_CanEnumerateAssistants()
    {
        AssertSyncOnly();

        const int TestAssistantCount = 10;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}",
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        CollectionResult<Assistant> assistants = client.GetAssistants(new AssistantCollectionOptions() { Order = AssistantCollectionOrder.Descending });

        int lastIdSeen = int.MaxValue;

        foreach (Assistant assistant in assistants)
        {
            Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");
            if (assistant.Name?.StartsWith("Test Assistant ") == true)
            {
                Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                Assert.That(seenId, Is.LessThan(lastIdSeen));
                lastIdSeen = seenId;
            }
            count++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
    }

    [Test]
    public async Task Pagination_CanPageThroughAssistantCollectionAsync()
    {
        AssertAsyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        int pageCount = 0;
        AsyncCollectionResult<Assistant> assistants = client.GetAssistantsAsync(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        int lastIdSeen = int.MaxValue;

        await foreach (ClientResult page in assistants.GetRawPagesAsync())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                }
                count++;
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    [Test]
    public void Pagination_CanPageThroughAssistantCollection()
    {
        AssertSyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        int pageCount = 0;
        CollectionResult<Assistant> assistants = client.GetAssistants(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        int lastIdSeen = int.MaxValue;

        foreach (ClientResult page in assistants.GetRawPages())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                }
                count++;
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    private static IEnumerable<Assistant> GetAssistantsFromPage(ClientResult page)
    {
        PipelineResponse response = page.GetRawResponse();
        JsonDocument doc = JsonDocument.Parse(response.Content);
        IEnumerable<JsonElement> els = doc.RootElement.GetProperty("data").EnumerateArray();

        // TODO: improve perf
        return els.Select(el => ModelReaderWriter.Read<Assistant>(BinaryData.FromString(el.GetRawText())));
    }

    [Test]
    public async Task Pagination_CanRehydrateAssistantPageCollectionFromBytesAsync()
    {
        AssertAsyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        AsyncCollectionResult<Assistant> assistants = client.GetAssistantsAsync(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        // Simulate rehydration of the collection
        ClientResult firstPage = await assistants.GetRawPagesAsync().FirstAsync();
        BinaryData rehydrationTokenBytes = assistants.GetContinuationToken(firstPage).ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationTokenBytes);

        // This starts the collection on the second page.
        AsyncCollectionResult<Assistant> rehydratedAssistants = client.GetAssistantsAsync(rehydrationToken);

        // We already got the first page, so account for that.
        int count = TestPageSizeLimit;
        int pageCount = 1;

        int lastIdSeen = int.MaxValue;

        await foreach (ClientResult page in rehydratedAssistants.GetRawPagesAsync())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                }
                count++;
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        // We should only see eight items and four pages because we rehydrated the
        // collection starting on the second page.
        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    [Test]
    public void Pagination_CanRehydrateAssistantPageCollectionFromBytes()
    {
        AssertSyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        CollectionResult<Assistant> assistants = client.GetAssistants(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        // Simulate rehydration of the collection
        ClientResult firstPage = assistants.GetRawPages().First();
        BinaryData rehydrationTokenBytes = assistants.GetContinuationToken(firstPage).ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationTokenBytes);

        // This starts the collection on the second page.
        CollectionResult<Assistant> rehydratedAssistants = client.GetAssistants(rehydrationToken);

        // We already got the first page, so account for that.
        int count = TestPageSizeLimit;
        int pageCount = 1;

        int lastIdSeen = int.MaxValue;

        foreach (ClientResult page in rehydratedAssistants.GetRawPages())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                }

                count++;
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        // We should only see eight items and four pages because we rehydrated the
        // collection starting on the second page.
        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    [Test]
    public async Task Pagination_CanRehydrateAssistantPageCollectionFromPageTokenAsync()
    {
        AssertAsyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        List<Assistant> createdAssistants = [];
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));

            createdAssistants.Add(assistant);
        }

        AsyncCollectionResult<Assistant> assistants = client.GetAssistantsAsync(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        // Since we asked for descending order, reverse the order of createdAssistants.
        createdAssistants.Reverse();

        // Call the rehydration method, passing a typed OpenAIPageToken
        ClientResult firstPage = await assistants.GetRawPagesAsync().FirstAsync();
        ContinuationToken nextPageToken = assistants.GetContinuationToken(firstPage);
        AsyncCollectionResult<Assistant> rehydratedAssistantCollection = client.GetAssistantsAsync(nextPageToken);

        // Since we're asking for the next page after the first one, remove the first two items from the 
        // createdAssistants
        createdAssistants = createdAssistants.Skip(TestPageSizeLimit).ToList();

        // We already got the first page, so account for that.
        int count = TestPageSizeLimit;
        int pageCount = 1;

        int lastIdSeen = int.MaxValue;

        List<Assistant> rehydratedAssistants = [];

        await foreach (ClientResult page in rehydratedAssistantCollection.GetRawPagesAsync())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                    count++;
                }

                rehydratedAssistants.Add(assistant);
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(createdAssistants[0].Id, Is.EqualTo(rehydratedAssistants[0].Id));

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    [Test]
    public void Pagination_CanRehydrateAssistantPageCollectionFromPageToken()
    {
        AssertSyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        List<Assistant> createdAssistants = [];
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));

            createdAssistants.Add(assistant);
        }

        CollectionResult<Assistant> assistants = client.GetAssistants(
            new AssistantCollectionOptions()
            {
                Order = AssistantCollectionOrder.Descending,
                PageSizeLimit = TestPageSizeLimit
            });

        // Since we asked for descending order, reverse the order of createdAssistants.
        createdAssistants.Reverse();

        // Call the rehydration method, passing a typed OpenAIPageToken
        ClientResult firstPage = assistants.GetRawPages().First();
        ContinuationToken nextPageToken = assistants.GetContinuationToken(firstPage);
        CollectionResult<Assistant> rehydratedAssistantCollection = client.GetAssistants(nextPageToken);

        // Since we're asking for the next page after the first one, remove the first two items from the 
        // createdAssistants
        createdAssistants = createdAssistants.Skip(TestPageSizeLimit).ToList();

        // We already got the first page, so account for that.
        int count = TestPageSizeLimit;
        int pageCount = 1;

        int lastIdSeen = int.MaxValue;

        List<Assistant> rehydratedAssistants = [];

        foreach (ClientResult page in rehydratedAssistantCollection.GetRawPages())
        {
            foreach (Assistant assistant in GetAssistantsFromPage(page))
            {
                if (assistant.Name?.StartsWith("Test Assistant ") == true)
                {
                    Assert.That(int.TryParse(assistant.Name["Test Assistant ".Length..], out int seenId), Is.True);
                    Assert.That(seenId, Is.LessThan(lastIdSeen));
                    lastIdSeen = seenId;
                    count++;
                }
                rehydratedAssistants.Add(assistant);
            }

            pageCount++;
            if (lastIdSeen == 0 || count > 100)
            {
                break;
            }
        }

        Assert.That(createdAssistants[0].Id, Is.EqualTo(rehydratedAssistants[0].Id));

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(TestAssistantCount / TestPageSizeLimit));
    }

    [Test]
    public async Task Pagination_CanCastAssistantPageCollectionToConvenienceFromProtocolAsync()
    {
        AssertAsyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        List<Assistant> createdAssistants = [];
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));

            createdAssistants.Add(assistant);
        }

        // Call the protocol method
        AsyncCollectionResult assistantsProtocol = client.GetAssistantsAsync(limit: TestPageSizeLimit, order: "desc", after: null, before: null, options: default);

        // Cast to the convenience type
        AsyncCollectionResult<Assistant> assistants = (AsyncCollectionResult<Assistant>)assistantsProtocol;

        // Since we asked for descending order, reverse the order of createdAssistants.
        createdAssistants.Reverse();

        int count = 0;

        // Validate that the protocol return type cast to the convenience return type
        // functions correctly as the convenience return type.
        await foreach (Assistant assistant in assistants)
        {
            Assert.AreEqual(createdAssistants[count++].Id, assistant.Id);

            if (count >= createdAssistants.Count)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
    }

    [Test]
    public void Pagination_CanCastAssistantPageCollectionToConvenienceFromProtocol()
    {
        AssertSyncOnly();

        const int TestAssistantCount = 10;
        const int TestPageSizeLimit = 2;

        AssistantClient client = GetTestClient();

        // Create assistant collection
        List<Assistant> createdAssistants = [];
        for (int i = 0; i < TestAssistantCount; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));

            createdAssistants.Add(assistant);
        }

        // Call the protocol method
        CollectionResult assistantsProtocol = client.GetAssistants(limit: TestPageSizeLimit, order: "desc", after: null, before: null, options: default);

        // Cast to the convenience type
        CollectionResult<Assistant> assistants = (CollectionResult<Assistant>)assistantsProtocol;

        // Since we asked for descending order, reverse the order of createdAssistants.
        createdAssistants.Reverse();

        int count = 0;

        // Validate that the protocol return type cast to the convenience return type
        // functions correctly as the convenience return type.
        foreach (Assistant assistant in assistants)
        {
            Assert.AreEqual(createdAssistants[count++].Id, assistant.Id);

            if (count >= createdAssistants.Count)
            {
                break;
            }
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(TestAssistantCount));
    }

    [Test]
    public async Task Pagination_CanRehydrateRunStepPageCollectionFromBytesAsync()
    {
        AssertAsyncOnly();

        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o", new AssistantCreationOptions()
        {
            Tools = { new CodeInterpreterToolDefinition() },
            Instructions = "You help the user with mathematical descriptions and visualizations.",
        });
        Validate(assistant);

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile equationFile = fileClient.UploadFile(
            BinaryData.FromString("""
            x,y
            2,5
            7,14,
            8,22
            """).ToStream(),
            "text/csv",
            FileUploadPurpose.Assistants);
        Validate(equationFile);

        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages =
            {
                "Describe the contents of any available tool resource file."
                + " Graph a linear regression and provide the coefficient of correlation."
                + " Explain any code executed to evaluate.",
            },
            ToolResources = new()
            {
                CodeInterpreter = new()
                {
                    FileIds = { equationFile.Id },
                }
            }
        });
        Validate(thread);

        ThreadRun run = client.CreateRun(thread.Id, assistant.Id);
        Validate(run);

        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(1000);
            run = client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.Usage?.TotalTokenCount, Is.GreaterThan(0));

        IReadOnlyList<string> runSteps;
        IReadOnlyList<string> rehydratedRunSteps;
        {
            const int numPerPage = 2;
            AsyncCollectionResult<RunStep> results = client.GetRunStepsAsync(run.ThreadId, run.Id, new() { PageSizeLimit = numPerPage });
            runSteps = await results
                .Skip(numPerPage)
                .Select(r => r.Id)
                .ToListAsync();

            // Simulate rehydration of the collection
            ContinuationToken rehydrationToken = results.GetContinuationToken(await results.GetRawPagesAsync().FirstOrDefaultAsync());
            results = client.GetRunStepsAsync(rehydrationToken);
            rehydratedRunSteps = await results
                .Select(r => r.Id)
                .ToListAsync();
        }

        CollectionAssert.AreEqual(runSteps, rehydratedRunSteps);
    }

    [Test]
    public void Pagination_CanRehydrateRunStepPageCollectionFromBytes()
    {
        AssertSyncOnly();

        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o", new AssistantCreationOptions()
        {
            Tools = { new CodeInterpreterToolDefinition() },
            Instructions = "You help the user with mathematical descriptions and visualizations.",
        });
        Validate(assistant);

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile equationFile = fileClient.UploadFile(
            BinaryData.FromString("""
            x,y
            2,5
            7,14,
            8,22
            """).ToStream(),
            "text/csv",
            FileUploadPurpose.Assistants);
        Validate(equationFile);

        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages =
            {
                "Describe the contents of any available tool resource file."
                + " Graph a linear regression and provide the coefficient of correlation."
                + " Explain any code executed to evaluate.",
            },
            ToolResources = new()
            {
                CodeInterpreter = new()
                {
                    FileIds = { equationFile.Id },
                }
            }
        });
        Validate(thread);

        ThreadRun run = client.CreateRun(thread.Id, assistant.Id);
        Validate(run);

        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(1000);
            run = client.GetRun(run.ThreadId, run.Id);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.Usage?.TotalTokenCount, Is.GreaterThan(0));

        IReadOnlyList<string> runSteps;
        IReadOnlyList<string> rehydratedRunSteps;
        {
            const int numPerPage = 2;
            CollectionResult<RunStep> results = client.GetRunSteps(run.ThreadId, run.Id, new() { PageSizeLimit = numPerPage });
            runSteps = results
                .Skip(numPerPage)
                .Select(r => r.Id)
                .ToList();

            // Simulate rehydration of the collection
            ContinuationToken rehydrationToken = results.GetContinuationToken(results.GetRawPages().FirstOrDefault());
            rehydratedRunSteps = client.GetRunSteps(rehydrationToken)
                .Select(r => r.Id)
                .ToList();
        }

        CollectionAssert.AreEqual(runSteps, rehydratedRunSteps);
    }

    [Test]
    public async Task MessagesWithRoles()
    {
        AssistantClient client = GetTestClient();
        const string userMessageText = "Hello, assistant!";
        const string assistantMessageText = "Hi there, user.";
        ThreadCreationOptions threadCreationOptions = new ThreadCreationOptions()
        {
            InitialMessages =
            {
                new ThreadInitializationMessage(MessageRole.User, [userMessageText]),
                new ThreadInitializationMessage(MessageRole.Assistant, [assistantMessageText]),
            }
        };
        AssistantThread thread = IsAsync
            ? await client.CreateThreadAsync(threadCreationOptions)
            : client.CreateThread(threadCreationOptions);
        Validate(thread);
        List<ThreadMessage> messages = [];
        async ValueTask RefreshMessageListAsync()
        {
            messages.Clear();
            if (IsAsync)
            {
                await foreach (ThreadMessage message in client.GetMessagesAsync(thread.Id))
                {
                    messages.Add(message);
                }
            }
            else
            {
                foreach (ThreadMessage message in client.GetMessages(thread.Id))
                {
                    messages.Add(message);
                }
            }
        }
        await RefreshMessageListAsync();
        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages[1].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[1].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[1].Content[0].Text, Is.EqualTo(userMessageText));
        Assert.That(messages[1].Content[0].Text, Is.EqualTo(userMessageText));
        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[0].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[0].Content[0].Text, Is.EqualTo(assistantMessageText));
        Assert.That(messages[0].Content[0].Text, Is.EqualTo(assistantMessageText));
        ThreadMessage userMessage = IsAsync
            ? await client.CreateMessageAsync(thread.Id, MessageRole.User, [MessageContent.FromText(userMessageText)])
            : client.CreateMessage(thread.Id, MessageRole.User, [MessageContent.FromText(userMessageText)]);
        ThreadMessage assistantMessage = IsAsync
            ? await client.CreateMessageAsync(thread.Id, MessageRole.Assistant, [assistantMessageText])
            : client.CreateMessage(thread.Id, MessageRole.Assistant, [assistantMessageText]);
        await RefreshMessageListAsync();
        Assert.That(messages.Count, Is.EqualTo(4));
        Assert.That(messages[3].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[3].Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages[3].Content[0].Text, Is.EqualTo(userMessageText));
        Assert.That(messages[3].Content[0].Text, Is.EqualTo(userMessageText));
        Assert.That(messages[2].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[2].Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages[2].Content[0].Text, Is.EqualTo(assistantMessageText));
        Assert.That(messages[2].Content[0].Text, Is.EqualTo(assistantMessageText));
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
        if (target is Assistant assistant)
        {
            Assert.That(assistant?.Id, Is.Not.Null);
            _assistantsToDelete.Add(assistant);
        }
        else if (target is AssistantThread thread)
        {
            Assert.That(thread?.Id, Is.Not.Null);
            _threadsToDelete.Add(thread);
        }
        else if (target is ThreadMessage message)
        {
            Assert.That(message?.Id, Is.Not.Null);
            _messagesToDelete.Add(message);
        }
        else if (target is ThreadRun run)
        {
            Assert.That(run?.Id, Is.Not.Null);
        }
        else if (target is OpenAIFile file)
        {
            Assert.That(file?.Id, Is.Not.Null);
            _filesToDelete.Add(file);
        }
        else
        {
            throw new NotImplementedException($"{nameof(Validate)} helper not implemented for: {typeof(T)}");
        }
    }
}

#pragma warning restore OPENAI001
