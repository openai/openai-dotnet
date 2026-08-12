using System;

#nullable enable

namespace OpenAI;

internal sealed class DataUriValue
{
    private string? _value;

    public DataUriValue(string value, bool parseDataUri)
    {
        _value = value;
        if (parseDataUri)
        {
            DataEncodingHelpers.TryParseDataUri(value, out BinaryData? bytes, out string? mediaType);
            Bytes = bytes;
            MediaType = mediaType;
        }
    }

    public DataUriValue(BinaryData bytes, string mediaType)
    {
        Bytes = bytes;
        MediaType = mediaType;
    }

    public BinaryData? Bytes { get; }

    public string? MediaType { get; }

    public bool IsDataUri => Bytes is not null;

    public string GetValue(bool cache)
    {
        if (_value is not null)
        {
            return _value;
        }

        string value = DataEncodingHelpers.CreateDataUri(Bytes!, MediaType!);
        if (cache)
        {
            _value = value;
        }

        return value;
    }
}
