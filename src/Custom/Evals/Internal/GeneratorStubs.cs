using OpenAI.Files;
using System.ClientModel;

namespace OpenAI.Evals;

[CodeGenType("CreateEvalRequest")] public partial class InternalCreateEvalRequest {}
[CodeGenType("CreateEvalRunRequest")] public partial class InternalCreateEvalRunRequest {}
[CodeGenType("DeleteEvalResponse")] public partial class InternalDeleteEvalResponse {}
[CodeGenType("DeleteEvalResponseObject")] public readonly partial struct InternalDeleteEvalResponseObject {}
[CodeGenType("DeleteEvalRunResponse")] public partial class InternalDeleteEvalRunResponse {}
[CodeGenType("DeleteEvalRunResponseObject")] public readonly partial struct InternalDeleteEvalRunResponseObject {}
[CodeGenType("Eval")] public partial class InternalEval {}
[CodeGenType("EvalApiError")] public partial class InternalEvalApiError {}
[CodeGenType("EvalCompletionsRunDataSourceParams")] public partial class InternalEvalCompletionsRunDataSourceParams {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages1")] public partial class InternalEvalCompletionsRunDataSourceParamsInputMessages1 {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages1Type")] public readonly partial struct InternalEvalCompletionsRunDataSourceParamsInputMessages1Type {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages2")] public partial class InternalEvalCompletionsRunDataSourceParamsInputMessages2 {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages2Type")] public readonly partial struct InternalEvalCompletionsRunDataSourceParamsInputMessages2Type {}
[CodeGenType("EvalCompletionsRunDataSourceParamsSamplingParams")] public partial class InternalEvalCompletionsRunDataSourceParamsSamplingParams {}
[CodeGenType("EvalCustomDataSourceConfigParams")] public partial class InternalEvalCustomDataSourceConfigParams {}
[CodeGenType("EvalCustomDataSourceConfigResource")] public partial class InternalEvalCustomDataSourceConfigResource {}
[CodeGenType("EvalDataSourceConfigParams")] public partial class InternalEvalDataSourceConfigParams {}
[CodeGenType("EvalDataSourceConfigResource")] public partial class InternalEvalDataSourceConfigResource {}
[CodeGenType("EvalDataSourceConfigType")] public readonly partial struct InternalEvalDataSourceConfigType {}
[CodeGenType("EvalGraderParams")] public partial class InternalEvalGraderParams {}
[CodeGenType("EvalGraderResource")] public partial class InternalEvalGraderResource {}
[CodeGenType("EvalGraderType")] public readonly partial struct InternalEvalGraderType {}
[CodeGenType("EvalItem")] public partial class InternalEvalItem {}
[CodeGenType("EvalItemContent")] public partial class InternalEvalItemContent {}
[CodeGenType("EvalItemContentInputText")] public partial class InternalEvalItemContentInputText {}
[CodeGenType("EvalItemContentOutputText")] public partial class InternalEvalItemContentOutputText {}
[CodeGenType("EvalItemContentType")] public readonly partial struct InternalEvalItemContentType {}
[CodeGenType("EvalItemRole")] public readonly partial struct InternalEvalItemRole {}
[CodeGenType("EvalItemType")] public readonly partial struct InternalEvalItemType {}
[CodeGenType("EvalJsonlRunDataSourceParams")] public partial class InternalEvalJsonlRunDataSourceParams {}
[CodeGenType("EvalLabelModelGraderParams")] public partial class InternalEvalLabelModelGraderParams {}
[CodeGenType("EvalLabelModelGraderParamsInput")] public partial class InternalEvalLabelModelGraderParamsInput {}
[CodeGenType("EvalLabelModelGraderResource")] public partial class InternalEvalLabelModelGraderResource {}
[CodeGenType("EvalList")] public partial class InternalEvalList {}
[CodeGenType("EvalListObject")] public readonly partial struct InternalEvalListObject {}
[CodeGenType("EvalLogsDataSourceConfigParams")] public partial class InternalEvalLogsDataSourceConfigParams {}
[CodeGenType("EvalObject")] public readonly partial struct InternalEvalObject {}
[CodeGenType("EvalPythonGraderParams")] public partial class InternalEvalPythonGraderParams {}
[CodeGenType("EvalPythonGraderResource")] public partial class InternalEvalPythonGraderResource {}
[CodeGenType("EvalResponsesRunDataSourceParams")] public partial class InternalEvalResponsesRunDataSourceParams {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages1")] public partial class InternalEvalResponsesRunDataSourceParamsInputMessages1 {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages1Type")] public readonly partial struct InternalEvalResponsesRunDataSourceParamsInputMessages1Type {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages2")] public partial class InternalEvalResponsesRunDataSourceParamsInputMessages2 {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages2Type")] public readonly partial struct InternalEvalResponsesRunDataSourceParamsInputMessages2Type {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessagesTemplate1")] public partial class InternalEvalResponsesRunDataSourceParamsInputMessagesTemplate1 {}
[CodeGenType("EvalResponsesRunDataSourceParamsSamplingParams")] public partial class InternalEvalResponsesRunDataSourceParamsSamplingParams {}
[CodeGenType("EvalRun")] public partial class InternalEvalRun {}
[CodeGenType("EvalRunDataContentSource")] public partial class InternalEvalRunDataContentSource {}
[CodeGenType("EvalRunDataContentSourceType")] public readonly partial struct InternalEvalRunDataContentSourceType {}
[CodeGenType("EvalRunDataSourceParams")] public partial class InternalEvalRunDataSourceParams {}
[CodeGenType("EvalRunDataSourceResource")] public partial class InternalEvalRunDataSourceResource {}
[CodeGenType("EvalRunDataSourceType")] public readonly partial struct InternalEvalRunDataSourceType {}
[CodeGenType("EvalRunFileContentDataContentSource")] public partial class InternalEvalRunFileContentDataContentSource {}
[CodeGenType("EvalRunFileContentDataContentSourceContent")] public partial class InternalEvalRunFileContentDataContentSourceContent {}
[CodeGenType("EvalRunFileIdDataContentSource")] public partial class InternalEvalRunFileIdDataContentSource {}
[CodeGenType("EvalRunList")] public partial class InternalEvalRunList {}
[CodeGenType("EvalRunListObject")] public readonly partial struct InternalEvalRunListObject {}
[CodeGenType("EvalRunObject")] public readonly partial struct InternalEvalRunObject {}
[CodeGenType("EvalRunOutputItem")] public partial class InternalEvalRunOutputItem {}
[CodeGenType("EvalRunOutputItemList")] public partial class InternalEvalRunOutputItemList {}
[CodeGenType("EvalRunOutputItemListObject")] public readonly partial struct InternalEvalRunOutputItemListObject {}
[CodeGenType("EvalRunOutputItemObject")] public readonly partial struct InternalEvalRunOutputItemObject {}
[CodeGenType("EvalRunOutputItemSample")] public partial class InternalEvalRunOutputItemSample {}
[CodeGenType("EvalRunOutputItemSampleInput")] public partial class InternalEvalRunOutputItemSampleInput {}
[CodeGenType("EvalRunOutputItemSampleOutput")] public partial class InternalEvalRunOutputItemSampleOutput {}
[CodeGenType("EvalRunOutputItemSampleUsage")] public partial class InternalEvalRunOutputItemSampleUsage {}
[CodeGenType("EvalRunPerModelUsage")] public partial class InternalEvalRunPerModelUsage {}
[CodeGenType("EvalRunPerTestingCriteriaResult")] public partial class InternalEvalRunPerTestingCriteriaResult {}
[CodeGenType("EvalRunResponsesDataContentSource")] public partial class InternalEvalRunResponsesDataContentSource {}
[CodeGenType("EvalRunResultCounts")] public partial class InternalEvalRunResultCounts {}
[CodeGenType("EvalRunStoredCompletionsDataContentSource")] public partial class InternalEvalRunStoredCompletionsDataContentSource {}
[CodeGenType("EvalScoreModelGraderParams")] public partial class InternalEvalScoreModelGraderParams {}
[CodeGenType("EvalScoreModelGraderResource")] public partial class InternalEvalScoreModelGraderResource {}
[CodeGenType("EvalsErrorNotFoundResponse")] public partial class InternalEvalsErrorNotFoundResponse {}
[CodeGenType("EvalsErrorResponse")] public partial class InternalEvalsErrorResponse {}
[CodeGenType("EvalStoredCompletionsDataSourceConfigResource")] public partial class InternalEvalStoredCompletionsDataSourceConfigResource {}
[CodeGenType("EvalStringCheckGraderParams")] public partial class InternalEvalStringCheckGraderParams {}
[CodeGenType("EvalStringCheckGraderResource")] public partial class InternalEvalStringCheckGraderResource {}
[CodeGenType("EvalStringCheckGraderResourceOperation")] public readonly partial struct InternalEvalStringCheckGraderResourceOperation {}
[CodeGenType("EvalTextSimilarityGraderParams")] public partial class InternalEvalTextSimilarityGraderParams {}
[CodeGenType("EvalTextSimilarityGraderResource")] public partial class InternalEvalTextSimilarityGraderResource {}
[CodeGenType("EvalTextSimilarityGraderResourceEvaluationMetric")] public readonly partial struct InternalEvalTextSimilarityGraderResourceEvaluationMetric {}
[CodeGenType("GetEvalRunOutputItemsRequestStatus")] public readonly partial struct InternalGetEvalRunOutputItemsRequestStatus {}
[CodeGenType("GetEvalRunsRequestStatus")] public readonly partial struct InternalGetEvalRunsRequestStatus {}
[CodeGenType("ListEvalsRequestOrderBy")] public readonly partial struct InternalListEvalsRequestOrderBy {}
[CodeGenType("MetadataPropertyForRequest")] public partial class InternalMetadataPropertyForRequest
{
    public static implicit operator BinaryContent(InternalMetadataPropertyForRequest internalMetadataPropertyForRequest)
    {
        if (internalMetadataPropertyForRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalMetadataPropertyForRequest, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("UnknownEvalDataSourceConfigParams")] public partial class InternalUnknownEvalDataSourceConfigParams {}
[CodeGenType("UnknownEvalDataSourceConfigResource")] public partial class InternalUnknownEvalDataSourceConfigResource {}
[CodeGenType("UnknownEvalGraderParams")] public partial class InternalUnknownEvalGraderParams {}
[CodeGenType("UnknownEvalGraderResource")] public partial class InternalUnknownEvalGraderResource {}
[CodeGenType("UnknownEvalItemContent")] public partial class InternalUnknownEvalItemContent {}
[CodeGenType("UnknownEvalRunDataContentSource")] public partial class InternalUnknownEvalRunDataContentSource {}
[CodeGenType("UnknownEvalRunDataSourceParams")] public partial class InternalUnknownEvalRunDataSourceParams {}
[CodeGenType("UpdateEvalRequest")] public partial class InternalUpdateEvalRequest {}
[CodeGenType("EvalLogsDataSourceConfigResource")] public partial class InternalEvalLogsDataSourceConfigResource {}
[CodeGenType("EvalGraderLabelModelResource")] public partial class InternalEvalGraderLabelModelResource {}
[CodeGenType("EvalGraderTextSimilarityResource")] public partial class InternalEvalGraderTextSimilarityResource {}
[CodeGenType("EvalGraderPythonResource")] public partial class InternalEvalGraderPythonResource {}
[CodeGenType("EvalGraderScoreModelResource")] public partial class InternalEvalGraderScoreModelResource {}
[CodeGenType("EvalStoredCompletionsDataSourceConfigParams")] public partial class InternalEvalStoredCompletionsDataSourceConfigParams {}
[CodeGenType("EvalGraderLabelModelParams")] public partial class InternalEvalGraderLabelModelParams {}
[CodeGenType("EvalGraderLabelModelParamsInput")] public partial class InternalEvalGraderLabelModelParamsInput {}
[CodeGenType("EvalGraderStringCheckParams")] public partial class InternalEvalGraderStringCheckParams {}
[CodeGenType("EvalGraderStringCheckParamsOperation")] public readonly partial struct InternalEvalGraderStringCheckParamsOperation {}
[CodeGenType("EvalGraderTextSimilarityParams")] public partial class InternalEvalGraderTextSimilarityParams {}
[CodeGenType("EvalGraderPythonParams")] public partial class InternalEvalGraderPythonParams {}
[CodeGenType("EvalGraderScoreModelParams")] public partial class InternalEvalGraderScoreModelParams {}
[CodeGenType("EvalResponsesRunDataSourceParamsSamplingParamsText")] public partial class InternalEvalResponsesRunDataSourceParamsSamplingParamsText {}