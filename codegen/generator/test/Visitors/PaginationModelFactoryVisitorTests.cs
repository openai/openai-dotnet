using System.Linq;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
public class PaginationModelFactoryVisitorTests
{
    [SetUp]
    public void Setup()
    {
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
    }

    [Test]
    public void VisitMethod_ReordersFactorySignatureWithoutChangingBodyOrParameters()
    {
        var factory = new TestModelFactoryProvider();
        var pageSizeLimit = new ParameterProvider("pageSizeLimit", $"", typeof(int?));
        var order = new ParameterProvider("order", $"", typeof(string));
        var afterId = new ParameterProvider("afterId", $"", typeof(string));
        var beforeId = new ParameterProvider("beforeId", $"", typeof(string));
        MethodBodyStatement body = MethodBodyStatement.Empty;
        var method = new MethodProvider(
            new MethodSignature(
                "AssistantCollectionOptions",
                $"",
                MethodSignatureModifiers.Public | MethodSignatureModifiers.Static,
                typeof(object),
                $"",
                [pageSizeLimit, order, afterId, beforeId]),
            body,
            factory);

        new TestPaginationModelFactoryVisitor().Apply(method);

        Assert.That(method.Signature.Parameters.Select(parameter => parameter.Name),
            Is.EqualTo(new[] { "afterId", "beforeId", "pageSizeLimit", "order" }));
        Assert.Multiple(() =>
        {
            Assert.That(method.Signature.Parameters[0], Is.SameAs(afterId));
            Assert.That(method.Signature.Parameters[1], Is.SameAs(beforeId));
            Assert.That(method.Signature.Parameters[2], Is.SameAs(pageSizeLimit));
            Assert.That(method.Signature.Parameters[3], Is.SameAs(order));
            Assert.That(method.BodyStatements, Is.SameAs(body));
        });
    }

    private sealed class TestPaginationModelFactoryVisitor : PaginationModelFactoryVisitor
    {
        public MethodProvider? Apply(MethodProvider method) => base.VisitMethod(method);
    }

    private sealed class TestModelFactoryProvider : ModelFactoryProvider
    {
        public TestModelFactoryProvider() : base([]) { }

        protected override string BuildName() => nameof(TestModelFactoryProvider);
        protected override string BuildRelativeFilePath() => $"{Name}.cs";
    }
}
