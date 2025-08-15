import "../assistants";

using TypeSpec.OpenAPI;

namespace OpenAI;

union MessageContentType {
  /** The content is a text message. */
  text: "text",

  /** The content is an image file. */
  image_file: "image_file",

  /** The content is an image URL. */
  image_url: "image_url",

  /** The content is a refusal message. */
  refusal: "refusal",
}

/**
 * Represents a single piece of content in an Assistants API message.
 */
@discriminator("type")
model MessageContent {
  type: MessageContentType;
}

union MessageContentTextAnnotationType {
  string,
  file_citation: "file_citation",
  file_path: "file_path",
}

@discriminator("type")
model MessageContentTextObjectAnnotation {
  /** The discriminated type identifier for the content item. */
  type: MessageContentTextAnnotationType;
}

/**
 * Represents a single piece of incremental content in an Assistants API streaming response.
 */
@discriminator("type")
model MessageDeltaContent {
  /** The discriminated type identifier for the content item. */
  type: MessageContentType;
}

@discriminator("type")
model MessageDeltaTextContentAnnotation {
  /** The discriminated type identifier for the content item. */
  type: MessageContentTextAnnotationType;
}
