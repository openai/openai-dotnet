using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.FineTuning;
using OpenAI.VectorStores;
using OpenAI.Videos;

namespace OpenAI.Tests.SourceCompatibility;

public class PaginationNamedArgumentCompatibilityTests
{
    [Test]
    public void LegacyPaginationNamedArgumentsCompile()
    {
        Assert.Pass("The compile-only calls below verify the legacy named-argument surface.");
    }

    private static void CompileOnly(
        AssistantClient assistantClient,
        BatchClient batchClient,
        ChatClient chatClient,
        FineTuningClient fineTuningClient,
        VectorStoreClient vectorStoreClient,
        VideoClient videoClient)
    {
        _ = assistantClient.GetAssistants(limit: null, order: null, after: null, before: null, options: null);
        _ = assistantClient.GetAssistantsAsync(limit: null, order: null, after: null, before: null, options: null);

        _ = batchClient.GetBatches(after: null, limit: null, options: null);
        _ = batchClient.GetBatchesAsync(after: null, limit: null, options: null);

        _ = chatClient.GetChatCompletions(after: null, limit: null, order: null, metadata: null, model: null, options: null);
        _ = chatClient.GetChatCompletionsAsync(after: null, limit: null, order: null, metadata: null, model: null, options: null);
        _ = chatClient.GetChatCompletionMessages(completionId: "completion", after: null, limit: null, order: null, options: null);
        _ = chatClient.GetChatCompletionMessagesAsync(completionId: "completion", after: null, limit: null, order: null, options: null);

        _ = fineTuningClient.GetFineTuningCheckpointPermissions(
            fineTunedModelCheckpoint: "checkpoint", after: null, limit: null, order: null, projectId: null, options: null);
        _ = fineTuningClient.GetFineTuningCheckpointPermissionsAsync(
            fineTunedModelCheckpoint: "checkpoint", after: null, limit: null, order: null, projectId: null, options: null);

        _ = vectorStoreClient.GetVectorStores(limit: null, order: null, after: null, before: null, options: null);
        _ = vectorStoreClient.GetVectorStoresAsync(limit: null, order: null, after: null, before: null, options: null);
        _ = vectorStoreClient.GetVectorStoreFiles(
            vectorStoreId: "vector_store", limit: null, order: null, after: null, before: null, filter: null, options: null);
        _ = vectorStoreClient.GetVectorStoreFilesAsync(
            vectorStoreId: "vector_store", limit: null, order: null, after: null, before: null, filter: null, options: null);
        _ = vectorStoreClient.GetVectorStoreFilesInBatch(
            vectorStoreId: "vector_store", batchId: "batch", limit: null, order: null, after: null, before: null, filter: null, options: null);
        _ = vectorStoreClient.GetVectorStoreFilesInBatchAsync(
            vectorStoreId: "vector_store", batchId: "batch", limit: null, order: null, after: null, before: null, filter: null, options: null);

        _ = videoClient.GetVideos(limit: null, order: null, after: null, options: null);
        _ = videoClient.GetVideosAsync(limit: null, order: null, after: null, options: null);
    }
}
