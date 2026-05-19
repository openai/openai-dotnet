using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System;
using System.Collections.Generic;

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
    }
}
