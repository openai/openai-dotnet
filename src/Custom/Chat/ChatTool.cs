using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.Json;

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

    // CUSTOM: Added custom constructor.
    /// <summary>
    /// Creates a new instance of <see cref="ChatTool"/>.
    /// </summary>
    /// <param name="functionName"> The <c>name</c> of the function. </param>
    /// <param name="functionDescription"> The <c>description</c> of the function. </param>
    /// <param name="functionParameters"> The <c>parameters</c> into the function, in JSON Schema format. </param>
    public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        InternalFunctionDefinition function = new(functionName)
        {
            Description = functionDescription,
            Parameters = functionParameters
        };

        return new(function);
    }

    /// <summary>
    /// Creates tools from public static methods of a type.
    /// </summary>
    /// <param name="functionsHolder"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static ChatTool[] CreateFunctionTools(Type functionsHolder)
    {
        if (functionsHolder == null) throw new ArgumentNullException(nameof(functionsHolder));

        List<ChatTool> tools = [];

        foreach (MethodInfo method in functionsHolder.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
            string name = method.Name;
            string description = method.GetCustomAttribute<DescriptionAttribute>()?.Description;
            BinaryData parametersJson = null;
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0) {
                var memoryStream = new MemoryStream();
                Utf8JsonWriter json = new Utf8JsonWriter(memoryStream);
                json.WriteStartObject();
                json.WriteString("type"u8, "object"u8);
                json.WritePropertyName("properties"u8);
                json.WriteStartObject(); // start of "properties"

                List<string> requiredParameters = new List<string>();
                foreach (var parameter in parameters) {
                    if (!parameter.IsOptional) {
                        requiredParameters.Add(parameter.Name);
                    }
                    json.WritePropertyName(parameter.Name);
                    json.WriteStartObject();
                    WriteParameterType(ref json, parameter.ParameterType);
                    string parameterDescription = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    if (parameterDescription != null) {
                        json.WriteString("description"u8, parameterDescription);
                    }
                    json.WriteEndObject();
                }
                json.WriteEndObject(); // end of "properties"
                if (requiredParameters.Count > 0) {
                    json.WritePropertyName("required"u8);
                    json.WriteStartArray();
                    foreach (var parameter in requiredParameters) {
                        json.WriteStringValue(parameter);
                    }
                    json.WriteEndArray();
                }
                json.WriteEndObject();
                json.Flush();

                var buffer = memoryStream.GetBuffer();
                parametersJson = new BinaryData(buffer.AsMemory(0, (int)memoryStream.Length));

            }
            ChatTool tool = CreateFunctionTool(name, description, parametersJson);
            tools.Add(tool);
        }

        return tools.ToArray();

        void WriteParameterType(ref Utf8JsonWriter json, Type type)
        {
            if (type == typeof(string)) {
                json.WriteString("type"u8, "string"u8);
                return;
            }
            if (type == typeof(Boolean)) {
                json.WriteString("type"u8, "boolean"u8);
                return;
            }
            if (type == typeof(Int32) || type == typeof(Int64) || type == typeof(Int16) || type == typeof(SByte) ||
                type == typeof(UInt32) || type == typeof(UInt64) || type == typeof(UInt16) || type == typeof(Byte)
            ) {
                json.WriteString("type"u8, "integer"u8);
                return;
            }
            if (type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal)) {
                json.WriteString("type"u8, "number"u8);
                return;
            }
            if (type.IsEnum) {
                json.WriteString("type"u8, "string"u8);
                json.WritePropertyName("enum"u8);
                json.WriteStartArray();
                foreach (var enumValue in type.GetEnumNames()) {
                    json.WriteStringValue(enumValue);
                }
                json.WriteEndArray();
                return;
            }

            throw new NotSupportedException($"{type.Name} is not a supported parameter type. Only string, bool, int, uint, long, ulong, short, ushort, byte, sbyte, and enum parameters are supported.");
        }
    }
}
