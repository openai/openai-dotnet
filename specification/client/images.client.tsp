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