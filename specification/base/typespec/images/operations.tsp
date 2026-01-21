import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";
import "@typespec/sse";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
using TypeSpec.SSE;
namespace OpenAI;
/** Creates an edited or extended image given one or more source images and a prompt. This endpoint only supports `gpt-image-1` and `dall-e-2`. */
#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create image edit")
@post
@route("/images/edits")
@extension(
  "x-oaiMeta",
  #{
    name: "Create image edit",
    group: "images",
    returns: "Returns an [image](https://platform.openai.com/docs/api-reference/images/object) object.",
    examples: #[
      #{
        title: "Edit image",
        request: #{
          curl: """
            curl -s -D >(grep -i x-request-id >&2) \\
              -o >(jq -r '.data[0].b64_json' | base64 --decode > gift-basket.png) \\
              -X POST "https://api.openai.com/v1/images/edits" \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -F "model=gpt-image-1.5" \\
              -F "image[]=@body-lotion.png" \\
              -F "image[]=@bath-bomb.png" \\
              -F "image[]=@incense-kit.png" \\
              -F "image[]=@soap.png" \\
              -F 'prompt=Create a lovely gift basket with these four items in it'
            
            """,
          python: """
            import os
            from openai import OpenAI
            
            client = OpenAI(
                api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
            )
            images_response = client.images.edit(
                image=b"raw file contents",
                prompt="A cute baby sea otter wearing a beret",
            )
            print(images_response)
            """,
          `node.js`: """
            import OpenAI from 'openai';
            
            const client = new OpenAI({
              apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
            });
            
            const imagesResponse = await client.images.edit({
              image: fs.createReadStream('path/to/file'),
              prompt: 'A cute baby sea otter wearing a beret',
            });
            
            console.log(imagesResponse);
            """,
          go: """
            package main
            
            import (
              "bytes"
              "context"
              "fmt"
              "io"
            
              "github.com/openai/openai-go"
              "github.com/openai/openai-go/option"
            )
            
            func main() {
              client := openai.NewClient(
                option.WithAPIKey("My API Key"),
              )
              imagesResponse, err := client.Images.Edit(context.TODO(), openai.ImageEditParams{
                Image: openai.ImageEditParamsImageUnion{
                  OfFile: io.Reader(bytes.NewBuffer([]byte("some file contents"))),
                },
                Prompt: "A cute baby sea otter wearing a beret",
              })
              if err != nil {
                panic(err.Error())
              }
              fmt.Printf("%+v
            ", imagesResponse)
            }
            
            """,
          java: """
            package com.openai.example;
            
            import com.openai.client.OpenAIClient;
            import com.openai.client.okhttp.OpenAIOkHttpClient;
            import com.openai.models.images.ImageEditParams;
            import com.openai.models.images.ImagesResponse;
            import java.io.ByteArrayInputStream;
            import java.io.InputStream;
            
            public final class Main {
                private Main() {}
            
                public static void main(String[] args) {
                    OpenAIClient client = OpenAIOkHttpClient.fromEnv();
            
                    ImageEditParams params = ImageEditParams.builder()
                        .image(ByteArrayInputStream("some content".getBytes()))
                        .prompt("A cute baby sea otter wearing a beret")
                        .build();
                    ImagesResponse imagesResponse = client.images().edit(params);
                }
            }
            """,
          ruby: """
            require "openai"
            
            openai = OpenAI::Client.new(api_key: "My API Key")
            
            images_response = openai.images.edit(image: Pathname(__FILE__), prompt: "A cute baby sea otter wearing a beret")
            
            puts(images_response)
            """,
        },
      },
      #{
        title: "Streaming",
        request: #{
          curl: """
            curl -s -N -X POST "https://api.openai.com/v1/images/edits" \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -F "model=gpt-image-1.5" \\
              -F "image[]=@body-lotion.png" \\
              -F "image[]=@bath-bomb.png" \\
              -F "image[]=@incense-kit.png" \\
              -F "image[]=@soap.png" \\
              -F 'prompt=Create a lovely gift basket with these four items in it' \\
              -F "stream=true"
            
            """,
          python: """
            import os
            from openai import OpenAI
            
            client = OpenAI(
                api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
            )
            images_response = client.images.edit(
                image=b"raw file contents",
                prompt="A cute baby sea otter wearing a beret",
            )
            print(images_response)
            """,
          `node.js`: """
            import OpenAI from 'openai';
            
            const client = new OpenAI({
              apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
            });
            
            const imagesResponse = await client.images.edit({
              image: fs.createReadStream('path/to/file'),
              prompt: 'A cute baby sea otter wearing a beret',
            });
            
            console.log(imagesResponse);
            """,
          go: """
            package main
            
            import (
              "bytes"
              "context"
              "fmt"
              "io"
            
              "github.com/openai/openai-go"
              "github.com/openai/openai-go/option"
            )
            
            func main() {
              client := openai.NewClient(
                option.WithAPIKey("My API Key"),
              )
              imagesResponse, err := client.Images.Edit(context.TODO(), openai.ImageEditParams{
                Image: openai.ImageEditParamsImageUnion{
                  OfFile: io.Reader(bytes.NewBuffer([]byte("some file contents"))),
                },
                Prompt: "A cute baby sea otter wearing a beret",
              })
              if err != nil {
                panic(err.Error())
              }
              fmt.Printf("%+v
            ", imagesResponse)
            }
            
            """,
          java: """
            package com.openai.example;
            
            import com.openai.client.OpenAIClient;
            import com.openai.client.okhttp.OpenAIOkHttpClient;
            import com.openai.models.images.ImageEditParams;
            import com.openai.models.images.ImagesResponse;
            import java.io.ByteArrayInputStream;
            import java.io.InputStream;
            
            public final class Main {
                private Main() {}
            
                public static void main(String[] args) {
                    OpenAIClient client = OpenAIOkHttpClient.fromEnv();
            
                    ImageEditParams params = ImageEditParams.builder()
                        .image(ByteArrayInputStream("some content".getBytes()))
                        .prompt("A cute baby sea otter wearing a beret")
                        .build();
                    ImagesResponse imagesResponse = client.images().edit(params);
                }
            }
            """,
          ruby: """
            require "openai"
            
            openai = OpenAI::Client.new(api_key: "My API Key")
            
            images_response = openai.images.edit(image: Pathname(__FILE__), prompt: "A cute baby sea otter wearing a beret")
            
            puts(images_response)
            """,
        },
        response: """
          event: image_edit.partial_image
          data: {"type":"image_edit.partial_image","b64_json":"...","partial_image_index":0}
          
          event: image_edit.completed
          data: {"type":"image_edit.completed","b64_json":"...","usage":{"total_tokens":100,"input_tokens":50,"output_tokens":50,"input_tokens_details":{"text_tokens":10,"image_tokens":40}}}
          
          """,
      }
    ],
  }
)
@tag("Images")
op createImageEdit(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateImageEditRequest,
): ImagesResponse | SSEStream<CreateImageEditStreamingResponse>;

/** Creates an image given a prompt. [Learn more](https://platform.openai.com/docs/guides/images). */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create image")
@post
@route("/images/generations")
@extension(
  "x-oaiMeta",
  #{
    name: "Create image",
    group: "images",
    returns: "Returns an [image](https://platform.openai.com/docs/api-reference/images/object) object.",
    examples: #[
      #{
        title: "Generate image",
        request: #{
          curl: """
            curl https://api.openai.com/v1/images/generations \\
              -H "Content-Type: application/json" \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -d '{
                "model": "gpt-image-1.5",
                "prompt": "A cute baby sea otter",
                "n": 1,
                "size": "1024x1024"
              }'
            
            """,
          python: """
            import os
            from openai import OpenAI
            
            client = OpenAI(
                api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
            )
            images_response = client.images.generate(
                prompt="A cute baby sea otter",
            )
            print(images_response)
            """,
          `node.js`: """
            import OpenAI from 'openai';
            
            const client = new OpenAI({
              apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
            });
            
            const imagesResponse = await client.images.generate({ prompt: 'A cute baby sea otter' });
            
            console.log(imagesResponse);
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
              imagesResponse, err := client.Images.Generate(context.TODO(), openai.ImageGenerateParams{
                Prompt: "A cute baby sea otter",
              })
              if err != nil {
                panic(err.Error())
              }
              fmt.Printf("%+v
            ", imagesResponse)
            }
            
            """,
          java: """
            package com.openai.example;
            
            import com.openai.client.OpenAIClient;
            import com.openai.client.okhttp.OpenAIOkHttpClient;
            import com.openai.models.images.ImageGenerateParams;
            import com.openai.models.images.ImagesResponse;
            
            public final class Main {
                private Main() {}
            
                public static void main(String[] args) {
                    OpenAIClient client = OpenAIOkHttpClient.fromEnv();
            
                    ImageGenerateParams params = ImageGenerateParams.builder()
                        .prompt("A cute baby sea otter")
                        .build();
                    ImagesResponse imagesResponse = client.images().generate(params);
                }
            }
            """,
          ruby: """
            require "openai"
            
            openai = OpenAI::Client.new(api_key: "My API Key")
            
            images_response = openai.images.generate(prompt: "A cute baby sea otter")
            
            puts(images_response)
            """,
        },
        response: """
          {
            "created": 1713833628,
            "data": [
              {
                "b64_json": "..."
              }
            ],
            "usage": {
              "total_tokens": 100,
              "input_tokens": 50,
              "output_tokens": 50,
              "input_tokens_details": {
                "text_tokens": 10,
                "image_tokens": 40
              }
            }
          }
          
          """,
      },
      #{
        title: "Streaming",
        request: #{
          curl: """
            curl https://api.openai.com/v1/images/generations \\
              -H "Content-Type: application/json" \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -d '{
                "model": "gpt-image-1.5",
                "prompt": "A cute baby sea otter",
                "n": 1,
                "size": "1024x1024",
                "stream": true
              }' \\
              --no-buffer
            
            """,
          python: """
            import os
            from openai import OpenAI
            
            client = OpenAI(
                api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
            )
            images_response = client.images.generate(
                prompt="A cute baby sea otter",
            )
            print(images_response)
            """,
          `node.js`: """
            import OpenAI from 'openai';
            
            const client = new OpenAI({
              apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
            });
            
            const imagesResponse = await client.images.generate({ prompt: 'A cute baby sea otter' });
            
            console.log(imagesResponse);
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
              imagesResponse, err := client.Images.Generate(context.TODO(), openai.ImageGenerateParams{
                Prompt: "A cute baby sea otter",
              })
              if err != nil {
                panic(err.Error())
              }
              fmt.Printf("%+v
            ", imagesResponse)
            }
            
            """,
          java: """
            package com.openai.example;
            
            import com.openai.client.OpenAIClient;
            import com.openai.client.okhttp.OpenAIOkHttpClient;
            import com.openai.models.images.ImageGenerateParams;
            import com.openai.models.images.ImagesResponse;
            
            public final class Main {
                private Main() {}
            
                public static void main(String[] args) {
                    OpenAIClient client = OpenAIOkHttpClient.fromEnv();
            
                    ImageGenerateParams params = ImageGenerateParams.builder()
                        .prompt("A cute baby sea otter")
                        .build();
                    ImagesResponse imagesResponse = client.images().generate(params);
                }
            }
            """,
          ruby: """
            require "openai"
            
            openai = OpenAI::Client.new(api_key: "My API Key")
            
            images_response = openai.images.generate(prompt: "A cute baby sea otter")
            
            puts(images_response)
            """,
        },
        response: """
          event: image_generation.partial_image
          data: {"type":"image_generation.partial_image","b64_json":"...","partial_image_index":0}
          
          event: image_generation.completed
          data: {"type":"image_generation.completed","b64_json":"...","usage":{"total_tokens":100,"input_tokens":50,"output_tokens":50,"input_tokens_details":{"text_tokens":10,"image_tokens":40}}}
          
          """,
      }
    ],
  }
)
@tag("Images")
op createImage(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateImageRequest,
): ImagesResponse | SSEStream<CreateImageStreamingResponse>;

/** Creates a variation of a given image. This endpoint only supports `dall-e-2`. */
#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create image variation")
@post
@route("/images/variations")
@extension(
  "x-oaiMeta",
  #{
    name: "Create image variation",
    group: "images",
    returns: "Returns a list of [image](https://platform.openai.com/docs/api-reference/images/object) objects.",
    examples: #{
      response: """
        {
          "created": 1589478378,
          "data": [
            {
              "url": "https://..."
            },
            {
              "url": "https://..."
            }
          ]
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/images/variations \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -F image="@otter.png" \\
            -F n=2 \\
            -F size="1024x1024"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          images_response = client.images.create_variation(
              image=b"raw file contents",
          )
          print(images_response.created)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const imagesResponse = await client.images.createVariation({ image: fs.createReadStream('otter.png') });
          
          console.log(imagesResponse.created);
          """,
        csharp: """
          using System;
          
          using OpenAI.Images;
          
          ImageClient client = new(
              model: "dall-e-2",
              apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
          );
          
          GeneratedImage image = client.GenerateImageVariation(imageFilePath: "otter.png");
          
          Console.WriteLine(image.ImageUri);
          
          """,
        go: """
          package main
          
          import (
            "bytes"
            "context"
            "fmt"
            "io"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            imagesResponse, err := client.Images.NewVariation(context.TODO(), openai.ImageNewVariationParams{
              Image: io.Reader(bytes.NewBuffer([]byte("some file contents"))),
            })
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", imagesResponse.Created)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.images.ImageCreateVariationParams;
          import com.openai.models.images.ImagesResponse;
          import java.io.ByteArrayInputStream;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  ImageCreateVariationParams params = ImageCreateVariationParams.builder()
                      .image(ByteArrayInputStream("some content".getBytes()))
                      .build();
                  ImagesResponse imagesResponse = client.images().createVariation(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          images_response = openai.images.create_variation(image: Pathname(__FILE__))
          
          puts(images_response)
          """,
      },
    },
  }
)
@tag("Images")
op createImageVariation(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateImageVariationRequest,
): ImagesResponse;
