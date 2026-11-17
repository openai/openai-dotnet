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
using System;
using System.Linq;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
// Exercise only the passes participating in each cross-visitor contract, not a copy of the full pipeline.
public class ExtensibleModelVisitorIntegrationTests
{
    [SetUp]
    public void Setup() =>
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");

    // Extensibility must finalize the renamed discriminator after protocol models introduce a setter.
    [Test]
    public void RemovesProtocolSetterFromRenamedDiscriminator()
    {
        var model = CreateModel();
        var discriminatorProperty = model.Properties.Single(property => property.IsDiscriminator);
        var input = InputFactory.Property("type", InputPrimitiveType.String, isRequired: true, isDiscriminator: true);
        new TestKindRenameVisitor().Apply(input, discriminatorProperty);
        new TestProtocolModelVisitor().Apply(input, discriminatorProperty);
        Assert.That(discriminatorProperty.Name, Is.EqualTo("Kind"));
        Assert.That(((AutoPropertyBody)discriminatorProperty.Body).HasSetter, Is.True);

        new TestExtensibleModelVisitor().Apply(model);

        var source = new TypeProviderWriter(model).Write().Content;
        Assert.That(source, Does.Contain("public global::OpenAI.Responses.TestKind Kind { get; }"));
        Assert.That(source, Does.Contain("protected internal TestModel(global::OpenAI.Responses.TestKind kind)"));
        Assert.That(source, Does.Contain("Kind = kind;"));
    }

    // A later annotation pass must see the promoted enum and add exactly one experimental attribute.
    [Test]
    public void ExperimentalPassAnnotatesPromotedEnumOnce()
    {
        var model = CreateModel(enumAccess: "internal");
        var discriminatorProperty = model.Properties.Single(property => property.IsDiscriminator);
        var discriminatorType = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[discriminatorProperty.Type]!;
        var experimentalVisitor = new TestExperimentalAttributeVisitor();
        experimentalVisitor.Apply(discriminatorType);
        Assert.That(discriminatorType.Attributes, Is.Empty);

        new TestExtensibleModelVisitor().Apply(model);
        Assert.That(discriminatorType.Type.IsPublic, Is.True);
        Assert.That(discriminatorType.Attributes, Is.Empty);

        experimentalVisitor.Apply(discriminatorType);
        experimentalVisitor.Apply(discriminatorType);

        Assert.That(discriminatorType.Attributes.Count(attribute =>
            attribute.Type.Equals(typeof(System.Diagnostics.CodeAnalysis.ExperimentalAttribute))), Is.EqualTo(1));
        var source = new TypeProviderWriter(discriminatorType).Write().Content;
        Assert.That(source, Does.Contain("public readonly partial struct TestKind"));
        Assert.That(source, Does.Contain("ExperimentalAttribute"));
    }

    // Every supported override wins over extensibility for both the property and its initialization constructor.
    [TestCase(CodeGenVisibility.Internal)]
    [TestCase(CodeGenVisibility.Public)]
    [TestCase(CodeGenVisibility.ProtectedInternal)]
    public void ExplicitVisibilityOverridesTakePrecedence(CodeGenVisibility visibility)
    {
        var model = CreateModel();
        var discriminatorProperty = model.Properties.Single(property => property.IsDiscriminator);
        model.Update(attributes:
        [
            VisibilityAttribute("TestModel", visibility, new TypeOfExpression(discriminatorProperty.Type)),
            VisibilityAttribute(discriminatorProperty.Name, visibility),
        ]);

        new TestExtensibleModelVisitor().Apply(model);
        new TestVisibilityVisitor().Apply(model);

        var expected = visibility switch
        {
            CodeGenVisibility.Internal => MethodSignatureModifiers.Internal,
            CodeGenVisibility.Public => MethodSignatureModifiers.Public,
            CodeGenVisibility.ProtectedInternal => MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal,
            _ => throw new ArgumentOutOfRangeException(nameof(visibility)),
        };
        Assert.That(model.Constructors.Single(constructor => constructor.Signature.Parameters.Count == 1).Signature.Modifiers,
            Is.EqualTo(expected));
        Assert.That(discriminatorProperty.Modifiers, Is.EqualTo(expected));
    }

    // Visiting a child after finalizing its base must not undo the base's explicit visibility override.
    [Test]
    public void InheritedDiscriminatorPreservesBaseVisibilityOverride()
    {
        var rootInput = InputFactory.Model("Root", "OpenAI.Responses",
            properties: [InputFactory.Property("type", InputFactory.StringEnum("InheritedKind", [], isExtensible: true),
                isRequired: true, isDiscriminator: true)]);
        var input = InputFactory.Model("Child", "OpenAI.Responses", properties: [], baseModel: rootInput);
        var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;
        var root = model.BaseModelProvider!;
        var discriminatorProperty = root.Properties.Single(property => property.IsDiscriminator);
        root.Update(attributes: [VisibilityAttribute(discriminatorProperty.Name, CodeGenVisibility.Internal)]);
        new TestExtensibleModelVisitor().Apply(root);
        new TestVisibilityVisitor().Apply(root);

        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(discriminatorProperty.Modifiers, Is.EqualTo(MethodSignatureModifiers.Internal));
    }

    // A minimal discriminated base suffices for interaction tests; eligibility variations stay in the unit fixture.
    private static ModelProvider CreateModel(string enumAccess = "public") =>
        ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(InputFactory.Model("TestModel", "OpenAI.Responses",
            properties: [InputFactory.Property("type",
                InputFactory.StringEnum("TestKind", [("Known", "known")], access: enumAccess, isExtensible: true,
                    clientNamespace: "OpenAI.Responses"), isRequired: true, isDiscriminator: true)]))!;

    // Enum attribute arguments are represented by their underlying integer literals in generator metadata.
    private static AttributeStatement VisibilityAttribute(string name, CodeGenVisibility visibility, params ValueExpression[] parameters) =>
        new(new CSharpType(typeof(CodeGenVisibilityAttribute)),
            [new LiteralExpression(name), new LiteralExpression((int)visibility), .. parameters]);

    // Expose only the protected hooks needed by these focused interactions.
    private sealed class TestExtensibleModelVisitor : ExtensibleModelVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
    }

    private sealed class TestKindRenameVisitor : KindRenameVisitor
    {
        public PropertyProvider? Apply(InputProperty input, PropertyProvider property) => PreVisitProperty(input, property);
    }

    private sealed class TestProtocolModelVisitor : ProtocolModelVisitor
    {
        public PropertyProvider? Apply(InputProperty input, PropertyProvider property) => PreVisitProperty(input, property);
    }

    private sealed class TestVisibilityVisitor : VisibilityVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
    }

    private sealed class TestExperimentalAttributeVisitor : ExperimentalAttributeVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => VisitType(type);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    private sealed class CodeGenVisibilityAttribute : Attribute;

    // Mirror OpenAI's internal customization enum without taking a dependency on the SDK assembly.
    public enum CodeGenVisibility
    {
        Internal,
        Public,
        ProtectedInternal,
    }
}
