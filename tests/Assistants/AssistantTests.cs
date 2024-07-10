﻿using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.Fixtures)]
public partial class AssistantTests
{
    [Test]
    public void BasicAssistantOperationsWork()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-3.5-turbo");
        Validate(assistant);
        Assert.That(assistant.Name, Is.Null.Or.Empty);
        assistant = client.ModifyAssistant(assistant.Id, new AssistantModificationOptions()
        {
            Name = "test assistant name",
        });
        Assert.That(assistant.Name, Is.EqualTo("test assistant name"));
        bool deleted = client.DeleteAssistant(assistant.Id);
        Assert.That(deleted, Is.True);
        _assistantsToDelete.Remove(assistant);
        assistant = client.CreateAssistant("gpt-3.5-turbo", new AssistantCreationOptions()
        {
            Metadata =
            {
                [s_cleanupMetadataKey] = "hello!"
            },
        });
        Validate(assistant);
        Assistant retrievedAssistant = client.GetAssistant(assistant.Id);
        Assert.That(retrievedAssistant.Id, Is.EqualTo(assistant.Id));
        Assert.That(retrievedAssistant.Metadata.TryGetValue(s_cleanupMetadataKey, out string metadataValue) && metadataValue == "hello!");
        Assistant modifiedAssistant = client.ModifyAssistant(assistant.Id, new AssistantModificationOptions()
        {
            Metadata =
            {
                [s_cleanupMetadataKey] = "goodbye!",
            },
        });
        Assert.That(modifiedAssistant.Id, Is.EqualTo(assistant.Id));
        PageableCollection<Assistant> recentAssistants = client.GetAssistants();
        Assistant listedAssistant = recentAssistants.FirstOrDefault(pageItem => pageItem.Id == assistant.Id);
        Assert.That(listedAssistant, Is.Not.Null);
        Assert.That(listedAssistant.Metadata.TryGetValue(s_cleanupMetadataKey, out string newMetadataValue) && newMetadataValue == "goodbye!");
    }

    [Test]
    public void BasicThreadOperationsWork()
    {
        AssistantClient client = GetTestClient();
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        Assert.That(thread.CreatedAt, Is.GreaterThan(s_2024));
        bool deleted = client.DeleteThread(thread.Id);
        Assert.That(deleted, Is.True);
        _threadsToDelete.Remove(thread);

        ThreadCreationOptions options = new()
        {
            Metadata =
            {
                ["threadMetadata"] = "threadMetadataValue",
            }
        };
        thread = client.CreateThread(options);
        Validate(thread);
        Assert.That(thread.Metadata.TryGetValue("threadMetadata", out string threadMetadataValue) && threadMetadataValue == "threadMetadataValue");
        AssistantThread retrievedThread = client.GetThread(thread.Id);
        Assert.That(retrievedThread.Id, Is.EqualTo(thread.Id));
        thread = client.ModifyThread(thread, new ThreadModificationOptions()
        {
            Metadata =
            {
                ["threadMetadata"] = "newThreadMetadataValue",
            },
        });
        Assert.That(thread.Metadata.TryGetValue("threadMetadata", out threadMetadataValue) && threadMetadataValue == "newThreadMetadataValue");
    }

    [Test]
    public void BasicMessageOperationsWork()
    {
        AssistantClient client = GetTestClient();
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        ThreadMessage message = client.CreateMessage(thread, MessageRole.User, ["Hello, world!"]);
        Validate(message);
        Assert.That(message.CreatedAt, Is.GreaterThan(s_2024));
        Assert.That(message.Content?.Count, Is.EqualTo(1));
        Assert.That(message.Content[0], Is.Not.Null);
        Assert.That(message.Content[0].Text, Is.EqualTo("Hello, world!"));
        bool deleted = client.DeleteMessage(message);
        Assert.That(deleted, Is.True);
        _messagesToDelete.Remove(message);

        message = client.CreateMessage(thread, MessageRole.User, ["Goodbye, world!"], new MessageCreationOptions()
        {
            Metadata =
            {
                ["messageMetadata"] = "messageMetadataValue",
            },
        });
        Validate(message);
        Assert.That(message.Metadata.TryGetValue("messageMetadata", out string metadataValue) && metadataValue == "messageMetadataValue");

        ThreadMessage retrievedMessage = client.GetMessage(thread.Id, message.Id);
        Assert.That(retrievedMessage.Id, Is.EqualTo(message.Id));

        message = client.ModifyMessage(message, new MessageModificationOptions()
        {
            Metadata =
            {
                ["messageMetadata"] = "newValue",
            }
        });
        Assert.That(message.Metadata.TryGetValue("messageMetadata", out metadataValue) && metadataValue == "newValue");

        PageableCollection<ThreadMessage> messagePage = client.GetMessages(thread);
        Assert.That(messagePage.Count, Is.EqualTo(1));
        Assert.That(messagePage.First().Id, Is.EqualTo(message.Id));
        Assert.That(messagePage.First().Metadata.TryGetValue("messageMetadata", out metadataValue) && metadataValue == "newValue");
    }

    [Test]
    public void ThreadWithInitialMessagesWorks()
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
                        MessageContent.FromImageUrl(new Uri("https://test.openai.com/image.png"))
                    ])
                {
                    Metadata =
                    {
                        ["messageMetadata"] = "messageMetadataValue",
                    },
                },
            },
        };
        AssistantThread thread = client.CreateThread(options);
        Validate(thread);
        PageableCollection<ThreadMessage> messages = client.GetMessages(thread, resultOrder: ListOrder.OldestFirst);
        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages.First().Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages.First().Content?.Count, Is.EqualTo(1));
        Assert.That(messages.First().Content[0].Text, Is.EqualTo("Hello, world!"));
        Assert.That(messages.ElementAt(1).Content?.Count, Is.EqualTo(2));
        Assert.That(messages.ElementAt(1).Content[0], Is.Not.Null);
        Assert.That(messages.ElementAt(1).Content[0].Text, Is.EqualTo("Can you describe this image for me?"));
        Assert.That(messages.ElementAt(1).Content[1], Is.Not.Null);
        Assert.That(messages.ElementAt(1).Content[1].ImageUrl.AbsoluteUri, Is.EqualTo("https://test.openai.com/image.png"));
    }

    [Test]
    public void BasicRunOperationsWork()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-3.5-turbo");
        Validate(assistant);
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        PageableCollection<ThreadRun> runs = client.GetRuns(thread);
        Assert.That(runs.Count, Is.EqualTo(0));
        ThreadMessage message = client.CreateMessage(thread.Id, MessageRole.User, ["Hello, assistant!"]);
        Validate(message);
        ThreadRun run = client.CreateRun(thread.Id, assistant.Id);
        Validate(run);
        Assert.That(run.Status, Is.EqualTo(RunStatus.Queued));
        Assert.That(run.CreatedAt, Is.GreaterThan(s_2024));
        ThreadRun retrievedRun = client.GetRun(thread.Id, run.Id);
        Assert.That(retrievedRun.Id, Is.EqualTo(run.Id));
        runs = client.GetRuns(thread);
        Assert.That(runs.Count, Is.EqualTo(1));
        Assert.That(runs.First().Id, Is.EqualTo(run.Id));

        PageableCollection<ThreadMessage> messages = client.GetMessages(thread);
        Assert.That(messages.Count, Is.GreaterThanOrEqualTo(1));
        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(500);
            run = client.GetRun(run);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.CompletedAt, Is.GreaterThan(s_2024));
        Assert.That(run.RequiredActions.Count, Is.EqualTo(0));
        Assert.That(run.AssistantId, Is.EqualTo(assistant.Id));
        Assert.That(run.FailedAt, Is.Null);
        Assert.That(run.IncompleteDetails, Is.Null);

        messages = client.GetMessages(thread);
        Assert.That(messages.Count, Is.EqualTo(2));

        Assert.That(messages.ElementAt(0).Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages.ElementAt(1).Role, Is.EqualTo(MessageRole.User));
        Assert.That(messages.ElementAt(1).Id, Is.EqualTo(message.Id));
    }

    [Test]
    public void BasicRunStepFunctionalityWorks()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4o", new AssistantCreationOptions()
        {
            Tools = { new CodeInterpreterToolDefinition() },
            Instructions = "You help the user with mathematical descriptions and visualizations.",
        });
        Validate(assistant);

        FileClient fileClient = new();
        OpenAIFileInfo equationFile = fileClient.UploadFile(
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

        ThreadRun run = client.CreateRun(thread, assistant);
        Validate(run);

        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(1000);
            run = client.GetRun(run);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));
        Assert.That(run.Usage?.TotalTokens, Is.GreaterThan(0));

        PageableCollection<RunStep> runSteps = client.GetRunSteps(run);
        Assert.That(runSteps.Count, Is.GreaterThan(1));
        Assert.Multiple(() =>
        {
            Assert.That(runSteps.First().AssistantId, Is.EqualTo(assistant.Id));
            Assert.That(runSteps.First().ThreadId, Is.EqualTo(thread.Id));
            Assert.That(runSteps.First().RunId, Is.EqualTo(run.Id));
            Assert.That(runSteps.First().CreatedAt, Is.GreaterThan(s_2024));
            Assert.That(runSteps.First().CompletedAt, Is.GreaterThan(s_2024));
        });
        RunStepDetails details = runSteps.First().Details;
        Assert.That(details?.CreatedMessageId, Is.Not.Null.And.Not.Empty);

        string rawContent = runSteps.GetRawResponse().Content.ToString();
        details = runSteps.ElementAt(1).Details;
        Assert.Multiple(() =>
        {
            Assert.That(details?.ToolCalls.Count, Is.GreaterThan(0));
            Assert.That(details.ToolCalls[0].ToolKind, Is.EqualTo(RunStepToolCallKind.CodeInterpreter));
            Assert.That(details.ToolCalls[0].ToolCallId, Is.Not.Null.And.Not.Empty);
            Assert.That(details.ToolCalls[0].CodeInterpreterInput, Is.Not.Null.And.Not.Empty);
            Assert.That(details.ToolCalls[0].CodeInterpreterOutputs?.Count, Is.GreaterThan(0));
            Assert.That(details.ToolCalls[0].CodeInterpreterOutputs[0].ImageFileId, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public void SettingResponseFormatWorks()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-4-turbo", new()
        {
            ResponseFormat = AssistantResponseFormat.JsonObject,
        });
        Validate(assistant);
        Assert.That(assistant.ResponseFormat, Is.EqualTo(AssistantResponseFormat.JsonObject));
        assistant = client.ModifyAssistant(assistant, new()
        {
            ResponseFormat = AssistantResponseFormat.Text,
        });
        Assert.That(assistant.ResponseFormat, Is.EqualTo(AssistantResponseFormat.Text));
        AssistantThread thread = client.CreateThread();
        Validate(thread);
        ThreadMessage message = client.CreateMessage(thread, MessageRole.User, ["Write some JSON for me!"]);
        Validate(message);
        ThreadRun run = client.CreateRun(thread, assistant, new()
        {
            ResponseFormat = AssistantResponseFormat.JsonObject,
        });
        Validate(run);
        Assert.That(run.ResponseFormat, Is.EqualTo(AssistantResponseFormat.JsonObject));
    }

    [Test]
    public void FunctionToolsWork()
    {
        AssistantClient client = GetTestClient();
        Assistant assistant = client.CreateAssistant("gpt-3.5-turbo", new AssistantCreationOptions()
        {
            Tools =
            {
                new FunctionToolDefinition()
                {
                    FunctionName = "get_favorite_food_for_day_of_week",
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
        });
        Validate(assistant);
        Assert.That(assistant.Tools?.Count, Is.EqualTo(1));

        FunctionToolDefinition responseToolDefinition = assistant.Tools[0] as FunctionToolDefinition;
        Assert.That(responseToolDefinition?.FunctionName, Is.EqualTo("get_favorite_food_for_day_of_week"));
        Assert.That(responseToolDefinition?.Parameters, Is.Not.Null);

        ThreadRun run = client.CreateThreadAndRun(
            assistant,
            new ThreadCreationOptions()
            {
                InitialMessages = { "What should I eat on Thursday?" },
            },
            new RunCreationOptions()
            {
                AdditionalInstructions = "Call provided tools when appropriate.",
            });
        Validate(run);

        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(500);
            run = client.GetRun(run);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.RequiresAction));
        Assert.That(run.RequiredActions?.Count, Is.EqualTo(1));
        Assert.That(run.RequiredActions[0].ToolCallId, Is.Not.Null.And.Not.Empty);
        Assert.That(run.RequiredActions[0].FunctionName, Is.EqualTo("get_favorite_food_for_day_of_week"));
        Assert.That(run.RequiredActions[0].FunctionArguments, Is.Not.Null.And.Not.Empty);

        run = client.SubmitToolOutputsToRun(run, [new(run.RequiredActions[0].ToolCallId, "tacos")]);
        Assert.That(run.Status.IsTerminal, Is.False);

        for (int i = 0; i < 10 && !run.Status.IsTerminal; i++)
        {
            Thread.Sleep(500);
            run = client.GetRun(run);
        }
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));

        PageableCollection<ThreadMessage> messages = client.GetMessages(run.ThreadId, resultOrder: ListOrder.NewestFirst);
        Assert.That(messages.Count, Is.GreaterThan(1));
        Assert.That(messages.First().Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(messages.First().Content?[0], Is.Not.Null);
        Assert.That(messages.First().Content[0].Text.ToLowerInvariant(), Does.Contain("tacos"));
    }

    [Test]
    public async Task StreamingRunWorks()
    {
        AssistantClient client = new();
        Assistant assistant = await client.CreateAssistantAsync("gpt-3.5-turbo");
        Validate(assistant);

        AssistantThread thread = await client.CreateThreadAsync(new ThreadCreationOptions()
        {
            InitialMessages = { "Hello there, assistant! How are you today?", },
        });
        Validate(thread);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        AsyncResultCollection<StreamingUpdate> streamingResult
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

    [TestCase]
    public async Task StreamingToolCall()
    {
        AssistantClient client = GetTestClient();
        FunctionToolDefinition getWeatherTool = new("get_current_weather", "Gets the user's current weather");
        Assistant assistant = await client.CreateAssistantAsync("gpt-3.5-turbo", new()
        {
            Tools = { getWeatherTool }
        });
        Validate(assistant);

        Stopwatch stopwatch = Stopwatch.StartNew();
        void Print(string message) => Console.WriteLine($"[{stopwatch.ElapsedMilliseconds,6}] {message}");

        Print(" >>> Beginning call ... ");
        AsyncResultCollection<StreamingUpdate> asyncResults = client.CreateThreadAndRunStreamingAsync(
            assistant,
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
                asyncResults = client.SubmitToolOutputsToRunStreamingAsync(run, toolOutputs);
            }
        } while (run?.Status.IsTerminal == false);
    }

    [Test]
    public void BasicFileSearchWorks()
    {
        // First, we need to upload a simple test file.
        FileClient fileClient = new();
        OpenAIFileInfo testFile = fileClient.UploadFile(
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

        // Create an assistant, using the creation helper to make a new vector store
        Assistant assistant = client.CreateAssistant("gpt-4-turbo", new()
        {
            Tools = { new FileSearchToolDefinition() },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([testFile.Id]),
                    }
                }
            }
        });
        Validate(assistant);
        Assert.That(assistant.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        string createdVectorStoreId = assistant.ToolResources.FileSearch.VectorStoreIds[0];
        _vectorStoreIdsToDelete.Add(createdVectorStoreId);

        // Modify an assistant to use the existing vector store
        assistant = client.ModifyAssistant(assistant, new AssistantModificationOptions()
        {
            ToolResources = new()
            {
                FileSearch = new()
                {
                    VectorStoreIds = { assistant.ToolResources.FileSearch.VectorStoreIds[0] },
                },
            },
        });
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
        thread = client.ModifyThread(thread, new ThreadModificationOptions()
        {
            ToolResources = new()
            {
                FileSearch = new()
                {
                    VectorStoreIds = { createdVectorStoreId },
                }
            }
        });
        Assert.That(thread.ToolResources?.FileSearch?.VectorStoreIds, Has.Count.EqualTo(1));
        Assert.That(thread.ToolResources.FileSearch.VectorStoreIds[0], Is.EqualTo(createdVectorStoreId));

        ThreadRun run = client.CreateRun(thread, assistant);
        Validate(run);
        do
        {
            Thread.Sleep(1000);
            run = client.GetRun(run);
        } while (run?.Status.IsTerminal == false);
        Assert.That(run.Status, Is.EqualTo(RunStatus.Completed));

        PageableCollection<ThreadMessage> messages = client.GetMessages(thread, resultOrder: ListOrder.NewestFirst);
        foreach (ThreadMessage message in messages)
        {
            foreach (MessageContent content in message.Content)
            {
                Console.WriteLine(content.Text);
                foreach (TextAnnotation annotation in content.TextAnnotations)
                {
                    Console.WriteLine($"  --> From file: {annotation.InputFileId}, replacement: {annotation.TextToReplace}");
                }
            }
        }
        Assert.That(messages.Count() > 1);
        Assert.That(messages.Any(message => message.Content.Any(content => content.Text.ToLower().Contains("cake"))));
    }

    [Test]
    public async Task CanEnumerateAssistants()
    {
        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < 10; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-3.5-turbo", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}",
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        AsyncPageableCollection<Assistant> assistants = client.GetAssistantsAsync(ListOrder.NewestFirst);

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

        Assert.That(count, Is.GreaterThanOrEqualTo(10));
    }

    [Test]
    public async Task CanPageThroughAssistantCollection()
    {
        AssistantClient client = GetTestClient();

        // Create assistant collection
        for (int i = 0; i < 10; i++)
        {
            Assistant assistant = client.CreateAssistant("gpt-3.5-turbo", new AssistantCreationOptions()
            {
                Name = $"Test Assistant {i}"
            });
            Validate(assistant);
            Assert.That(assistant.Name, Is.EqualTo($"Test Assistant {i}"));
        }

        // Page through collection
        int count = 0;
        int pageCount = 0;
        AsyncPageableCollection<Assistant> assistants = client.GetAssistantsAsync(ListOrder.NewestFirst);
        IAsyncEnumerable<ResultPage<Assistant>> pages = assistants.AsPages(pageSizeHint: 2);

        int lastIdSeen = int.MaxValue;

        await foreach (ResultPage<Assistant> page in pages)
        {
            foreach (Assistant assistant in page)
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

        Assert.That(count, Is.GreaterThanOrEqualTo(10));
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(5));
    }

    [Test]
    public async Task MessagesWithRoles()
    {
        AssistantClient client = GetTestClient();
        const string userMessageText = "Hello, assistant!";
        const string assistantMessageText = "Hi there, user.";
        AssistantThread thread = await client.CreateThreadAsync(new ThreadCreationOptions()
        {
            InitialMessages =
            {
                new ThreadInitializationMessage(MessageRole.User, [userMessageText]),
                new ThreadInitializationMessage(MessageRole.Assistant, [assistantMessageText]),
            }
        });
        Validate(thread);
        List<ThreadMessage> messages = [];
        async Task RefreshMessageListAsync()
        {
            messages.Clear();
            await foreach (ThreadMessage message in client.GetMessagesAsync(thread))
            {
                messages.Add(message);
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
        ThreadMessage userMessage = await client.CreateMessageAsync(
            thread,
            MessageRole.User,
            [
                MessageContent.FromText(userMessageText)
            ]);
        ThreadMessage assistantMessage = await client.CreateMessageAsync(
            thread,
            MessageRole.Assistant,
            [assistantMessageText]);
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

    [Test]
    [Category("smoke")]
    public void RunStepDeserialization()
    {
        BinaryData runStepData = BinaryData.FromString(
            """
            {
              "id": "step_Ksdfr5ooy26sayKbIQu2d2Vb",
              "object": "thread.run.step",
              "created_at": 1718906747,
              "run_id": "run_vvuLqtPTte9qCnRb7a5MQPgB",
              "assistant_id": "asst_UyBYTjqlwhSOdHOEzwwGZM6d",
              "thread_id": "thread_lIk2yQzSGHzXrzA4K6N8uPae",
              "type": "tool_calls",
              "status": "completed",
              "cancelled_at": null,
              "completed_at": 1718906749,
              "expires_at": null,
              "failed_at": null,
              "last_error": null,
              "step_details": {
                "type": "tool_calls",
                "tool_calls": [
                  {
                    "id": "call_DUP8WOybwaxKcMoxtr6cJDw1",
                    "type": "code_interpreter",
                    "code_interpreter": {
                      "input": "# Let's read the content of the uploaded file to understand its content.\r\nfile_path = '/mnt/data/assistant-SvXXKd0VKpGbVq9rBDlvZTn0'\r\nwith open(file_path, 'r') as file:\r\n    content = file.read()\r\n\r\n# Output the first few lines of the file to understand its structure and content\r\ncontent[:2000]",
                      "outputs": [
                        {
                          "type": "logs",
                          "logs": "'Index,Value\\nIndex #1,1\\nIndex #2,4\\nIndex #3,9\\nIndex #4,16\\nIndex #5,25\\nIndex #6,36\\nIndex #7,49\\nIndex #8,64\\nIndex #9,81\\nIndex #10,100\\nIndex #11,121\\nIndex #12,144\\nIndex #13,169\\nIndex #14,196\\nIndex #15,225\\nIndex #16,256\\nIndex #17,289\\nIndex #18,324\\nIndex #19,361\\nIndex #20,400\\nIndex #21,441\\nIndex #22,484\\nIndex #23,529\\nIndex #24,576\\nIndex #25,625\\nIndex #26,676\\nIndex #27,729\\nIndex #28,784\\nIndex #29,841\\nIndex #30,900\\nIndex #31,961\\nIndex #32,1024\\nIndex #33,1089\\nIndex #34,1156\\nIndex #35,1225\\nIndex #36,1296\\nIndex #37,1369\\nIndex #38,1444\\nIndex #39,1521\\nIndex #40,1600\\nIndex #41,1681\\nIndex #42,1764\\nIndex #43,1849\\nIndex #44,1936\\nIndex #45,2025\\nIndex #46,2116\\nIndex #47,2209\\nIndex #48,2304\\nIndex #49,2401\\nIndex #50,2500\\nIndex #51,2601\\nIndex #52,2704\\nIndex #53,2809\\nIndex #54,2916\\nIndex #55,3025\\nIndex #56,3136\\nIndex #57,3249\\nIndex #58,3364\\nIndex #59,3481\\nIndex #60,3600\\nIndex #61,3721\\nIndex #62,3844\\nIndex #63,3969\\nIndex #64,4096\\nIndex #65,4225\\nIndex #66,4356\\nIndex #67,4489\\nIndex #68,4624\\nIndex #69,4761\\nIndex #70,4900\\nIndex #71,5041\\nIndex #72,5184\\nIndex #73,5329\\nIndex #74,5476\\nIndex #75,5625\\nIndex #76,5776\\nIndex #77,5929\\nIndex #78,6084\\nIndex #79,6241\\nIndex #80,6400\\nIndex #81,6561\\nIndex #82,6724\\nIndex #83,6889\\nIndex #84,7056\\nIndex #85,7225\\nIndex #86,7396\\nIndex #87,7569\\nIndex #88,7744\\nIndex #89,7921\\nIndex #90,8100\\nIndex #91,8281\\nIndex #92,8464\\nIndex #93,8649\\nIndex #94,8836\\nIndex #95,9025\\nIndex #96,9216\\nIndex #97,9409\\nIndex #98,9604\\nIndex #99,9801\\nIndex #100,10000\\nIndex #101,10201\\nIndex #102,10404\\nIndex #103,10609\\nIndex #104,10816\\nIndex #105,11025\\nIndex #106,11236\\nIndex #107,11449\\nIndex #108,11664\\nIndex #109,11881\\nIndex #110,12100\\nIndex #111,12321\\nIndex #112,12544\\nIndex #113,12769\\nIndex #114,12996\\nIndex #115,13225\\nIndex #116,13456\\nIndex #117,13689\\nIndex #118,13924\\nIndex #119,14161\\nIndex #120,14400\\nIndex #121,14641\\nIndex #122,14884\\nIndex #123,15129\\nIndex #124,15376\\nIndex #125,15625\\nIndex #126,15876\\nIndex #127,16129\\nIndex #128,16384\\nIndex #129,16641\\nIndex #130,16900\\nIndex #131,17161\\nIndex #132,'"
                        }
                      ]
                    }
                  }
                ]
              },
              "usage": {
                "prompt_tokens": 201,
                "completion_tokens": 84,
                "total_tokens": 285
              }
            }
            """);
        RunStep deserializedRunStep = ModelReaderWriter.Read<RunStep>(runStepData);
        Assert.That(deserializedRunStep.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(deserializedRunStep.AssistantId, Is.Not.Null.And.Not.Empty);
        Assert.That(deserializedRunStep.Details, Is.Not.Null);
        Assert.That(deserializedRunStep.Details.ToolCalls, Has.Count.EqualTo(1));
        Assert.That(deserializedRunStep.Details.ToolCalls[0].CodeInterpreterOutputs, Has.Count.EqualTo(1));
        Assert.That(deserializedRunStep.Details.ToolCalls[0].CodeInterpreterOutputs[0].Logs, Is.Not.Null.And.Not.Empty);
    }

    [OneTimeTearDown]
    protected void Cleanup()
    {
        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPEN_API_KEY")))
        {
            return;
        }

        AssistantClient client = new();
        FileClient fileClient = new();
        VectorStoreClient vectorStoreClient = new();
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
        foreach (OpenAIFileInfo file in _filesToDelete)
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
        else if (target is OpenAIFileInfo file)
        {
            Assert.That(file?.Id, Is.Not.Null);
            _filesToDelete.Add(file);
        }
        else
        {
            throw new NotImplementedException($"{nameof(Validate)} helper not implemented for: {typeof(T)}");
        }
    }

    private readonly List<Assistant> _assistantsToDelete = [];
    private readonly List<AssistantThread> _threadsToDelete = [];
    private readonly List<ThreadMessage> _messagesToDelete = [];
    private readonly List<OpenAIFileInfo> _filesToDelete = [];
    private readonly List<string> _vectorStoreIdsToDelete = [];

    private static AssistantClient GetTestClient() => GetTestClient<AssistantClient>(TestScenario.Assistants);

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly string s_testAssistantName = $".NET SDK Test Assistant - Please Delete Me";
    private static readonly string s_cleanupMetadataKey = $"test_metadata_cleanup_eligible";
}

#pragma warning restore OPENAI001
