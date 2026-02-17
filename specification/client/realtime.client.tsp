import "../base/typespec/realtime/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@alternateType(MCPTool_GA.server_url, url);
@@alternateType(MCPTool_GA.allowed_tools, MCPToolFilter_GA | null);
@@clientName(MCPTool_GA.server_url, "ServerUri");
@@clientName(MCPTool_GA.authorization, "AuthorizationToken");
@@clientName(MCPToolFilter_GA.read_only, "IsReadOnly");



// TEMP
@@access(RealtimeCreateClientSecretResponse, Access.public);
@@usage(RealtimeCreateClientSecretResponse, Usage.output);


// ------------ JsonPatch ------------

// models.tsp
@@dynamicModel(RealtimeCreateClientSecretRequest_GA);
@@dynamicModel(RealtimeCreateClientSecretResponse_GA);

@@dynamicModel(DotNetRealtimeSessionCreateRequestGADefaultTracing_GA);
@@dynamicModel(DotNetRealtimeTracing_GA);

@@dynamicModel(DotNetRealtimeDefaultTruncation_GA);
@@dynamicModel(DotNetRealtimeTruncation_GA);
@@dynamicModel(RealtimeTruncationRetentionRatio_GA);

@@dynamicModel(DotNetRealtimeDefaultToolChoice_GA);
@@dynamicModel(DotNetRealtimeToolChoice_GA);
@@dynamicModel(ToolChoiceFunction_GA);
@@dynamicModel(ToolChoiceMCP_GA);

@@dynamicModel(DotNetRealtimeDefaultToolCallApprovalPolicy_GA);
@@dynamicModel(DotNetRealtimeToolCallApprovalPolicy_GA);

@@dynamicModel(RealtimeFunctionTool_GA);
@@dynamicModel(MCPTool_GA);

@@dynamicModel(RealtimeSessionCreateRequestGA_GA);
@@dynamicModel(RealtimeTranscriptionSessionCreateRequestGA_GA);

@@dynamicModel(RealtimeSessionCreateResponseGA_GA);
@@dynamicModel(RealtimeTranscriptionSessionCreateResponseGA_GA);

@@dynamicModel(RealtimeAudioFormatsAudioPcm_GA);
@@dynamicModel(RealtimeAudioFormatsAudioPcmu_GA);
@@dynamicModel(RealtimeAudioFormatsAudioPcma_GA);