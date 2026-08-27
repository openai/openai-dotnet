using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System.IO;

namespace OpenAILibraryPlugin.Tests.Visitors
{
    [Category("Visitor")]
    public class ResponsesDirectoryVisitorTests
    {
        [SetUp]
        public void Setup()
        {
            MockHelpers.LoadMockGenerator(
                configurationJson: "{ \"package-name\": \"TestLibrary\" }",
                inputNamespace: "Contoso");
        }

        [Test]
        public void VisitType_DerivesResponsesNamespaceFromInputLibrary()
        {
            var type = new TestTypeProvider(
                "Contoso.Responses",
                Path.Combine("src", "Generated", "ResponsesClient.cs"));

            new TestResponsesDirectoryVisitor().Apply(type);

            Assert.That(
                type.RelativeFilePath,
                Is.EqualTo(Path.Combine("..", "Contoso.Responses", "src", "Generated", "ResponsesClient.cs")));
        }

        [Test]
        public void VisitType_RoutesNestedResponsesNamespaces()
        {
            var type = new TestTypeProvider(
                "Contoso.Responses.Events",
                Path.Combine("src", "Generated", "ResponseEvent.cs"));

            new TestResponsesDirectoryVisitor().Apply(type);

            Assert.That(
                type.RelativeFilePath,
                Is.EqualTo(Path.Combine("..", "Contoso.Responses", "src", "Generated", "ResponseEvent.cs")));
        }

        [Test]
        public void VisitType_RemovesRedundantResponsesModelDirectory()
        {
            var type = new TestTypeProvider(
                "Contoso.Responses",
                Path.Combine("src", "Generated", "Models", "Responses", "ResponseItem.cs"));

            new TestResponsesDirectoryVisitor().Apply(type);

            Assert.That(
                type.RelativeFilePath,
                Is.EqualTo(Path.Combine("..", "Contoso.Responses", "src", "Generated", "Models", "ResponseItem.cs")));
        }

        [TestCase("Contoso.ResponsesExtra")]
        [TestCase("Contoso.Chat")]
        public void VisitType_DoesNotRouteOtherNamespaces(string modelNamespace)
        {
            string path = Path.Combine("src", "Generated", "Other.cs");
            var type = new TestTypeProvider(modelNamespace, path);

            new TestResponsesDirectoryVisitor().Apply(type);

            Assert.That(type.RelativeFilePath, Is.EqualTo(path));
        }

        [Test]
        public void VisitType_DoesNotRouteInternalResponsesFiles()
        {
            string path = Path.Combine("src", "Generated", "Internal", "ResponseInternal.cs");
            var type = new TestTypeProvider("Contoso.Responses", path);

            new TestResponsesDirectoryVisitor().Apply(type);

            Assert.That(type.RelativeFilePath, Is.EqualTo(path));
        }

        private sealed class TestResponsesDirectoryVisitor : ResponsesDirectoryVisitor
        {
            public TypeProvider? Apply(TypeProvider type) => base.VisitType(type);
        }

        private sealed class TestTypeProvider : TypeProvider
        {
            private readonly string _namespace;
            private readonly string _relativeFilePath;

            public TestTypeProvider(string modelNamespace, string relativeFilePath)
            {
                _namespace = modelNamespace;
                _relativeFilePath = relativeFilePath;
            }

            protected override string BuildNamespace() => _namespace;

            protected override string BuildRelativeFilePath() => _relativeFilePath;

            protected override string BuildName() => "TestModel";
        }
    }
}
