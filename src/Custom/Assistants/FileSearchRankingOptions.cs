using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("FileSearchRankingOptions")]
[CodeGenSuppress(nameof(FileSearchRankingOptions), typeof(float))]
[CodeGenSuppress(nameof(FileSearchRankingOptions), typeof(FileSearchRankingOptions), typeof(float), typeof(IDictionary<string, BinaryData>))]
public partial class FileSearchRankingOptions
{
    required public float ScoreThreshold
    {
        get => _scoreThreshold;
        set => _scoreThreshold = value;
    }

    [CodeGenMember("ScoreThreshold")]
    private float _scoreThreshold;

    public FileSearchRankingOptions()
    { }

    [SetsRequiredMembers]
    public FileSearchRankingOptions(float scoreThreshold)
    {
        ScoreThreshold = scoreThreshold;
    }

    [SetsRequiredMembers]
    internal FileSearchRankingOptions(FileSearchRanker? ranker, float scoreThreshold, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Ranker = ranker;
        ScoreThreshold = scoreThreshold;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }
}
