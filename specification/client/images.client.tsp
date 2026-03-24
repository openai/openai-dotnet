import "../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(createImage, "GenerateImages");
@@clientName(createImageEdit, "GenerateImageEdits");
@@clientName(createImageVariation, "GenerateImageVariations");

@@clientLocation(createImage, "Images");
@@clientLocation(createImageEdit, "Images");
@@clientLocation(createImageVariation, "Images");

@@clientName(ImagesResponse.data, "Items");

@@usage(ImageEditPartialImageEvent, Usage.output);
@@usage(ImageEditCompletedEvent, Usage.output);
@@usage(ImageGenPartialImageEvent, Usage.output);
@@usage(ImageGenCompletedEvent, Usage.output);

@@alternateType(ImageEditCompletedEvent.size, DotNetImageStreamingSize);
@@alternateType(ImageEditCompletedEvent.quality, DotNetImageStreamingQuality);
@@alternateType(ImageEditCompletedEvent.background, DotNetImageStreamingBackground);
@@alternateType(ImageEditCompletedEvent.output_format, DotNetImageStreamingOutputFormat);

@@alternateType(ImageEditPartialImageEvent.size, DotNetImageStreamingSize);
@@alternateType(ImageEditPartialImageEvent.quality, DotNetImageStreamingQuality);
@@alternateType(ImageEditPartialImageEvent.background, DotNetImageStreamingBackground);
@@alternateType(ImageEditPartialImageEvent.output_format, DotNetImageStreamingOutputFormat);

@@alternateType(ImageGenCompletedEvent.size, DotNetImageStreamingSize);
@@alternateType(ImageGenCompletedEvent.quality, DotNetImageStreamingQuality);
@@alternateType(ImageGenCompletedEvent.background, DotNetImageStreamingBackground);
@@alternateType(ImageGenCompletedEvent.output_format, DotNetImageStreamingOutputFormat);

@@alternateType(ImageGenPartialImageEvent.size, DotNetImageStreamingSize);
@@alternateType(ImageGenPartialImageEvent.quality, DotNetImageStreamingQuality);
@@alternateType(ImageGenPartialImageEvent.background, DotNetImageStreamingBackground);
@@alternateType(ImageGenPartialImageEvent.output_format, DotNetImageStreamingOutputFormat);