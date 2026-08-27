# Feature lifecycle in the OpenAI .NET client library

This document explains how client APIs for new OpenAI service features are introduced in the OpenAI client library for .NET and what to expect as they mature.

## Goals

The library is designed to make new OpenAI service capabilities available quickly while providing developers with a high-quality, idiomatic .NET experience.

The library provides three complementary client API layers. Applications can use the layer that best fits their scenario and move to a lower-level layer when they need more control.

- **Convenience APIs** are opinionated, high-level client APIs designed to make common scenarios easy to discover and implement.
- **Protocol models** cover the majority of scenarios with .NET types that more closely follow the REST contract.
- **Protocol methods** are the low-level, JSON-focused escape hatch for advanced, specialized, or highly customized scenarios.

Not every OpenAI service feature needs all three layers. A straightforward or less commonly used feature may be fully supported by protocol models without requiring a convenience API.

## Feature lifecycle

### 1. Available in the OpenAI REST API

A new capability is added to the OpenAI service and documented in the REST API specification.

### 2. Available in the client library through protocol methods

Protocol methods generally fast-follow new OpenAI service features. They provide the earliest client library access to an HTTP operation and are the most flexible layer in the library.

Protocol methods are JSON-focused. They accept request bodies as [`BinaryContent`](https://learn.microsoft.com/dotnet/api/system.clientmodel.binarycontent) and return a [`ClientResult`](https://learn.microsoft.com/dotnet/api/system.clientmodel.clientresult). The result's raw [`PipelineResponse`](https://learn.microsoft.com/dotnet/api/system.clientmodel.primitives.pipelineresponse) exposes response content as [`BinaryData`](https://learn.microsoft.com/dotnet/api/system.binarydata). They are intended for scenarios that need complete control over the request, response, headers, or other details of the underlying REST operation.

This layer is appropriate when an application needs a service feature immediately, has advanced requirements, or needs behavior not represented by higher-level client APIs. It also requires more work from the application, including constructing valid JSON and interpreting the service response.

### 3. Available in the client library through protocol models

Protocol models generally follow protocol methods once the library has completed the design work needed to represent the service's REST request and response shapes as .NET types.

These models more closely follow the REST contract while providing an object-based alternative to raw JSON. They are intended to cover the majority of application scenarios without imposing the opinions and abstractions of a convenience API.

For many straightforward or less frequently used service features, protocol models are the final client API form. Applications that need lower-level control can continue to use protocol methods.

### 4. Available in the client library through a convenience API

Convenience APIs are considered for service feature areas that are complex, broadly used, or benefit substantially from a guided .NET experience.

They provide opinionated models, abstractions, validation, and workflow-focused methods that make common scenarios easy to discover and implement. They are optimized for typical "Hello World" usage rather than every possible service scenario.

Because convenience APIs require significant design to establish an idiomatic and durable .NET experience, they typically follow protocol methods and protocol models. Applications that need more control can continue to use protocol models or protocol methods.

## Stable and experimental APIs

A client API feature area can be stable while new client APIs are still introduced within it. Stability of a client API feature area means that its established public API is covered by the library's strong backward compatibility commitment.

For a stable client API feature area, new protocol methods are introduced as stable because their public shape closely reflects the REST operation. New protocol models and convenience APIs are introduced as experimental while their .NET design is validated.

Protocol models are generally promoted to stable after a short period of feedback and refinement. Convenience APIs undergo a more thorough feedback and tuning process because they introduce opinionated abstractions intended for long-term use.

Experimental APIs are marked with .NET's `[Experimental]` attribute. The C# compiler reports an error when code consumes an experimental API. Each experimental feature has its own diagnostic ID, so acknowledging one feature does not acknowledge another.

To acknowledge use of an experimental feature for a project, add its diagnostic ID to the `NoWarn` property. For example, to use an API marked with `OPENAI001`:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);OPENAI001</NoWarn>
</PropertyGroup>
```

A source-level suppression is also possible when its narrower scope is appropriate:

```csharp
#pragma warning disable OPENAI001
// Code that uses the experimental API.
#pragma warning restore OPENAI001
```

Suppress an experimental diagnostic only after deciding that the API's potential compile-time and behavioral changes are acceptable for the application. See [Preview APIs](https://learn.microsoft.com/dotnet/fundamentals/apicompat/preview-apis) for the .NET behavior and additional suppression options.

When an individual protocol model or convenience API becomes stable, it is covered by the library's strong backward compatibility commitment. Breaking changes are considered only in rare and critical circumstances, such as a severe security issue.
