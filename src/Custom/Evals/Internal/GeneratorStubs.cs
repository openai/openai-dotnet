using OpenAI.Files;
using System.ClientModel;

namespace OpenAI.Evals;

[CodeGenType("CreateEvalRequest")] internal partial class InternalCreateEvalRequest {}
[CodeGenType("CreateEvalRunRequest")] internal partial class InternalCreateEvalRunRequest {}
[CodeGenType("DeleteEvalResponse")] internal partial class InternalDeleteEvalResponse {}
[CodeGenType("DeleteEvalResponseObject")] internal readonly partial struct InternalDeleteEvalResponseObject {}
[CodeGenType("DeleteEvalRunResponse")] internal partial class InternalDeleteEvalRunResponse {}
[CodeGenType("DeleteEvalRunResponseObject")] internal readonly partial struct InternalDeleteEvalRunResponseObject {}
[CodeGenType("Eval")] internal partial class InternalEval {}
[CodeGenType("EvalApiError")] internal partial class InternalEvalApiError {}
[CodeGenType("EvalCompletionsRunDataSourceParams")] internal partial class InternalEvalCompletionsRunDataSourceParams {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages1")] internal partial class InternalEvalCompletionsRunDataSourceParamsInputMessages1 {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages1Type")] internal readonly partial struct InternalEvalCompletionsRunDataSourceParamsInputMessages1Type {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages2")] internal partial class InternalEvalCompletionsRunDataSourceParamsInputMessages2 {}
[CodeGenType("EvalCompletionsRunDataSourceParamsInputMessages2Type")] internal readonly partial struct InternalEvalCompletionsRunDataSourceParamsInputMessages2Type {}
[CodeGenType("EvalCompletionsRunDataSourceParamsSamplingParams")] internal partial class InternalEvalCompletionsRunDataSourceParamsSamplingParams {}
[CodeGenType("EvalCustomDataSourceConfigParams")] internal partial class InternalEvalCustomDataSourceConfigParams {}
[CodeGenType("EvalCustomDataSourceConfigResource")] internal partial class InternalEvalCustomDataSourceConfigResource {}
[CodeGenType("EvalDataSourceConfigParams")] internal partial class InternalEvalDataSourceConfigParams {}
[CodeGenType("EvalDataSourceConfigResource")] internal partial class InternalEvalDataSourceConfigResource {}
[CodeGenType("EvalDataSourceConfigType")] internal readonly partial struct InternalEvalDataSourceConfigType {}
[CodeGenType("EvalGraderParams")] internal partial class InternalEvalGraderParams {}
[CodeGenType("EvalGraderResource")] internal partial class InternalEvalGraderResource {}
[CodeGenType("EvalGraderType")] internal readonly partial struct InternalEvalGraderType {}
[CodeGenType("EvalItem")] internal partial class InternalEvalItem {}
[CodeGenType("EvalItemContent")] internal partial class InternalEvalItemContent {}
[CodeGenType("EvalItemContentInputText")] internal partial class InternalEvalItemContentInputText {}
[CodeGenType("EvalItemContentOutputText")] internal partial class InternalEvalItemContentOutputText {}
[CodeGenType("EvalItemContentType")] internal readonly partial struct InternalEvalItemContentType {}
[CodeGenType("EvalItemRole")] internal readonly partial struct InternalEvalItemRole {}
[CodeGenType("EvalItemType")] internal readonly partial struct InternalEvalItemType {}
[CodeGenType("EvalJsonlRunDataSourceParams")] internal partial class InternalEvalJsonlRunDataSourceParams {}
[CodeGenType("EvalLabelModelGraderParams")] internal partial class InternalEvalLabelModelGraderParams {}
[CodeGenType("EvalLabelModelGraderParamsInput")] internal partial class InternalEvalLabelModelGraderParamsInput {}
[CodeGenType("EvalLabelModelGraderResource")] internal partial class InternalEvalLabelModelGraderResource {}
[CodeGenType("EvalList")] internal partial class InternalEvalList {}
[CodeGenType("EvalListObject")] internal readonly partial struct InternalEvalListObject {}
[CodeGenType("EvalLogsDataSourceConfigParams")] internal partial class InternalEvalLogsDataSourceConfigParams {}
[CodeGenType("EvalObject")] internal readonly partial struct InternalEvalObject {}
[CodeGenType("EvalPythonGraderParams")] internal partial class InternalEvalPythonGraderParams {}
[CodeGenType("EvalPythonGraderResource")] internal partial class InternalEvalPythonGraderResource {}
[CodeGenType("EvalResponsesRunDataSourceParams")] internal partial class InternalEvalResponsesRunDataSourceParams {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages1")] internal partial class InternalEvalResponsesRunDataSourceParamsInputMessages1 {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages1Type")] internal readonly partial struct InternalEvalResponsesRunDataSourceParamsInputMessages1Type {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages2")] internal partial class InternalEvalResponsesRunDataSourceParamsInputMessages2 {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessages2Type")] internal readonly partial struct InternalEvalResponsesRunDataSourceParamsInputMessages2Type {}
[CodeGenType("EvalResponsesRunDataSourceParamsInputMessagesTemplate1")] internal partial class InternalEvalResponsesRunDataSourceParamsInputMessagesTemplate1 {}
[CodeGenType("EvalResponsesRunDataSourceParamsSamplingParams")] internal partial class InternalEvalResponsesRunDataSourceParamsSamplingParams {}
[CodeGenType("EvalRun")] internal partial class InternalEvalRun {}
[CodeGenType("EvalRunDataContentSource")] internal partial class InternalEvalRunDataContentSource {}
[CodeGenType("EvalRunDataContentSourceType")] internal readonly partial struct InternalEvalRunDataContentSourceType {}
[CodeGenType("EvalRunDataSourceParams")] internal partial class InternalEvalRunDataSourceParams {}
[CodeGenType("EvalRunDataSourceResource")] internal partial class InternalEvalRunDataSourceResource {}
[CodeGenType("EvalRunDataSourceType")] internal readonly partial struct InternalEvalRunDataSourceType {}
[CodeGenType("EvalRunFileContentDataContentSource")] internal partial class InternalEvalRunFileContentDataContentSource {}
[CodeGenType("EvalRunFileContentDataContentSourceContent")] internal partial class InternalEvalRunFileContentDataContentSourceContent {}
[CodeGenType("EvalRunFileIdDataContentSource")] internal partial class InternalEvalRunFileIdDataContentSource {}
[CodeGenType("EvalRunList")] internal partial class InternalEvalRunList {}
[CodeGenType("EvalRunListObject")] internal readonly partial struct InternalEvalRunListObject {}
[CodeGenType("EvalRunObject")] internal readonly partial struct InternalEvalRunObject {}
[CodeGenType("EvalRunOutputItem")] internal partial class InternalEvalRunOutputItem {}
[CodeGenType("EvalRunOutputItemList")] internal partial class InternalEvalRunOutputItemList {}
[CodeGenType("EvalRunOutputItemListObject")] internal readonly partial struct InternalEvalRunOutputItemListObject {}
[CodeGenType("EvalRunOutputItemObject")] internal readonly partial struct InternalEvalRunOutputItemObject {}
[CodeGenType("EvalRunOutputItemSample")] internal partial class InternalEvalRunOutputItemSample {}
[CodeGenType("EvalRunOutputItemSampleInput")] internal partial class InternalEvalRunOutputItemSampleInput {}
[CodeGenType("EvalRunOutputItemSampleOutput")] internal partial class InternalEvalRunOutputItemSampleOutput {}
[CodeGenType("EvalRunOutputItemSampleUsage")] internal partial class InternalEvalRunOutputItemSampleUsage {}
[CodeGenType("EvalRunPerModelUsage")] internal partial class InternalEvalRunPerModelUsage {}
[CodeGenType("EvalRunPerTestingCriteriaResult")] internal partial class InternalEvalRunPerTestingCriteriaResult {}
[CodeGenType("EvalRunResponsesDataContentSource")] internal partial class InternalEvalRunResponsesDataContentSource {}
[CodeGenType("EvalRunResultCounts")] internal partial class InternalEvalRunResultCounts {}
[CodeGenType("EvalRunStoredCompletionsDataContentSource")] internal partial class InternalEvalRunStoredCompletionsDataContentSource {}
[CodeGenType("EvalScoreModelGraderParams")] internal partial class InternalEvalScoreModelGraderParams {}
[CodeGenType("EvalScoreModelGraderResource")] internal partial class InternalEvalScoreModelGraderResource {}
[CodeGenType("EvalsErrorNotFoundResponse")] internal partial class InternalEvalsErrorNotFoundResponse {}
[CodeGenType("EvalsErrorResponse")] internal partial class InternalEvalsErrorResponse {}
[CodeGenType("EvalStoredCompletionsDataSourceConfigResource")] internal partial class InternalEvalStoredCompletionsDataSourceConfigResource {}
[CodeGenType("EvalStringCheckGraderParams")] internal partial class InternalEvalStringCheckGraderParams {}
[CodeGenType("EvalStringCheckGraderResource")] internal partial class InternalEvalStringCheckGraderResource {}
[CodeGenType("EvalStringCheckGraderResourceOperation")] internal readonly partial struct InternalEvalStringCheckGraderResourceOperation {}
[CodeGenType("EvalTextSimilarityGraderParams")] internal partial class InternalEvalTextSimilarityGraderParams {}
[CodeGenType("EvalTextSimilarityGraderResource")] internal partial class InternalEvalTextSimilarityGraderResource {}
[CodeGenType("EvalTextSimilarityGraderResourceEvaluationMetric")] internal readonly partial struct InternalEvalTextSimilarityGraderResourceEvaluationMetric {}
[CodeGenType("GetEvalRunOutputItemsRequestStatus")] internal readonly partial struct InternalGetEvalRunOutputItemsRequestStatus {}
[CodeGenType("GetEvalRunsRequestStatus")] internal readonly partial struct InternalGetEvalRunsRequestStatus {}
[CodeGenType("ListEvalsRequestOrderBy")] internal readonly partial struct InternalListEvalsRequestOrderBy {}
[CodeGenType("MetadataPropertyForRequest")] internal partial class InternalMetadataPropertyForRequest
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
[CodeGenType("UnknownEvalDataSourceConfigParams")] internal partial class InternalUnknownEvalDataSourceConfigParams {}
[CodeGenType("UnknownEvalDataSourceConfigResource")] internal partial class InternalUnknownEvalDataSourceConfigResource {}
[CodeGenType("UnknownEvalGraderParams")] internal partial class InternalUnknownEvalGraderParams {}
[CodeGenType("UnknownEvalGraderResource")] internal partial class InternalUnknownEvalGraderResource {}
[CodeGenType("UnknownEvalItemContent")] internal partial class InternalUnknownEvalItemContent {}
[CodeGenType("UnknownEvalRunDataContentSource")] internal partial class InternalUnknownEvalRunDataContentSource {}
[CodeGenType("UnknownEvalRunDataSourceParams")] internal partial class InternalUnknownEvalRunDataSourceParams {}
[CodeGenType("UpdateEvalRequest")] internal partial class InternalUpdateEvalRequest {}
[CodeGenType("EvalLogsDataSourceConfigResource")] internal partial class InternalEvalLogsDataSourceConfigResource {}
[CodeGenType("EvalGraderLabelModelResource")] internal partial class InternalEvalGraderLabelModelResource {}
[CodeGenType("EvalGraderTextSimilarityResource")] internal partial class InternalEvalGraderTextSimilarityResource {}
[CodeGenType("EvalGraderPythonResource")] internal partial class InternalEvalGraderPythonResource {}
[CodeGenType("EvalGraderScoreModelResource")] internal partial class InternalEvalGraderScoreModelResource {}
[CodeGenType("EvalStoredCompletionsDataSourceConfigParams")] internal partial class InternalEvalStoredCompletionsDataSourceConfigParams {}
[CodeGenType("EvalGraderLabelModelParams")] internal partial class InternalEvalGraderLabelModelParams {}
[CodeGenType("EvalGraderLabelModelParamsInput")] internal partial class InternalEvalGraderLabelModelParamsInput {}
[CodeGenType("EvalGraderStringCheckParams")] internal partial class InternalEvalGraderStringCheckParams {}
[CodeGenType("EvalGraderStringCheckParamsOperation")] internal readonly partial struct InternalEvalGraderStringCheckParamsOperation {}
[CodeGenType("EvalGraderTextSimilarityParams")] internal partial class InternalEvalGraderTextSimilarityParams {}
[CodeGenType("EvalGraderPythonParams")] internal partial class InternalEvalGraderPythonParams {}
[CodeGenType("EvalGraderScoreModelParams")] internal partial class InternalEvalGraderScoreModelParams {}
[CodeGenType("EvalResponsesRunDataSourceParamsSamplingParamsText")] internal partial class InternalEvalResponsesRunDataSourceParamsSamplingParamsText {}