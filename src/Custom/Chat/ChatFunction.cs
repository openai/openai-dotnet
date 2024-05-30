using System;

namespace OpenAI.Chat;

/// <summary>
/// Represents the definition of a function that the model may call, as supplied in a chat completion request.
/// </summary>
[CodeGenModel("ChatCompletionFunctions")]
[CodeGenSuppress("ChatFunction", typeof(string))]
public partial class ChatFunction
{
    // CUSTOM: Added custom constructor.
    /// <summary>
    /// Creates a new instance of <see cref="ChatFunction"/>.
    /// </summary>
    /// <param name="functionName"> The <c>name</c> of the function. </param>
    /// <param name="functionDescription"> The <c>description</c> of the function. </param>
    /// <param name="functionParameters"> The <c>parameters</c> into the function, in JSON Schema format. </param>
    public ChatFunction(string functionName, string functionDescription = null, BinaryData functionParameters = null)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        FunctionName = functionName;
        FunctionDescription = functionDescription;
        FunctionParameters = functionParameters;
    }

    // CUSTOM: Renamed.
    /// <summary> The name of the function to be called. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64. </summary>
    [CodeGenMember("Name")]
    public string FunctionName { get; }

    // CUSTOM: Renamed
    /// <summary> A description of what the function does, used by the model to choose when and how to call the function. </summary>
    [CodeGenMember("Description")]
    public string FunctionDescription { get; init; }

    // CUSTOM: Changed type to BinarayData.
    /// <summary> Gets or sets the parameters. </summary>
    [CodeGenMember("Parameters")]
    public BinaryData FunctionParameters { get; init; }
}