import "./models.tsp";

namespace OpenAI;

// Tool customization (apply_discriminator): Apply discriminated type base for tools
@doc("""
  A tool that generates images using a model like `gpt-image-1`.
  """)
model ImageGenTool extends Tool {
  @doc("""
    The type of the image generation tool. Always `image_generation`.
    """)
  type: ToolType.image_generation;

  @doc("""
    The image generation model to use. Default: `gpt-image-1`.
    """)
  `model`?: "gpt-image-1" = "gpt-image-1";

  @doc("""
    The quality of the generated image. One of `low`, `medium`, `high`,
    or `auto`. Default: `auto`.
    """)
  quality?: "low" | "medium" | "high" | "auto" = "auto";

  @doc("""
    The size of the generated image. One of `1024x1024`, `1024x1536`,
    `1536x1024`, or `auto`. Default: `auto`.
    """)
  size?: "1024x1024" | "1024x1536" | "1536x1024" | "auto" = "auto";

  @doc("""
    The output format of the generated image. One of `png`, `webp`, or
    `jpeg`. Default: `png`.
    """)
  output_format?: "png" | "webp" | "jpeg" = "png";

  /** Compression level for the output image. Default: 100. */
  @minValue(0)
  @maxValue(100)
  output_compression?: int32 = 100;

  @doc("""
    Moderation level for the generated image. Default: `auto`.
    """)
  moderation?: "auto" | "low" = "auto";

  @doc("""
    Background type for the generated image. One of `transparent`,
    `opaque`, or `auto`. Default: `auto`.
    """)
  background?: "transparent" | "opaque" | "auto" = "auto";

  @doc("""
    Control how much effort the model will exert to match the style and features,
    especially facial features, of input images. This parameter is only supported
    for `gpt-image-1`. Supports `high` and `low`. Defaults to `low`.
    """)
  input_fidelity?: "low" | "high" = "low";

  @doc("""
    Optional mask for inpainting. Contains `image_url`
    (string, optional) and `file_id` (string, optional).
    """)
  input_image_mask?: {
    /** Base64-encoded mask image. */
    image_url?: url;

    /** File ID for the mask image. */
    file_id?: string;
  };

  /** Number of partial images to generate in streaming mode, from 0 (default value) to 3. */
  @minValue(0)
  @maxValue(3)
  partial_images?: int32 = 0;
}

// Tool customization: Remove shared sequence_number property that was moved to the common parent
/** Emitted when an image generation tool call has completed and the final image is available. */
model ResponseImageGenCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.completed'. */
  type: ResponseStreamEventType.response_image_generation_call_completed;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

// Tool customization: Remove shared sequence_number property that was moved to the common parent
/** Emitted when an image generation tool call is actively generating an image (intermediate state). */
model ResponseImageGenCallGeneratingEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.generating'. */
  type: ResponseStreamEventType.response_image_generation_call_generating;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

// Tool customization: Remove shared sequence_number property that was moved to the common parent
/** Emitted when an image generation tool call is in progress. */
model ResponseImageGenCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.in_progress'. */
  type: ResponseStreamEventType.response_image_generation_call_in_progress;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

// Tool customization: Remove shared sequence_number property that was moved to the common parent
/** Emitted when a partial image is available during image generation streaming. */
model ResponseImageGenCallPartialImageEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.partial_image'. */
  type: ResponseStreamEventType.response_image_generation_call_partial_image;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;

  /** 0-based index for the partial image (backend is 1-based, but this is 0-based for the user). */
  partial_image_index: int32;

  /** Base64-encoded partial image data, suitable for rendering as an image. */
  @encode("base64", string)
  partial_image_b64: bytes;
}
