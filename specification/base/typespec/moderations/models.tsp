/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model CreateModerationRequest {
  /**
   * Input (or inputs) to classify. Can be a single string, an array of strings, or
   * an array of multi-modal input objects similar to other models.
   */
  input: string | string[] | ({
    @doc("""
      Always `image_url`.
      """)
    type: "image_url";

    /** Contains either an image URL or a data URL for a base64 encoded image. */
    image_url: {
      /** Either a URL of the image or the base64 encoded image data. */
      url: string;
    };
  } | {
    @doc("""
      Always `text`.
      """)
    type: "text";

    /** A string of text to classify. */
    text: string;
  })[];

  /**
   * The content moderation model you would like to use. Learn more in
   * [the moderation guide](/docs/guides/moderation), and learn about
   * available models [here](/docs/models#moderation).
   */
  @extension("x-oaiTypeLabel", "string")
  `model`?:
    | string
    | "omni-moderation-latest"
    | "omni-moderation-2024-09-26"
    | "text-moderation-latest"
    | "text-moderation-stable" = "omni-moderation-latest";
}

/** Represents if a given text input is potentially harmful. */
model CreateModerationResponse {
  /** The unique identifier for the moderation request. */
  id: string;

  /** The model used to generate the moderation results. */
  `model`: string;

  // Tool customization: Employ shared applied input type definitions via customized result items
  /** A list of moderation objects. */
  results: CreateModerationResponseResult[];
}
