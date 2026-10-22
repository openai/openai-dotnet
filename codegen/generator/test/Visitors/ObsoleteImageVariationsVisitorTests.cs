using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
public class ObsoleteImageVariationsVisitorTests
{
    [SetUp]
    public void Setup()
    {
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
    }

    [TestCase("GenerateImageVariations")]
    [TestCase("GenerateImageVariationsAsync")]
    public void VisitMethod_MarksProtocolOverloadsObsolete(string methodName)
    {
        var method = CreateMethod(methodName);
        var visitor = new TestObsoleteImageVariationsVisitor();

        Assert.That(visitor.Apply(method), Is.SameAs(method));
        visitor.Apply(method);

        Assert.That(method.Signature.Attributes, Has.Count.EqualTo(1));
        var attribute = method.Signature.Attributes.Single();
        Assert.That(attribute.Type.Equals(typeof(ObsoleteAttribute)), Is.True);
        Assert.That(attribute.ToDisplayString(), Does.Contain(
            "\"This endpoint is retired and no longer available. Use the image edits endpoint " +
            "with a GPT Image model and a prompt to create a variation of an image.\""));
    }

    [TestCase("ImageClient", "OpenAI.Images", "GenerateImages")]
    [TestCase("ImageClient", "OpenAI.Images", "GenerateImageEditsAsync")]
    [TestCase("ImageClient", "OpenAI.Images", "CreateGenerateImageVariationsRequest")]
    [TestCase("OtherClient", "OpenAI.Images", "GenerateImageVariations")]
    [TestCase("ImageClient", "Other.Images", "GenerateImageVariationsAsync")]
    public void VisitMethod_DoesNotMarkUnrelatedMethodsObsolete(
        string clientName, string clientNamespace, string methodName)
    {
        var method = CreateMethod(methodName, clientName, clientNamespace);

        Assert.That(new TestObsoleteImageVariationsVisitor().Apply(method), Is.SameAs(method));

        Assert.That(method.Signature.Attributes, Is.Empty);
    }

    [TestCase(typeof(Stream))]
    [TestCase(typeof(string))]
    public void VisitMethod_DoesNotMarkConvenienceOverloadsObsolete(Type imageType)
    {
        var method = CreateMethod("GenerateImageVariations");
        method.Signature.Update(parameters: [new ParameterProvider("image", $"", imageType)]);

        new TestObsoleteImageVariationsVisitor().Apply(method);

        Assert.That(method.Signature.Attributes, Is.Empty);
    }

    [Test]
    public void VisitMethod_DoesNotMarkParameterlessMethodsObsolete()
    {
        var method = CreateMethod("GenerateImageVariations");
        method.Signature.Update(parameters: []);

        new TestObsoleteImageVariationsVisitor().Apply(method);

        Assert.That(method.Signature.Attributes, Is.Empty);
    }

    [Test]
    public void VisitMethod_PreservesExistingAttributes()
    {
        var method = CreateMethod("GenerateImageVariations");
        var existingAttribute = new AttributeStatement(typeof(CLSCompliantAttribute), Snippet.True);
        method.Signature.Update(attributes: [existingAttribute]);

        new TestObsoleteImageVariationsVisitor().Apply(method);

        Assert.That(method.Signature.Attributes, Has.Count.EqualTo(2));
        Assert.That(method.Signature.Attributes[0], Is.SameAs(existingAttribute));
        Assert.That(method.Signature.Attributes[1].Type.Equals(typeof(ObsoleteAttribute)), Is.True);
    }

    [Test]
    public void VisitMethod_PreservesExistingObsoleteAttribute()
    {
        var method = CreateMethod("GenerateImageVariations");
        var existingAttribute = new AttributeStatement(
            typeof(ObsoleteAttribute), Snippet.Literal("Existing message"));
        method.Signature.Update(attributes: [existingAttribute]);

        new TestObsoleteImageVariationsVisitor().Apply(method);

        Assert.That(method.Signature.Attributes, Is.EqualTo(new[] { existingAttribute }));
    }

    private static MethodProvider CreateMethod(
        string methodName,
        string clientName = "ImageClient",
        string clientNamespace = "OpenAI.Images")
    {
        var type = new TestTypeProvider(clientName, clientNamespace);
        return new MethodProvider(
            new MethodSignature(methodName, $"", MethodSignatureModifiers.Public,
                methodName.EndsWith("Async") ? typeof(Task<ClientResult>) : typeof(ClientResult), $"",
                [
                    new ParameterProvider("content", $"", typeof(BinaryContent)),
                    new ParameterProvider("contentType", $"", typeof(string)),
                    new ParameterProvider("options", $"", typeof(RequestOptions)),
                ]), MethodBodyStatement.Empty, type);
    }

    private sealed class TestObsoleteImageVariationsVisitor : ObsoleteImageVariationsVisitor
    {
        public MethodProvider? Apply(MethodProvider method) => VisitMethod(method);
    }

    private sealed class TestTypeProvider : TypeProvider
    {
        private readonly string _name;
        private readonly string _namespace;

        public TestTypeProvider(string name, string clientNamespace)
        {
            _name = name;
            _namespace = clientNamespace;
        }

        protected override string BuildName() => _name;
        protected override string BuildNamespace() => _namespace;
        protected override string BuildRelativeFilePath() => $"{Name}.cs";
    }
}
