using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example06_StructuredOutputs()
    {
        ChatClient client = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        List<ChatMessage> messages =
        [
            new UserChatMessage("How can I solve 8x + 7 = -23?"),
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "math_reasoning",
                jsonSchema: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "explanation": { "type": "string" },
                                        "output": { "type": "string" }
                                    },
                                    "required": ["explanation", "output"],
                                    "additionalProperties": false
                                }
                            },
                            "final_answer": { "type": "string" }
                        },
                        "required": ["steps", "final_answer"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = client.CompleteChat(messages, options);

        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);

        Console.WriteLine($"Final answer: {structuredJson.RootElement.GetProperty("final_answer")}");
        Console.WriteLine("Reasoning steps:");

        foreach (JsonElement stepElement in structuredJson.RootElement.GetProperty("steps").EnumerateArray())
        {
            Console.WriteLine($"  - Explanation: {stepElement.GetProperty("explanation")}");
            Console.WriteLine($"    Output: {stepElement.GetProperty("output")}");
        }
    }
}
