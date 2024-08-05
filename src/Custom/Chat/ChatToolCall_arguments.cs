using System;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

/// <summary>
/// A base representation of an item in an <c>assistant</c> role response's <c>tool_calls</c> that specifies
/// parameterized resolution against a previously defined tool that is needed for the model to continue the logical
/// conversation.
/// </summary>
public partial class ChatToolCall
{
    /// <summary>
    /// Parses a specific argument from function call arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public T GetFunctionArgument<T>(string parameterName, T defaultValue)
    {
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

        if (TryReadArgumentValue<T>(parameterName, out var value))
        {
            return value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Parses a specific argument from function call arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public T GetFunctionArgument<T>(string parameterName)
    {
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

        if (TryReadArgumentValue<T>(parameterName, out var value))
        {
            return value;
        }
        throw new ArgumentOutOfRangeException(nameof(parameterName));
    }

    private bool TryReadArgumentValue<T>(string propertyName, out T value)
    {
        // TODO: FunctionArguments should return BinaryData. Once it does, eliminate the transcoding call. 
        Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(FunctionArguments));

        if (!reader.Read()) throw new InvalidOperationException("Malformed JSON");
        if (reader.TokenType != JsonTokenType.StartObject) throw new InvalidOperationException("Malformed JSON");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(propertyName))
            {
                if (!reader.Read()) throw new InvalidOperationException("Malformed JSON");
                return TryReadValue<T>(ref reader, out value);
            }
        }
        value = default;
        return false;
    }
    private static bool TryReadValue<T>(ref Utf8JsonReader reader, out T value)
    {
        object readValue = null;
        if (typeof(T).IsEnum  && reader.TokenType == JsonTokenType.String)
        {
            var enumValueName = reader.GetString();
            readValue = Enum.Parse(typeof(T), enumValueName, ignoreCase: true);
        }
        else if (typeof(T) == typeof(string) && reader.TokenType == JsonTokenType.String)
        {
            readValue = (object)reader.GetString();
        }
        else if (typeof(T) == typeof(bool) && (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False))
        {
            readValue = (object)reader.GetBoolean();
        }
        else if (typeof(T) == typeof(double) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetDouble();
        }
        else if (typeof(T) == typeof(float) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetSingle();
        }
        else if (typeof(T) == typeof(int) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetInt32();
        }
        else if (typeof(T) == typeof(long) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (T)(object)reader.GetInt64();
        }
        else if (typeof(T) == typeof(short) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetInt16();
        }
        else if (typeof(T) == typeof(sbyte) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetSByte();
        }
        else if (typeof(T) == typeof(ulong) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetUInt64();
        }
        else if (typeof(T) == typeof(uint) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetUInt32();
        }
        else if (typeof(T) == typeof(ushort) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetUInt16();
        }
        else if (typeof(T) == typeof(byte) && reader.TokenType == JsonTokenType.Number)
        {
            readValue = (object)reader.GetByte();
        }
        else
        {
            value = default(T);
            return false;
        }

        value = (T)readValue;
        return true;
    }
}