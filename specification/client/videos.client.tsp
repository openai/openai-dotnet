import "../base/typespec/videos/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

// Move operations to Videos interface
@@clientLocation(ListVideos, "Videos");
@@clientLocation(createVideo, "Videos");
@@clientLocation(DeleteVideo, "Videos");
@@clientLocation(GetVideo, "Videos");
@@clientLocation(RetrieveVideoContent, "Videos");
@@clientLocation(CreateVideoRemix, "Videos");

// Not generate convenience methods for these operations
@@convenientAPI(ListVideos, false);
@@convenientAPI(createVideo, false);
@@convenientAPI(DeleteVideo, false);
@@convenientAPI(GetVideo, false);
@@convenientAPI(RetrieveVideoContent, false);
@@convenientAPI(CreateVideoRemix, false);

// Rename method
@@clientName(RetrieveVideoContent, "DownloadVideo");
