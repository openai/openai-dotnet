using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
public class ConstructorFixupVisitorTests
{
    [SetUp]
    public void Setup()
    {
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
    }

    [Test]
    public void PostVisitType_InitializesCollectionInsideSuppressionStatement()
    {
        var inputType = InputFactory.Model(
            "TestModel",
            "Samples",
            properties:
            [
                InputFactory.Property(
                    "items",
                    InputFactory.Array(InputPrimitiveType.String),
                    isRequired: true,
                    isReadOnly: true),
            ]);
        ModelProvider model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)!;
        ConstructorProvider constructor = model.Constructors
            .Concat(model.SerializationProviders.SelectMany(provider => provider.Constructors))
            .Single(candidate => candidate.Signature.Parameters.Any(parameter => parameter.Name == "items"));
        List<MethodBodyStatement> statements = constructor.BodyStatements!.ToList();
        int assignmentIndex = statements.FindIndex(
            statement => statement.ToDisplayString().Contains("Items = items;"));
        Assert.That(assignmentIndex, Is.GreaterThanOrEqualTo(0));
        statements[assignmentIndex] = new SuppressionStatement(
            statements[assignmentIndex],
            new LiteralExpression("OPENAI001"),
            "Experimental collection");
        constructor.Update(bodyStatements: statements);

        new TestConstructorFixupVisitor().Apply(model);

        Assert.That(
            constructor.BodyStatements!.ToDisplayString(),
            Does.Contain("Items = (items ?? new global::Samples.ChangeTrackingList<string>());"));
        Assert.That(constructor.BodyStatements.Any(statement => statement is SuppressionStatement), Is.True);
    }

    [Test]
    public void PostVisitType_DefaultConstructorPassesStringLiteralInitializer()
    {
        ModelProvider model = CreateModelWithDefaultConstructor();
        SetInitializer(model, "Object", Literal("list"));

        new TestConstructorFixupVisitor().Apply(model);

        Assert.That(GetDefaultConstructorArguments(model), Is.EqualTo("null, \"list\", default, default, null"));
    }

    [Test]
    public void PostVisitType_DefaultConstructorPassesEnumMemberInitializer()
    {
        ModelProvider model = CreateModelWithDefaultConstructor();
        PropertyProvider statusProperty = model.Properties.Single(property => property.Name == "Status");
        SetInitializer(model, "Status", new MemberExpression(Static(statusProperty.Type), "Active"));

        new TestConstructorFixupVisitor().Apply(model);

        Assert.That(GetDefaultConstructorArguments(model), Is.EqualTo("null, null, default, global::Sample.Models.TestStatus.Active, null"));
    }

    [TestCase("Count", "default")]
    [TestCase("Items", "[]")]
    public void PostVisitType_DefaultConstructorIgnoresSourceTextInitializerForNonStringParameter(string propertyName, string sourceText)
    {
        ModelProvider model = CreateModelWithDefaultConstructor();
        SetInitializer(model, propertyName, Literal(sourceText));

        new TestConstructorFixupVisitor().Apply(model);

        Assert.That(GetDefaultConstructorArguments(model), Is.EqualTo("null, null, default, default, null"));
    }

    private static ModelProvider CreateModelWithDefaultConstructor()
    {
        var inputType = InputFactory.Model(
            "TestModel",
            "Samples",
            properties:
            [
                // A collection property makes the visitor chain the default constructor to the serialization constructor.
                InputFactory.Property("items", InputFactory.Array(InputPrimitiveType.String), isRequired: true),
                InputFactory.Property("object", InputPrimitiveType.String, isRequired: true),
                InputFactory.Property("count", InputPrimitiveType.Int32, isRequired: true),
                InputFactory.Property(
                    "status",
                    InputFactory.StringEnum("TestStatus", [("Active", "active")], isExtensible: false),
                    isRequired: true),
            ]);
        return ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(inputType)!;
    }

    private static void SetInitializer(ModelProvider model, string propertyName, ValueExpression initializer)
    {
        PropertyProvider property = model.Properties.Single(candidate => candidate.Name == propertyName);
        property.Update(body: new AutoPropertyBody(false, InitializationExpression: initializer));
    }

    private static string GetDefaultConstructorArguments(ModelProvider model)
    {
        ConstructorProvider defaultConstructor = model.Constructors
            .Concat(model.SerializationProviders.SelectMany(provider => provider.Constructors))
            .Single(candidate => candidate.Signature.Parameters.Count == 0);
        return string.Join(", ", defaultConstructor.Signature.Initializer!.Arguments.Select(argument => argument.ToDisplayString()));
    }

    private sealed class TestConstructorFixupVisitor : ConstructorFixupVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
    }
}
