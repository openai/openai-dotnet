import "../base/typespec/embeddings/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@clientName(createEmbedding, "GenerateEmbeddings");

@@clientLocation(createEmbedding, "Embeddings");

@@clientName(CreateEmbeddingResponse.data, "Items");

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateEmbeddingRequest);
@@dynamicModel(CreateEmbeddingResponse);
@@dynamicModel(Embedding);
