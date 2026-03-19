import "../../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

// Named unions to replace inline unions in image streaming events
// These fix duplicate-client-name errors for unnamed inline unions

union GeneratedImageSize {
  string,
  Size1024x1024: "1024x1024",
  Size1024x1536: "1024x1536",
  Size1536x1024: "1536x1024",
  Auto: "auto",
}

union GeneratedImageQuality {
  string,
  Low: "low",
  Medium: "medium",
  High: "high",
  Auto: "auto",
}

union GeneratedImageBackground {
  string,
  Transparent: "transparent",
  Opaque: "opaque",
  Auto: "auto",
}

union GeneratedImageOutputFormat {
  string,
  Png: "png",
  Webp: "webp",
  Jpeg: "jpeg",
}

// Override inline union types with named unions for ImageEditCompletedEvent
@@alternateType(ImageEditCompletedEvent.size, GeneratedImageSize);
@@alternateType(ImageEditCompletedEvent.quality, GeneratedImageQuality);
@@alternateType(ImageEditCompletedEvent.background, GeneratedImageBackground);
@@alternateType(ImageEditCompletedEvent.output_format, GeneratedImageOutputFormat);

// Override inline union types with named unions for ImageEditPartialImageEvent
@@alternateType(ImageEditPartialImageEvent.size, GeneratedImageSize);
@@alternateType(ImageEditPartialImageEvent.quality, GeneratedImageQuality);
@@alternateType(ImageEditPartialImageEvent.background, GeneratedImageBackground);
@@alternateType(ImageEditPartialImageEvent.output_format, GeneratedImageOutputFormat);

// Override inline union types with named unions for ImageGenCompletedEvent
@@alternateType(ImageGenCompletedEvent.size, GeneratedImageSize);
@@alternateType(ImageGenCompletedEvent.quality, GeneratedImageQuality);
@@alternateType(ImageGenCompletedEvent.background, GeneratedImageBackground);
@@alternateType(ImageGenCompletedEvent.output_format, GeneratedImageOutputFormat);

// Override inline union types with named unions for ImageGenPartialImageEvent
@@alternateType(ImageGenPartialImageEvent.size, GeneratedImageSize);
@@alternateType(ImageGenPartialImageEvent.quality, GeneratedImageQuality);
@@alternateType(ImageGenPartialImageEvent.background, GeneratedImageBackground);
@@alternateType(ImageGenPartialImageEvent.output_format, GeneratedImageOutputFormat);
