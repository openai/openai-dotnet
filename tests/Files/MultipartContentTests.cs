using NUnit.Framework;
using OpenAI;
using System.Net.Http;

namespace OpenAI.Tests.Files;

[Category("Smoke")]
public class MultipartContentTests
{
    [Test]
    public void MultipartBoundaryAvoidsUnderscore()
    {
        for (int i = 0; i < 256; i++)
        {
            string boundary = MultiPartFormDataBinaryContent.CreateBoundary();

            Assert.That(boundary, Does.Not.Contain("_"));
            Assert.DoesNotThrow(() => new MultipartFormDataContent(boundary).Dispose());
        }
    }
}
