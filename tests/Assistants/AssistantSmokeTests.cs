﻿using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.Fixtures)]
[Category("Assistants")]
[Category("Smoke")]
public partial class AssistantSmokeTests
{
    [Test]
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
