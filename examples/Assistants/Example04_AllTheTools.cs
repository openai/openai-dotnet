using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class AssistantExamples
{
    [Test]
    public void Example04_AllTheTools()
    {
        #region Define a function tool
        static string GetNameOfFamilyMember(string relation)
            => relation switch
            {
                { } when relation.Contains("father") => "John Doe",
                { } when relation.Contains("mother") => "Jane Doe",
                _ => throw new ArgumentException(relation, nameof(relation))
            };

        FunctionToolDefinition getNameOfFamilyMemberTool = new(nameof(GetNameOfFamilyMember))
        {
            Description = "Provided a family relation type like 'father' or 'mother', "
                + "gets the name of the related person from the user.",
            Parameters = BinaryData.FromString("""
            {
              "type": "object",
              "properties": {
                "relation": {
                  "type": "string",
                    "description": "The relation to the user to query, e.g. 'mother' or 'father'"
                  }
               },
               "required": [ "relation" ]
            }
            """),
        };

        #region Upload a mock file for use with file search
        OpenAIFileClient fileClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFile favoriteNumberFile = fileClient.UploadFile(
            BinaryData.FromString("""
                This file contains the favorite numbers for individuals.

                John Doe: 14
                Bob Doe: 32
                Jane Doe: 44
                """).ToStream(),
            "favorite_numbers.txt",
            FileUploadPurpose.Assistants);
        #endregion

        #region Create an assistant with functions, file search, and code interpreter all enabled
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        Assistant assistant = client.CreateAssistant("gpt-4-turbo", new AssistantCreationOptions()
        {
            Instructions = "Use functions to resolve family relations into the names of people. Use file search to "
                + " look up the favorite numbers of people. Use code interpreter to create graphs of lines.",
            Tools = { getNameOfFamilyMemberTool, new FileSearchToolDefinition(), new CodeInterpreterToolDefinition() },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([favoriteNumberFile.Id]),
                    },
                },
            },
        });
        #endregion

        #region Create a new thread and start a run
        AssistantThread thread = client.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages =
            {
                "Create a graph of a line with a slope that's my father's favorite number "
                + "and an offset that's my mother's favorite number.",
                "Include people's names in your response and cite where you found them."
            }
        });

        ThreadRun run = client.CreateRun(thread.Id, assistant.Id);
        #endregion

        #region Complete the run, calling functions as needed
        // Poll the run until it is no longer queued or in progress.
        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            run = client.GetRun(run.ThreadId, run.Id);

            // If the run requires action, resolve them.
            if (run.Status == RunStatus.RequiresAction)
            {
                List<ToolOutput> toolOutputs = [];

                foreach (RequiredAction action in run.RequiredActions)
                {
                    switch (action.FunctionName)
                    {
                        case nameof(GetNameOfFamilyMember):
                            {
                                using JsonDocument argumentsDocument = JsonDocument.Parse(action.FunctionArguments);
                                string relation = argumentsDocument.RootElement.TryGetProperty("relation", out JsonElement relationProperty)
                                    ? relationProperty.GetString()
                                    : null;
                                string toolResult = GetNameOfFamilyMember(relation);
                                toolOutputs.Add(new ToolOutput(action.ToolCallId, toolResult));
                                break;
                            }

                        default:
                            {
                                // Handle other or unexpected calls.
                                throw new NotImplementedException();
                            }
                    }
                }

                // Submit the tool outputs to the assistant, which returns the run to the queued state.
                run = client.SubmitToolOutputsToRun(run.ThreadId, run.Id, toolOutputs);
            }
        }
        #endregion

        #region
        // With the run complete, list the messages and display their content
        if (run.Status == RunStatus.Completed)
        {
            CollectionResult<ThreadMessage> messages
                = client.GetMessages(run.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });
            foreach (ThreadMessage message in messages)
            {
                Console.WriteLine($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    Console.WriteLine($"{contentItem.Text}");

                    if (contentItem.ImageFileId is not null)
                    {
                        Console.WriteLine($" <Image File ID> {contentItem.ImageFileId}");
                    }

                    // Include annotations, if any.
                    if (contentItem.TextAnnotations.Count > 0)
                    {
                        Console.WriteLine();
                        foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                        {
                            Console.WriteLine($"* File ID used by file_search: {annotation.InputFileId}");
                            Console.WriteLine($"* File ID created by code_interpreter: {annotation.OutputFileId}");
                            Console.WriteLine($"* Text to replace: {annotation.TextToReplace}");
                            Console.WriteLine($"* Message content index range: {annotation.StartIndex}-{annotation.EndIndex}");
                        }
                    }

                }
                Console.WriteLine();
            }
            #endregion

            #region List run steps for details about tool calls
            CollectionResult<RunStep> runSteps = client.GetRunSteps(
                run.ThreadId,
                run.Id,
                new RunStepCollectionOptions()
                {
                    Order = RunStepCollectionOrder.Ascending
                });
            foreach (RunStep step in runSteps)
            {
                Console.WriteLine($"Run step: {step.Status}");
                foreach (RunStepToolCall toolCall in step.Details.ToolCalls)
                {
                    Console.WriteLine($" --> Tool call: {toolCall.Kind}");
                    foreach (RunStepCodeInterpreterOutput output in toolCall.CodeInterpreterOutputs)
                    {
                        Console.WriteLine($"    --> Output: {output.ImageFileId}");
                    }
                }
            }
            #endregion
        }
        else
        {
            throw new NotImplementedException(run.Status.ToString());
        }
        #endregion

        #region Clean up any temporary resources that are no longer needed
        _ = client.DeleteThread(thread.Id);
        _ = client.DeleteAssistant(assistant.Id);
        _ = fileClient.DeleteFile(favoriteNumberFile.Id);
        #endregion
    }
}

#pragma warning restore OPENAI001