import "../base/typespec/embeddings/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(Embeddings.createEmbedding, "GenerateEmbeddings");
