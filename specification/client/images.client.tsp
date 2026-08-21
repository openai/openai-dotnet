import "../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.Http;

@@alternateType(CreateImageEditRequest.n, HttpPart<int32 | null>);
@@alternateType(CreateImageEditRequest.output_compression, HttpPart<int32 | null>);
@@alternateType(CreateImageRequest.n, int32 | null);
@@alternateType(CreateImageRequest.output_compression, int32 | null);
@@alternateType(CreateImageVariationRequest.n, HttpPart<int32 | null>);
@@alternateType(ImagesUsage.total_tokens, int32);
@@alternateType(ImagesUsage.input_tokens, int32);
@@alternateType(ImagesUsage.output_tokens, int32);
@@alternateType(ImagesUsageInputTokensDetails.text_tokens, int32);
@@alternateType(ImagesUsageInputTokensDetails.image_tokens, int32);
@@alternateType(ImageEditPartialImageEvent.partial_image_index, int32);
@@alternateType(ImageGenPartialImageEvent.partial_image_index, int32);
@@alternateType(ImageGenUsage.input_tokens, int32);
@@alternateType(ImageGenUsage.total_tokens, int32);
@@alternateType(ImageGenUsage.output_tokens, int32);
@@alternateType(ImageGenOutputTokensDetails.image_tokens, int32);
@@alternateType(ImageGenOutputTokensDetails.text_tokens, int32);
@@alternateType(ImageGenInputUsageDetails.text_tokens, int32);
@@alternateType(ImageGenInputUsageDetails.image_tokens, int32);

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