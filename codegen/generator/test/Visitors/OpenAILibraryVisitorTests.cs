using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using System.Linq;

namespace OpenAILibraryPlugin.Tests.Visitors
{
    [Category("Visitor")]
    public class OpenAILibraryVisitorTests
    {
        [SetUp]
        public void Setup()
        {
            MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
        }

        // This test validates that the serialization is updated correctly for both dynamic and non-dynamic models.
        [TestCase(true)]
        [TestCase(false)]
        public void TestVisitMethod_JsonModelWriteCore(bool isDynamicModel)
        {
            var visitor = new TestOpenAILibraryVisitor();

            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: isDynamicModel, properties: [
                InputFactory.Property("cat", InputPrimitiveType.String),
                InputFactory.Property("requiredDog", InputPrimitiveType.String, isRequired: true)
            ]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType);
            Assert.That(model, Is.Not.Null);

            var jsonWriteCoreMethod = model!.SerializationProviders
                .OfType<MrwSerializationTypeDefinition>()
                .FirstOrDefault()?
                .Methods
                .OfType<MethodProvider>()
                .First(m => m.Signature.Name == "JsonModelWriteCore");
            Assert.That(jsonWriteCoreMethod, Is.Not.Null);

            // Invoke the visitor
            jsonWriteCoreMethod = visitor.InvokeVisitMethod(jsonWriteCoreMethod!);
            Assert.That(jsonWriteCoreMethod!.BodyStatements, Is.Not.Null);

            var methodBody = jsonWriteCoreMethod!.BodyStatements!.ToDisplayString();
            Assert.That(methodBody, Is.EqualTo(Helpers.GetExpectedFromFile(isDynamicModel.ToString())));
        }

        // This test validates that the serialization for known properties that should have additional conditions
        // is updated correctly.
        [TestCase(true)]
        [TestCase(false)]
        public void TestVisitMethod_JsonModelWriteCore_CustomConditions(bool isDynamicModel)
        {
            var visitor = new TestOpenAILibraryVisitor();

            var inputType = InputFactory.Model("ChatCompletionOptions", "Samples", isDynamicModel: isDynamicModel, properties: [
                InputFactory.Property("model", InputPrimitiveType.String, isRequired: true),
            ]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType);
            Assert.That(model, Is.Not.Null);

            var jsonWriteCoreMethod = model!.SerializationProviders
                .OfType<MrwSerializationTypeDefinition>()
                .FirstOrDefault()?
                .Methods
                .OfType<MethodProvider>()
                .First(m => m.Signature.Name == "JsonModelWriteCore");
            Assert.That(jsonWriteCoreMethod, Is.Not.Null);

            // Invoke the visitor
            jsonWriteCoreMethod = visitor.InvokeVisitMethod(jsonWriteCoreMethod!);
            Assert.That(jsonWriteCoreMethod!.BodyStatements, Is.Not.Null);

            var methodBody = jsonWriteCoreMethod!.BodyStatements!.ToDisplayString();
            Assert.That(methodBody, Is.EqualTo(Helpers.GetExpectedFromFile(isDynamicModel.ToString())));
        }

        private class TestOpenAILibraryVisitor : OpenAILibraryVisitor
        {
            public MethodProvider? InvokeVisitMethod(MethodProvider method)
            {
                return base.VisitMethod(method);
            }
        }

        private class TestTypeProvider : TypeProvider
        {
            protected override string BuildNamespace() => "Samples";

            protected override string BuildRelativeFilePath() => $"{Name}.cs";

            protected override string BuildName() => "TestModel";
        }
    }
}