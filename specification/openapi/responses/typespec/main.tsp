import "@typespec/http";
import "@typespec/openapi";
import "@typespec/openapi3";

using Http;
using OpenAPI;

@service(#{ title: "OpenAI API" })
@info(#{
  version: "2.3.0",
  contact: #{ name: "OpenAI Support", url: "https://help.openai.com/" },
  license: #{ name: "MIT", identifier: "MIT" },
  termsOfService: "https://openai.com/policies/terms-of-use",
})
@server("https://api.openai.com/v1")
@tagMetadata(#[#{ name: "Responses" }])
namespace OpenAI;

model createResponse_request_object {
  access_programs?: createResponse_request_object_AccessPrograms_object;
  background?: boolean | null;
  context_management?: createResponse_request_object_ContextManagement_array_item_object[] | null;
  conversation?: createResponse_request_object_Conversation_union | null;
  include?: createResponse_request_object_Include_array_item_enum[] | null;
  input?: createResponse_request_object_Input_union;
  instructions?: string | null;
  max_output_tokens?: integer | null;
  max_tool_calls?: integer | null;
  metadata?: createResponse_request_object_Metadata_dictionary | null;
  `model`?: string | createResponse_request_object_Model_enum;
  moderation?: createResponse_request_object_Moderation_object | null;
  parallel_tool_calls?: boolean | null;
  previous_response_id?: string | null;
  prompt?: createResponse_request_object_Prompt_object | null;
  prompt_cache_key?: string | null;
  prompt_cache_options?: createResponse_request_object_PromptCacheOptions_object;
  #deprecated "deprecated"
  prompt_cache_retention?: createResponse_request_object_PromptCacheRetention_enum | null;
  reasoning?: createResponse_request_object_Reasoning_object | null;
  safety_identifier?: string | null;
  service_tier?: createResponse_request_object_ServiceTier_enum | null;
  store?: boolean | null;
  stream?: boolean | null;
  stream_options?: createResponse_request_object_StreamOptions_object | null;
  temperature?: numeric | null;
  text?: createResponse_request_object_Text_object;
  tool_choice?: createResponse_request_object_ToolChoice_union;
  tools?: createResponse_request_object_Tools_array_item_discriminatedObject[];
  top_logprobs?: integer | null;
  top_p?: numeric | null;
  #deprecated "deprecated"
  truncation?: createResponse_request_object_Truncation_enum | null;
  #deprecated "deprecated"
  user?: string | null;
}

model createResponse_request_object_AccessPrograms_object {
  cyber?: createResponse_request_object_AccessPrograms_object_Cyber_enum;
}

enum createResponse_request_object_AccessPrograms_object_Cyber_enum {
  daybreak_blue,
  daybreak_red,
  standard,
}

model createResponse_request_object_ContextManagement_array_item_object {
  type: string;
  compact_threshold?: integer | null;
}

@oneOf
union createResponse_request_object_Conversation_union {
  createResponse_request_object_Conversation_union_object,
  string,
}

model createResponse_request_object_Conversation_union_object {
  id: string;
}

enum createResponse_request_object_Include_array_item_enum {
  `code_interpreter_call.outputs`,
  `computer_call_output.output.image_url`,
  `file_search_call.results`,
  `message.input_image.image_url`,
  `message.output_text.logprobs`,
  `reasoning.encrypted_content`,
  `web_search_call.action.sources`,
  `web_search_call.results`,
}

@oneOf
union createResponse_request_object_Input_union {
  createResponse_request_object_Input_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfAdditionalTools
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "additional_tools";
  role: "developer";
  tools: createResponse_request_object_Tools_array_item_discriminatedObject[];
  id?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "apply_patch_call";
  call_id: string;
  operation: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Status_enum;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string | null;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject_typeOfCreateFile
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject {
  type: "create_file";
  diff: string;
  path: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject_typeOfDeleteFile
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject {
  type: "delete_file";
  path: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject_typeOfUpdateFile
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Operation_discriminatedObject {
  type: "update_file";
  diff: string;
  path: string;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Status_enum {
  completed,
  in_progress,
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject_typeOfDirect
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject {
  type: "direct";
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject_typeOfProgram
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject {
  type: "program";
  caller_id: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "apply_patch_call_output";
  call_id: string;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCallOutput_Status_enum;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string | null;
  output?: string | null;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCallOutput_Status_enum {
  completed,
  failed,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "code_interpreter_call";
  code: string | null;
  container_id: string;
  id: string;
  outputs:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject[]
    | null;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Status_enum;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject_typeOfImage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject {
  type: "image";
  url: url;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject_typeOfLogs
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Outputs_array_item_discriminatedObject {
  type: "logs";
  logs: string;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCodeInterpreterCall_Status_enum {
  completed,
  failed,
  in_progress,
  incomplete,
  interpreting,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCompaction
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "compaction";
  encrypted_content: string;
  id?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCompactionTrigger
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "compaction_trigger";
  id?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "computer_call";
  call_id: string;
  id: string;
  pending_safety_checks: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_PendingSafetyChecks_array_item_object[];
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum;
  action?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject;
  actions?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject[];
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_PendingSafetyChecks_array_item_object {
  id: string;
  code?: string | null;
  message?: string | null;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum {
  completed,
  in_progress,
  incomplete,
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfClick
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "click";
  button: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfClick_Button_enum;
  x: integer;
  y: integer;
  keys?: string[] | null;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfClick_Button_enum {
  back,
  forward,
  left,
  right,
  wheel,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfDoubleClick
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "double_click";
  keys: string[] | null;
  x: integer;
  y: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfDrag
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "drag";
  path: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfDrag_Path_array_item_object[];
  keys?: string[] | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfDrag_Path_array_item_object {
  x: integer;
  y: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfKeypress
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "keypress";
  keys: string[];
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfMove
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "move";
  x: integer;
  y: integer;
  keys?: string[] | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfScreenshot
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "screenshot";
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfScroll
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "scroll";
  scroll_x: integer;
  scroll_y: integer;
  x: integer;
  y: integer;
  keys?: string[] | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfType
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "type";
  text: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject_typeOfWait
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Action_discriminatedObject {
  type: "wait";
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "computer_call_output";
  call_id: string;
  output: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCallOutput_Output_discriminatedObject;
  acknowledged_safety_checks?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_PendingSafetyChecks_array_item_object[]
    | null;
  id?: string | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCallOutput_Output_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCallOutput_Output_discriminatedObject_typeOfComputerScreenshot
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCallOutput_Output_discriminatedObject {
  type: "computer_screenshot";
  file_id?: string;
  image_url?: url;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "configuration_update";
  id?: string | null;
  reasoning?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Reasoning_object;
  text?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Reasoning_object {
  effort?: createResponse_request_object_Reasoning_object_Effort_enum | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object {
  format: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject_typeOfJsonSchema
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject {
  type: "json_schema";
  name: string;
  schema: createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema_Schema_dictionary;
  description?: string;
  strict?: boolean | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject_typeOfText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfConfigurationUpdate_Text_object_Format_discriminatedObject {
  type: "text";
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCustomToolCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "custom_tool_call";
  call_id: string;
  input: string;
  name: string;
  async?: boolean;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string;
  `namespace`?: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCustomToolCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "custom_tool_call_output";
  call_id: string;
  output: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCustomToolCallOutput_Output_union;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string;
}

@oneOf
union createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfCustomToolCallOutput_Output_union {
  createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject[],
  string,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "file_search_call";
  id: string;
  queries: string[];
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Status_enum;
  results?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object[]
    | null;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Status_enum {
  completed,
  failed,
  in_progress,
  incomplete,
  searching,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object {
  attributes?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object_Attributes_dictionary
    | null;
  file_id?: string;
  filename?: string;
  score?: float32;
  text?: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object_Attributes_dictionary {
  ...Record<createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object_Attributes_dictionary_value_union>;
}

@oneOf
union createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Results_array_item_object_Attributes_dictionary_value_union {
  boolean,
  numeric,
  string,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "function_call";
  arguments: string;
  call_id: string;
  name: string;
  async?: boolean;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string;
  `namespace`?: string;
  status?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "function_call_output";
  output: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union;
  call_id?: string | null;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string | null;
  name?: string | null;
  `namespace`?: string | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

@oneOf
union createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union {
  createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject_typeOfInputFile
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject {
  type: "input_file";
  detail?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_Detail_enum;
  file_data?: string | null;
  file_id?: string | null;
  file_url?: url | null;
  filename?: string | null;
  prompt_cache_breakpoint?:
    | createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject_typeOfInputImage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject {
  type: "input_image";
  detail?:
    | createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputImage_Detail_enum
    | null;
  file_id?: string | null;
  image_url?: url | null;
  prompt_cache_breakpoint?:
    | createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject_typeOfInputText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFunctionCallOutput_Output_union_array_item_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?:
    | createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "image_generation_call";
  id: string;
  result: string | null;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Status_enum;
  action?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Action_enum
    | null;
  background?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Background_enum
    | null;
  error?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object;
  output_format?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_OutputFormat_enum
    | null;
  quality?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Quality_enum
    | null;
  revised_prompt?: string | null;
  size?:
    | string
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Size_enum
    | null;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Status_enum {
  completed,
  failed,
  generating,
  in_progress,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object {
  type: string;
  code: string | null;
  message: string;
  param: string | null;
  moderation_details?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object_ModerationDetails_object;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object_ModerationDetails_object {
  categories: string[];
  moderation_stage: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object_ModerationDetails_object_ModerationStage_enum;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Error_object_ModerationDetails_object_ModerationStage_enum {
  input,
  output,
  `unknown`,
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfImageGenerationCall_Size_enum {
  `1024x1024`,
  `1024x1536`,
  `1536x1024`,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfItemReference
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "item_reference";
  id: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "local_shell_call";
  action: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCall_Action_discriminatedObject;
  call_id: string;
  id: string;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCall_Action_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCall_Action_discriminatedObject_typeOfExec
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCall_Action_discriminatedObject {
  type: "exec";
  command: string[];
  env: createResponse_request_object_Metadata_dictionary;
  timeout_ms?: integer | null;
  user?: string | null;
  working_directory?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfLocalShellCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "local_shell_call_output";
  id: string;
  output: string;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpApprovalRequest
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "mcp_approval_request";
  arguments: string;
  id: string;
  name: string;
  server_label: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpApprovalResponse
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "mcp_approval_response";
  approval_request_id: string;
  approve: boolean;
  id?: string | null;
  reason?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "mcp_call";
  arguments: string;
  id: string;
  name: string;
  server_label: string;
  approval_request_id?: string | null;
  error?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject
    | null;
  output?: string | null;
  status?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Status_enum;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject_typeOfHttpError
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject {
  type: "http_error";
  code: integer;
  message: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject_typeOfMcpProtocolError
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject {
  type: "mcp_protocol_error";
  code: integer;
  message: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject_typeOfMcpToolExecutionError
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Error_discriminatedObject {
  type: "mcp_tool_execution_error";
  content: unknown;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpCall_Status_enum {
  calling,
  completed,
  failed,
  in_progress,
  incomplete,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "mcp_list_tools";
  id: string;
  server_label: string;
  tools: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools_Tools_array_item_object[];
  error?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools_Tools_array_item_object {
  input_schema: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools_Tools_array_item_object_InputSchema_object;
  name: string;
  annotations?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools_Tools_array_item_object_InputSchema_object
    | null;
  description?: string | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMcpListTools_Tools_array_item_object_InputSchema_object {}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "message";
  content: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject[];
  role: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Role_enum;
  id?: string;
  phase?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Phase_enum
    | null;
  status?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfInputFile
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: "input_file";
  detail?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_Detail_enum;
  file_data?: string;
  file_id?: string | null;
  file_url?: url;
  filename?: string;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfInputImage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: "input_image";
  detail: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputImage_Detail_enum;
  file_id?: string | null;
  image_url?: url | null;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfInputText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: "output_text";
  annotations: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject[];
  logprobs: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Logprobs_array_item_object[];
  text: string;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject_typeOfContainerFileCitation
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject {
  type: "container_file_citation";
  container_id: string;
  end_index: integer;
  file_id: string;
  filename: string;
  start_index: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject_typeOfFileCitation
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject {
  type: "file_citation";
  file_id: string;
  filename: string;
  index: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject_typeOfFilePath
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject {
  type: "file_path";
  file_id: string;
  index: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject_typeOfUrlCitation
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Annotations_array_item_discriminatedObject {
  type: "url_citation";
  end_index: integer;
  start_index: integer;
  title: string;
  url: url;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Logprobs_array_item_object {
  bytes: integer[];
  logprob: numeric;
  token: string;
  top_logprobs: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Logprobs_array_item_object_TopLogprobs_array_item_object[];
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfOutputText_Logprobs_array_item_object_TopLogprobs_array_item_object {
  bytes: integer[];
  logprob: numeric;
  token: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject_typeOfRefusal
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_array_item_discriminatedObject {
  type: "refusal";
  refusal: string;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Role_enum {
  assistant,
  developer,
  system,
  user,
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Phase_enum {
  commentary,
  final_answer,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfProgram
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "program";
  call_id: string;
  code: string;
  fingerprint: string;
  id: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfProgramOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "program_output";
  call_id: string;
  id: string;
  result: string;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfProgramOutput_Status_enum;
}

enum createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfProgramOutput_Status_enum {
  completed,
  incomplete,
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "reasoning";
  id: string;
  summary: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Summary_array_item_discriminatedObject[];
  content?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Content_array_item_discriminatedObject[];
  encrypted_content?: string | null;
  status?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Summary_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Summary_array_item_discriminatedObject_typeOfSummaryText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Summary_array_item_discriminatedObject {
  type: "summary_text";
  text: string;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Content_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Content_array_item_discriminatedObject_typeOfReasoningText
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfReasoning_Content_array_item_discriminatedObject {
  type: "reasoning_text";
  text: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "shell_call";
  action: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Action_object;
  call_id: string;
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  environment?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject
    | null;
  id?: string | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Action_object {
  commands: string[];
  max_output_length?: integer | null;
  timeout_ms?: integer | null;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject_typeOfContainerReference
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject {
  type: "container_reference";
  container_id: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject_typeOfLocal
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCall_Environment_discriminatedObject {
  type: "local";
  skills?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfLocal_Skills_array_item_object[];
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "shell_call_output";
  call_id: string;
  output: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object[];
  caller?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfApplyPatchCall_Caller_discriminatedObject
    | null;
  id?: string | null;
  max_output_length?: integer | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object {
  outcome: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject;
  stderr: string;
  stdout: string;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject_typeOfExit
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject {
  type: "exit";
  exit_code: integer;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject_typeOfTimeout
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfShellCallOutput_Output_array_item_object_Outcome_discriminatedObject {
  type: "timeout";
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfToolSearchCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "tool_search_call";
  arguments: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Parameters_object;
  call_id?: string | null;
  execution?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Execution_enum;
  id?: string | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfToolSearchOutput
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "tool_search_output";
  tools: createResponse_request_object_Tools_array_item_discriminatedObject[];
  call_id?: string | null;
  execution?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Execution_enum;
  id?: string | null;
  status?:
    | createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfComputerCall_Status_enum
    | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall
  extends createResponse_request_object_Input_union_array_item_discriminatedObject {
  type: "web_search_call";
  id: string;
  status: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfFileSearchCall_Status_enum;
  action?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject;
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfFindInPage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject {
  type: "find_in_page";
  pattern: string;
  url?: url | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfOpenPage
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject {
  type: "open_page";
  url?: url | null;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfSearch
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject {
  type: "search";
  queries?: string[];
  #deprecated "deprecated"
  query?: string;
  sources?: createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfSearch_Sources_array_item_discriminatedObject[];
}

@discriminator("type")
model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfSearch_Sources_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfSearch_Sources_array_item_discriminatedObject_typeOfUrl
  extends createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfWebSearchCall_Action_discriminatedObject_typeOfSearch_Sources_array_item_discriminatedObject {
  type: "url";
  url: url;
}

model createResponse_request_object_Metadata_dictionary {
  ...Record<string>;
}

enum createResponse_request_object_Model_enum {
  `chatgpt-4o-latest`,
  `codex-mini-latest`,
  `computer-use-preview`,
  `computer-use-preview-2025-03-11`,
  `gpt-3.5-turbo`,
  `gpt-3.5-turbo-0125`,
  `gpt-3.5-turbo-0301`,
  `gpt-3.5-turbo-0613`,
  `gpt-3.5-turbo-1106`,
  `gpt-3.5-turbo-16k`,
  `gpt-3.5-turbo-16k-0613`,
  `gpt-4`,
  `gpt-4-0125-preview`,
  `gpt-4-0314`,
  `gpt-4-0613`,
  `gpt-4-1106-preview`,
  `gpt-4-32k`,
  `gpt-4-32k-0314`,
  `gpt-4-32k-0613`,
  `gpt-4-turbo`,
  `gpt-4-turbo-2024-04-09`,
  `gpt-4-turbo-preview`,
  `gpt-4-vision-preview`,
  `gpt-4.1`,
  `gpt-4.1-2025-04-14`,
  `gpt-4.1-mini`,
  `gpt-4.1-mini-2025-04-14`,
  `gpt-4.1-nano`,
  `gpt-4.1-nano-2025-04-14`,
  `gpt-4o`,
  `gpt-4o-2024-05-13`,
  `gpt-4o-2024-08-06`,
  `gpt-4o-2024-11-20`,
  `gpt-4o-audio-preview`,
  `gpt-4o-audio-preview-2024-10-01`,
  `gpt-4o-audio-preview-2024-12-17`,
  `gpt-4o-audio-preview-2025-06-03`,
  `gpt-4o-mini`,
  `gpt-4o-mini-2024-07-18`,
  `gpt-4o-mini-audio-preview`,
  `gpt-4o-mini-audio-preview-2024-12-17`,
  `gpt-4o-mini-search-preview`,
  `gpt-4o-mini-search-preview-2025-03-11`,
  `gpt-4o-search-preview`,
  `gpt-4o-search-preview-2025-03-11`,
  `gpt-5`,
  `gpt-5-2025-08-07`,
  `gpt-5-chat-latest`,
  `gpt-5-codex`,
  `gpt-5-mini`,
  `gpt-5-mini-2025-08-07`,
  `gpt-5-nano`,
  `gpt-5-nano-2025-08-07`,
  `gpt-5-pro`,
  `gpt-5-pro-2025-10-06`,
  `gpt-5.1`,
  `gpt-5.1-2025-11-13`,
  `gpt-5.1-chat-latest`,
  `gpt-5.1-codex`,
  `gpt-5.1-codex-max`,
  `gpt-5.1-mini`,
  `gpt-5.2`,
  `gpt-5.2-2025-12-11`,
  `gpt-5.2-chat-latest`,
  `gpt-5.2-pro`,
  `gpt-5.2-pro-2025-12-11`,
  `gpt-5.3-chat-latest`,
  `gpt-5.4`,
  `gpt-5.4-mini`,
  `gpt-5.4-mini-2026-03-17`,
  `gpt-5.4-nano`,
  `gpt-5.4-nano-2026-03-17`,
  `gpt-5.5`,
  `gpt-5.5-2026-04-23`,
  `gpt-5.5-pro`,
  `gpt-5.5-pro-2026-04-23`,
  `gpt-5.6-cyber`,
  `gpt-5.6-luna`,
  `gpt-5.6-sol`,
  `gpt-5.6-terra`,
  `gpt-6-astra`,
  `gpt-6-luna`,
  `gpt-6-sol`,
  `gpt-6.1-sol`,
  `gpt-6.1-terra`,
  `gpt-audio-mini`,
  `gpt-audio-mini-2025-12-15`,
  `gpt-daybreak-blue-latest`,
  `gpt-daybreak-red-latest`,
  `gpt-rosalind-research`,
  o1,
  `o1-2024-12-17`,
  `o1-mini`,
  `o1-mini-2024-09-12`,
  `o1-preview`,
  `o1-preview-2024-09-12`,
  `o1-pro`,
  `o1-pro-2025-03-19`,
  o3,
  `o3-2025-04-16`,
  `o3-deep-research`,
  `o3-deep-research-2025-06-26`,
  `o3-mini`,
  `o3-mini-2025-01-31`,
  `o3-pro`,
  `o3-pro-2025-06-10`,
  `o4-mini`,
  `o4-mini-2025-04-16`,
  `o4-mini-deep-research`,
  `o4-mini-deep-research-2025-06-26`,
}

model createResponse_request_object_Moderation_object {
  `model`: string;
  policy?: createResponse_request_object_Moderation_object_Policy_object | null;
}

model createResponse_request_object_Moderation_object_Policy_object {
  input?: createResponse_request_object_Moderation_object_Policy_object_Input_object | null;
  output?: createResponse_request_object_Moderation_object_Policy_object_Input_object | null;
}

model createResponse_request_object_Moderation_object_Policy_object_Input_object {
  mode: createResponse_request_object_Moderation_object_Policy_object_Input_object_Mode_enum;
}

enum createResponse_request_object_Moderation_object_Policy_object_Input_object_Mode_enum {
  block,
  score,
}

model createResponse_request_object_Prompt_object {
  id: string;
  variables?: createResponse_request_object_Prompt_object_Variables_dictionary | null;
  version?: string | null;
}

model createResponse_request_object_Prompt_object_Variables_dictionary {
  ...Record<createResponse_request_object_Prompt_object_Variables_dictionary_value_union>;
}

@oneOf
union createResponse_request_object_Prompt_object_Variables_dictionary_value_union {
  string,
  createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject,
}

@discriminator("type")
model createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject {
  type: string;
}

model createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile
  extends createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject {
  type: "input_file";
  detail?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_Detail_enum;
  file_data?: string;
  file_id?: string | null;
  file_url?: url;
  filename?: string;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

enum createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_Detail_enum {
  auto,
  high,
  low,
}

model createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object {
  mode: "explicit";
  ttl?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object_Ttl_enum;
}

enum createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object_Ttl_enum {
  `12h`,
  `30m`,
}

model createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputImage
  extends createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject {
  type: "input_image";
  detail: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputImage_Detail_enum;
  file_id?: string | null;
  image_url?: url | null;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

enum createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputImage_Detail_enum {
  auto,
  high,
  low,
  original,
}

model createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputText
  extends createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?: createResponse_request_object_Prompt_object_Variables_dictionary_value_union_discriminatedObject_typeOfInputFile_PromptCacheBreakpoint_object;
}

model createResponse_request_object_PromptCacheOptions_object {
  comparison_response_id?: string | null;
  mode?: createResponse_request_object_PromptCacheOptions_object_Mode_enum;
  prewarm?: boolean;
  ttl?: "30m";
}

enum createResponse_request_object_PromptCacheOptions_object_Mode_enum {
  explicit,
  implicit,
}

enum createResponse_request_object_PromptCacheRetention_enum {
  `24h`,
  in_memory,
}

model createResponse_request_object_Reasoning_object {
  context?: createResponse_request_object_Reasoning_object_Context_enum | null;
  effort?: createResponse_request_object_Reasoning_object_Effort_enum | null;
  generate_summary?: createResponse_request_object_Reasoning_object_GenerateSummary_enum | null;
  mode?: string | createResponse_request_object_Reasoning_object_Mode_enum;
  summary?: createResponse_request_object_Reasoning_object_Summary_enum | null;
}

enum createResponse_request_object_Reasoning_object_Context_enum {
  all_turns,
  auto,
  current_turn,
}

enum createResponse_request_object_Reasoning_object_Effort_enum {
  high,
  low,
  max,
  medium,
  minimal,
  none,
  xhigh,
}

#deprecated "deprecated"
enum createResponse_request_object_Reasoning_object_GenerateSummary_enum {
  auto,
  concise,
  detailed,
}

enum createResponse_request_object_Reasoning_object_Mode_enum {
  pro,
  standard,
}

enum createResponse_request_object_Reasoning_object_Summary_enum {
  auto,
  concise,
  detailed,
}

enum createResponse_request_object_ServiceTier_enum {
  auto,
  default,
  fast,
  flex,
  priority,
  scale,
  ultrafast,
}

model createResponse_request_object_StreamOptions_object {
  include_obfuscation?: boolean;
}

model createResponse_request_object_Text_object {
  format?: createResponse_request_object_Text_object_Format_discriminatedObject;
  verbosity?: createResponse_request_object_Text_object_Verbosity_enum | null;
}

@discriminator("type")
model createResponse_request_object_Text_object_Format_discriminatedObject {
  type: string;
}

model createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonObject
  extends createResponse_request_object_Text_object_Format_discriminatedObject {
  type: "json_object";
}

model createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema
  extends createResponse_request_object_Text_object_Format_discriminatedObject {
  type: "json_schema";
  name: string;
  schema: createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema_Schema_dictionary;
  description?: string;
  strict?: boolean | null;
}

model createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema_Schema_dictionary {
  ...Record<unknown>;
}

model createResponse_request_object_Text_object_Format_discriminatedObject_typeOfText
  extends createResponse_request_object_Text_object_Format_discriminatedObject {
  type: "text";
}

enum createResponse_request_object_Text_object_Verbosity_enum {
  high,
  low,
  medium,
}

@oneOf
union createResponse_request_object_ToolChoice_union {
  createResponse_request_object_ToolChoice_union_enum,
  createResponse_request_object_ToolChoice_union_discriminatedObject,
}

enum createResponse_request_object_ToolChoice_union_enum {
  auto,
  none,
  required,
}

@discriminator("type")
model createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: string;
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfAllowedTools
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "allowed_tools";
  mode: createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfAllowedTools_Mode_enum;
  tools: createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema_Schema_dictionary[];
}

enum createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfAllowedTools_Mode_enum {
  auto,
  required,
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfApplyPatch
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "apply_patch";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfCodeInterpreter
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "code_interpreter";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfComputer
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "computer";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfComputerUse
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "computer_use";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfComputerUsePreview
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "computer_use_preview";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfCustom
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "custom";
  name: string;
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfFileSearch
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "file_search";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfFunction
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "function";
  name: string;
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfImageGeneration
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "image_generation";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfMcp
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "mcp";
  server_label: string;
  name?: string | null;
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfProgrammaticToolCalling
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "programmatic_tool_calling";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfShell
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "shell";
}

model createResponse_request_object_ToolChoice_union_discriminatedObject_typeOfWebSearchPreview
  extends createResponse_request_object_ToolChoice_union_discriminatedObject {
  type: "web_search_preview";
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "apply_patch";
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum {
  direct,
  programmatic,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "code_interpreter";
  container: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
}

@oneOf
union createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union {
  string,
  createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union_discriminatedObject,
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union_discriminatedObject_typeOfAuto
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCodeInterpreter_Container_union_discriminatedObject {
  type: "auto";
  file_ids?: string[];
  memory_limit?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_MemoryLimit_enum
    | null;
  network_policy?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfComputer
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "computer";
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfComputerUsePreview
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "computer_use_preview";
  display_height: integer;
  display_width: integer;
  environment: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfComputerUsePreview_Environment_enum;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfComputerUsePreview_Environment_enum {
  browser,
  linux,
  mac,
  ubuntu,
  windows,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "custom";
  name: string;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  async?: boolean;
  defer_loading?: boolean;
  description?: string;
  format?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject;
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject_typeOfGrammar
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject {
  type: "grammar";
  definition: string;
  syntax: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject_typeOfGrammar_Syntax_enum;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject_typeOfGrammar_Syntax_enum {
  lark,
  regex,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject_typeOfText
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject {
  type: "text";
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "file_search";
  vector_store_ids: string[];
  filters?:
    | createResponse_request_object_Text_object_Format_discriminatedObject_typeOfJsonSchema_Schema_dictionary
    | null;
  max_num_results?: integer;
  ranking_options?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object {
  hybrid_search?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object_HybridSearch_object;
  ranker?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object_Ranker_enum;
  score_threshold?: numeric;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object_HybridSearch_object {
  embedding_weight: numeric;
  text_weight: numeric;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFileSearch_RankingOptions_object_Ranker_enum {
  auto,
  `default-2024-11-15`,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFunction
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "function";
  name: string;
  parameters:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFunction_Parameters_dictionary
    | null;
  strict: boolean | null;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  async?: boolean;
  defer_loading?: boolean;
  description?: string | null;
  output_schema?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFunction_Parameters_dictionary
    | null;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFunction_Parameters_dictionary {
  ...Record<unknown>;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "image_generation";
  action?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Action_enum;
  background?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Background_enum;
  input_fidelity?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_InputFidelity_enum
    | null;
  input_image_mask?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_InputImageMask_object;
  `model`?:
    | string
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Model_enum;
  moderation?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Moderation_enum;
  output_compression?: integer;
  output_format?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_OutputFormat_enum;
  partial_images?: integer;
  quality?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Quality_enum;
  size?:
    | string
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Size_enum;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Action_enum {
  auto,
  edit,
  generate,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Background_enum {
  auto,
  opaque,
  transparent,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_InputFidelity_enum {
  high,
  low,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_InputImageMask_object {
  file_id?: string;
  image_url?: string;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Model_enum {
  `gpt-image-1`,
  `gpt-image-1-mini`,
  `gpt-image-1.5`,
  `gpt-image-2`,
  `gpt-image-2-2026-04-21`,
  `gpt-image-2.5-flare`,
  `gpt-image-2.5-flare-2026-09-08`,
  `gpt-image-2.5-sunburst`,
  `gpt-image-2.5-sunburst-2026-09-08`,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Moderation_enum {
  auto,
  low,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_OutputFormat_enum {
  jpeg,
  png,
  webp,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Quality_enum {
  auto,
  high,
  low,
  max,
  medium,
  xhigh,
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfImageGeneration_Size_enum {
  `1024x1024`,
  `1024x1536`,
  `1536x1024`,
  auto,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfLocalShell
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "local_shell";
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "mcp";
  server_label: string;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  allowed_tools?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union
    | null;
  authorization?: string;
  connector_id?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_ConnectorId_enum;
  defer_loading?: boolean;
  headers?: createResponse_request_object_Metadata_dictionary | null;
  require_approval?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union
    | null;
  server_description?: string;
  server_url?: url;
  tunnel_id?: string;
}

@oneOf
union createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union {
  string[],
  createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union_object,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union_object {
  read_only?: boolean;
  tool_names?: string[];
}

#deprecated "deprecated"
enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_ConnectorId_enum {
  connector_dropbox,
  connector_gmail,
  connector_googlecalendar,
  connector_googledrive,
  connector_microsoftteams,
  connector_outlookcalendar,
  connector_outlookemail,
  connector_sharepoint,
}

@oneOf
union createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union {
  createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union_object,
  createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union_enum,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union_object {
  always?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union_object;
  `never`?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_AllowedTools_union_object;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfMcp_RequireApproval_union_enum {
  always,
  `never`,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "namespace";
  description: string;
  name: string;
  tools: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject[];
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject_typeOfCustom
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject {
  type: "custom";
  name: string;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  async?: boolean;
  defer_loading?: boolean;
  description?: string;
  format?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfCustom_Format_discriminatedObject;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject_typeOfFunction
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfNamespace_Tools_array_item_discriminatedObject {
  type: "function";
  name: string;
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  async?: boolean;
  defer_loading?: boolean;
  description?: string | null;
  output_schema?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfFunction_Parameters_dictionary
    | null;
  parameters?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Parameters_object
    | null;
  strict?: boolean | null;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfProgrammaticToolCalling
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "programmatic_tool_calling";
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "shell";
  allowed_callers?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfApplyPatch_AllowedCallers_array_item_enum[]
    | null;
  environment?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject
    | null;
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject {
  type: "container_auto";
  file_ids?: string[];
  memory_limit?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_MemoryLimit_enum
    | null;
  network_policy?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject;
  skills?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject[];
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_MemoryLimit_enum {
  `16g`,
  `1g`,
  `4g`,
  `64g`,
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject_typeOfAllowlist
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject {
  type: "allowlist";
  allowed_domains: string[];
  domain_secrets?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject_typeOfAllowlist_DomainSecrets_array_item_object[];
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject_typeOfAllowlist_DomainSecrets_array_item_object {
  domain: string;
  name: string;
  value: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject_typeOfDisabled
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_NetworkPolicy_discriminatedObject {
  type: "disabled";
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfInline
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject {
  type: "inline";
  description: string;
  name: string;
  source: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfInline_Source_discriminatedObject;
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfInline_Source_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfInline_Source_discriminatedObject_typeOfBase64
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfInline_Source_discriminatedObject {
  type: "base64";
  data: string;
  media_type: "application/zip";
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject_typeOfSkillReference
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerAuto_Skills_array_item_discriminatedObject {
  type: "skill_reference";
  skill_id: string;
  version?: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfContainerReference
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject {
  type: "container_reference";
  container_id: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfLocal
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject {
  type: "local";
  skills?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfLocal_Skills_array_item_object[];
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfShell_Environment_discriminatedObject_typeOfLocal_Skills_array_item_object {
  description: string;
  name: string;
  path: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "tool_search";
  description?: string | null;
  execution?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Execution_enum;
  parameters?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Parameters_object
    | null;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Execution_enum {
  client,
  server,
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfToolSearch_Parameters_object {}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "web_search";
  external_web_access?: boolean;
  filters?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_Filters_object
    | null;
  search_context_size?: createResponse_request_object_Text_object_Verbosity_enum;
  user_location?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_UserLocation_discriminatedObject
    | null;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_Filters_object {
  allowed_domains?: string[] | null;
}

@discriminator("type")
model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_UserLocation_discriminatedObject {
  type: string;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_UserLocation_discriminatedObject_typeOfApproximate
  extends createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_UserLocation_discriminatedObject {
  type: "approximate";
  city?: string | null;
  country?: string | null;
  region?: string | null;
  timezone?: string | null;
}

model createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearchPreview
  extends createResponse_request_object_Tools_array_item_discriminatedObject {
  type: "web_search_preview";
  search_content_types?: createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearchPreview_SearchContentTypes_array_item_enum[];
  search_context_size?: createResponse_request_object_Text_object_Verbosity_enum;
  user_location?:
    | createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearch_UserLocation_discriminatedObject
    | null;
}

enum createResponse_request_object_Tools_array_item_discriminatedObject_typeOfWebSearchPreview_SearchContentTypes_array_item_enum {
  image,
  text,
}

enum createResponse_request_object_Truncation_enum {
  auto,
  disabled,
}

@tag("Responses")
@route("/responses")
@post
@summary("Create a model response")
op createResponse(
  @body body: createResponse_request_object,
): GeneratedHelpers.DefaultResponse<Description = "Responses omitted by RequestBody extraction mode.">;

namespace GeneratedHelpers {
  @doc(Description)
  @error
  model DefaultResponse<
    Description extends valueof string = "",
    Body = void,
    Headers extends {} = {}
  > {
    @body body: Body;
    ...Headers;
  }
}
