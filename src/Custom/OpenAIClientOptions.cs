using System;
using System.ClientModel.Primitives;

namespace OpenAI;

/// <summary>
/// Client-level options for the OpenAI service.
/// </summary>
[CodeGenModel("OpenAIClientOptions")]
public partial class OpenAIClientOptions : ClientPipelineOptions
{
    /// <summary>
    /// A non-default base endpoint that clients should use when connecting.
    /// </summary>
    public Uri Endpoint { get; init; }

    /// <summary>
    /// An optional application ID to use as part of the request User-Agent header.
    /// </summary>
    public string ApplicationId { get; init; }
}
