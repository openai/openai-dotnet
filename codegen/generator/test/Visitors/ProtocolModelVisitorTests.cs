using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors
{
    [Category("Visitor")]
    public class ProtocolModelVisitorTests
    {
        [SetUp]
        public void Setup()
        {
            MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
        }

        [TestCase("OpenAI.Containers")]
        [TestCase("OpenAI.Conversations")]
        [TestCase("OpenAI.Responses")]
        public void PreVisitProperty_AddsSetterForProtocolNamespace(string modelNamespace)
        {
            var (inputProperty, property) = CreateProperty(modelNamespace);

            var result = new TestProtocolModelVisitor().InvokePreVisitProperty(inputProperty, property);

            Assert.That(result!.Body, Is.EqualTo(new AutoPropertyBody(HasSetter: true)));
        }

        [Test]
        public void PreVisitProperty_DoesNotAddSetterOutsideProtocolNamespace()
        {
            var (inputProperty, property) = CreateProperty("OpenAI.Chat");
            var originalBody = property.Body;

            var result = new TestProtocolModelVisitor().InvokePreVisitProperty(inputProperty, property);

            Assert.That(result!.Body, Is.SameAs(originalBody));
        }

        private static (InputModelProperty InputProperty, PropertyProvider Property) CreateProperty(string modelNamespace)
        {
            var inputProperty = InputFactory.Property("value", InputPrimitiveType.String, isReadOnly: true);
            var inputType = InputFactory.Model(
                "UnlistedModel",
                modelNamespace,
                properties: [inputProperty]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType);
            Assert.That(model, Is.Not.Null);

            return (inputProperty, model!.Properties[0]);
        }

        private sealed class TestProtocolModelVisitor : ProtocolModelVisitor
        {
            public PropertyProvider? InvokePreVisitProperty(InputProperty inputProperty, PropertyProvider property) =>
                base.PreVisitProperty(inputProperty, property);
        }
    }
}
