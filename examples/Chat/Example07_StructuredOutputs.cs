﻿using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example07_StructuredOutputs()
    {
        ChatClient client = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "math_reasoning",
                jsonSchema: BinaryData.FromString("""
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
                    """),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion chatCompletion = client.CompleteChat(
            [ new UserChatMessage("How can I solve 8x + 7 = -23?") ],
            options);

        using JsonDocument structuredJson = JsonDocument.Parse(chatCompletion.ToString());

        Console.WriteLine($"Final answer: {structuredJson.RootElement.GetProperty("final_answer").GetString()}");
        Console.WriteLine("Reasoning steps:");

        foreach (JsonElement stepElement in structuredJson.RootElement.GetProperty("steps").EnumerateArray())
        {
            Console.WriteLine($"  - Explanation: {stepElement.GetProperty("explanation").GetString()}");
            Console.WriteLine($"    Output: {stepElement.GetProperty("output")}");
        }
    }
}
