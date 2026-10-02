using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Shared helpers for building small specification documents on disk.
/// </summary>
/// <remarks>
/// The processor only loads from a file, which is also the path a real run takes, so the tests
/// exercise the same entry point rather than a parallel one built for testing.
/// </remarks>
internal static class SpecFixture
{
    /// <summary> Writes YAML to a temporary file and loads it as a specification document. </summary>
    /// <param name="yaml"> The document body. </param>
    /// <returns> The loaded document. </returns>
    public static SpecDocument Load(string yaml)
    {
        var path = Path.Combine(Path.GetTempPath(), $"spec-processor-test-{Guid.NewGuid():N}.yml");

        try
        {
            File.WriteAllText(path, yaml);
            return SpecDocument.Load(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// A specification exercising the constructs most likely to be compared unstably: a
    /// discriminator, a composition, defaults, descriptions, enums, and nested properties.
    /// </summary>
    public const string RichSpec = """
        openapi: "3.1.0"
        info:
          title: Test
          version: "1.0.0"
        paths:
          /responses:
            post:
              operationId: createResponse
              tags:
                - Responses
              summary: Create a response.
              description: Creates a response.
              requestBody:
                content:
                  application/json:
                    schema:
                      $ref: "#/components/schemas/CreateResponseBody"
              responses:
                "200":
                  description: Success
                  content:
                    application/json:
                      schema:
                        $ref: "#/components/schemas/Response"
        components:
          schemas:
            CreateResponseBody:
              type: object
              description: The body.
              required:
                - model
              properties:
                model:
                  type: string
                  description: The model.
                temperature:
                  type: number
                  default: 1
                  minimum: 0
                  maximum: 2
                mode:
                  type: string
                  enum:
                    - auto
                    - manual
                nested:
                  type: object
                  properties:
                    depth:
                      type: integer
                      default: 3
            Response:
              type: object
              properties:
                id:
                  type: string
                item:
                  $ref: "#/components/schemas/Item"
            Item:
              oneOf:
                - $ref: "#/components/schemas/TextItem"
                - $ref: "#/components/schemas/ImageItem"
              discriminator:
                propertyName: type
                mapping:
                  text: "#/components/schemas/TextItem"
                  image: "#/components/schemas/ImageItem"
            TextItem:
              type: object
              required:
                - type
              properties:
                type:
                  type: string
                  enum:
                    - text
                text:
                  type: string
            ImageItem:
              type: object
              required:
                - type
              properties:
                type:
                  type: string
                  enum:
                    - image
                url:
                  type: string
                  format: uri
        """;
}
