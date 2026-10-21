# OpenAI REST API — Spec Diff Report

| | |
|---|---|
| **New spec** | v2.3.0 (2026-08-06T23:54:47Z) |
| **Previous spec** | v2.3.0 (2026-08-06T23:52:10Z) |

> **Legend**: `+` added · `-` removed · `▲` changed · `↔` renamed

> **Scope**: this is a structural comparison, not a complete semantic one. It
> covers paths, operations, schemas, properties, required flags, types, enum values,
> and the common schema constraints. Serialization details such as parameter `style`
> and `explode`, request-body encodings, response headers, callbacks, and links are
> not compared. Treat a clean report as "nothing structural moved," not as proof that
> nothing at all did.

> **Heuristics**: renames, possible duplicates, and anomalies are suggestions produced
> by name and shape similarity. They are review prompts, not established facts, and
> should be confirmed by a human before anything is based on them.

---

## Where this came from

| | |
|---|---|
| **Source** | `https://raw.githubusercontent.com/openai/openai-openapi/dc708bbe9a149bc35132c567ef3a3fdd7a24ab49/openapi.yaml` |
| **Upstream commit** | `dc708bbe9a149bc35132c567ef3a3fdd7a24ab49` |
| **Source content hash** | `ab0c5306e390c64efbf50bbf71f02aa0dad2dafcaa96066a592186daa6103b87` |
| **Previous content hash** | `ab0c5306e390c64efbf50bbf71f02aa0dad2dafcaa96066a592186daa6103b87` |
| **Feature specifications** | 24 |
| **Source repairs** | 1 line(s) repaired before parsing |
| **Metadata schema** | v1 |
| **Comparison scope** | v1 |

Every figure here is written from the same record as `.spec-metadata.json`, so the
report and the metadata cannot disagree about what was processed.

---

## UNASSIGNED paths (1)

These paths matched no feature area, so they appear in no feature specification
below. This usually means upstream added a path or a tag the split does not know
about, and the feature map needs a reviewed update.

`new` means the previous snapshot had no such gap and this one does, which is the case
that needs a decision. `unchanged` is a gap that was already known, listed so it stays
visible without reading as a fresh regression.

| Status | Path | Methods | Operations | Tags | Why |
|---|---|---|---|---|---|
| unchanged | `/content_provenance_checks` | POST | `Createcontentprovenancecheck` | untagged | The path carries no tags, and no feature area claims its prefix. |

---

## Summary

| Feature | Paths +/- | Ops +/-/▲ | Schemas +/-/▲/↔ | Total Changes |
|---------|:---------:|:---------:|:----------------:|:-------------:|
| [Responses](#responses) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Conversations](#conversations) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Chat](#chat) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Audio](#audio) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Videos](#videos) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Images](#images) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Embeddings](#embeddings) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Evals](#evals) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Graders](#graders) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Fine Tuning](#fine-tuning) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Batch](#batch) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Files](#files) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Uploads](#uploads) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Models](#models) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Moderations](#moderations) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Vector Stores](#vector-stores) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Containers](#containers) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Skills](#skills) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Realtime](#realtime) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Assistants](#assistants) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Messages](#messages) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Runs](#runs) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Threads](#threads) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |
| [Administration](#administration) | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲0 | 0 |

**Total changes across all features**: 0

---

## Responses

<details><summary>0 changes · `responses.yml`</summary>

<details><summary>Structurally Equivalent Schemas (93)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **ApplyPatchCreateFileOperation**, **ApplyPatchCreateFileOperationParam**, **ApplyPatchUpdateFileOperation**, **ApplyPatchUpdateFileOperationParam**, **BetaApplyPatchCreateFileOperation**, **BetaApplyPatchCreateFileOperationParam**, **BetaApplyPatchUpdateFileOperation**, **BetaApplyPatchUpdateFileOperationParam**
  Signature: `object{diff*:string,path*:string,type*:string enum}`

- **ApplyPatchDeleteFileOperation**, **ApplyPatchDeleteFileOperationParam**, **BetaApplyPatchDeleteFileOperation**, **BetaApplyPatchDeleteFileOperationParam**
  Signature: `object{path*:string,type*:string enum}`

- **ApplyPatchToolParam**, **BetaApplyPatchToolParam**
  Signature: `object{allowed_callers:anyOf(array | null),type*:string enum}`

- **ApproximateLocation**, **BetaApproximateLocation**
  Signature: `object{city:anyOf(string | null),country:anyOf(string | null),region:anyOf(string | null),timezone:anyOf(string | null),type*:string enum}`

- **BetaClickButtonType**, **ClickButtonType**
  Signature: `string enum[back,forward,left,right,wheel]`

- **BetaCodeInterpreterOutputImage**, **CodeInterpreterOutputImage**
  Signature: `object{type*:string enum,url*:string(uri)}`

- **BetaCodeInterpreterOutputLogs**, **CodeInterpreterOutputLogs**
  Signature: `object{logs*:string,type*:string enum}`

- **BetaComparisonFilter**, **ComparisonFilter**
  Signature: `object{key*:string,type*:string enum,value*:oneOf(string | number | boolean | array)}`

- **BetaCompoundFilter**, **CompoundFilter**
  Signature: `object{filters*:array,type*:string enum}`

- **BetaComputerCallOutputStatus**, **ComputerCallOutputStatus**
  Signature: `string enum[completed,failed,incomplete]`

- **BetaComputerCallSafetyCheckParam**, **ComputerCallSafetyCheckParam**
  Signature: `object{code:anyOf(string | null),id*:string,message:anyOf(string | null)}`

- **BetaComputerEnvironment**, **ComputerEnvironment**
  Signature: `string enum[browser,linux,mac,ubuntu,windows]`

- **BetaComputerScreenshotImage**, **ComputerScreenshotImage**
  Signature: `object{file_id:string,image_url:string(uri),type*:string enum}`

- **BetaContainerFileCitationBody**, **BetaContainerFileCitationParam**, **ContainerFileCitationBody**
  Signature: `object{container_id*:string,end_index*:integer,file_id*:string,filename*:string,start_index*:integer,type*:string enum}`

- **BetaContainerMemoryLimit**, **ContainerMemoryLimit**
  Signature: `string enum[16g,1g,4g,64g]`

- **BetaContainerNetworkPolicyDomainSecretParam**, **ContainerNetworkPolicyDomainSecretParam**
  Signature: `object{domain*:string,name*:string,value*:string}`

- **BetaContainerReferenceParam**, **BetaContainerReferenceResource**, **ContainerReferenceParam**, **ContainerReferenceResource**
  Signature: `object{container_id*:string,type*:string enum}`

- **BetaContextManagementParam**, **ContextManagementParam**
  Signature: `object{compact_threshold:anyOf(integer | null),type*:string}`

- **BetaCoordParam**, **CoordParam**
  Signature: `object{x*:integer,y*:integer}`

- **BetaDetailEnum**, **BetaImageDetail**, **DetailEnum**, **ImageDetail**
  Signature: `string enum[auto,high,low,original]`

- **BetaDoubleClickAction**, **DoubleClickAction**
  Signature: `object{keys*:anyOf(array | null),type*:string enum,x*:integer,y*:integer}`

- **BetaEncryptedContent**, **BetaEncryptedContentParam**
  Signature: `object{encrypted_content*:string,type*:string enum}`

- **BetaError**, **Error**
  Signature: `object{code*:anyOf(string | null),message*:string,param*:anyOf(string | null),type*:string}`

- **BetaFileCitationBody**, **BetaFileCitationParam**, **FileCitationBody**
  Signature: `object{file_id*:string,filename*:string,index*:integer,type*:string enum}`

- **BetaFileDetailEnum**, **BetaFileInputDetail**, **FileDetailEnum**, **FileInputDetail**
  Signature: `string enum[auto,high,low]`

- **BetaFilePath**, **FilePath**
  Signature: `object{file_id*:string,index*:integer,type*:string enum}`

- **BetaFunctionAndCustomToolCallOutput**, **BetaInputContent**
  Signature: `oneOf(BetaInputFileContent,BetaInputImageContent,BetaInputTextContent)`

- **BetaFunctionCallItemStatus**, **BetaFunctionCallOutputStatusEnum**, **BetaFunctionCallStatus**, **BetaFunctionShellCallItemStatus**, **BetaFunctionShellCallOutputStatusEnum**, **BetaFunctionShellCallStatus**, **BetaMessageStatus**, **FunctionCallItemStatus**, **FunctionCallOutputStatusEnum**, **FunctionCallStatus**, **FunctionShellCallItemStatus**, **FunctionShellCallOutputStatusEnum**, **FunctionShellCallStatus**, **MessageStatus**
  Signature: `string enum[completed,in_progress,incomplete]`

- **BetaFunctionShellAction**, **FunctionShellAction**
  Signature: `object{commands*:array<string>,max_output_length*:anyOf(integer | null),timeout_ms*:anyOf(integer | null)}`

- **BetaFunctionShellActionParam**, **FunctionShellActionParam**
  Signature: `object{commands*:array<string>,max_output_length:anyOf(integer | null),timeout_ms:anyOf(integer | null)}`

- **BetaFunctionShellCallOutputExitOutcome**, **BetaFunctionShellCallOutputExitOutcomeParam**, **FunctionShellCallOutputExitOutcome**, **FunctionShellCallOutputExitOutcomeParam**
  Signature: `object{exit_code*:integer,type*:string enum}`

- **BetaFunctionShellToolParam**, **FunctionShellToolParam**
  Signature: `object{allowed_callers:anyOf(array | null),environment:anyOf(inline | null),type*:string enum}`

- **BetaFunctionTool**, **FunctionTool**
  Signature: `object{allowed_callers:anyOf(array | null),defer_loading:boolean,description:anyOf(string | null),name*:string,output_schema:anyOf(object | null),parameters*:anyOf(object | null),strict*:anyOf(boolean | null),type*:string enum}`

- **BetaHybridSearchOptions**, **HybridSearchOptions**
  Signature: `object{embedding_weight*:number,text_weight*:number}`

- **BetaImageGenActionEnum**, **ImageGenActionEnum**
  Signature: `string enum[auto,edit,generate]`

- **BetaIncludeEnum**, **IncludeEnum**
  Signature: `string enum[code_interpreter_call.outputs,computer_call_output.output.image_url,file_search_call.results,message.input_image.image_url,message.output_text.logprobs,reasoning.encrypted_content,web_search_call.action.sources,web_search_call.results]`

- **BetaInlineSkillSourceParam**, **InlineSkillSourceParam**
  Signature: `object{data*:string,media_type*:string enum,type*:string enum}`

- **BetaInputParam**, **InputParam**
  Signature: `oneOf(array,string)`

- **BetaKeyPressAction**, **KeyPressAction**
  Signature: `object{keys*:array<string>,type*:string enum}`

- **BetaLocalShellExecAction**, **LocalShellExecAction**
  Signature: `object{command*:array<string>,env*:object,timeout_ms:anyOf(integer | null),type*:string enum,user:anyOf(string | null),working_directory:anyOf(string | null)}`

- **BetaLocalSkillParam**, **LocalSkillParam**
  Signature: `object{description*:string,name*:string,path*:string}`

- **BetaMCPListToolsTool**, **MCPListToolsTool**
  Signature: `object{annotations:anyOf(object | null),description:anyOf(string | null),input_schema*:object,name*:string}`

- **BetaMCPTool**, **MCPTool**
  Signature: `object{allowed_callers:anyOf(array | null),allowed_tools:anyOf(inline | null),authorization:string,connector_id:string enum,defer_loading:boolean,headers:anyOf(object | null),require_approval:anyOf(inline | null),server_description:string,server_label*:string,server_url:string(uri),tunnel_id:string,type*:string enum}`

- **BetaMCPToolCallStatus**, **MCPToolCallStatus**
  Signature: `string enum[calling,completed,failed,in_progress,incomplete]`

- **BetaMCPToolFilter**, **MCPToolFilter**
  Signature: `object{read_only:boolean,tool_names:array<string>}`

- **BetaMessageRole**, **MessageRole**
  Signature: `string enum[assistant,critic,developer,discriminator,system,tool,unknown,user]`

- **BetaMetadata**, **BetaPrompt**, **BetaResponseError**, **BetaResponsePromptVariables**, **BetaResponseStreamOptions**, **BetaVectorStoreFileAttributes**, **BetaWebSearchApproximateLocation**, **Metadata**, **Prompt**, **ResponseError**, **ResponsePromptVariables**, **ResponseStreamOptions**, **VectorStoreFileAttributes**, **WebSearchApproximateLocation**
  Signature: `anyOf(null,object)`

- **BetaModelIdsShared**, **BetaPersonalityEnum**, **BetaReasoningModeEnum**, **ModelIdsShared**, **PersonalityEnum**, **ReasoningModeEnum**
  Signature: `anyOf(string,string)`

- **BetaModerationErrorBody**, **ModerationErrorBody**
  Signature: `object{code*:string,message*:string,type*:string enum}`

- **BetaModerationResultBody**, **ModerationResultBody**
  Signature: `object{categories*:object,category_applied_input_types*:object,category_scores*:object,flagged*:boolean,model*:string,type*:string enum}`

- **BetaMoveParam**, **MoveParam**
  Signature: `object{keys:anyOf(array | null),type*:string enum,x*:integer,y*:integer}`

- **BetaMultiAgentAction**, **BetaMultiAgentAction1**
  Signature: `string enum[followup_task,interrupt_agent,list_agents,send_message,spawn_agent,wait_agent]`

- **BetaNamespaceToolParam**, **NamespaceToolParam**
  Signature: `object{description*:string,name*:string,tools*:array,type*:string enum}`

- **BetaProgramToolCallCaller**, **BetaProgramToolCallCallerParam**, **ProgramToolCallCaller**, **ProgramToolCallCallerParam**
  Signature: `object{caller_id*:string,type*:string enum}`

- **BetaReasoningEffort**, **BetaServiceTier**, **BetaVerbosity**, **ReasoningEffort**, **ServiceTier**, **Verbosity**
  Signature: `anyOf(null,string)`

- **BetaReasoningTextContent**, **BetaSummaryTextContent**, **BetaTextContent**, **BetaTypeParam**, **ReasoningTextContent**, **SummaryTextContent**, **TextContent**, **TypeParam**
  Signature: `object{text*:string,type*:string enum}`

- **BetaRefusalContent**, **RefusalContent**
  Signature: `object{refusal*:string,type*:string enum}`

- **BetaResponseAudioDeltaEvent**, **BetaResponseAudioTranscriptDeltaEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),delta*:string,sequence_number*:integer,type*:string enum}`

- **BetaResponseAudioDoneEvent**, **BetaResponseAudioTranscriptDoneEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),sequence_number*:integer,type*:string enum}`

- **BetaResponseCodeInterpreterCallCodeDeltaEvent**, **BetaResponseCustomToolCallInputDeltaEvent**, **BetaResponseFunctionCallArgumentsDeltaEvent**, **BetaResponseMCPCallArgumentsDeltaEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),delta*:string,item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **BetaResponseCodeInterpreterCallCompletedEvent**, **BetaResponseCodeInterpreterCallInProgressEvent**, **BetaResponseCodeInterpreterCallInterpretingEvent**, **BetaResponseFileSearchCallCompletedEvent**, **BetaResponseFileSearchCallInProgressEvent**, **BetaResponseFileSearchCallSearchingEvent**, **BetaResponseImageGenCallCompletedEvent**, **BetaResponseImageGenCallGeneratingEvent**, **BetaResponseImageGenCallInProgressEvent**, **BetaResponseMCPCallCompletedEvent**, **BetaResponseMCPCallFailedEvent**, **BetaResponseMCPCallInProgressEvent**, **BetaResponseMCPListToolsCompletedEvent**, **BetaResponseMCPListToolsFailedEvent**, **BetaResponseMCPListToolsInProgressEvent**, **BetaResponseWebSearchCallCompletedEvent**, **BetaResponseWebSearchCallInProgressEvent**, **BetaResponseWebSearchCallSearchingEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **BetaResponseCompletedEvent**, **BetaResponseCreatedEvent**, **BetaResponseFailedEvent**, **BetaResponseInProgressEvent**, **BetaResponseIncompleteEvent**, **BetaResponseQueuedEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),response*:BetaResponse,sequence_number*:integer,type*:string enum}`

- **BetaResponseContentPartAddedEvent**, **BetaResponseContentPartDoneEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),content_index*:integer,item_id*:string,output_index*:integer,part*:BetaOutputContent,sequence_number*:integer,type*:string enum}`

- **BetaResponseErrorCode**, **ResponseErrorCode**
  Signature: `string enum[bio_policy,data_residency_mismatch,empty_image_file,failed_to_download_image,image_content_policy_violation,image_file_not_found,image_file_too_large,image_parse_error,image_too_large,image_too_small,invalid_base64_image,invalid_image,invalid_image_format,invalid_image_mode,invalid_image_url,invalid_prompt,rate_limit_exceeded,server_error,unsupported_image_media_type,vector_store_timeout]`

- **BetaResponseFormatJsonSchemaSchema**, **ResponseFormatJsonSchemaSchema**
  Signature: `object`

- **BetaResponseLogProb**, **ResponseLogProb**
  Signature: `object{logprob*:number,token*:string,top_logprobs:array<object>}`

- **BetaResponseOutputItemAddedEvent**, **BetaResponseOutputItemDoneEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),item*:BetaOutputItem,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **BetaResponseReasoningTextDeltaEvent**, **BetaResponseRefusalDeltaEvent**
  Signature: `object{agent:anyOf(BetaAgentTag | null),content_index*:integer,delta*:string,item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **BetaResponseUsage**, **ResponseUsage**
  Signature: `object{input_tokens*:integer,input_tokens_details*:object,output_tokens*:integer,output_tokens_details*:object,total_tokens*:integer}`

- **BetaScrollParam**, **ScrollParam**
  Signature: `object{keys:anyOf(array | null),scroll_x*:integer,scroll_y*:integer,type*:string enum,x*:integer,y*:integer}`

- **BetaSearchContextSize**, **SearchContextSize**
  Signature: `string enum[high,low,medium]`

- **BetaServiceTierEnum**, **ServiceTierEnum**
  Signature: `string enum[auto,default,fast,flex,priority]`

- **BetaSkillReferenceParam**, **SkillReferenceParam**
  Signature: `object{skill_id*:string,type*:string enum,version:string}`

- **BetaTokenCountsResource**, **TokenCountsResource**
  Signature: `object{input_tokens*:integer,object*:string enum}`

- **BetaToolChoiceAllowed**, **ToolChoiceAllowed**
  Signature: `object{mode*:string enum,tools*:array<object>,type*:string enum}`

- **BetaToolChoiceCustom**, **BetaToolChoiceFunction**, **ToolChoiceCustom**, **ToolChoiceFunction**
  Signature: `object{name*:string,type*:string enum}`

- **BetaToolChoiceMCP**, **ToolChoiceMCP**
  Signature: `object{name:anyOf(string | null),server_label*:string,type*:string enum}`

- **BetaToolChoiceOptions**, **ToolChoiceOptions**
  Signature: `string enum[auto,none,required]`

- **BetaTopLogProb**, **TopLogProb**
  Signature: `object{bytes*:array<integer>,logprob*:number,token*:string}`

- **BetaUrlCitationBody**, **BetaUrlCitationParam**, **UrlCitationBody**
  Signature: `object{end_index*:integer,start_index*:integer,title*:string,type*:string enum,url*:string(uri)}`

- **BetaWebSearchActionFind**, **WebSearchActionFind**
  Signature: `object{pattern*:string,type*:string enum,url*:string(uri)}`

- **BetaWebSearchActionOpenPage**, **WebSearchActionOpenPage**
  Signature: `object{type*:string enum,url:anyOf(string | null)}`

- **BetaWebSearchActionSearch**, **WebSearchActionSearch**
  Signature: `object{queries:array<string>,query:string,sources:array<object>,type*:string enum}`

- **FunctionAndCustomToolCallOutput**, **InputContent**
  Signature: `oneOf(InputFileContent,InputImageContent,InputTextContent)`

- **Program**, **ProgramItemParam**
  Signature: `object{call_id*:string,code*:string,fingerprint*:string,id*:string,type*:string enum}`

- **ResponseAudioDeltaEvent**, **ResponseAudioTranscriptDeltaEvent**
  Signature: `object{delta*:string,sequence_number*:integer,type*:string enum}`

- **ResponseAudioDoneEvent**, **ResponseAudioTranscriptDoneEvent**
  Signature: `object{sequence_number*:integer,type*:string enum}`

- **ResponseCodeInterpreterCallCodeDeltaEvent**, **ResponseCustomToolCallInputDeltaEvent**, **ResponseFunctionCallArgumentsDeltaEvent**, **ResponseMCPCallArgumentsDeltaEvent**
  Signature: `object{delta*:string,item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **ResponseCodeInterpreterCallCompletedEvent**, **ResponseCodeInterpreterCallInProgressEvent**, **ResponseCodeInterpreterCallInterpretingEvent**, **ResponseFileSearchCallCompletedEvent**, **ResponseFileSearchCallInProgressEvent**, **ResponseFileSearchCallSearchingEvent**, **ResponseImageGenCallCompletedEvent**, **ResponseImageGenCallGeneratingEvent**, **ResponseImageGenCallInProgressEvent**, **ResponseMCPCallCompletedEvent**, **ResponseMCPCallFailedEvent**, **ResponseMCPCallInProgressEvent**, **ResponseMCPListToolsCompletedEvent**, **ResponseMCPListToolsFailedEvent**, **ResponseMCPListToolsInProgressEvent**, **ResponseWebSearchCallCompletedEvent**, **ResponseWebSearchCallInProgressEvent**, **ResponseWebSearchCallSearchingEvent**
  Signature: `object{item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **ResponseCompletedEvent**, **ResponseCreatedEvent**, **ResponseFailedEvent**, **ResponseInProgressEvent**, **ResponseIncompleteEvent**, **ResponseQueuedEvent**
  Signature: `object{response*:Response,sequence_number*:integer,type*:string enum}`

- **ResponseContentPartAddedEvent**, **ResponseContentPartDoneEvent**
  Signature: `object{content_index*:integer,item_id*:string,output_index*:integer,part*:OutputContent,sequence_number*:integer,type*:string enum}`

- **ResponseOutputItemAddedEvent**, **ResponseOutputItemDoneEvent**
  Signature: `object{item*:OutputItem,output_index*:integer,sequence_number*:integer,type*:string enum}`

- **ResponseReasoningTextDeltaEvent**, **ResponseRefusalDeltaEvent**
  Signature: `object{content_index*:integer,delta*:string,item_id*:string,output_index*:integer,sequence_number*:integer,type*:string enum}`

</details>

<details><summary>Duplicate Operations (6)</summary>

> The following operations share identical request and/or response schema definitions.

- **createResponse** (line 19) ↔ **getResponse** (line 42)
  Shared: response: `Response`


<details><summary>Schema: <code>Response</code></summary>

```yaml
# schema: allOf
  | ModelResponseProperties
  | ResponseProperties
  | object
```

</details>

- **createResponse** (line 19) ↔ **cancelResponse** (line 113)
  Shared: response: `Response`

- **getResponse** (line 42) ↔ **cancelResponse** (line 113)
  Shared: response: `Response`

- **beta_createResponse** (line 234) ↔ **beta_getResponse** (line 271)
  Shared: response: `BetaResponse`


<details><summary>Schema: <code>BetaResponse</code></summary>

```yaml
# schema: allOf
  | BetaModelResponseProperties
  | BetaResponseProperties
  | object
```

</details>

- **beta_createResponse** (line 234) ↔ **beta_cancelResponse** (line 368)
  Shared: response: `BetaResponse`

- **beta_getResponse** (line 271) ↔ **beta_cancelResponse** (line 368)
  Shared: response: `BetaResponse`

</details>

</details>

---

## Conversations

<details><summary>0 changes · `conversations.yml`</summary>

<details><summary>Structurally Equivalent Schemas (12)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **ApplyPatchCreateFileOperation**, **ApplyPatchCreateFileOperationParam**, **ApplyPatchUpdateFileOperation**, **ApplyPatchUpdateFileOperationParam**
  Signature: `object{diff*:string,path*:string,type*:string enum}`

- **ApplyPatchDeleteFileOperation**, **ApplyPatchDeleteFileOperationParam**
  Signature: `object{path*:string,type*:string enum}`

- **ContainerReferenceParam**, **ContainerReferenceResource**
  Signature: `object{container_id*:string,type*:string enum}`

- **DetailEnum**, **ImageDetail**
  Signature: `string enum[auto,high,low,original]`

- **FileDetailEnum**, **FileInputDetail**
  Signature: `string enum[auto,high,low]`

- **FunctionAndCustomToolCallOutput**, **InputContent**
  Signature: `oneOf(InputFileContent,InputImageContent,InputTextContent)`

- **FunctionCallItemStatus**, **FunctionCallOutputStatusEnum**, **FunctionCallStatus**, **FunctionShellCallItemStatus**, **FunctionShellCallOutputStatusEnum**, **FunctionShellCallStatus**, **MessageStatus**
  Signature: `string enum[completed,in_progress,incomplete]`

- **FunctionShellCallOutputExitOutcome**, **FunctionShellCallOutputExitOutcomeParam**
  Signature: `object{exit_code*:integer,type*:string enum}`

- **Metadata**, **VectorStoreFileAttributes**, **WebSearchApproximateLocation**
  Signature: `anyOf(null,object)`

- **Program**, **ProgramItemParam**
  Signature: `object{call_id*:string,code*:string,fingerprint*:string,id*:string,type*:string enum}`

- **ProgramToolCallCaller**, **ProgramToolCallCallerParam**
  Signature: `object{caller_id*:string,type*:string enum}`

- **ReasoningTextContent**, **SummaryTextContent**, **TextContent**, **TypeParam**
  Signature: `object{text*:string,type*:string enum}`

</details>

<details><summary>Duplicate Operations (7)</summary>

> The following operations share identical request and/or response schema definitions.

- **listConversationItems** (line 66) ↔ **createConversationItems** (line 22)
  Shared: response: `ConversationItemList`


<details><summary>Schema: <code>ConversationItemList</code></summary>

```yaml
# schema: object
  object: string enum (required)
  data: array<ConversationItem> (required)
  has_more: boolean (required)
  first_id: string (required)
  last_id: string (required)
```

</details>

- **deleteConversationItem** (line 171) ↔ **createConversation** (line 199)
  Shared: response: `ConversationResource`


<details><summary>Schema: <code>ConversationResource</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  metadata: unknown (required)
  created_at: integer(unixtime) (required)
```

</details>

- **deleteConversationItem** (line 171) ↔ **getConversation** (line 218)
  Shared: response: `ConversationResource`

- **deleteConversationItem** (line 171) ↔ **updateConversation** (line 258)
  Shared: response: `ConversationResource`

- **createConversation** (line 199) ↔ **getConversation** (line 218)
  Shared: response: `ConversationResource`

- **createConversation** (line 199) ↔ **updateConversation** (line 258)
  Shared: response: `ConversationResource`

- **getConversation** (line 218) ↔ **updateConversation** (line 258)
  Shared: response: `ConversationResource`

</details>

</details>

---

## Chat

<details><summary>0 changes · `chat.yml`</summary>

<details><summary>Structurally Equivalent Schemas (8)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **ChatCompletionNamedToolChoiceCustom**, **CustomToolChatCompletions**
  Signature: `object{custom*:object,type*:string enum}`

- **ChatCompletionRequestDeveloperMessage**, **ChatCompletionRequestSystemMessage**, **ChatCompletionRequestUserMessage**
  Signature: `object{content*:oneOf(string | array),name:string,role*:string enum}`

- **ChatCompletionRequestSystemMessageContentPart**, **ChatCompletionRequestToolMessageContentPart**
  Signature: `oneOf(ChatCompletionRequestMessageContentPartText)`

- **ChatCompletionStreamOptions**, **Metadata**
  Signature: `anyOf(null,object)`

- **CreateChatCompletionResponse**, **CreateChatCompletionStreamResponse**
  Signature: `object{choices*:array<object>,created*:integer(unixtime),id*:string,model*:string,moderation:anyOf(ChatCompletionModeration | null),object*:string enum,service_tier:ServiceTier,system_fingerprint:string,usage:CompletionUsage}`

- **FunctionParameters**, **ResponseFormatJsonSchemaSchema**
  Signature: `object`

- **ModelIdsShared**, **VoiceIdsShared**
  Signature: `anyOf(string,string)`

- **ReasoningEffort**, **ServiceTier**, **Verbosity**
  Signature: `anyOf(null,string)`

</details>

<details><summary>Duplicate Operations (3)</summary>

> The following operations share identical request and/or response schema definitions.

- **createChatCompletion** (line 73) ↔ **getChatCompletion** (line 100)
  Shared: response: `CreateChatCompletionResponse`


<details><summary>Schema: <code>CreateChatCompletionResponse</code></summary>

```yaml
# schema: object
  id: string (required)
  choices: array<object> (required)
  created: integer(unixtime) (required)
  model: string (required)
  service_tier: ServiceTier
  system_fingerprint: string
  object: string enum (required)
  usage: CompletionUsage
  moderation: anyOf(ChatCompletionModeration | null)
```

</details>

- **createChatCompletion** (line 73) ↔ **updateChatCompletion** (line 120)
  Shared: response: `CreateChatCompletionResponse`

- **getChatCompletion** (line 100) ↔ **updateChatCompletion** (line 120)
  Shared: response: `CreateChatCompletionResponse`

</details>

</details>

---

## Audio

<details><summary>0 changes · `audio.yml`</summary>

<details><summary>Structurally Equivalent Schemas (1)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **TranscriptTextSegmentEvent**, **TranscriptionDiarizedSegment**
  Signature: `object{end*:number(double),id*:string,speaker*:string,start*:number(double),text*:string,type*:string enum}`

</details>

<details><summary>Duplicate Operations (3)</summary>

> The following operations share identical request and/or response schema definitions.

- **createVoiceConsent** (line 101) ↔ **getVoiceConsent** (line 156)
  Shared: response: `VoiceConsentResource`


<details><summary>Schema: <code>VoiceConsentResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: string (required)
  language: string (required)
  created_at: integer(unixtime) (required)
```

</details>

- **createVoiceConsent** (line 101) ↔ **updateVoiceConsent** (line 179)
  Shared: response: `VoiceConsentResource`

- **getVoiceConsent** (line 156) ↔ **updateVoiceConsent** (line 179)
  Shared: response: `VoiceConsentResource`

</details>

</details>

---

## Videos

_No changes detected._

## Images

<details><summary>0 changes · `images.yml`</summary>

<details><summary>Structurally Equivalent Schemas (3)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **ImageEditCompletedEvent**, **ImageGenCompletedEvent**
  Signature: `object{b64_json*:string,background*:string enum,created_at*:integer(unixtime),output_format*:string enum,quality*:string enum,size*:string enum,type*:string enum,usage*:ImagesUsage}`

- **ImageEditPartialImageEvent**, **ImageGenPartialImageEvent**
  Signature: `object{b64_json*:string,background*:string enum,created_at*:integer(unixtime),output_format*:string enum,partial_image_index*:integer,quality*:string enum,size*:string enum,type*:string enum}`

- **ImageGenInputUsageDetails**, **ImageGenOutputTokensDetails**
  Signature: `object{image_tokens*:integer,text_tokens*:integer}`

</details>

<details><summary>Duplicate Operations (3)</summary>

> The following operations share identical request and/or response schema definitions.

- **createImageEdit** (line 22) ↔ **createImage** (line 85)
  Shared: response: `ImagesResponse`


<details><summary>Schema: <code>ImagesResponse</code></summary>

```yaml
# schema: object
  created: integer(unixtime) (required)
  data: array<Image>
  background: string enum
  output_format: string enum
  size: string enum
  quality: string enum
  usage: ImageGenUsage
```

</details>

- **createImageEdit** (line 22) ↔ **createImageVariation** (line 108)
  Shared: response: `ImagesResponse`

- **createImage** (line 85) ↔ **createImageVariation** (line 108)
  Shared: response: `ImagesResponse`

</details>

</details>

---

## Embeddings

_No changes detected._

## Evals

<details><summary>0 changes · `evals.yml`</summary>

<details><summary>Structurally Equivalent Schemas (4)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **CreateEvalLogsDataSourceConfig**, **CreateEvalStoredCompletionsDataSourceConfig**
  Signature: `object{metadata:object,type*:string enum}`

- **EvalLogsDataSourceConfig**, **EvalStoredCompletionsDataSourceConfig**
  Signature: `object{metadata:Metadata,schema*:object,type*:string enum}`

- **FunctionParameters**, **ResponseFormatJsonSchemaSchema**
  Signature: `object`

- **Metadata**, **WebSearchApproximateLocation**
  Signature: `anyOf(null,object)`

</details>

<details><summary>Duplicate Operations (6)</summary>

> The following operations share identical request and/or response schema definitions.

- **createEval** (line 70) ↔ **getEval** (line 90)
  Shared: response: `Eval`


<details><summary>Schema: <code>Eval</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: string (required)
  data_source_config: oneOf(EvalCustomDataSourceConfig | EvalLogsDataSourceConfig | EvalStoredCompletionsDataSourceConfig) (required)
  testing_criteria: array (required)
  created_at: integer(unixtime) (required)
  metadata: Metadata (required)
```

</details>

- **createEval** (line 70) ↔ **updateEval** (line 110)
  Shared: response: `Eval`

- **getEval** (line 90) ↔ **updateEval** (line 110)
  Shared: response: `Eval`

- **createEvalRun** (line 239) ↔ **getEvalRun** (line 272)
  Shared: response: `EvalRun`


<details><summary>Schema: <code>EvalRun</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  eval_id: string (required)
  status: string (required)
  model: string (required)
  name: string (required)
  created_at: integer(unixtime) (required)
  report_url: string(uri) (required)
  result_counts: object (required)
  per_model_usage: array<object> (required)
  per_testing_criteria_results: array<object> (required)
  data_source: oneOf(CreateEvalJsonlRunDataSource | CreateEvalCompletionsRunDataSource | CreateEvalResponsesRunDataSource) (required)
  metadata: Metadata (required)
  error: EvalApiError (required)
```

</details>

- **createEvalRun** (line 239) ↔ **cancelEvalRun** (line 298)
  Shared: response: `EvalRun`

- **getEvalRun** (line 272) ↔ **cancelEvalRun** (line 298)
  Shared: response: `EvalRun`

</details>

<details><summary>⚠ Spec Anomalies (1)</summary>

> The following inconsistencies were detected in the spec and may indicate errors or intentional divergence.

- 🟡 **input-output-divergence** at `/evals/{eval_id}/runs.post`
  Request `CreateEvalRunRequest` and response `EvalRun` in `createEvalRun` share only 3/3 input properties. Response has 8 additional non-standard fields: eval_id, status, model, report_url, result_counts

</details>

</details>

---

## Graders

_No changes detected._

## Fine Tuning

_No changes detected._

## Batch

_No changes detected._

## Files

_No changes detected._

## Uploads

_No changes detected._

## Models

_No changes detected._

## Moderations

_No changes detected._

## Vector Stores

<details><summary>0 changes · `vector-stores.yml`</summary>

<details><summary>Structurally Equivalent Schemas (3)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **DeleteVectorStoreFileResponse**, **DeleteVectorStoreResponse**
  Signature: `object{deleted*:boolean,id*:string,object*:string enum}`

- **Metadata**, **VectorStoreFileAttributes**
  Signature: `anyOf(null,object)`

- **StaticChunkingStrategyRequestParam**, **StaticChunkingStrategyResponseParam**
  Signature: `object{static*:StaticChunkingStrategy,type*:string enum}`

</details>

<details><summary>Duplicate Operations (10)</summary>

> The following operations share identical request and/or response schema definitions.

- **createVectorStore** (line 62) ↔ **getVectorStore** (line 81)
  Shared: response: `VectorStoreObject`


<details><summary>Schema: <code>VectorStoreObject</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  created_at: integer(unixtime) (required)
  name: string (required)
  usage_bytes: integer (required)
  file_counts: object (required)
  status: string enum (required)
  expires_after: VectorStoreExpirationAfter
  expires_at: anyOf(integer | null)
  last_active_at: anyOf(integer | null) (required)
  metadata: Metadata (required)
```

</details>

- **createVectorStore** (line 62) ↔ **modifyVectorStore** (line 100)
  Shared: response: `VectorStoreObject`

- **getVectorStore** (line 81) ↔ **modifyVectorStore** (line 100)
  Shared: response: `VectorStoreObject`

- **createVectorStoreFileBatch** (line 145) ↔ **getVectorStoreFileBatch** (line 175)
  Shared: response: `VectorStoreFileBatchObject`


<details><summary>Schema: <code>VectorStoreFileBatchObject</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  created_at: integer(unixtime) (required)
  vector_store_id: string (required)
  status: string enum (required)
  file_counts: object (required)
```

</details>

- **createVectorStoreFileBatch** (line 145) ↔ **cancelVectorStoreFileBatch** (line 203)
  Shared: response: `VectorStoreFileBatchObject`

- **getVectorStoreFileBatch** (line 175) ↔ **cancelVectorStoreFileBatch** (line 203)
  Shared: response: `VectorStoreFileBatchObject`

- **listFilesInVectorStoreBatch** (line 229) ↔ **listVectorStoreFiles** (line 295)
  Shared: response: `ListVectorStoreFilesResponse`


<details><summary>Schema: <code>ListVectorStoreFilesResponse</code></summary>

```yaml
# schema: object
  object: string (required)
  data: array<VectorStoreFileObject> (required)
  first_id: string (required)
  last_id: string (required)
  has_more: boolean (required)
```

</details>

- **createVectorStoreFile** (line 354) ↔ **getVectorStoreFile** (line 384)
  Shared: response: `VectorStoreFileObject`


<details><summary>Schema: <code>VectorStoreFileObject</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  usage_bytes: integer (required)
  created_at: integer(unixtime) (required)
  vector_store_id: string (required)
  status: string enum (required)
  last_error: anyOf(object | null) (required)
  chunking_strategy: oneOf(StaticChunkingStrategyResponseParam | OtherChunkingStrategyResponseParam)
  attributes: VectorStoreFileAttributes
```

</details>

- **createVectorStoreFile** (line 354) ↔ **updateVectorStoreFileAttributes** (line 436)
  Shared: response: `VectorStoreFileObject`

- **getVectorStoreFile** (line 384) ↔ **updateVectorStoreFileAttributes** (line 436)
  Shared: response: `VectorStoreFileObject`

</details>

</details>

---

## Containers

_No changes detected._

## Skills

_No changes detected._

## Realtime

<details><summary>0 changes · `realtime.yml`</summary>

<details><summary>Structurally Equivalent Schemas (1)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **Prompt**, **ResponsePromptVariables**
  Signature: `anyOf(null,object)`

</details>

</details>

---

## Assistants

<details><summary>0 changes · `assistants.yml`</summary>

<details><summary>Structurally Equivalent Schemas (1)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **FunctionParameters**, **ResponseFormatJsonSchemaSchema**
  Signature: `object`

</details>

<details><summary>Duplicate Operations (3)</summary>

> The following operations share identical request and/or response schema definitions.

- **createAssistant** (line 66) ↔ **getAssistant** (line 86)
  Shared: response: `AssistantObject`


<details><summary>Schema: <code>AssistantObject</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  created_at: integer(unixtime) (required)
  name: anyOf(string | null) (required)
  description: anyOf(string | null) (required)
  model: string (required)
  instructions: anyOf(string | null) (required)
  tools: array (required)
  tool_resources: anyOf(object | null)
  metadata: Metadata (required)
  temperature: anyOf(number | null)
  top_p: anyOf(number | null)
  response_format: anyOf(AssistantsApiResponseFormatOption | null)
```

</details>

- **createAssistant** (line 66) ↔ **modifyAssistant** (line 106)
  Shared: response: `AssistantObject`

- **getAssistant** (line 86) ↔ **modifyAssistant** (line 106)
  Shared: response: `AssistantObject`

</details>

<details><summary>⚠ Spec Anomalies (2)</summary>

> The following inconsistencies were detected in the spec and may indicate errors or intentional divergence.

- 🟡 **model-mismatch** at `/assistants.post`
  Request model type `anyOf(string | AssistantSupportedModels)` differs from response model type `string` in `createAssistant`

- 🟡 **model-mismatch** at `/assistants/{assistant_id}.post`
  Request model type `anyOf(string | AssistantSupportedModels)` differs from response model type `string` in `modifyAssistant`

</details>

</details>

---

## Messages

_No changes detected._

## Runs

<details><summary>0 changes · `runs.yml`</summary>

<details><summary>Structurally Equivalent Schemas (3)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **FunctionParameters**, **ResponseFormatJsonSchemaSchema**
  Signature: `object`

- **Metadata**, **RunCompletionUsage**, **RunStepCompletionUsage**
  Signature: `anyOf(null,object)`

- **RunStepDetailsToolCallsFunctionObject**, **RunToolCallObject**
  Signature: `object{function*:object,id*:string,type*:string enum}`

</details>

<details><summary>Duplicate Operations (15)</summary>

> The following operations share identical request and/or response schema definitions.

- **createThreadAndRun** (line 22) ↔ **createRun** (line 90)
  Shared: response: `RunObject`


<details><summary>Schema: <code>RunObject</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  created_at: integer(unixtime) (required)
  thread_id: string (required)
  assistant_id: string (required)
  status: string enum (required)
  required_action: object (required)
  last_error: object (required)
  expires_at: integer(unixtime) (required)
  started_at: integer(unixtime) (required)
  cancelled_at: integer(unixtime) (required)
  failed_at: integer(unixtime) (required)
  completed_at: integer(unixtime) (required)
  incomplete_details: object (required)
  model: string (required)
  instructions: string (required)
  tools: array (required)
  metadata: Metadata (required)
  usage: RunCompletionUsage (required)
  temperature: number
  top_p: number
  max_prompt_tokens: integer (required)
  max_completion_tokens: integer (required)
  truncation_strategy: allOf(TruncationObject | inline) (required)
  tool_choice: allOf(AssistantsApiToolChoiceOption | inline) (required)
  parallel_tool_calls: ParallelToolCalls (required)
  response_format: AssistantsApiResponseFormatOption (required)
```

</details>

- **createThreadAndRun** (line 22) ↔ **getRun** (line 127)
  Shared: response: `RunObject`

- **createThreadAndRun** (line 22) ↔ **modifyRun** (line 152)
  Shared: response: `RunObject`

- **createThreadAndRun** (line 22) ↔ **cancelRun** (line 184)
  Shared: response: `RunObject`

- **createThreadAndRun** (line 22) ↔ **submitToolOuputsToRun** (line 320)
  Shared: response: `RunObject`

- **createRun** (line 90) ↔ **getRun** (line 127)
  Shared: response: `RunObject`

- **createRun** (line 90) ↔ **modifyRun** (line 152)
  Shared: response: `RunObject`

- **createRun** (line 90) ↔ **cancelRun** (line 184)
  Shared: response: `RunObject`

- **createRun** (line 90) ↔ **submitToolOuputsToRun** (line 320)
  Shared: response: `RunObject`

- **getRun** (line 127) ↔ **modifyRun** (line 152)
  Shared: response: `RunObject`

- **getRun** (line 127) ↔ **cancelRun** (line 184)
  Shared: response: `RunObject`

- **getRun** (line 127) ↔ **submitToolOuputsToRun** (line 320)
  Shared: response: `RunObject`

- **modifyRun** (line 152) ↔ **cancelRun** (line 184)
  Shared: response: `RunObject`

- **modifyRun** (line 152) ↔ **submitToolOuputsToRun** (line 320)
  Shared: response: `RunObject`

- **cancelRun** (line 184) ↔ **submitToolOuputsToRun** (line 320)
  Shared: response: `RunObject`

</details>

<details><summary>⚠ Spec Anomalies (2)</summary>

> The following inconsistencies were detected in the spec and may indicate errors or intentional divergence.

- 🟡 **model-mismatch** at `/threads/runs.post`
  Request model type `anyOf(string | string)` differs from response model type `string` in `createThreadAndRun`

- 🟡 **model-mismatch** at `/threads/{thread_id}/runs.post`
  Request model type `anyOf(string | AssistantSupportedModels)` differs from response model type `string` in `createRun`

</details>

</details>

---

## Threads

_No changes detected._

## Administration

<details><summary>0 changes · `administration.yml`</summary>

<details><summary>Structurally Equivalent Schemas (12)</summary>

> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.

- **GroupDeletedResource**, **InviteDeleteResponse**, **OrganizationSpendAlertDeletedResource**, **ProjectApiKeyDeleteResponse**, **ProjectServiceAccountDeleteResponse**, **ProjectSpendAlertDeletedResource**, **ProjectUserDeleteResponse**, **RoleDeletedResource**, **UserDeleteResponse**
  Signature: `object{deleted*:boolean,id*:string,object*:string enum}`

- **GroupUserDeletedResource**, **OrganizationSpendLimitDeletedResource**, **ProjectGroupDeletedResource**, **ProjectModelPermissionsDeleteResponse**, **ProjectSpendLimitDeletedResource**
  Signature: `object{deleted*:boolean,object*:string enum}`

- **OrganizationCertificate**, **OrganizationProjectCertificate**
  Signature: `object{active*:boolean,certificate_details*:object,created_at*:integer(unixtime),id*:string,name*:anyOf(string | null),object*:string enum}`

- **OrganizationCertificateActivationResponse**, **OrganizationCertificateDeactivationResponse**
  Signature: `object{data*:array<OrganizationCertificate>,object*:string enum}`

- **OrganizationDataRetention**, **ProjectDataRetention**
  Signature: `object{object*:string enum,type*:string enum}`

- **OrganizationProjectCertificateActivationResponse**, **OrganizationProjectCertificateDeactivationResponse**
  Signature: `object{data*:array<OrganizationProjectCertificate>,object*:string enum}`

- **OrganizationSpendAlert**, **ProjectSpendAlert**
  Signature: `object{currency*:string enum,id*:string,interval*:string enum,notification_channel*:SpendAlertNotificationChannel,object*:string enum,threshold_amount*:integer}`

- **OrganizationSpendLimitResource**, **ProjectSpendLimitResource**
  Signature: `object{currency*:SpendLimitCurrency,enforcement*:SpendLimitEnforcement,interval*:SpendLimitInterval,object*:string enum,threshold_amount*:integer}`

- **ProjectServiceAccountApiKey**, **ServiceAccountApiKeyBody**
  Signature: `object{created_at*:integer(unixtime),id*:string,name*:string,object*:string enum,value*:string}`

- **SpendLimitCurrency**, **SpendLimitEnforcementStatus**, **SpendLimitInterval**
  Signature: `anyOf(string,string)`

- **UpdateOrganizationSpendLimitBody**, **UpdateProjectSpendLimitBody**
  Signature: `object{currency*:string enum,interval*:string enum,threshold_amount*:integer}`

- **UsageEmbeddingsResult**, **UsageModerationsResult**
  Signature: `object{api_key_id:anyOf(string | null),input_tokens*:integer,model:anyOf(string | null),num_model_requests*:integer,object*:string enum,project_id:anyOf(string | null),user_id:anyOf(string | null)}`

</details>

<details><summary>Duplicate Operations (135)</summary>

> The following operations share identical request and/or response schema definitions.

- **uploadCertificate** (line 292) ↔ **getCertificate** (line 367)
  Shared: response: `Certificate`


<details><summary>Schema: <code>Certificate</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: anyOf(string | null) (required)
  created_at: integer(unixtime) (required)
  certificate_details: object (required)
  active: boolean
```

</details>

- **uploadCertificate** (line 292) ↔ **modifyCertificate** (line 400)
  Shared: response: `Certificate`

- **activateOrganizationCertificates** (line 317) ↔ **deactivateOrganizationCertificates** (line 342)
  Shared: request: `ToggleCertificatesRequest`


<details><summary>Schema: <code>ToggleCertificatesRequest</code></summary>

```yaml
# schema: object
  certificate_ids: array<string> (required)
```

</details>

- **activateOrganizationCertificates** (line 317) ↔ **activateProjectCertificates** (line 1352)
  Shared: request: `ToggleCertificatesRequest`

- **activateOrganizationCertificates** (line 317) ↔ **deactivateProjectCertificates** (line 1384)
  Shared: request: `ToggleCertificatesRequest`

- **deactivateOrganizationCertificates** (line 342) ↔ **activateProjectCertificates** (line 1352)
  Shared: request: `ToggleCertificatesRequest`

- **deactivateOrganizationCertificates** (line 342) ↔ **deactivateProjectCertificates** (line 1384)
  Shared: request: `ToggleCertificatesRequest`

- **getCertificate** (line 367) ↔ **modifyCertificate** (line 400)
  Shared: response: `Certificate`

- **usage-costs** (line 453) ↔ **usage-audio-speeches** (line 2600)
  Shared: response: `UsageResponse`


<details><summary>Schema: <code>UsageResponse</code></summary>

```yaml
# schema: object
  object: string enum (required)
  data: array<UsageTimeBucket> (required)
  has_more: boolean (required)
  next_page: anyOf(string | null) (required)
```

</details>

- **usage-costs** (line 453) ↔ **usage-audio-transcriptions** (line 2699)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-code-interpreter-sessions** (line 2798)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-completions** (line 2870)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-embeddings** (line 2978)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-costs** (line 453) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **retrieve-organization-data-retention** (line 531) ↔ **update-organization-data-retention** (line 545)
  Shared: response: `OrganizationDataRetention`


<details><summary>Schema: <code>OrganizationDataRetention</code></summary>

```yaml
# schema: object
  object: string enum (required)
  type: string enum (required)
```

</details>

- **create-group** (line 609) ↔ **retrieve-group** (line 631)
  Shared: response: `GroupResponse`


<details><summary>Schema: <code>GroupResponse</code></summary>

```yaml
# schema: object
  id: string (required)
  name: string (required)
  created_at: integer(unixtime) (required)
  is_scim_managed: boolean (required)
  group_type: string enum (required)
```

</details>

- **list-group-role-assignments** (line 702) ↔ **list-user-role-assignments** (line 3696)
  Shared: response: `RoleListResource`


<details><summary>Schema: <code>RoleListResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  data: array<AssignedRoleDetails> (required)
  has_more: boolean (required)
  next: anyOf(string | null) (required)
```

</details>

- **list-group-role-assignments** (line 702) ↔ **list-project-group-role-assignments** (line 3824)
  Shared: response: `RoleListResource`

- **list-group-role-assignments** (line 702) ↔ **list-project-user-role-assignments** (line 4140)
  Shared: response: `RoleListResource`

- **assign-group-role** (line 746) ↔ **assign-user-role** (line 3740)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`


<details><summary>Schema: <code>PublicAssignOrganizationGroupRoleBody</code></summary>

```yaml
# schema: object
  role_id: string (required)
```

</details>

- **assign-group-role** (line 746) ↔ **assign-project-group-role** (line 3874)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`, response: `GroupRoleAssignment`


<details><summary>Schema: <code>GroupRoleAssignment</code></summary>

```yaml
# schema: object
  object: string enum (required)
  group: Group (required)
  role: Role (required)
```

</details>

- **assign-group-role** (line 746) ↔ **assign-project-user-role** (line 4190)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`

- **retrieve-group-role** (line 775) ↔ **retrieve-user-role** (line 3769)
  Shared: response: `AssignedRoleDetails`


<details><summary>Schema: <code>AssignedRoleDetails</code></summary>

```yaml
# schema: object
  id: string (required)
  name: string (required)
  permissions: array<string> (required)
  resource_type: string (required)
  predefined_role: boolean (required)
  description: anyOf(string | null) (required)
  created_at: anyOf(integer | null) (required)
  updated_at: anyOf(integer | null) (required)
  created_by: anyOf(string | null) (required)
  created_by_user_obj: anyOf(object | null) (required)
  metadata: anyOf(object | null) (required)
  assignment_sources: anyOf(array | null) (required)
```

</details>

- **retrieve-group-role** (line 775) ↔ **retrieve-project-group-role** (line 3909)
  Shared: response: `AssignedRoleDetails`

- **retrieve-group-role** (line 775) ↔ **retrieve-project-user-role** (line 4225)
  Shared: response: `AssignedRoleDetails`

- **unassign-group-role** (line 802) ↔ **unassign-user-role** (line 3796)
  Shared: response: `DeletedRoleAssignmentResource`


<details><summary>Schema: <code>DeletedRoleAssignmentResource</code></summary>

```yaml
# schema: object
  object: string (required)
  deleted: boolean (required)
```

</details>

- **unassign-group-role** (line 802) ↔ **unassign-project-group-role** (line 3942)
  Shared: response: `DeletedRoleAssignmentResource`

- **unassign-group-role** (line 802) ↔ **unassign-project-user-role** (line 4258)
  Shared: response: `DeletedRoleAssignmentResource`

- **inviteUser** (line 992) ↔ **retrieve-invite** (line 1014)
  Shared: response: `Invite`


<details><summary>Schema: <code>Invite</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  email: string (required)
  role: string enum (required)
  status: string enum (required)
  created_at: integer(unixtime) (required)
  expires_at: anyOf(integer | null)
  accepted_at: anyOf(integer | null)
  projects: array<object> (required)
```

</details>

- **create-project** (line 1093) ↔ **retrieve-project** (line 1115)
  Shared: response: `Project`


<details><summary>Schema: <code>Project</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  name: anyOf(string | null)
  created_at: integer(unixtime) (required)
  archived_at: anyOf(integer | null)
  status: anyOf(string | null)
  external_key_id: anyOf(string | null)
```

</details>

- **create-project** (line 1093) ↔ **modify-project** (line 1136)
  Shared: response: `Project`

- **create-project** (line 1093) ↔ **archive-project** (line 1283)
  Shared: response: `Project`

- **retrieve-project** (line 1115) ↔ **modify-project** (line 1136)
  Shared: response: `Project`

- **retrieve-project** (line 1115) ↔ **archive-project** (line 1283)
  Shared: response: `Project`

- **modify-project** (line 1136) ↔ **archive-project** (line 1283)
  Shared: response: `Project`

- **activateProjectCertificates** (line 1352) ↔ **deactivateProjectCertificates** (line 1384)
  Shared: request: `ToggleCertificatesRequest`

- **retrieve-project-data-retention** (line 1414) ↔ **update-project-data-retention** (line 1435)
  Shared: response: `ProjectDataRetention`


<details><summary>Schema: <code>ProjectDataRetention</code></summary>

```yaml
# schema: object
  object: string enum (required)
  type: string enum (required)
```

</details>

- **add-project-group** (line 1510) ↔ **retrieve-project-group** (line 1539)
  Shared: response: `ProjectGroup`


<details><summary>Schema: <code>ProjectGroup</code></summary>

```yaml
# schema: object
  object: string enum (required)
  project_id: string (required)
  group_id: string (required)
  group_name: string (required)
  group_type: string enum (required)
  created_at: integer(unixtime) (required)
```

</details>

- **retrieve-project-hosted-tool-permissions** (line 1604) ↔ **update-project-hosted-tool-permissions** (line 1625)
  Shared: response: `ProjectHostedToolPermissions`


<details><summary>Schema: <code>ProjectHostedToolPermissions</code></summary>

```yaml
# schema: object
  file_search: HostedToolPermission (required)
  web_search: HostedToolPermission (required)
  image_generation: HostedToolPermission (required)
  mcp: HostedToolPermission (required)
  code_interpreter: HostedToolPermission (required)
```

</details>

- **retrieve-project-model-permissions** (line 1654) ↔ **update-project-model-permissions** (line 1675)
  Shared: response: `ProjectModelPermissions`


<details><summary>Schema: <code>ProjectModelPermissions</code></summary>

```yaml
# schema: object
  object: string enum (required)
  mode: string enum (required)
  model_ids: array<string> (required)
```

</details>

- **retrieve-project-service-account** (line 1887) ↔ **update-project-service-account** (line 1914)
  Shared: response: `ProjectServiceAccount`


<details><summary>Schema: <code>ProjectServiceAccount</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: string (required)
  role: string enum (required)
  created_at: integer(unixtime) (required)
```

</details>

- **create-project-spend-alert** (line 2030) ↔ **retrieve-project-spend-alert** (line 2059)
  Shared: response: `ProjectSpendAlert`


<details><summary>Schema: <code>ProjectSpendAlert</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  threshold_amount: integer (required)
  currency: string enum (required)
  interval: string enum (required)
  notification_channel: SpendAlertNotificationChannel (required)
```

</details>

- **create-project-spend-alert** (line 2030) ↔ **update-project-spend-alert** (line 2086)
  Shared: request: `CreateSpendAlertBody`, response: `ProjectSpendAlert`


<details><summary>Schema: <code>CreateSpendAlertBody</code></summary>

```yaml
# schema: object
  threshold_amount: integer (required)
  currency: string enum (required)
  interval: string enum (required)
  notification_channel: SpendAlertNotificationChannel (required)
```

</details>

- **create-project-spend-alert** (line 2030) ↔ **create-organization-spend-alert** (line 2507)
  Shared: request: `CreateSpendAlertBody`

- **create-project-spend-alert** (line 2030) ↔ **update-organization-spend-alert** (line 2550)
  Shared: request: `CreateSpendAlertBody`

- **retrieve-project-spend-alert** (line 2059) ↔ **update-project-spend-alert** (line 2086)
  Shared: response: `ProjectSpendAlert`

- **update-project-spend-alert** (line 2086) ↔ **create-organization-spend-alert** (line 2507)
  Shared: request: `CreateSpendAlertBody`

- **update-project-spend-alert** (line 2086) ↔ **update-organization-spend-alert** (line 2550)
  Shared: request: `CreateSpendAlertBody`

- **create-project-user** (line 2190) ↔ **retrieve-project-user** (line 2225)
  Shared: response: `ProjectUser`


<details><summary>Schema: <code>ProjectUser</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: anyOf(string | null)
  email: anyOf(string | null)
  role: string (required)
  added_at: integer(unixtime) (required)
```

</details>

- **create-project-user** (line 2190) ↔ **modify-project-user** (line 2252)
  Shared: response: `ProjectUser`

- **retrieve-project-user** (line 2225) ↔ **modify-project-user** (line 2252)
  Shared: response: `ProjectUser`

- **list-roles** (line 2329) ↔ **list-project-roles** (line 3976)
  Shared: response: `PublicRoleListResource`


<details><summary>Schema: <code>PublicRoleListResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  data: array<Role> (required)
  has_more: boolean (required)
  next: anyOf(string | null) (required)
```

</details>

- **create-role** (line 2369) ↔ **retrieve-role** (line 2391)
  Shared: response: `Role`


<details><summary>Schema: <code>Role</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: string (required)
  description: anyOf(string | null) (required)
  permissions: array<string> (required)
  resource_type: string (required)
  predefined_role: boolean (required)
```

</details>

- **create-role** (line 2369) ↔ **update-role** (line 2412)
  Shared: response: `Role`

- **create-role** (line 2369) ↔ **create-project-role** (line 4022)
  Shared: request: `PublicCreateOrganizationRoleBody`, response: `Role`


<details><summary>Schema: <code>PublicCreateOrganizationRoleBody</code></summary>

```yaml
# schema: object
  role_name: string (required)
  permissions: array<string> (required)
  description: anyOf(string | null)
```

</details>

- **create-role** (line 2369) ↔ **retrieve-project-role** (line 4051)
  Shared: response: `Role`

- **create-role** (line 2369) ↔ **update-project-role** (line 4078)
  Shared: response: `Role`

- **retrieve-role** (line 2391) ↔ **update-role** (line 2412)
  Shared: response: `Role`

- **retrieve-role** (line 2391) ↔ **create-project-role** (line 4022)
  Shared: response: `Role`

- **retrieve-role** (line 2391) ↔ **retrieve-project-role** (line 4051)
  Shared: response: `Role`

- **retrieve-role** (line 2391) ↔ **update-project-role** (line 4078)
  Shared: response: `Role`

- **update-role** (line 2412) ↔ **create-project-role** (line 4022)
  Shared: response: `Role`

- **update-role** (line 2412) ↔ **retrieve-project-role** (line 4051)
  Shared: response: `Role`

- **update-role** (line 2412) ↔ **update-project-role** (line 4078)
  Shared: request: `PublicUpdateOrganizationRoleBody`, response: `Role`


<details><summary>Schema: <code>PublicUpdateOrganizationRoleBody</code></summary>

```yaml
# schema: object
  permissions: anyOf(array | null)
  description: anyOf(string | null)
  role_name: anyOf(string | null)
```

</details>

- **delete-role** (line 2440) ↔ **delete-project-role** (line 4112)
  Shared: response: `RoleDeletedResource`


<details><summary>Schema: <code>RoleDeletedResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  deleted: boolean (required)
```

</details>

- **create-organization-spend-alert** (line 2507) ↔ **retrieve-organization-spend-alert** (line 2529)
  Shared: response: `OrganizationSpendAlert`


<details><summary>Schema: <code>OrganizationSpendAlert</code></summary>

```yaml
# schema: object
  id: string (required)
  object: string enum (required)
  threshold_amount: integer (required)
  currency: string enum (required)
  interval: string enum (required)
  notification_channel: SpendAlertNotificationChannel (required)
```

</details>

- **create-organization-spend-alert** (line 2507) ↔ **update-organization-spend-alert** (line 2550)
  Shared: request: `CreateSpendAlertBody`, response: `OrganizationSpendAlert`

- **retrieve-organization-spend-alert** (line 2529) ↔ **update-organization-spend-alert** (line 2550)
  Shared: response: `OrganizationSpendAlert`

- **usage-audio-speeches** (line 2600) ↔ **usage-audio-transcriptions** (line 2699)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-code-interpreter-sessions** (line 2798)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-completions** (line 2870)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-embeddings** (line 2978)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-audio-speeches** (line 2600) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-code-interpreter-sessions** (line 2798)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-completions** (line 2870)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-embeddings** (line 2978)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-audio-transcriptions** (line 2699) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-completions** (line 2870)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-embeddings** (line 2978)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-code-interpreter-sessions** (line 2798) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-embeddings** (line 2978)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-completions** (line 2870) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-embeddings** (line 2978) ↔ **usage-file-search-calls** (line 3077)
  Shared: response: `UsageResponse`

- **usage-embeddings** (line 2978) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-embeddings** (line 2978) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-embeddings** (line 2978) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-embeddings** (line 2978) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-file-search-calls** (line 3077) ↔ **usage-images** (line 3176)
  Shared: response: `UsageResponse`

- **usage-file-search-calls** (line 3077) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-file-search-calls** (line 3077) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-file-search-calls** (line 3077) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-images** (line 3176) ↔ **usage-moderations** (line 3303)
  Shared: response: `UsageResponse`

- **usage-images** (line 3176) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-images** (line 3176) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-moderations** (line 3303) ↔ **usage-vector-stores** (line 3402)
  Shared: response: `UsageResponse`

- **usage-moderations** (line 3303) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **usage-vector-stores** (line 3402) ↔ **usage-web-search-calls** (line 3474)
  Shared: response: `UsageResponse`

- **retrieve-user** (line 3625) ↔ **modify-user** (line 3646)
  Shared: response: `User`


<details><summary>Schema: <code>User</code></summary>

```yaml
# schema: object
  object: string enum (required)
  id: string (required)
  name: anyOf(string | null)
  email: anyOf(string | null)
  role: anyOf(string | null)
  added_at: integer(unixtime) (required)
  is_default: boolean
  created: integer(unixtime)
  user: object
  is_service_account: boolean
  is_scale_tier_authorized_purchaser: anyOf(boolean | null)
  is_scim_managed: boolean
  api_key_last_used_at: anyOf(integer | null)
  technical_level: anyOf(string | null)
  developer_persona: anyOf(string | null)
  projects: anyOf(object | null)
```

</details>

- **list-user-role-assignments** (line 3696) ↔ **list-project-group-role-assignments** (line 3824)
  Shared: response: `RoleListResource`

- **list-user-role-assignments** (line 3696) ↔ **list-project-user-role-assignments** (line 4140)
  Shared: response: `RoleListResource`

- **assign-user-role** (line 3740) ↔ **assign-project-group-role** (line 3874)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`

- **assign-user-role** (line 3740) ↔ **assign-project-user-role** (line 4190)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`, response: `UserRoleAssignment`


<details><summary>Schema: <code>UserRoleAssignment</code></summary>

```yaml
# schema: object
  object: string enum (required)
  user: User (required)
  role: Role (required)
```

</details>

- **retrieve-user-role** (line 3769) ↔ **retrieve-project-group-role** (line 3909)
  Shared: response: `AssignedRoleDetails`

- **retrieve-user-role** (line 3769) ↔ **retrieve-project-user-role** (line 4225)
  Shared: response: `AssignedRoleDetails`

- **unassign-user-role** (line 3796) ↔ **unassign-project-group-role** (line 3942)
  Shared: response: `DeletedRoleAssignmentResource`

- **unassign-user-role** (line 3796) ↔ **unassign-project-user-role** (line 4258)
  Shared: response: `DeletedRoleAssignmentResource`

- **list-project-group-role-assignments** (line 3824) ↔ **list-project-user-role-assignments** (line 4140)
  Shared: response: `RoleListResource`

- **assign-project-group-role** (line 3874) ↔ **assign-project-user-role** (line 4190)
  Shared: request: `PublicAssignOrganizationGroupRoleBody`

- **retrieve-project-group-role** (line 3909) ↔ **retrieve-project-user-role** (line 4225)
  Shared: response: `AssignedRoleDetails`

- **unassign-project-group-role** (line 3942) ↔ **unassign-project-user-role** (line 4258)
  Shared: response: `DeletedRoleAssignmentResource`

- **create-project-role** (line 4022) ↔ **retrieve-project-role** (line 4051)
  Shared: response: `Role`

- **create-project-role** (line 4022) ↔ **update-project-role** (line 4078)
  Shared: response: `Role`

- **retrieve-project-role** (line 4051) ↔ **update-project-role** (line 4078)
  Shared: response: `Role`

- **Getorganizationspendlimit** (line 4292) ↔ **Updateorganizationspendlimit** (line 4305)
  Shared: response: `OrganizationSpendLimitResource`


<details><summary>Schema: <code>OrganizationSpendLimitResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  threshold_amount: integer (required)
  currency: SpendLimitCurrency (required)
  interval: SpendLimitInterval (required)
  enforcement: SpendLimitEnforcement (required)
```

</details>

- **Getprojectspendlimit** (line 4339) ↔ **Updateprojectspendlimit** (line 4379)
  Shared: response: `ProjectSpendLimitResource`


<details><summary>Schema: <code>ProjectSpendLimitResource</code></summary>

```yaml
# schema: object
  object: string enum (required)
  threshold_amount: integer (required)
  currency: SpendLimitCurrency (required)
  interval: SpendLimitInterval (required)
  enforcement: SpendLimitEnforcement (required)
```

</details>

</details>

<details><summary>⚠ Spec Anomalies (8)</summary>

> The following inconsistencies were detected in the spec and may indicate errors or intentional divergence.

- 🟡 **input-output-divergence** at `/organization/invites.post`
  Request `InviteRequest` and response `Invite` in `inviteUser` share only 3/3 input properties. Response has 3 additional non-standard fields: status, expires_at, accepted_at

- 🟡 **input-output-divergence** at `/organization/projects/{project_id}/groups.post`
  Request `InviteProjectGroupBody` and response `ProjectGroup` in `add-project-group` share only 1/2 input properties. Response has 3 additional non-standard fields: project_id, group_name, group_type

- 🟡 **input-output-divergence** at `/organization/projects/{project_id}/users/{user_id}.post`
  Request `ProjectUserUpdateRequest` and response `ProjectUser` in `modify-project-user` share only 1/1 input properties. Response has 3 additional non-standard fields: name, email, added_at

- 🟡 **input-output-divergence** at `/organization/roles.post`
  Request `PublicCreateOrganizationRoleBody` and response `Role` in `create-role` share only 2/3 input properties. Response has 3 additional non-standard fields: name, resource_type, predefined_role

- 🟡 **input-output-divergence** at `/organization/roles/{role_id}.post`
  Request `PublicUpdateOrganizationRoleBody` and response `Role` in `update-role` share only 2/3 input properties. Response has 3 additional non-standard fields: name, resource_type, predefined_role

- 🟡 **input-output-divergence** at `/organization/users/{user_id}.post`
  Request `UserRoleUpdateRequest` and response `User` in `modify-user` share only 3/4 input properties. Response has 11 additional non-standard fields: name, email, added_at, is_default, created

- 🟡 **input-output-divergence** at `/projects/{project_id}/roles.post`
  Request `PublicCreateOrganizationRoleBody` and response `Role` in `create-project-role` share only 2/3 input properties. Response has 3 additional non-standard fields: name, resource_type, predefined_role

- 🟡 **input-output-divergence** at `/projects/{project_id}/roles/{role_id}.post`
  Request `PublicUpdateOrganizationRoleBody` and response `Role` in `update-project-role` share only 2/3 input properties. Response has 3 additional non-standard fields: name, resource_type, predefined_role

</details>

</details>

---

