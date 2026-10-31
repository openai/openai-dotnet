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
namespace OpenAI;

model createDecision_request_object {
  input: createDecision_request_object_Input_union;
  `model`: string;
  questions: createDecision_request_object_Questions_array_item_discriminatedObject[];
  prompt_cache_key?: string | null;
  prompt_cache_options?: createDecision_request_object_PromptCacheOptions_object;
  safety_identifier?: string | null;
}

@oneOf
union createDecision_request_object_Input_union {
  createDecision_request_object_Input_union_array_item_union[],
  string,
}

@oneOf
union createDecision_request_object_Input_union_array_item_union {
  createDecision_request_object_Input_union_array_item_union_discriminatedObject1,
  createDecision_request_object_Input_union_array_item_union_discriminatedObject2,
}

@discriminator("role")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject1 {
  role: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1 {
  type: "message";
  content: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union;
  role: "assistant";
  phase?:
    | createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Phase_enum
    | null;
}

@oneOf
union createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union {
  createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject_typeOfOutputText
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject {
  type: "output_text";
  text: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject_typeOfRefusal
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Content_union_array_item_discriminatedObject {
  type: "refusal";
  refusal: string;
}

enum createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfAssistant_Phase_enum {
  commentary,
  final_answer,
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1 {
  type: "message";
  content: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union;
  role: "developer";
}

@oneOf
union createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union {
  createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object;
}

union createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object {
  {
    mode: "explicit",
  },
  null,
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfSystem
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1 {
  type: "message";
  content: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union;
  role: "system";
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1 {
  type: "message";
  content: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union;
  role: "user";
}

@oneOf
union createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union {
  createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject_typeOfInputImage
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject {
  type: "input_image";
  image_url: string;
  detail?:
    | createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum
    | null;
  prompt_cache_breakpoint?: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject_typeOfInputJson
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject {
  type: "input_json";
  data: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject_typeOfInputText
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfUser_Content_union_array_item_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object;
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject2 {
  type: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2 {
  type: "custom_tool_call";
  call_id: string;
  input: string;
  name: string;
  id?: string | null;
  `namespace`?: string | null;
  status?:
    | createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall_Status_enum
    | null;
}

enum createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall_Status_enum {
  completed,
  in_progress,
  incomplete,
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2 {
  type: "custom_tool_call_output";
  call_id: string;
  output: createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union;
  id?: string | null;
  status?:
    | createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall_Status_enum
    | null;
}

@oneOf
union createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union {
  createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject_typeOfInputImage
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject {
  type: "input_image";
  image_url: string;
  detail?:
    | createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum
    | null;
  prompt_cache_breakpoint?: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject_typeOfInputText
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union_array_item_discriminatedObject {
  type: "input_text";
  text: string;
  prompt_cache_breakpoint?: createDecision_request_object_Input_union_array_item_union_discriminatedObject1_roleOfDeveloper_Content_union_array_item_discriminatedObject_typeOfInputText_PromptCacheBreakpoint_object;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfFunctionCall
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2 {
  type: "function_call";
  arguments: string;
  call_id: string;
  name: string;
  id?: string | null;
  `namespace`?: string | null;
  status?:
    | createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall_Status_enum
    | null;
}

model createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfFunctionCallOutput
  extends createDecision_request_object_Input_union_array_item_union_discriminatedObject2 {
  type: "function_call_output";
  call_id: string;
  output: createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCallOutput_Output_union;
  id?: string | null;
  status?:
    | createDecision_request_object_Input_union_array_item_union_discriminatedObject2_typeOfCustomToolCall_Status_enum
    | null;
}

@discriminator("type")
model createDecision_request_object_Questions_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: "choice";
  choices: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Choices_array_item_object[];
  instructions: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union;
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Choices_array_item_object {
  value: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union;
  description?: string;
}

@oneOf
union createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union {
  createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject {
  type: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputImage
  extends createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject {
  type: "input_image";
  image_url: string;
  detail?:
    | createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum
    | null;
}

enum createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum {
  auto,
  high,
  low,
  original,
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson
  extends createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject {
  type: "input_json";
  data: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union;
}

@oneOf
union createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union {
  createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union_dictionary[],
  createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union_dictionary,
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputJson_Data_union_dictionary {
  ...Record<unknown | null>;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject_typeOfInputText
  extends createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union_array_item_discriminatedObject {
  type: "input_text";
  text: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfPredicate
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: "predicate";
  instructions: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union;
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: "score";
  instructions: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Instructions_union;
  levels: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore_Levels_array_item_object[];
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore_Levels_array_item_object {
  label: string;
  description?: string;
}

union createDecision_request_object_PromptCacheOptions_object {
  {
    mode?: "explicit" | "implicit" | null | null,
    ttl?: "30m",
  },
  null,
}

model createDecision_response_200_object {
  answers: createDecision_response_200_object_Answers_array_item_discriminatedObject[];
  `model`: string;
  usage: createDecision_response_200_object_Usage_object;
}

@discriminator("type")
model createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: string;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: "choice";
  choice: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union;
  confidence: numeric;
  name: string | null;
  probabilities: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Probabilities_array_item_object[];
}

@oneOf
union createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union {
  boolean,
  string,
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Probabilities_array_item_object {
  probability: numeric;
  value: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfPredicate
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: "predicate";
  name: string | null;
  probability: numeric;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfRefusal
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: "refusal";
  name: string | null;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfScore
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: "score";
  confidence: numeric;
  name: string | null;
  probabilities: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfScore_Probabilities_array_item_object[];
  score: numeric;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfScore_Probabilities_array_item_object {
  label: string;
  probability: numeric;
  value: int64;
}

model createDecision_response_200_object_Usage_object {
  input_tokens: int64;
  input_tokens_details: createDecision_response_200_object_Usage_object_InputTokensDetails_object;
  output_tokens: int64;
  output_tokens_details: createDecision_response_200_object_Usage_object_OutputTokensDetails_object;
  total_tokens: int64;
}

model createDecision_response_200_object_Usage_object_InputTokensDetails_object {
  cache_write_tokens: int64;
  cached_tokens: int64;
}

model createDecision_response_200_object_Usage_object_OutputTokensDetails_object {
  reasoning_tokens: int64;
}

@tag("Decisions")
@route("/decisions")
@post
@summary("Create a decision")
op createDecision(@body body: createDecision_request_object): createDecision_response_200_object;
