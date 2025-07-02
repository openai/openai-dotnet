using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Models;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIModelsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Models.ModelDeletionResult"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Models.ModelDeletionResult"/> instance for mocking. </returns>
    public static ModelDeletionResult ModelDeletionResult(string modelId = null, bool deleted = default)
    {
        return new ModelDeletionResult(
            deleted,
            modelId,
            "model",
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Models.OpenAIModel"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Models.OpenAIModel"/> instance for mocking. </returns>
    public static OpenAIModel OpenAIModel(string id = null, DateTimeOffset createdAt = default, string ownedBy = null)
    {
        return new OpenAIModel(
            id,
            ownedBy,
            "model",
            createdAt,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Models.OpenAIModelCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Models.OpenAIModelCollection"/> instance for mocking. </returns>
    public static OpenAIModelCollection OpenAIModelCollection(IEnumerable<OpenAIModel> items = null)
    {
        items ??= new List<OpenAIModel>();

        return new OpenAIModelCollection(
            "list",
            items.ToList(),
            serializedAdditionalRawData: null);
    }
}
