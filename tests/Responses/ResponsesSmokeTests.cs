using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Responses;

[Category("Responses")]
[Category("Smoke")]
public partial class ResponsesSmokeTests
{
    [Test]
    public void SerializingMessagesWorks()
    {
        ResponseItem userMessageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("hello, world!"),
            ]);
        string serializedMessage = ModelReaderWriter.Write(userMessageItem).ToString().ToLower();

        Assert.That(serializedMessage, Does.Contain("hello, world"));
        Assert.That(serializedMessage, Does.Contain("message"));
        Assert.That(serializedMessage, Does.Contain("user"));
        Console.WriteLine(serializedMessage);

        ResponseItem deserializedMessage = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(serializedMessage));
        Assert.That(deserializedMessage, Is.Not.Null);
    }
}