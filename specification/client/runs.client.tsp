import "../base/typespec/runs/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@access(RunStepDeltaObject, Access.public);
@@usage(RunStepDeltaObject, Usage.output);

@@access(RunStepDeltaStepDetailsMessageCreationObject, Access.public);
@@usage(RunStepDeltaStepDetailsMessageCreationObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsCodeObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsCodeObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsFileSearchObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsFileSearchObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsFunctionObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsFunctionObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsCodeOutputImageObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsCodeOutputImageObject, Usage.output);

@@access(RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject, Access.public);
@@usage(RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject, Usage.output);

@@visibility(RunStepObject.metadata, Lifecycle.Read);

@@visibility(RunObject.metadata, Lifecycle.Read);
@@visibility(RunObject.tools, Lifecycle.Read);

@@visibility(RunStepDetailsToolCallsFileSearchResultObject.content, Lifecycle.Read);

@@visibility(ListRunStepsResponse.data, Lifecycle.Read);
@@visibility(ListRunsResponse.data, Lifecycle.Read);
