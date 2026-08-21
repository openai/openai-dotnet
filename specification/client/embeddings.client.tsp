import "../base/typespec/embeddings/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@alternateType(CreateEmbeddingRequest.dimensions, int32);
@@alternateType(Embedding.index, int32);
@@alternateType(CreateEmbeddingResponseUsage.prompt_tokens, int32);
@@alternateType(CreateEmbeddingResponseUsage.total_tokens, int32);

@@clientName(createEmbedding, "GenerateEmbeddings");

@@clientLocation(createEmbedding, "Embeddings");

@@clientName(CreateEmbeddingResponse.data, "Items");

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateEmbeddingRequest);
@@dynamicModel(CreateEmbeddingResponse);
@@dynamicModel(Embedding);
