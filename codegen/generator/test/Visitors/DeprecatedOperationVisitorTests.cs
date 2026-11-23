using System;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
public class DeprecatedOperationVisitorTests
{
    [TestCase("Use the replacement operation.")]
    [TestCase(null)]
    [TestCase("")]
    public void VisitMethod_UsesOperationDeprecationMessage(string? message)
    {
        var methods = CreateClientMethods("SampleClient", "Samples", "GetSample", message);
        var visitor = new TestDeprecatedOperationVisitor();

        foreach (var method in methods)
        {
            visitor.Apply(method);

            var attributes = method.Signature.Attributes
                .Where(attribute => attribute.Type.Equals(typeof(ObsoleteAttribute)))
                .Select(attribute => attribute.ToDisplayString())
                .ToArray();
            if (message is null)
            {
                Assert.That(attributes, Is.Empty, method.Signature.Name);
            }
            else
            {
                Assert.That(attributes.Single(), Does.Contain($"\"{message}\""), method.Signature.Name);
            }
        }
    }

    [Test]
    public void VisitMethod_PreservesExistingObsoleteAttribute()
    {
        var methods = CreateClientMethods("SampleClient", "Samples", "GetSample", "Use the replacement operation.");
        var visitor = new TestDeprecatedOperationVisitor();
        var existing = new AttributeStatement(typeof(ObsoleteAttribute), Snippet.Literal("Use the custom replacement."));

        foreach (var method in methods)
        {
            method.Signature.Update(attributes: [existing]);

            visitor.Apply(method);

            Assert.That(method.Signature.Attributes.Single().ToDisplayString(), Is.EqualTo(existing.ToDisplayString()), method.Signature.Name);
        }
    }

    [Test]
    public void VisitMethod_DoesNotDeprecateImageVariationsByName()
    {
        var methods = CreateClientMethods("ImageClient", "OpenAI.Images", "GenerateImageVariations");
        var visitor = new TestDeprecatedOperationVisitor();

        foreach (var method in methods)
        {
            visitor.Apply(method);

            Assert.That(method.Signature.Attributes.Any(attribute => attribute.Type.Equals(typeof(ObsoleteAttribute))), Is.False, method.Signature.Name);
        }
    }

    private static ScmMethodProvider[] CreateClientMethods(string clientName, string clientNamespace, string methodName, string? message = null)
    {
        var operation = InputFactory.Operation(methodName, deprecated: message);
        var client = InputFactory.Client(clientName, clientNamespace, methods: [InputFactory.BasicServiceMethod(methodName, operation)]);
        MockHelpers.LoadMockGenerator(clients: () => [client], configurationJson: "{ \"package-name\": \"TestLibrary\" }");

        var typeFactory = (ScmTypeFactory)ScmCodeModelGenerator.Instance.TypeFactory;
        var methods = typeFactory.CreateClient(client)!.Methods.OfType<ScmMethodProvider>().ToArray();
        Assert.That(methods.Select(method => method.Signature.Name), Does.Contain(methodName).And.Contain(methodName + "Async"));
        return methods;
    }

    private sealed class TestDeprecatedOperationVisitor : DeprecatedOperationVisitor
    {
        public MethodProvider? Apply(MethodProvider method) => base.VisitMethod(method);
    }
}
