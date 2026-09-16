import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

// ------------ Session input ------------

/** The type of content included in an input message. */
union InputContentType {
  string,
  input_text: "input_text",
  input_image: "input_image",
}

@discriminator("type")
model InputContentParam {
  type: InputContentType;
}

/** Text input to the model. */
model InputContentParamInputText extends InputContentParam {
  /** The type of the object. Always `input_text`. */
  type: InputContentType.input_text;

  /** The text sent to the model. */
  text: string;
}

/** Image input to the model. */
model InputContentParamInputImage extends InputContentParam {
  /** The type of the object. Always `input_image`. */
  type: InputContentType.input_image;

  /** The URL of the image sent to the model. */
  image_url: string;
}

/** A user message submitted to a session. */
model InputMessageParam {
  /** The type of the input item. Always `message`. */
  type?: "message";

  /** The role of the message author. Always `user`. */
  role: "user";

  /** The content of the message. */
  @maxItems(16384)
  content: InputContentParam[];
}
