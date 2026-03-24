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
[CodeGenType("DotNetImageStreamingSize")] internal readonly partial struct InternalDotNetImageStreamingSize { }
[CodeGenType("DotNetImageStreamingQuality")] internal readonly partial struct InternalDotNetImageStreamingQuality { }
[CodeGenType("DotNetImageStreamingBackground")] internal readonly partial struct InternalDotNetImageStreamingBackground { }
[CodeGenType("DotNetImageStreamingOutputFormat")] internal readonly partial struct InternalDotNetImageStreamingOutputFormat { }
[CodeGenType("ImagesUsage")] internal partial class InternalImagesUsage { }
[CodeGenType("ImagesUsageInputTokensDetails")] internal partial class InternalImagesUsageInputTokensDetails { }

// Streaming image edit generation.
[CodeGenType("ImageEditPartialImageEvent")] internal partial class InternalImageEditPartialImageEvent { }
[CodeGenType("ImageEditCompletedEvent")] internal partial class InternalImageEditCompletedEvent { }