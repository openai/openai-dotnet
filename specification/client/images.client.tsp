import "../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(createImage, "GenerateImages");
@@clientName(createImageStreaming, "GenerateImagesStreaming");
@@clientName(createImageEdit, "GenerateImageEdits");
@@clientName(createImageEditStreaming, "GenerateImageEditsStreaming");
@@clientName(createImageVariation, "GenerateImageVariations");

@@clientLocation(createImage, "Images");
@@clientLocation(createImageStreaming, "Images");
@@clientLocation(createImageEdit, "Images");
@@clientLocation(createImageEditStreaming, "Images");
@@clientLocation(createImageVariation, "Images");

@@clientName(ImagesResponse.data, "Items");

@@usage(ImageEditPartialImageEvent, Usage.output);
@@usage(ImageEditCompletedEvent, Usage.output);
@@usage(ImageGenPartialImageEvent, Usage.output);
@@usage(ImageGenCompletedEvent, Usage.output);