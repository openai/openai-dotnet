using OpenAI.Containers;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Graders;

[Experimental("OPENAI002")][CodeGenType("GraderStringCheckOperation")] internal readonly partial struct InternalGraderStringCheckOperation {}
[Experimental("OPENAI002")][CodeGenType("GraderType")] internal readonly partial struct InternalGraderType {}
[Experimental("OPENAI002")][CodeGenType("GraderTextSimilarityEvaluationMetric")] internal readonly partial struct InternalGraderTextSimilarityEvaluationMetric {}
[CodeGenType("GraderStringCheck")] internal partial class InternalGraderStringCheck {}
[CodeGenType("Grader")] internal partial class InternalGrader {}
[CodeGenType("UnknownGrader")] internal partial class InternalUnknownGrader {}
[CodeGenType("GraderLabelModel")] internal partial class InternalGraderLabelModel {}
[CodeGenType("GraderTextSimilarity")] internal partial class InternalGraderTextSimilarity {}
[CodeGenType("GraderPython")] internal partial class InternalGraderPython {}
[CodeGenType("GraderScoreModel")] internal partial class InternalGraderScoreModel {}
[CodeGenType("GraderMulti")] internal partial class InternalGraderMulti {}
[CodeGenType("FineTuneReinforcementHyperparameters")] internal partial class InternalFineTuneReinforcementHyperparameters { }

[CodeGenType("RunGraderRequest")]
internal partial class InternalRunGraderRequest
{
    public static implicit operator BinaryContent(InternalRunGraderRequest internalRunGraderRequest)
    {
        if (internalRunGraderRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalRunGraderRequest, ModelSerializationExtensions.WireOptions);
    }
    
    public static explicit operator InternalRunGraderRequest(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalRunGraderRequest(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("RunGraderResponse")]
internal partial class InternalRunGraderResponse
{
    public static explicit operator InternalRunGraderResponse(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalRunGraderResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("RunGraderResponseMetadata")] internal partial class InternalRunGraderResponseMetadata {}
[CodeGenType("RunGraderResponseMetadataErrors")] internal partial class InternalRunGraderResponseMetadataErrors {}
[CodeGenType("ValidateGraderRequest")]
internal partial class InternalValidateGraderRequest
{
    public static implicit operator BinaryContent(InternalValidateGraderRequest internalValidateGraderRequest)
    {
        if (internalValidateGraderRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalValidateGraderRequest, ModelSerializationExtensions.WireOptions);
    }

    public static explicit operator InternalValidateGraderRequest(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalValidateGraderRequest(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("ValidateGraderResponse")]
internal partial class InternalValidateGraderResponse
{
    public static explicit operator InternalValidateGraderResponse(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalValidateGraderResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
