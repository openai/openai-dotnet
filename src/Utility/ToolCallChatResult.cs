using System.Collections.Generic;
using OpenAI.Chat;

namespace OpenAI;

/// <summary>
/// Represents the result of executing tool calls, including success and failure messages.
/// </summary>
public struct ToolCallChatResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolCallChatResult"/> struct.
    /// </summary>
    /// <param name="messages">A collection of tool chat messages representing successful tool call results.</param>
    /// <param name="failed">An optional list of tool call IDs that failed to execute.</param>
    public ToolCallChatResult(IEnumerable<ToolChatMessage> messages, List<string> failed = null)
    {
        Messages = messages;
        Failed = failed;
    }

    /// <summary>
    /// Gets the collection of tool chat messages representing successful tool call results.
    /// </summary>
    public IEnumerable<ToolChatMessage> Messages { get; }

    /// <summary>
    /// Gets the optional list of tool call IDs that failed to execute.
    /// </summary>
    public List<string> Failed { get; }
}
