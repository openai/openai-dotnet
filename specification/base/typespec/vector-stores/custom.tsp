import "../../entrypoints/sdk.dotnet/specialized-types.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

union ListVectorStoreFilesFilter {
  string,
  "in_progress",
  "completed",
  "failed",
  "cancelled",
}

@discriminator("type")
model ChunkingStrategyResponseParam {
  type: "static" | "other";
}

enum VectorStoreExpirationAnchor {
  last_active_at,
}
