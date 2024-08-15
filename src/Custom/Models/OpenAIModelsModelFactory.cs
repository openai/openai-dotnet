using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Models;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIModelsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Models.OpenAIModelInfo"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Models.OpenAIModelInfo"/> instance for mocking. </returns>
    public static OpenAIModelInfo OpenAIModelInfo(string id = null, DateTimeOffset createdAt = default, string ownedBy = null)
    {
        return new OpenAIModelInfo(
            id,
            createdAt,
            InternalModelObject.Model,
            ownedBy,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Models.OpenAIModelInfoCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Models.OpenAIModelInfoCollection"/> instance for mocking. </returns>
    public static OpenAIModelInfoCollection OpenAIModelInfoCollection(IEnumerable<OpenAIModelInfo> items = null)
    {
        items ??= new List<OpenAIModelInfo>();

        return new OpenAIModelInfoCollection(
            InternalListModelsResponseObject.List,
            items.ToList(),
            serializedAdditionalRawData: null);
    }
}
