using System;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI.Responses;

Console.WriteLine("Testing ResponsePrompt implementation...");

// Test 1: Basic prompt without variables
var basicPrompt = new ResponsePrompt()
{
    Id = "test-prompt-id",
    Version = "v1.0"
};

var basicJson = JsonSerializer.Serialize(basicPrompt);
Console.WriteLine($"Basic prompt JSON: {basicJson}");

var basicDeserialized = JsonSerializer.Deserialize<ResponsePrompt>(basicJson);
Console.WriteLine($"Basic deserialized - ID: {basicDeserialized?.Id}, Version: {basicDeserialized?.Version}");

// Test 2: ResponseCreationOptions integration
var options = new ResponseCreationOptions
{
    Prompt = basicPrompt
};

var optionsJson = JsonSerializer.Serialize(options);
Console.WriteLine($"Options JSON contains prompt: {optionsJson.Contains("test-prompt-id")}");

var deserializedOptions = JsonSerializer.Deserialize<ResponseCreationOptions>(optionsJson);
Console.WriteLine($"Options deserialized prompt ID: {deserializedOptions?.Prompt?.Id}");

Console.WriteLine("Basic functionality test completed!");
