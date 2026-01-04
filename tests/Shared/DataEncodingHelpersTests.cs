using NUnit.Framework;
using System;

namespace OpenAI.Tests.Shared;

public class DataEncodingHelpersTests
{
    [Test]
    public void CreateDataUri_Works()
    {
        byte[] data = [1, 2, 3, 4];
        BinaryData binaryData = BinaryData.FromBytes(data);
        string mediaType = "application/octet-stream";

        string dataUri = DataEncodingHelpers.CreateDataUri(binaryData, mediaType);

        Assert.That(dataUri, Is.EqualTo("data:application/octet-stream;base64,AQIDBA=="));
    }

    [Test]
    public void TryParseDataUri_Works()
    {
        string dataUri = "data:application/octet-stream;base64,AQIDBA==";

        bool result = DataEncodingHelpers.TryParseDataUri(dataUri, out BinaryData binaryData, out string mediaType);

        Assert.That(result, Is.True);
        Assert.That(mediaType, Is.EqualTo("application/octet-stream"));
        Assert.That(binaryData.ToArray(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
    }

    [Test]
    public void TryParseDataUri_InvalidFormat_ReturnsFalse()
    {
        string dataUri = "invalid-uri";

        bool result = DataEncodingHelpers.TryParseDataUri(dataUri, out BinaryData binaryData, out string mediaType);

        Assert.That(result, Is.False);
        Assert.That(binaryData, Is.Null);
        Assert.That(mediaType, Is.Null);
    }
}
