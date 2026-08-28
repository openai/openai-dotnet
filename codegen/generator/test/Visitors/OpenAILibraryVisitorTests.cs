using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.TypeSpec.Generator;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Expressions;
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
using System.IO;
using System.Linq;
using System.Reflection;

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
        public void TestVisitType_AdditionalProperties_ReadOnlyRootModel()
        {
            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: true);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)
                ?? throw new InvalidOperationException("Expected a model provider.");
            model.Update(modifiers: model.DeclarationModifiers | TypeSignatureModifiers.ReadOnly);

            var additionalPropertiesField = new FieldProvider(
                FieldModifiers.Private | FieldModifiers.ReadOnly,
                typeof(IDictionary<string, BinaryData>),
                "_additionalBinaryDataProperties",
                model);
            model.Update(fields: [.. model.Fields, additionalPropertiesField]);

            var visitor = new TestAdditionalPropertiesVisitor();
            visitor.InvokeVisitField(additionalPropertiesField);
            visitor.InvokeVisitType(model);

            Assert.That(additionalPropertiesField.Modifiers.HasFlag(FieldModifiers.ReadOnly), Is.True);
            var property = model.Properties.Single(p => p.Name == "SerializedAdditionalRawData");
            Assert.That(property.Body, Is.EqualTo(new ExpressionPropertyBody(additionalPropertiesField, null)));
        }

        [Test]
        public void TestVisitType_AdditionalProperties_DerivedModelDoesNotAddProperty()
        {
            var baseInputType = InputFactory.Model("BaseModel", "Samples", isDynamicModel: true);
            var derivedInputType = InputFactory.Model("DerivedModel", "Samples", baseModel: baseInputType, isDynamicModel: true);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(derivedInputType)
                ?? throw new InvalidOperationException("Expected a model provider.");
            Assert.That(model.BaseModelProvider, Is.Not.Null);

            var additionalPropertiesField = new FieldProvider(
                FieldModifiers.Private | FieldModifiers.ReadOnly,
                typeof(IDictionary<string, BinaryData>),
                "_additionalBinaryDataProperties",
                model);
            model.Update(fields: [.. model.Fields, additionalPropertiesField]);

            new TestAdditionalPropertiesVisitor().InvokeVisitType(model);

            Assert.That(model.Properties.Any(p => p.Name == "SerializedAdditionalRawData"), Is.False);
        }

        [Test]
        public void TestVisitType_AdditionalProperties_AbsentFieldDoesNotAddProperty()
        {
            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: true);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)
                ?? throw new InvalidOperationException("Expected a model provider.");

            new TestAdditionalPropertiesVisitor().InvokeVisitType(model);

            Assert.That(model.Properties.Any(p => p.Name == "SerializedAdditionalRawData"), Is.False);
        }

        [Test]
        public void TestVisitField_AdditionalProperties_NonTargetFieldIsUnchanged()
        {
            var type = new TestTypeProvider();
            var field = new FieldProvider(FieldModifiers.Private | FieldModifiers.ReadOnly, typeof(BinaryData), "_otherField", type);

            new TestAdditionalPropertiesVisitor().InvokeVisitField(field);

            Assert.That(field.Modifiers, Is.EqualTo(FieldModifiers.Private | FieldModifiers.ReadOnly));
        }

        [Test]
        public void TestVisitType_ModelSerializationExtensions()
        {
            var type = new ModelSerializationExtensionsTypeProvider();
            var visitor = new TestModelSerializationExtensionsVisitor();

            visitor.InvokeVisitType(type);

            var sentinelField = type.Fields.Single(f => f.Name == "_sentinelValue");
            var sentinelMethod = type.Methods.Single(m => m.Signature.Name == "IsSentinelValue");
            Assert.That(sentinelField.Modifiers, Is.EqualTo(FieldModifiers.Private | FieldModifiers.Static | FieldModifiers.ReadOnly));
            Assert.That(sentinelField.Type.FrameworkType, Is.EqualTo(typeof(BinaryData)));
            Assert.That(sentinelField.InitializationValue, Is.Not.Null);
            Assert.That(sentinelField.InitializationValue!.ToDisplayString(), Does.Contain("__EMPTY__"));
            Assert.That(sentinelMethod.Signature.Modifiers, Is.EqualTo(MethodSignatureModifiers.Internal | MethodSignatureModifiers.Static));
            Assert.That(sentinelMethod.Signature.ReturnType, Is.Not.Null);
            Assert.That(sentinelMethod.Signature.ReturnType!.FrameworkType, Is.EqualTo(typeof(bool)));
            Assert.That(sentinelMethod.BodyStatements, Is.Not.Null);
            Assert.That(sentinelMethod.BodyStatements!.ToDisplayString(), Does.Contain("sentinelSpan.SequenceEqual(valueSpan)"));

            AssertSentinelBehavior(sentinelField, sentinelMethod);
        }

        [Test]
        public void TestVisitType_ModelSerializationExtensions_NonTargetTypeIsUnchanged()
        {
            var type = new TestTypeProvider();

            new TestModelSerializationExtensionsVisitor().InvokeVisitType(type);

            Assert.That(type.Fields.Any(f => f.Name == "_sentinelValue"), Is.False);
            Assert.That(type.Methods.Any(m => m.Signature.Name == "IsSentinelValue"), Is.False);
        }

        [Test]
        public void TestConfigure_RegisteredSplitVisitorsProduceExpectedChanges()
        {
            var generator = new TestOpenAILibraryGenerator(
                MockHelpers.CreateMockGeneratorContext("{ \"package-name\": \"TestLibrary\" }").Object);
            generator.InvokeConfigure();

            var visitors = generator.Visitors.ToList();
            var additionalPropertiesIndex = visitors.FindIndex(v => v is AdditionalPropertiesVisitor);
            var modelSerializationExtensionsIndex = visitors.FindIndex(v => v is ModelSerializationExtensionsVisitor);
            var jsonModelWriteCoreIndex = visitors.FindIndex(v => v is JsonModelWriteCoreVisitor);
            Assert.That(additionalPropertiesIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(modelSerializationExtensionsIndex, Is.EqualTo(additionalPropertiesIndex + 1));
            Assert.That(jsonModelWriteCoreIndex, Is.EqualTo(modelSerializationExtensionsIndex + 1));

            // Restore the test singleton after configuring the real generator instance.
            MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");

            var inputType = InputFactory.Model("TestModel", "Samples", isDynamicModel: true, properties: [
                InputFactory.Property("cat", InputPrimitiveType.String),
                InputFactory.Property("requiredDog", InputPrimitiveType.String, isRequired: true)
            ]);
            ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)
                ?? throw new InvalidOperationException("Expected a model provider.");
            var additionalPropertiesField = new FieldProvider(
                FieldModifiers.Private | FieldModifiers.ReadOnly,
                typeof(IDictionary<string, BinaryData>),
                "_additionalBinaryDataProperties",
                model);
            model.Update(fields: [.. model.Fields, additionalPropertiesField]);

            InvokeRegisteredVisitor(visitors[additionalPropertiesIndex], "VisitField", additionalPropertiesField);
            InvokeRegisteredVisitor(visitors[additionalPropertiesIndex], "VisitType", model);
            Assert.That(additionalPropertiesField.Modifiers.HasFlag(FieldModifiers.ReadOnly), Is.False);
            Assert.That(model.Properties.Any(p => p.Name == "SerializedAdditionalRawData"), Is.True);

            var serializationExtensions = new ModelSerializationExtensionsTypeProvider();
            InvokeRegisteredVisitor(visitors[modelSerializationExtensionsIndex], "VisitType", serializationExtensions);
            Assert.That(serializationExtensions.Fields.Any(f => f.Name == "_sentinelValue"), Is.True);
            Assert.That(serializationExtensions.Methods.Any(m => m.Signature.Name == "IsSentinelValue"), Is.True);

            var jsonWriteCoreMethod = model.SerializationProviders
                .OfType<MrwSerializationTypeDefinition>()
                .FirstOrDefault()?
                .Methods
                .OfType<MethodProvider>()
                .First(m => m.Signature.Name == "JsonModelWriteCore")
                ?? throw new InvalidOperationException("Expected JsonModelWriteCore.");
            jsonWriteCoreMethod = InvokeRegisteredVisitor(visitors[jsonModelWriteCoreIndex], "VisitMethod", jsonWriteCoreMethod);
            Assert.That(
                jsonWriteCoreMethod.BodyStatements!.ToDisplayString(),
                Is.EqualTo(Helpers.GetExpectedFromFile()));
        }

        private static void AssertSentinelBehavior(FieldProvider sentinelField, MethodProvider sentinelMethod)
        {
            var source = $$"""
                using System;

                internal static class GeneratedModelSerializationExtensions
                {
                    private static readonly BinaryData _sentinelValue = {{sentinelField.InitializationValue!.ToDisplayString()}};

                    internal static bool IsSentinelValue(BinaryData value)
                    {
                        {{sentinelMethod.BodyStatements!.ToDisplayString()}}
                    }
                }
                """;
            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
            var trustedPlatformAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?
                .Split(Path.PathSeparator)
                ?? throw new InvalidOperationException("Expected trusted platform assemblies.");
            var references = trustedPlatformAssemblies
                .Append(typeof(BinaryData).Assembly.Location)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(path => MetadataReference.CreateFromFile(path));
            var compilation = CSharpCompilation.Create(
                $"SentinelBehavior_{Guid.NewGuid():N}",
                [syntaxTree],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var assemblyStream = new MemoryStream();
            var emitResult = compilation.Emit(assemblyStream);
            Assert.That(
                emitResult.Success,
                Is.True,
                string.Join(Environment.NewLine, emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));

            var assembly = Assembly.Load(assemblyStream.ToArray());
            var method = assembly.GetType("GeneratedModelSerializationExtensions")?
                .GetMethod("IsSentinelValue", BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("Expected generated sentinel method.");
            Assert.That(method.Invoke(null, [BinaryData.FromString("\"__EMPTY__\"")]), Is.True);
            Assert.That(method.Invoke(null, [BinaryData.FromString("__EMPTY__")]), Is.False);
            Assert.That(method.Invoke(null, [BinaryData.FromString("\"__EMPTY__x\"")]), Is.False);
        }

        private static T InvokeRegisteredVisitor<T>(LibraryVisitor visitor, string methodName, T provider)
            where T : class
        {
            var method = visitor.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .SingleOrDefault(m =>
                    m.Name == methodName &&
                    m.GetParameters() is [{ ParameterType: var parameterType }] &&
                    parameterType.IsAssignableFrom(provider.GetType()))
                ?? throw new InvalidOperationException($"Expected {visitor.GetType().Name}.{methodName}.");
            return (T)(method.Invoke(visitor, [provider])
                ?? throw new InvalidOperationException($"Expected {methodName} to return a provider."));
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

        private class TestOpenAILibraryGenerator : OpenAILibraryGenerator
        {
            public TestOpenAILibraryGenerator(GeneratorContext context) : base(context) { }

            public void InvokeConfigure() => base.Configure();
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
