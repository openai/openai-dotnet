import "@typespec/openapi";

using TypeSpec.OpenAPI;

namespace OpenAI.Decisions;

model createDecision_request_object {
  input: createDecision_request_object_Input_union;
  `model`: string;
  questions: createDecision_request_object_Questions_array_item_discriminatedObject[];
  safety_identifier?: string | null;
}

union createDecision_request_object_Input_union {
  createDecision_request_object_Input_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_discriminatedObject {
  type: createDecision_request_object_Input_union_array_item_discriminatedObject_discriminator;
}

union createDecision_request_object_Input_union_array_item_discriminatedObject_discriminator {
  string,
  message: "message",
}

model createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage
  extends createDecision_request_object_Input_union_array_item_discriminatedObject {
  type: createDecision_request_object_Input_union_array_item_discriminatedObject_discriminator.message;
  content: createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union;
  role: "user";
}

union createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union {
  createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject[],
  string,
}

@discriminator("type")
model createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject {
  type: createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_discriminator;
}

union createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_discriminator {
  string,
  input_image: "input_image",
  input_text: "input_text",
}

model createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_typeOfInputImage
  extends createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject {
  type: createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_discriminator.input_image;
  image_url: string;
  detail?:
    | createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum
    | null;
}

union createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum {
  string,
  auto: "auto",
  high: "high",
  low: "low",
  original: "original",
}

model createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_typeOfInputText
  extends createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject {
  type: createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_discriminator.input_text;
  text: string;
}

@discriminator("type")
model createDecision_request_object_Questions_array_item_discriminatedObject {
  type: createDecision_request_object_Questions_array_item_discriminatedObject_discriminator;
}

union createDecision_request_object_Questions_array_item_discriminatedObject_discriminator {
  string,
  choice: "choice",
  predicate: "predicate",
  score: "score",
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: createDecision_request_object_Questions_array_item_discriminatedObject_discriminator.choice;
  choices: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Choices_array_item_object[];
  instructions: string;
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfChoice_Choices_array_item_object {
  value: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union;
  description?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfPredicate
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: createDecision_request_object_Questions_array_item_discriminatedObject_discriminator.predicate;
  instructions: string;
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore
  extends createDecision_request_object_Questions_array_item_discriminatedObject {
  type: createDecision_request_object_Questions_array_item_discriminatedObject_discriminator.score;
  instructions: string;
  levels: createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore_Levels_array_item_object[];
  name?: string;
}

model createDecision_request_object_Questions_array_item_discriminatedObject_typeOfScore_Levels_array_item_object {
  label: string;
  description?: string;
}

model createDecision_response_200_object {
  answers: createDecision_response_200_object_Answers_array_item_discriminatedObject[];
  `model`: string;
  usage: createDecision_response_200_object_Usage_object;
}

@discriminator("type")
model createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator;
}

union createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator {
  string,
  choice: "choice",
  predicate: "predicate",
  refusal: "refusal",
  score: "score",
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator.choice;
  choice: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Choice_union;
  confidence: numeric;
  name: string | null;
  probabilities: createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfChoice_Probabilities_array_item_object[];
}

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
  type: createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator.predicate;
  name: string | null;
  probability: numeric;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfRefusal
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator.refusal;
  name: string | null;
}

model createDecision_response_200_object_Answers_array_item_discriminatedObject_typeOfScore
  extends createDecision_response_200_object_Answers_array_item_discriminatedObject {
  type: createDecision_response_200_object_Answers_array_item_discriminatedObject_discriminator.score;
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