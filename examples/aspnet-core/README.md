# OpenAI ASP.NET Core examples

This folder contains ASP.NET Core samples for the OpenAI .NET client library. Pick the sample that matches the version of the `OpenAI` NuGet package you're using.

## Choose your version

| OpenAI package version | Sample | Recommended |
| --- | --- | --- |
| **2.10.0 or later** | [v2.10.0/](v2.10.0/README.md) — .NET 10, configuration-driven `ResponsesClient` via `ResponsesClientSettings` (Responses API) | ✅ |
| **2.9.1 or earlier** | [v2.9.1/](v2.9.1/README.md) — .NET 10, manual `ResponsesClient` registration as a DI singleton (Responses API) | Legacy |

If you're starting a new project, use the [v2.10.0 sample](v2.10.0/README.md).
