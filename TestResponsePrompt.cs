using System;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI.Responses;

class TestResponsePrompt
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testing ResponsePrompt implementation...");
        
        // Test 1: Basic serialization
        var prompt = new ResponsePrompt("test-prompt-id", "v1.0");
        var json = JsonSerializer.Serialize(prompt);
        Console.WriteLine($"Serialized ResponsePrompt: {json}");
        
        // Test 2: With variables
        var variables = new Dictionary<string, object>
        {
            ["name"] = "John",
            ["age"] = 30,
            ["isActive"] = true
        };
        
        var promptWithVars = new ResponsePrompt("test-prompt-id", "v1.0", variables);
        var jsonWithVars = JsonSerializer.Serialize(promptWithVars);
        Console.WriteLine($"Serialized ResponsePrompt with variables: {jsonWithVars}");
        
        // Test 3: Deserialization
        var deserializedPrompt = JsonSerializer.Deserialize<ResponsePrompt>(jsonWithVars);
        Console.WriteLine($"Deserialized - ID: {deserializedPrompt.Id}, Version: {deserializedPrompt.Version}");
        Console.WriteLine($"Variables count: {deserializedPrompt.Variables?.Count ?? 0}");
        
        if (deserializedPrompt.Variables != null)
        {
            foreach (var kvp in deserializedPrompt.Variables)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value} (Type: {kvp.Value?.GetType().Name ?? "null"})");
            }
        }
        
        // Test 4: ResponseCreationOptions with prompt
        var options = new ResponseCreationOptions
        {
            Model = "gpt-4",
            Prompt = promptWithVars
        };
        
        var optionsJson = JsonSerializer.Serialize(options);
        Console.WriteLine($"Serialized ResponseCreationOptions: {optionsJson}");
        
        var deserializedOptions = JsonSerializer.Deserialize<ResponseCreationOptions>(optionsJson);
        Console.WriteLine($"Deserialized options - Model: {deserializedOptions.Model}");
        Console.WriteLine($"Prompt ID: {deserializedOptions.Prompt?.Id}");
        
        Console.WriteLine("All tests completed successfully!");
    }
}
