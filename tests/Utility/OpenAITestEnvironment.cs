using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ClientModel.TestFramework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Containers;
using OpenAI.Conversations;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.Responses;
using OpenAI.VectorStores;

namespace OpenAI.Tests;

#pragma warning disable OPENAI001
#pragma warning disable OPENAI002
#pragma warning disable OPENAI003

public class OpenAITestEnvironment : TestEnvironment
{
    // Prepare the environment file and global environment variables once
    // for all tests rather than per instance of the test environment.
    //
    // This call will modify the environment, setting the final state for variables
    // as well as initialize the EnvironmentFile dictionary used by the test framework.
    private static readonly Dictionary<string, string> EnvironmentFile;
    private static readonly object GlobalStateLock = new();

    private static bool s_baseInitialized = false;

    public ApiKeyCredential ApiKeyCredential => new(OpenApiKey);

    // Prefer the recorded variable, if it exists, falling back to
    // the live environment variable.
    public string OpenApiKey =>
        GetRecordedOptionalVariable(EnvironmentVariables.OpenAIKey, options => options.IsSecret("api-key"))
        ?? GetVariable(EnvironmentVariables.OpenAIKey);

    static OpenAITestEnvironment()
    {
        // In order to ensure that the environment is configured in the expected way,
        // it needs to be prepared before the base class is initialized - including its
        // static members.  Because accessing a static member of the base triggers constructors
        // to execute, preparing the environment should be the first thing done.
        //
        // Populate the environment with a preference for locality and respect for the existing environment.
        // Where multiple definitions exist, apply them in the following order, where the last in wins:
        //
        //   1) Default hard-coded values
        //   2) The .env file found in the repository root
        //   3) The .env file found in the local (execution) directory
        //   4) The existing environment variables in the process
        //
        EnvironmentFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { EnvironmentVariables.OpenAIKey, "api-key" },
            { EnvironmentVariables.TestMode, "Playback" }
        };
        _ = TryReadEnvFile(Path.Combine(RepositoryRoot, ".env"), EnvironmentFile);
        _ = TryReadEnvFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".env"), EnvironmentFile);


        // Set the environment variables for those members not already set.  Because these are given preference
        // when reading, there's no need to also update the environment dictionary.  This mutates the environment only
        // in the scope of the current process.

        foreach (var kvp in EnvironmentFile)
        {
            if (Environment.GetEnvironmentVariable(kvp.Key) is null)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }
    }

    public OpenAITestEnvironment() : base()
    {
        // Because this check runs for every instantiation, it is worth the minor performance cost of a volatile read to avoid
        // paying to acquire the lock only to find that initialization has already been completed by another thread.  The lock forces a full
        // a full memory fence, ensuring that all reads/writes while held will be seen correctly without an explicit volatile operation.
        if (Volatile.Read(ref s_baseInitialized) == false)
        {
            lock (GlobalStateLock)
            {
                // Double-check initialization now that the lock is held to avoid unnecessary initializations.
                if (s_baseInitialized)
                {
                    return;
                }

                DevCertPath = Path.Combine(
                    RepositoryRoot,
                    "tests",
                    "Utility",
                    "testproxy",
                    "dotnet-devcert.pfx");

                // OpenAI does not support bootstrapping of test resources, so disable it.
                DisableBootstrapping = true;

                // If a test mode was set, then use that as the default global mode for the environment.  Enum.Parse
                // will accept undefined members, such as "9999", so validate that the parsed value is defined before accepting it.
                var testMode = GetOptionalVariable(EnvironmentVariables.TestMode);

                if ((testMode is { }) && (Enum.TryParse<RecordedTestMode>(testMode, ignoreCase: true, out var parsedTestMode)))
                {
                    if (!Enum.IsDefined(typeof(RecordedTestMode), parsedTestMode))
                    {
                        throw new FormatException($"The test mode value '{testMode}' is not valid. Accepted values are: {string.Join(", ", Enum.GetNames(typeof(RecordedTestMode)))}");
                    }

                    GlobalTestMode = parsedTestMode;
                }

                s_baseInitialized = true;
            }
        }
    }

    public T GetTestClient<T>(string modelName = default, OpenAIClientOptions options = default)
    {
        options ??= new();
        var credential = ApiKeyCredential;

        object clientObject = typeof(T).Name switch
        {
            nameof(AssistantClient) => new AssistantClient(credential, options),
            nameof(AudioClient) => new AudioClient(modelName ?? TestModel.Audio_Whisper, credential, options),
            nameof(BatchClient) => new BatchClient(credential, options),
            nameof(ChatClient) => new ChatClient(modelName ?? TestModel.Chat, credential, options),
            nameof(ContainerClient) => new ContainerClient(credential, options),
            nameof(ConversationClient) => new ConversationClient(credential, options),
            nameof(EmbeddingClient) => new EmbeddingClient(modelName ?? TestModel.Embeddings, credential, options),
            nameof(OpenAIFileClient) => new OpenAIFileClient(credential, options),
            nameof(FineTuningClient) => new FineTuningClient(credential, options),
            nameof(ImageClient) => new ImageClient(modelName ?? TestModel.Images, credential, options),
            nameof(OpenAIModelClient) => new OpenAIModelClient(credential, options),
            nameof(ModerationClient) => new ModerationClient(modelName ?? TestModel.Moderations, credential, options),
            nameof(VectorStoreClient) => new VectorStoreClient(credential, options),
            nameof(OpenAIClient) => new OpenAIClient(credential, options),
            nameof(RealtimeClient) => new RealtimeClient(credential, CreateRealtimeClientOptions(options)),
            nameof(ResponsesClient) => new ResponsesClient(credential, options),
            _ => throw new NotImplementedException($"Unsupported client type: {typeof(T).Name}"),
        };

        return (T)clientObject;
    }

    public override Dictionary<string, string> ParseEnvironmentFile() =>  EnvironmentFile;

    public override Task WaitForEnvironmentAsync() => Task.CompletedTask;

    private static RealtimeClientOptions CreateRealtimeClientOptions(OpenAIClientOptions options) => new()
    {
        Endpoint = options?.Endpoint,
        OrganizationId = options?.OrganizationId,
        ProjectId = options?.ProjectId,
        UserAgentApplicationId = options?.UserAgentApplicationId,
    };

    private static bool TryReadEnvFile(string filePath,
                                       Dictionary<string, string> environment)
    {
        Debug.Assert(!string.IsNullOrEmpty(filePath), "The file path must be specified.");
        Debug.Assert(environment != null, "The environment dictionary must be specified.");

        if (!File.Exists(filePath))
        {
            return false;
        }

        var count = 0;

        foreach (var fileLine in File.ReadLines(filePath))
        {
            ++count;

            var line = fileLine.AsSpan();
            var firstCharacterPos = FindFirstCharacterIndex(line);

            if ((firstCharacterPos == -1) || (line[firstCharacterPos] == '#'))
            {
                continue;
            }

            var separator = line.IndexOf('=');

            if ((separator == -1) || (separator <= firstCharacterPos))
            {
                throw new FormatException($"The environment file is malformed at line {count}");
            }

            var length = separator - firstCharacterPos;
            var parsedKey = line.Slice(firstCharacterPos, length).Trim().ToString();
            var parsedValue = line.Slice(separator + 1).Trim().Trim('"').ToString();

            // Set the value unconditionally, clobbering any existing data.
            environment[parsedKey] = parsedValue;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindFirstCharacterIndex(ReadOnlySpan<char> line)
    {
        var position = 0;
        var length = line.Length;

        while ((position < length) && (char.IsWhiteSpace(line[position])))
        {
            ++position;
        }

        return (position < length) ? position : -1;
    }

    private static class EnvironmentVariables
    {
        public const string OpenAIKey = "OPENAI_API_KEY";
        public const string TestMode = "CLIENTMODEL_TEST_MODE";
    }
}