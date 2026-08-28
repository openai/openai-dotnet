using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Visitors;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
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
            var visitor = new TestJsonModelWriteCoreVisitor();

            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: isDynamicModel, properties: [
                InputFactory.Property("cat", InputPrimitiveType.String),
                InputFactory.Property("requiredDog", InputPrimitiveType.String, isRequired: true)
            ]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)!;
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
            var visitor = new TestJsonModelWriteCoreVisitor();

            var inputType = InputFactory.Model("ChatCompletionOptions", "Samples", isDynamicModel: isDynamicModel, properties: [
                InputFactory.Property("model", InputPrimitiveType.String, isRequired: true),
            ]);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)
                ?? throw new InvalidOperationException("Expected a model provider.");
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

        [Test]
        public void TestVisitType_AdditionalProperties()
        {
            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: true);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)
                ?? throw new InvalidOperationException("Expected a model provider.");

            var additionalPropertiesField = new FieldProvider(
                FieldModifiers.Private | FieldModifiers.ReadOnly,
                typeof(IDictionary<string, BinaryData>),
                "_additionalBinaryDataProperties",
                model);
            model.Update(fields: [.. model.Fields, additionalPropertiesField]);

            var visitor = new TestAdditionalPropertiesVisitor();
            Assert.That(additionalPropertiesField.Modifiers.HasFlag(FieldModifiers.ReadOnly), Is.True);

            visitor.InvokeVisitField(additionalPropertiesField);
            visitor.InvokeVisitType(model);

            Assert.That(additionalPropertiesField.Modifiers.HasFlag(FieldModifiers.ReadOnly), Is.False);
            Assert.That(model.Properties.Any(p => p.Name == "SerializedAdditionalRawData"), Is.True);
        }

        [Test]
        public void TestVisitType_ModelSerializationExtensions()
        {
            var type = new ModelSerializationExtensionsTypeProvider();
            var visitor = new TestModelSerializationExtensionsVisitor();

            visitor.InvokeVisitType(type);

            Assert.That(type.Fields.Any(f => f.Name == "_sentinelValue"), Is.True);
            Assert.That(type.Methods.Any(m => m.Signature.Name == "IsSentinelValue"), Is.True);
        }

        private class TestJsonModelWriteCoreVisitor : JsonModelWriteCoreVisitor
        {
            public MethodProvider? InvokeVisitMethod(MethodProvider method)
            {
                return base.VisitMethod(method);
            }
        }

        private class TestAdditionalPropertiesVisitor : AdditionalPropertiesVisitor
        {
            public TypeProvider InvokeVisitType(TypeProvider type) => base.VisitType(type);

            public FieldProvider InvokeVisitField(FieldProvider field) => base.VisitField(field);
        }

        private class TestModelSerializationExtensionsVisitor : ModelSerializationExtensionsVisitor
        {
            public TypeProvider InvokeVisitType(TypeProvider type) => base.VisitType(type);
        }

        private class ModelSerializationExtensionsTypeProvider : TypeProvider
        {
            protected override string BuildNamespace() => "Samples";

            protected override string BuildRelativeFilePath() => $"{Name}.cs";

            protected override string BuildName() => "ModelSerializationExtensions";
        }

        private class TestTypeProvider : TypeProvider
        {
            protected override string BuildNamespace() => "Samples";

            protected override string BuildRelativeFilePath() => $"{Name}.cs";

            protected override string BuildName() => "TestModel";
        }
    }
}
