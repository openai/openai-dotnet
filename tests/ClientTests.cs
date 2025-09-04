using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenAI.Tests.Miscellaneous;

public class ClientTests
{
    /// <summary>
    /// Gets all public client types from the OpenAI assembly that end with "Client"
    /// and have a private _endpoint field of type Uri.
    /// </summary>
    public static IEnumerable<Type> GetClientTypes()
    {
        var openAIAssembly = typeof(OpenAI.OpenAIClient).Assembly;
        
        return openAIAssembly.GetTypes()
            .Where(type => 
                type.IsPublic && 
                type.IsClass && 
                !type.IsAbstract &&
                type.Name.EndsWith("Client", StringComparison.Ordinal) &&
                HasEndpointField(type))
            .OrderBy(type => type.Name); // For consistent ordering in test results
    }

    /// <summary>
    /// Checks if the type has a private _endpoint field of type Uri.
    /// </summary>
    private static bool HasEndpointField(Type type)
    {
        var endpointField = type.GetField("_endpoint", BindingFlags.NonPublic | BindingFlags.Instance);
        return endpointField != null && endpointField.FieldType == typeof(Uri);
    }

    [Test]
    [TestCaseSource(nameof(GetClientTypes))]
    public void AllClientsHaveEndpoint(Type clientType)
    {
        // Validate that the client has a public get-only Endpoint property of type Uri
        var endpointProperty = clientType.GetProperty("Endpoint", BindingFlags.Public | BindingFlags.Instance);
        
        Assert.That(endpointProperty, Is.Not.Null, 
            $"Client type {clientType.Name} should have a public Endpoint property");
        
        Assert.That(endpointProperty.PropertyType, Is.EqualTo(typeof(Uri)), 
            $"Client type {clientType.Name} Endpoint property should be of type Uri");
        
        Assert.That(endpointProperty.CanRead, Is.True, 
            $"Client type {clientType.Name} Endpoint property should be readable");
        
        Assert.That(endpointProperty.CanWrite, Is.False, 
            $"Client type {clientType.Name} Endpoint property should be get-only");

        // Try to construct the client with minimum parameters and validate Endpoint is populated
        var instance = CreateClientInstance(clientType);
        Assert.That(instance, Is.Not.Null, 
            $"Should be able to create an instance of {clientType.Name}");

        var endpointValue = endpointProperty.GetValue(instance) as Uri;
        Assert.That(endpointValue, Is.Not.Null, 
            $"Client type {clientType.Name} Endpoint property should be populated after construction");
    }

    /// <summary>
    /// Creates an instance of the specified client type with minimum required parameters.
    /// </summary>
    private static object CreateClientInstance(Type clientType)
    {
        const string testApiKey = "test-api-key";
        const string testModel = "test-model";

        // Get all public constructors
        var constructors = clientType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length) // Try constructors with fewer parameters first
            .ToArray();

        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            bool canConstruct = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                var paramName = parameters[i].Name?.ToLowerInvariant();

                if (paramType == typeof(string))
                {
                    // Determine if this is likely an API key or model parameter
                    if (paramName?.Contains("key") == true || paramName?.Contains("api") == true)
                    {
                        args[i] = testApiKey;
                    }
                    else if (paramName?.Contains("model") == true)
                    {
                        args[i] = testModel;
                    }
                    else
                    {
                        // Default string parameter
                        args[i] = "test-value";
                    }
                }
                else if (paramType == typeof(ApiKeyCredential))
                {
                    args[i] = new ApiKeyCredential(testApiKey);
                }
                else if (paramType == typeof(OpenAI.OpenAIClientOptions))
                {
                    args[i] = new OpenAI.OpenAIClientOptions();
                }
                else if (paramType == typeof(AuthenticationPolicy))
                {
                    // Skip constructors that require AuthenticationPolicy as they're more complex
                    canConstruct = false;
                    break;
                }
                else if (paramType == typeof(ClientPipeline))
                {
                    // Skip constructors that require ClientPipeline as they're internal
                    canConstruct = false;
                    break;
                }
                else
                {
                    // Unknown parameter type, skip this constructor
                    canConstruct = false;
                    break;
                }
            }

            if (canConstruct)
            {
                try
                {
                    return Activator.CreateInstance(clientType, args);
                }
                catch
                {
                    // If construction fails, try the next constructor
                    continue;
                }
            }
        }

        throw new InvalidOperationException($"Could not create an instance of {clientType.Name} with available constructors");
    }
}
