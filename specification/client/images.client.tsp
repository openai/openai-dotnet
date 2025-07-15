import "../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(Images.createImage, "GenerateImages");
@@clientName(Images.createImageEdit, "GenerateImageEdits");
@@clientName(Images.createImageVariation, "GenerateImageVariations");
