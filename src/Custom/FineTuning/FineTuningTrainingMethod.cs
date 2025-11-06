using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuneMethod")]
public partial class FineTuningTrainingMethod
{
    private static readonly BinaryData Auto = new("\"auto\"");

    internal InternalFineTuningJobRequestMethodSupervised Supervised { get; set; }

    internal InternalFineTuningJobRequestMethodDpo Dpo { get; set; }

    public static FineTuningTrainingMethod CreateSupervised(
        HyperparameterBatchSize batchSize = null,
        HyperparameterEpochCount epochCount = null,
        HyperparameterLearningRate learningRate = null)
    {
        return new FineTuningTrainingMethod()
        {
            Kind = InternalFineTuneMethodType.Supervised,
            Supervised = new() {
                Hyperparameters = new() {
                    _BatchSize = batchSize is not null ? ModelReaderWriter.Write(batchSize, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                    _NEpochs = epochCount is not null ? ModelReaderWriter.Write(epochCount, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                    _LearningRateMultiplier = learningRate is not null ? ModelReaderWriter.Write(learningRate, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                },
            },
        };
    }

    public static FineTuningTrainingMethod CreateDirectPreferenceOptimization(
        HyperparameterBatchSize batchSize = null,
        HyperparameterEpochCount epochCount = null,
        HyperparameterLearningRate learningRate = null,
        HyperparameterBetaFactor betaFactor = null)
    {
        return new FineTuningTrainingMethod()
        {
            Kind = InternalFineTuneMethodType.Dpo,
            Dpo = new() {
                Hyperparameters = new() {
                    _Beta = betaFactor is not null ? ModelReaderWriter.Write(betaFactor, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                    _BatchSize = batchSize is not null ? ModelReaderWriter.Write(batchSize, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                    _NEpochs = epochCount is not null ? ModelReaderWriter.Write(epochCount, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                    _LearningRateMultiplier = learningRate is not null ? ModelReaderWriter.Write(learningRate, ModelReaderWriterOptions.Json, OpenAIContext.Default) : null,
                },
            },
        };
    }
}