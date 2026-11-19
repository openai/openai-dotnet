using NUnit.Framework;
using OpenAI.Images;
using System;
using System.Linq;
using System.Reflection;

namespace OpenAI.Tests.Images;

[Category("Images")]
public class ImageClientDeprecationTests
{
    [TestCase(nameof(ImageClient.GenerateImageVariation), 2)]
    [TestCase(nameof(ImageClient.GenerateImageVariationAsync), 2)]
    [TestCase(nameof(ImageClient.GenerateImageVariations), 3)]
    [TestCase(nameof(ImageClient.GenerateImageVariationsAsync), 3)]
    public void ImageVariationOverloadsAreObsolete(string methodName, int expectedOverloadCount)
    {
        const string expectedMessage = "This endpoint is retired and no longer available. Use the image edits endpoint with a GPT Image model and a prompt to create a variation of an image.";
        MethodInfo[] overloads = typeof(ImageClient).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => method.Name == methodName)
            .ToArray();

        Assert.That(overloads, Has.Length.EqualTo(expectedOverloadCount));
        foreach (MethodInfo method in overloads)
        {
            ObsoleteAttribute attribute = method.GetCustomAttribute<ObsoleteAttribute>();
            Assert.That(attribute, Is.Not.Null, method.ToString());
            Assert.That(attribute.Message, Is.EqualTo(expectedMessage), method.ToString());
            Assert.That(attribute.IsError, Is.False, method.ToString());
        }
    }
}
