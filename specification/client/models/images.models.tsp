import "../../base/typespec/images/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

union DotNetImageStreamingSize {
  string,
  `1024x1024`: "1024x1024",
  `1024x1536`: "1024x1536",
  `1536x1024`: "1536x1024",
  auto: "auto",
}

union DotNetImageStreamingQuality {
  string,
  low: "low",
  medium: "medium",
  high: "high",
  auto: "auto",
}

union DotNetImageStreamingBackground {
  string,
  transparent: "transparent",
  opaque: "opaque",
  auto: "auto",
}

union DotNetImageStreamingOutputFormat {
  string,
  png: "png",
  webp: "webp",
  jpeg: "jpeg",
}
