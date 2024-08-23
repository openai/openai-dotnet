using System;

namespace OpenAI.Chat;

/// <summary>
/// A base representation of a tool supplied to a chat completion request. Tools inform the model about additional,
/// caller-provided behaviors that can be invoked to provide prompt enrichment or custom actions.
/// </summary>
[CodeGenModel("ChatCompletionTool")]
public partial class ChatTool
{
    // CUSTOM: Made internal.
    /// <summary> Gets the function. </summary>
    [CodeGenMember("Function")]
    internal InternalFunctionDefinition Function { get; }

    // CUSTOM: Made internal.
    /// <summary> Initializes a new instance of <see cref="ChatTool"/>. </summary>
    /// <param name="function"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="function"/> is null. </exception>
    internal ChatTool(InternalFunctionDefinition function)
    {
        Kind = ChatToolKind.Function;

        Function = function;
    }

    // CUSTOM: Renamed.
    /// <summary> The type of the tool. Currently, only <see cref="ChatToolKind.Function"/> is supported. </summary>
    [CodeGenMember("Type")]
    public ChatToolKind Kind { get; } = ChatToolKind.Function;

    // CUSTOM: Flattened.
    /// <summary>
    /// The name of the function that the tool represents.
    /// </summary>
    public string FunctionName => Function?.Name;

    // CUSTOM: Flattened.
    /// <summary>
    /// A friendly description of the function. This supplements <see cref="FunctionName"/> in informing the model about when
    /// it should call the function.
    /// </summary>
    public string FunctionDescription => Function?.Description;

    // CUSTOM: Flattened.
    /// <summary>
    /// The parameter information for the function, provided in JSON Schema format.
    /// </summary>
    /// <remarks>
    /// The <see cref="BinaryData.FromObjectAsJson{T}(T, System.Text.Json.JsonSerializerOptions?)"/> method provides
    /// an easy definition interface using the <c>dynamic</c> type:
    /// <para><code>
    /// Parameters = BinaryData.FromObjectAsJson(new
    /// {
    ///     type = "object",
    ///     properties = new
    ///     {
    ///         your_function_argument = new
    ///         {
    ///             type = "string",
    ///             description = "the description of your function argument"
    ///         }
    ///     },
    ///     required = new[] { "your_function_argument" }
    /// })
    /// </code></para>
    /// </remarks>
    public BinaryData FunctionParameters => Function?.Parameters;

    public bool? StrictParameterSchemaEnabled => Function?.Strict;

    // CUSTOM: Added custom constructor.
    /// <summary>
    /// Creates a new instance of <see cref="ChatTool"/>.
    /// </summary>
    /// <param name="functionName"> The <c>name</c> of the function. </param>
    /// <param name="functionDescription"> The <c>description</c> of the function. </param>
    /// <param name="functionParameters"> The <c>parameters</c> into the function, in JSON Schema format. </param>
    public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? strictParameterSchemaEnabled = null)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        InternalFunctionDefinition function = new(functionName)
        {
            Description = functionDescription,
            Parameters = functionParameters,
            Strict = strictParameterSchemaEnabled,
        };

        return new(function);
    }
}
