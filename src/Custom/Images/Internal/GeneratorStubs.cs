namespace OpenAI.Images;

// CUSTOM: Made internal.

[CodeGenType("ImagesResponseBackground")] internal readonly partial struct InternalImagesResponseBackground { }
[CodeGenType("ImagesResponseOutputFormat")] internal readonly partial struct InternalImagesResponseOutputFormat { }
[CodeGenType("ImagesResponseQuality")] internal readonly partial struct InternalImagesResponseQuality { }
[CodeGenType("ImagesResponseSize")] internal readonly partial struct InternalImagesResponseSize { }

// Image generation.
[CodeGenType("CreateImageRequestModel")] internal readonly partial struct InternalCreateImageRequestModel { }

// Image edit generation.
[CodeGenType("CreateImageEditRequestBackground")] internal readonly partial struct InternalCreateImageEditRequestBackground { }
[CodeGenType("CreateImageEditRequestModel")] internal readonly partial struct InternalCreateImageEditRequestModel { }
[CodeGenType("CreateImageEditRequestOutputFormat")] internal readonly partial struct InternalCreateImageEditRequestOutputFormat { }
[CodeGenType("CreateImageEditRequestQuality")] internal readonly partial struct InternalCreateImageEditRequestQuality { }
[CodeGenType("CreateImageEditRequestResponseFormat")] internal readonly partial struct InternalCreateImageEditRequestResponseFormat { }
[CodeGenType("CreateImageEditRequestSize")] internal readonly partial struct InternalCreateImageEditRequestSize { }

// Image variation generation.
[CodeGenType("CreateImageVariationRequestModel")] internal readonly partial struct InternalCreateImageVariationRequestModel { }
[CodeGenType("CreateImageVariationRequestResponseFormat")] internal readonly partial struct InternalCreateImageVariationRequestResponseFormat { }
[CodeGenType("CreateImageVariationRequestSize")] internal readonly partial struct InternalCreateImageVariationRequestSize { }

// Streaming image generation.
[CodeGenType("ImageGenStreamEventType")] internal readonly partial struct InternalImageGenStreamEventType { }
[CodeGenType("ImageGenStreamEvent")] internal partial class InternalImageGenStreamEvent { }
[CodeGenType("ImageGenPartialImageEvent")] internal partial class InternalImageGenPartialImageEvent { }
[CodeGenType("ImageGenCompletedEvent")] internal partial class InternalImageGenCompletedEvent { }
[CodeGenType("UnknownImageGenStreamEvent")] internal partial class InternalUnknownImageGenStreamEvent { }
[CodeGenType("ImageGenPartialImageEventBackground")] internal readonly partial struct InternalImageGenPartialImageEventBackground { }
[CodeGenType("ImageGenPartialImageEventOutputFormat")] internal readonly partial struct InternalImageGenPartialImageEventOutputFormat { }
[CodeGenType("ImageGenPartialImageEventQuality")] internal readonly partial struct InternalImageGenPartialImageEventQuality { }
[CodeGenType("ImageGenPartialImageEventSize")] internal readonly partial struct InternalImageGenPartialImageEventSize { }
[CodeGenType("ImageGenCompletedEventBackground")] internal readonly partial struct InternalImageGenCompletedEventBackground { }
[CodeGenType("ImageGenCompletedEventOutputFormat")] internal readonly partial struct InternalImageGenCompletedEventOutputFormat { }
[CodeGenType("ImageGenCompletedEventQuality")] internal readonly partial struct InternalImageGenCompletedEventQuality { }
[CodeGenType("ImageGenCompletedEventSize")] internal readonly partial struct InternalImageGenCompletedEventSize { }
[CodeGenType("ImagesUsage")] internal partial class InternalImagesUsage { }
[CodeGenType("ImagesUsageInputTokensDetails")] internal partial class InternalImagesUsageInputTokensDetails { }

// Streaming image edit generation.
[CodeGenType("ImageEditStreamEventType")] internal readonly partial struct InternalImageEditStreamEventType { }
[CodeGenType("ImageEditStreamEvent")] internal partial class InternalImageEditStreamEvent { }
[CodeGenType("ImageEditPartialImageEvent")] internal partial class InternalImageEditPartialImageEvent { }
[CodeGenType("ImageEditCompletedEvent")] internal partial class InternalImageEditCompletedEvent { }
[CodeGenType("UnknownImageEditStreamEvent")] internal partial class InternalUnknownImageEditStreamEvent { }
[CodeGenType("ImageEditPartialImageEventBackground")] internal readonly partial struct InternalImageEditPartialImageEventBackground { }
[CodeGenType("ImageEditPartialImageEventOutputFormat")] internal readonly partial struct InternalImageEditPartialImageEventOutputFormat { }
[CodeGenType("ImageEditPartialImageEventQuality")] internal readonly partial struct InternalImageEditPartialImageEventQuality { }
[CodeGenType("ImageEditPartialImageEventSize")] internal readonly partial struct InternalImageEditPartialImageEventSize { }
[CodeGenType("ImageEditCompletedEventBackground")] internal readonly partial struct InternalImageEditCompletedEventBackground { }
[CodeGenType("ImageEditCompletedEventOutputFormat")] internal readonly partial struct InternalImageEditCompletedEventOutputFormat { }
[CodeGenType("ImageEditCompletedEventQuality")] internal readonly partial struct InternalImageEditCompletedEventQuality { }
[CodeGenType("ImageEditCompletedEventSize")] internal readonly partial struct InternalImageEditCompletedEventSize { }