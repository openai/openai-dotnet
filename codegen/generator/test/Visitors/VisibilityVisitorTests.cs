using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Tests.Visitors
{
    [Category("Visitor")]
    public class VisibilityVisitorTests
    {
        [SetUp]
        public void Setup()
        {
            MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
        }

        [Test]
        public void PostVisitType_AppliesProtectedInternalVisibilityToConstructor()
        {
            var typeProvider = new TestTypeProvider(
                attributes:
                [
                    new AttributeStatement(new CSharpType(typeof(CodeGenVisibilityAttribute)),
                        new LiteralExpression(nameof(TestTypeProvider)),
                        new LiteralExpression(2),
                        new TypeOfExpression(new CSharpType(typeof(string))),
                        new TypeOfExpression(new CSharpType(typeof(int))))
                ]);

            var constructor = typeProvider.Constructors[0];

            new TestVisibilityVisitor().Apply(typeProvider);

            Assert.That(constructor.Signature.Modifiers,
                Is.EqualTo(MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal));
        }

        // An already-matching override must not fall through to the non-public-type accessibility safeguard.
        [Test]
        public void MatchingOverrideDoesNotFallThroughToDefaultVisibility()
        {
            var input = InputFactory.Model("TestModel", "OpenAI.Responses",
                properties: [InputFactory.Property("type",
                    InputFactory.StringEnum("TestKind", [], access: "internal", isExtensible: true),
                    isRequired: true, isDiscriminator: true)]);
            var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;
            var discriminator = model.Properties.Single(property => property.IsDiscriminator);
            discriminator.Update(modifiers: MethodSignatureModifiers.Public);
            model.Update(attributes:
            [
                new AttributeStatement(new CSharpType(typeof(CodeGenVisibilityAttribute)),
                    new LiteralExpression(discriminator.Name), new LiteralExpression((int)CodeGenVisibility.Public))
            ]);

            new TestVisibilityVisitor().Apply(model);

            Assert.That(discriminator.Modifiers, Is.EqualTo(MethodSignatureModifiers.Public));
            Assert.That(((AutoPropertyBody)discriminator.Body).HasSetter, Is.False);
        }

        private sealed class TestVisibilityVisitor : VisibilityVisitor
        {
            public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
        }

        private sealed class TestTypeProvider : TypeProvider
        {
            private readonly IReadOnlyList<MethodBodyStatement> _attributes;

            public TestTypeProvider(IReadOnlyList<MethodBodyStatement> attributes)
            {
                _attributes = attributes;
            }

            protected override IReadOnlyList<MethodBodyStatement> BuildAttributes() => _attributes;

            protected override ConstructorProvider[] BuildConstructors() =>
            [
                new(
                    new ConstructorSignature(
                        type: Type,
                        description: null,
                        modifiers: MethodSignatureModifiers.Private | MethodSignatureModifiers.Protected,
                        parameters:
                        [
                            new ParameterProvider("kind", $"{nameof(String)} kind", new CSharpType(typeof(string))),
                            new ParameterProvider("sequenceNumber", $"{nameof(Int32)} sequenceNumber", new CSharpType(typeof(int))),
                        ]),
                    MethodBodyStatement.Empty,
                    this)
            ];

            protected override string BuildRelativeFilePath() => $"{BuildName()}.cs";

            protected override string BuildName() => nameof(TestTypeProvider);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        private sealed class CodeGenVisibilityAttribute : Attribute
        {
        }

        private enum CodeGenVisibility
        {
            Internal,
            Public,
        }
    }
}
