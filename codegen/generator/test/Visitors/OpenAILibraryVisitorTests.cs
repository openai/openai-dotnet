using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System;
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

        [Test]
        public void VisitType_AddsAdditionalRawDataPropertyToBaseModels()
        {
            var visitor = new TestAdditionalRawDataPropertyVisitor();
            var inputType = InputFactory.Model("TestModel", "Samples", properties: [InputFactory.Property("cat", InputPrimitiveType.String)]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType);

            visitor.InvokeVisitType(model!);

            Assert.That(model!.Properties.Any(property => property.Name == "SerializedAdditionalRawData"), Is.True);
        }

        [Test]
        public void VisitField_RemovesReadonlyFromAdditionalPropertiesFieldOnMutableModels()
        {
            var visitor = new TestAdditionalPropertiesFieldMutabilityVisitor();
            var inputType = InputFactory.Model("TestModel", "Samples", properties: [InputFactory.Property("cat", InputPrimitiveType.String)]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType);
            var field = model!.Fields.First(f => f.Name == "_additionalBinaryDataProperties");

            visitor.InvokeVisitField(field);

            Assert.That(field.Modifiers.HasFlag(FieldModifiers.ReadOnly), Is.False);
        }

        [Test]
        public void VisitType_AddsSentinelFieldAndMethodToModelSerializationExtensions()
        {
            var visitor = new TestModelSerializationSentinelVisitor();
            var typeProvider = new TestTypeProvider("ModelSerializationExtensions");

            visitor.InvokeVisitType(typeProvider);

            Assert.That(typeProvider.Fields.Any(field => field.Name == "_sentinelValue"), Is.True);
            Assert.That(typeProvider.Methods.Any(method => method.Signature.Name == "IsSentinelValue"), Is.True);
        }

        // This test validates that the split serialization visitors preserve behavior for both dynamic and non-dynamic models.
        [TestCase(true)]
        [TestCase(false)]
        public void TestVisitMethod_JsonModelWriteCore(bool isDynamicModel)
        {
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

            jsonWriteCoreMethod = ApplySerializationVisitors(jsonWriteCoreMethod!);
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

            jsonWriteCoreMethod = ApplySerializationVisitors(jsonWriteCoreMethod!);
            Assert.That(jsonWriteCoreMethod!.BodyStatements, Is.Not.Null);

            var methodBody = jsonWriteCoreMethod!.BodyStatements!.ToDisplayString();
            Assert.That(methodBody, Is.EqualTo(Helpers.GetExpectedFromFile(isDynamicModel.ToString())));
        }

        private static MethodProvider? ApplySerializationVisitors(MethodProvider method)
        {
            MethodProvider? updatedMethod = new TestAdditionalPropertiesWriteGuardVisitor().InvokeVisitMethod(method);
            updatedMethod = new TestOptionalDefinedPropertySerializationVisitor().InvokeVisitMethod(updatedMethod!);
            updatedMethod = new TestAdditionalPropertiesSentinelSkipVisitor().InvokeVisitMethod(updatedMethod!);
            return updatedMethod;
        }

        private sealed class TestAdditionalRawDataPropertyVisitor : AdditionalRawDataPropertyVisitor
        {
            public TypeProvider InvokeVisitType(TypeProvider type) => base.VisitType(type);
        }

        private sealed class TestAdditionalPropertiesFieldMutabilityVisitor : AdditionalPropertiesFieldMutabilityVisitor
        {
            public FieldProvider InvokeVisitField(FieldProvider field) => base.VisitField(field);
        }

        private sealed class TestModelSerializationSentinelVisitor : ModelSerializationSentinelVisitor
        {
            public TypeProvider InvokeVisitType(TypeProvider type) => base.VisitType(type);
        }

        private sealed class TestAdditionalPropertiesWriteGuardVisitor : AdditionalPropertiesWriteGuardVisitor
        {
            public MethodProvider InvokeVisitMethod(MethodProvider method) => base.VisitMethod(method);
        }

        private sealed class TestOptionalDefinedPropertySerializationVisitor : OptionalDefinedPropertySerializationVisitor
        {
            public MethodProvider InvokeVisitMethod(MethodProvider method) => base.VisitMethod(method);
        }

        private sealed class TestAdditionalPropertiesSentinelSkipVisitor : AdditionalPropertiesSentinelSkipVisitor
        {
            public MethodProvider InvokeVisitMethod(MethodProvider method) => base.VisitMethod(method);
        }

        private sealed class TestTypeProvider(string typeName) : TypeProvider
        {
            protected override string BuildNamespace() => "Samples";

            protected override string BuildRelativeFilePath() => $"{Name}.cs";

            protected override string BuildName() => typeName;
        }
    }
}
