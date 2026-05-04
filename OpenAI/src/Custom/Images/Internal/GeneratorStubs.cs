using Microsoft.TypeSpec.Generator.Customizations;

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
[CodeGenType("ImageGenPartialImageEvent")] internal partial class InternalImageGenPartialImageEvent { }
[CodeGenType("ImageGenCompletedEvent")] internal partial class InternalImageGenCompletedEvent { }
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
[CodeGenType("ImageEditPartialImageEvent")] internal partial class InternalImageEditPartialImageEvent { }
[CodeGenType("ImageEditCompletedEvent")] internal partial class InternalImageEditCompletedEvent { }
[CodeGenType("ImageEditPartialImageEventBackground")] internal readonly partial struct InternalImageEditPartialImageEventBackground { }
[CodeGenType("ImageEditPartialImageEventOutputFormat")] internal readonly partial struct InternalImageEditPartialImageEventOutputFormat { }
[CodeGenType("ImageEditPartialImageEventQuality")] internal readonly partial struct InternalImageEditPartialImageEventQuality { }
[CodeGenType("ImageEditPartialImageEventSize")] internal readonly partial struct InternalImageEditPartialImageEventSize { }
[CodeGenType("ImageEditCompletedEventBackground")] internal readonly partial struct InternalImageEditCompletedEventBackground { }
[CodeGenType("ImageEditCompletedEventOutputFormat")] internal readonly partial struct InternalImageEditCompletedEventOutputFormat { }
[CodeGenType("ImageEditCompletedEventQuality")] internal readonly partial struct InternalImageEditCompletedEventQuality { }
[CodeGenType("ImageEditCompletedEventSize")] internal readonly partial struct InternalImageEditCompletedEventSize { }

// Image edit (new image edit operation types).
[CodeGenType("CreateImageEditSize")] internal readonly partial struct InternalCreateImageEditSize {}
[CodeGenType("CreateImageEditQuality")] internal readonly partial struct InternalCreateImageEditQuality {}
[CodeGenType("CreateImageEditBackground")] internal readonly partial struct InternalCreateImageEditBackground {}
[CodeGenType("CreateImageEditOutputFormat")] internal readonly partial struct InternalCreateImageEditOutputFormat {}
[CodeGenType("CreateImageEditSize1")] internal readonly partial struct InternalCreateImageEditSize1 {}
[CodeGenType("CreateImageEditQuality1")] internal readonly partial struct InternalCreateImageEditQuality1 {}
[CodeGenType("CreateImageEditBackground1")] internal readonly partial struct InternalCreateImageEditBackground1 {}
[CodeGenType("CreateImageEditOutputFormat1")] internal readonly partial struct InternalCreateImageEditOutputFormat1 {}

// Image generation (new image generation operation types).
[CodeGenType("CreateImageSize")] internal readonly partial struct InternalCreateImageSize {}
[CodeGenType("CreateImageQuality")] internal readonly partial struct InternalCreateImageQuality {}
[CodeGenType("CreateImageBackground")] internal readonly partial struct InternalCreateImageBackground {}
[CodeGenType("CreateImageOutputFormat")] internal readonly partial struct InternalCreateImageOutputFormat {}
[CodeGenType("CreateImageSize1")] internal readonly partial struct InternalCreateImageSize1 {}
[CodeGenType("CreateImageQuality1")] internal readonly partial struct InternalCreateImageQuality1 {}
[CodeGenType("CreateImageBackground1")] internal readonly partial struct InternalCreateImageBackground1 {}
[CodeGenType("CreateImageOutputFormat1")] internal readonly partial struct InternalCreateImageOutputFormat1 {}