import "../base/typespec/embeddings/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@clientName(Embeddings.createEmbedding, "GenerateEmbeddings");

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateEmbeddingRequest);
@@dynamicModel(CreateEmbeddingResponse);
@@dynamicModel(Embedding);
