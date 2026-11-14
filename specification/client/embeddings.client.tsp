import "../base/typespec/embeddings/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateEmbeddingRequest.dimensions, int32);
@@alternateType(Embedding.index, int32);
@@alternateType(CreateEmbeddingResponseUsage.prompt_tokens, int32);
@@alternateType(CreateEmbeddingResponseUsage.total_tokens, int32);

@@clientName(createEmbedding, "GenerateEmbeddings");

@@clientLocation(createEmbedding, "Embeddings");

@@clientName(CreateEmbeddingResponse.data, "Items");

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateEmbeddingRequest, "csharp");
@@dynamicModel(CreateEmbeddingResponse, "csharp");
@@dynamicModel(Embedding, "csharp");
