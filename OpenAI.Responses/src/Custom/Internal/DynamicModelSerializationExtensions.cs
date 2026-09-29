namespace OpenAI.Responses;

internal static class DynamicModelSerializationExtensions
{
    public static string ToSerialString(this ResponseStatus value) => value.ToString();
    public static ResponseStatus ToResponseStatus(this string value) => new(value);

    public static string ToSerialString(this FunctionCallStatus value) => value.ToString();
    public static FunctionCallStatus ToFunctionCallStatus(this string value) => new(value);

    public static string ToSerialString(this FileSearchCallStatus value) => value.ToString();
    public static FileSearchCallStatus ToFileSearchCallStatus(this string value) => new(value);

    public static string ToSerialString(this WebSearchCallStatus value) => value.ToString();
    public static WebSearchCallStatus ToWebSearchCallStatus(this string value) => new(value);

    public static string ToSerialString(this CodeInterpreterCallStatus value) => value.ToString();
    public static CodeInterpreterCallStatus ToCodeInterpreterCallStatus(this string value) => new(value);

    public static string ToSerialString(this ImageGenerationCallStatus value) => value.ToString();
    public static ImageGenerationCallStatus ToImageGenerationCallStatus(this string value) => new(value);

    public static string ToSerialString(this ReasoningStatus value) => value.ToString();
    public static ReasoningStatus ToReasoningStatus(this string value) => new(value);

    public static string ToSerialString(this ComputerCallOutputStatus value) => value.ToString();
    public static ComputerCallOutputStatus ToComputerCallOutputStatus(this string value) => new(value);

    public static string ToSerialString(this ComputerCallStatus value) => value.ToString();
    public static ComputerCallStatus ToComputerCallStatus(this string value) => new(value);

    public static string ToSerialString(this FunctionCallOutputStatus value) => value.ToString();
    public static FunctionCallOutputStatus ToFunctionCallOutputStatus(this string value) => new(value);
}