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

    private sealed class TestConstructorFixupVisitor : ConstructorFixupVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
    }
}
