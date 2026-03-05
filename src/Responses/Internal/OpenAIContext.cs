#if OPENAI_RESPONSES
using OpenAI.Responses;

namespace OpenAI;

/// <summary>
/// Internal shim so that generated serialization code referencing <c>OpenAIContext.Default</c>
/// resolves to the Responses-specific <see cref="ResponsesModelContext"/>.
/// </summary>
internal static class OpenAIContext
{
    public static ResponsesModelContext Default => ResponsesModelContext.Default;
}
#endif
