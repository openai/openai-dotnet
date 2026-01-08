
import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
namespace OpenAI;
/** Creates an embedding vector representing the input text. */
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create embeddings")
@post
@route("/embeddings")
@extension(
  "x-oaiMeta",
  #{
    name: "Create embeddings",
    group: "embeddings",
    returns: "A list of [embedding](https://platform.openai.com/docs/api-reference/embeddings/object) objects.",
    examples: #{
      response: """
        {
          "object": "list",
          "data": [
            {
              "object": "embedding",
              "embedding": [
                0.0023064255,
                -0.009327292,
                .... (1536 floats total for ada-002)
                -0.0028842222,
              ],
              "index": 0
            }
          ],
          "model": "text-embedding-ada-002",
          "usage": {
            "prompt_tokens": 8,
            "total_tokens": 8
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/embeddings \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -d '{
              "input": "The food was delicious and the waiter...",
              "model": "text-embedding-ada-002",
              "encoding_format": "float"
            }'
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          create_embedding_response = client.embeddings.create(
              input="The quick brown fox jumped over the lazy dog",
              model="text-embedding-3-small",
          )
          print(create_embedding_response.data)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const createEmbeddingResponse = await client.embeddings.create({
            input: 'The quick brown fox jumped over the lazy dog',
            model: 'text-embedding-3-small',
          });
          
          console.log(createEmbeddingResponse.data);
          """,
        csharp: """
          using System;
          
          using OpenAI.Embeddings;
          
          EmbeddingClient client = new(
              model: "text-embedding-3-small",
              apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
          );
          
          OpenAIEmbedding embedding = client.GenerateEmbedding(input: "The quick brown fox jumped over the lazy dog");
          ReadOnlyMemory<float> vector = embedding.ToFloats();
          
          for (int i = 0; i < vector.Length; i++)
          {
              Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
          }
          
          """,
        go: """
          package main
          
          import (
            "context"
            "fmt"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            createEmbeddingResponse, err := client.Embeddings.New(context.TODO(), openai.EmbeddingNewParams{
              Input: openai.EmbeddingNewParamsInputUnion{
                OfString: openai.String("The quick brown fox jumped over the lazy dog"),
              },
              Model: openai.EmbeddingModelTextEmbeddingAda002,
            })
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", createEmbeddingResponse.Data)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.embeddings.CreateEmbeddingResponse;
          import com.openai.models.embeddings.EmbeddingCreateParams;
          import com.openai.models.embeddings.EmbeddingModel;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  EmbeddingCreateParams params = EmbeddingCreateParams.builder()
                      .input("The quick brown fox jumped over the lazy dog")
                      .model(EmbeddingModel.TEXT_EMBEDDING_ADA_002)
                      .build();
                  CreateEmbeddingResponse createEmbeddingResponse = client.embeddings().create(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          create_embedding_response = openai.embeddings.create(
            input: "The quick brown fox jumped over the lazy dog",
            model: :"text-embedding-ada-002"
          )
          
          puts(create_embedding_response)
          """,
      },
    },
  }
)
@tag("Embeddings")
op createEmbedding(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateEmbeddingRequest,
): CreateEmbeddingResponse;
