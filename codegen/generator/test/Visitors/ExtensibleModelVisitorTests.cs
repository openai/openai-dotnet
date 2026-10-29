using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System;
using System.Linq;
using System.Reflection;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
// Synthetic generator models exercise opt-in boundaries and emitted C# without requiring a full spec build.
// Apply only ExtensibleModelVisitor; cross-visitor contracts belong in ExtensibleModelVisitorIntegrationTests.
public class ExtensibleModelVisitorTests
{
    // Reset the generator singleton and type cache so tests cannot inherit another test's promoted enum.
    [SetUp]
    public void Setup() =>
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");

    // Both namespace casings must emit the same extensibility contract, retain required initialization,
    // and leave the full serialization constructor alone. Source assertions catch stale provider caches.
    [TestCase("OpenAI.Responses")]
    [TestCase("openai.responses")]
    public void EmitsExtensibilityContract(string modelNamespace)
    {
        var model = CreateModel(modelNamespace, enumAccess: "internal", additionalRequiredProperty: true);
        var constructor = model.Constructors.Single(c => c.Signature.Parameters.Count == 2);
        var originalBody = constructor.BodyStatements;
        var fullConstructor = model.FullConstructor;
        var fullModifiers = fullConstructor.Signature.Modifiers;
        var discriminator = model.Properties.Single(p => p.IsDiscriminator);
        Assert.That(model.DiscriminatorValue, Is.Null);
        Assert.That(model.IsUnknownDiscriminatorModel, Is.False);
        Assert.That(model.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public), Is.True);
        Assert.That(ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap.TryGetValue(discriminator.Type, out var kind), Is.True);
        Assert.That(kind, Is.InstanceOf<EnumProvider>());
        Assert.That(((EnumProvider)kind!).IsExtensible, Is.True);
        Assert.That(kind.EnumUnderlyingType, Is.EqualTo(new CSharpType(typeof(string))));
        var initialization = Snippet.Literal("initial");
        discriminator.Update(body: new AutoPropertyBody(HasSetter: true, InitializationExpression: initialization));

        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(discriminator.Name, Is.EqualTo("Kind"));
        Assert.That(discriminator.Modifiers, Is.EqualTo(MethodSignatureModifiers.Public));
        Assert.That(((AutoPropertyBody)discriminator.Body).HasSetter, Is.False);
        Assert.That(((AutoPropertyBody)discriminator.Body).InitializationExpression, Is.SameAs(initialization));
        Assert.That(discriminator.Type.IsPublic, Is.True);
        Assert.That(constructor.Signature.Modifiers,
            Is.EqualTo(MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal));
        Assert.That(constructor.Signature.Parameters.Select(p => p.Name), Is.EqualTo(new[] { "kind", "sequenceNumber" }));
        Assert.That(constructor.BodyStatements, Is.SameAs(originalBody));
        Assert.That(fullConstructor.Signature.Modifiers, Is.EqualTo(fullModifiers));

        var source = new TypeProviderWriter(model).Write().Content;
        Assert.That(source, Does.Contain("public global::OpenAI.Responses.TestKind Kind { get; }").IgnoreCase
            .Or.Contain("public TestKind Kind { get; }"));
        Assert.That(source, Does.Contain("protected internal TestModel("));
        Assert.That(source, Does.Contain("Kind = kind;"));
        Assert.That(source, Does.Contain("SequenceNumber = sequenceNumber;"));
        var enumProvider = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[discriminator.Type];
        var enumSource = new TypeProviderWriter(enumProvider!).Write().Content;
        Assert.That(enumSource, Does.Contain("public readonly partial struct TestKind"));
        Assert.That(enumSource, Does.Contain("public TestKind(string value)"));
        Assert.That(enumSource, Does.Contain("public static"));
    }

    // The ResponseItem-shaped base must keep exactly the discriminator-only initialization signature.
    [Test]
    public void PreservesSingleDiscriminatorConstructor()
    {
        var model = CreateModel();
        new TestExtensibleModelVisitor().Apply(model);
        var source = new TypeProviderWriter(model).Write().Content;
        Assert.That(source, Does.Contain("protected internal TestModel(global::OpenAI.Responses.TestKind kind)"));
        Assert.That(source, Does.Contain("Kind = kind;"));
    }

    // Repeated visits must preserve customized enum members, their accessibility, and the output path.
    [Test]
    public void PromotingEnumPreservesInternalMembersAndIsIdempotent()
    {
        var model = CreateModel(enumAccess: "internal");
        var discriminator = model.Properties.Single(p => p.IsDiscriminator);
        var kind = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[discriminator.Type]!;
        kind.Update(relativeFilePath: "../Responses/CustomKind.cs");
        var member = kind.Properties.Single(p => p.Name == "Known");
        member.Update(modifiers: MethodSignatureModifiers.Internal | MethodSignatureModifiers.Static);

        new TestExtensibleModelVisitor().Apply(model);
        var firstSource = new TypeProviderWriter(model).Write().Content;
        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(kind.Type.IsPublic, Is.True);
        Assert.That(kind.RelativeFilePath, Is.EqualTo("../Responses/CustomKind.cs"));
        Assert.That(kind.Properties.Single(p => p.Name == "Known"), Is.SameAs(member));
        Assert.That(member.Modifiers.HasFlag(MethodSignatureModifiers.Internal), Is.True);
        Assert.That(new TypeProviderWriter(model).Write().Content, Is.EqualTo(firstSource));
    }

    // Simulate a handwritten discriminator suppressing its generated property while constructor metadata remains.
    [Test]
    public void CustomizedDiscriminatorStillExposesInitializationConstructor()
    {
        var model = CreateModel(additionalRequiredProperty: true);
        var constructor = model.Constructors.Single(c => c.Signature.Parameters.Count == 2);
        model.Update(properties: []);

        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(constructor.Signature.Modifiers,
            Is.EqualTo(MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal));
        Assert.That(constructor.Signature.Parameters.Select(p => p.Name), Is.EqualTo(new[] { "kind", "sequenceNumber" }));
        Assert.That(new TypeProviderWriter(model).Write().Content,
            Does.Contain("protected internal TestModel(global::OpenAI.Responses.TestKind kind, int sequenceNumber)"));
    }

    // Both original-name and renamed handwritten internal discriminators opt out of contract promotion.
    [TestCase(false)]
    [TestCase(true)]
    public void ExplicitInternalDiscriminatorCustomizationRemainsAuthoritative(bool renamed)
    {
        var model = CreateModel(enumAccess: "internal");
        var constructor = model.Constructors.Single(c => c.Signature.Parameters.Count == 1);
        var discriminator = model.Properties.Single(p => p.IsDiscriminator);
        var originalModifiers = constructor.Signature.Modifiers;
        var customProperty = new PropertyProvider(null, MethodSignatureModifiers.Internal, discriminator.Type,
            renamed ? "InternalType" : discriminator.Name, new AutoPropertyBody(HasSetter: true),
            new CustomizedDiscriminatorType(discriminator));
        typeof(PropertyProvider).GetProperty(nameof(PropertyProvider.OriginalName))!.SetValue(customProperty, discriminator.Name);
        typeof(TypeProvider).GetField("_customCodeView", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(model, new Lazy<TypeProvider>(() => new CustomizedDiscriminatorType(customProperty)));

        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(constructor.Signature.Modifiers, Is.EqualTo(originalModifiers));
        Assert.That(discriminator.Type.IsPublic, Is.False);
        Assert.That(discriminator.Modifiers, Is.EqualTo(MethodSignatureModifiers.Internal));
    }

    // Namespace opt-in is exact (ignoring case), not inherited from protocol-model opt-in or namespace prefixes.
    [TestCase("OpenAI.Chat")]
    [TestCase("OpenAI.Realtime")]
    [TestCase("OpenAI.Responses.Other")]
    public void DoesNotOptInOtherNamespaces(string modelNamespace)
    {
        var model = CreateModel(modelNamespace);
        AssertUnchanged(model);
    }

    [TestCase(false, true, "public", null)] // Closed enum.
    [TestCase(true, false, "public", null)] // Ordinary property, not discriminator metadata.
    [TestCase(true, true, "internal", null)] // Non-public base.
    [TestCase(true, true, "public", "known")] // Fixed-discriminator leaf.
    public void LeavesIneligibleModelsUnchanged(bool isExtensible, bool isDiscriminator, string access, string? discriminatorValue)
    {
        var model = CreateModel(isExtensible: isExtensible, isDiscriminator: isDiscriminator,
            access: access, discriminatorValue: discriminatorValue);
        AssertUnchanged(model);
    }

    // Extensibility alone is insufficient: this contract requires string-backed values.
    [Test]
    public void LeavesNumericExtensibleDiscriminatorsUnchanged()
    {
        var inputEnum = new InputEnumType("NumericKind", "OpenAI.Responses", "NumericKind", "public",
            null, null, "Numeric kind", InputModelTypeUsage.Input | InputModelTypeUsage.Output,
            InputPrimitiveType.Int32, [], true);
        var model = CreateModel(inputEnum: inputEnum);
        AssertUnchanged(model);
    }

    // A raw string is not an enum provider, so this visitor must not invent a kind type.
    [Test]
    public void LeavesRawStringDiscriminatorsUnchanged()
    {
        var input = InputFactory.Model("StringDiscriminator", "OpenAI.Responses",
            properties: [InputFactory.Property("type", InputPrimitiveType.String, isRequired: true, isDiscriminator: true)]);
        AssertUnchanged(ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!);
    }

    // An opted-in child cannot expose or otherwise mutate a discriminator owned by a non-opted-in base.
    [Test]
    public void DoesNotMutateDiscriminatorInheritedFromNonOptedInNamespace()
    {
        var rootInput = InputFactory.Model("Root", "OpenAI.Chat",
            properties: [InputFactory.Property("type", InputFactory.StringEnum("OtherKind", [], isExtensible: true),
                isRequired: true, isDiscriminator: true)]);
        var input = InputFactory.Model("Child", "OpenAI.Responses", properties: [], baseModel: rootInput);
        var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;

        AssertUnchanged(model.BaseModelProvider!);
        var originalVisibility = model.BaseModelProvider!.Properties.Single().Modifiers;
        AssertUnchanged(model);
        Assert.That(model.BaseModelProvider.Properties.Single().Modifiers, Is.EqualTo(originalVisibility));
    }

    // Open intermediate bases accept an inherited kind, while fixed leaves keep their original constructors.
    [Test]
    public void SupportsInheritedDiscriminatorWithoutChangingFixedLeaf()
    {
        var rootInput = InputFactory.Model("Root", "OpenAI.Responses",
            properties: [InputFactory.Property("type", InputFactory.StringEnum("InheritedKind", [("Known", "known")], isExtensible: true),
                isRequired: true, isDiscriminator: true)]);
        var intermediateInput = InputFactory.Model("Intermediate", "OpenAI.Responses",
            properties: [], baseModel: rootInput);
        var leafInput = InputFactory.Model("Leaf", "OpenAI.Responses",
            properties: [], baseModel: rootInput, discriminatedKind: "known");
        var intermediate = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(intermediateInput)!;
        var leaf = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(leafInput)!;
        var leafModifiers = leaf.Constructors.Select(c => c.Signature.Modifiers).ToArray();

        new TestExtensibleModelVisitor().Apply(intermediate.BaseModelProvider!);
        new TestExtensibleModelVisitor().Apply(intermediate);
        new TestExtensibleModelVisitor().Apply(leaf);

        Assert.That(intermediate.BaseModelProvider!.Properties.Single(p => p.IsDiscriminator).Modifiers,
            Is.EqualTo(MethodSignatureModifiers.Public));
        Assert.That(intermediate.Constructors.Any(c =>
            c.Signature.Modifiers == (MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal)), Is.True,
            new TypeProviderWriter(intermediate).Write().Content);
        Assert.That(leaf.Constructors.Select(c => c.Signature.Modifiers), Is.EqualTo(leafModifiers));
    }

    // Defaults describe an eligible ResponseItem-shaped model. Each argument varies one eligibility condition;
    // the optional sequenceNumber models bases requiring more than the discriminator parameter.
    // Name the input property "kind" directly so these tests do not depend on KindRenameVisitor.
    private static ModelProvider CreateModel(
        string modelNamespace = "OpenAI.Responses",
        bool isExtensible = true,
        bool isDiscriminator = true,
        string access = "public",
        string enumAccess = "public",
        string? discriminatorValue = null,
        bool additionalRequiredProperty = false,
        InputEnumType? inputEnum = null)
    {
        inputEnum ??= InputFactory.StringEnum("TestKind", [("Known", "known")], access: enumAccess,
            isExtensible: isExtensible, clientNamespace: modelNamespace);
        InputModelProperty[] properties = additionalRequiredProperty
            ? [InputFactory.Property("kind", inputEnum, isRequired: true, isDiscriminator: isDiscriminator),
                InputFactory.Property("sequenceNumber", InputPrimitiveType.Int32, isRequired: true)]
            : [InputFactory.Property("kind", inputEnum, isRequired: true, isDiscriminator: isDiscriminator)];
        return ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(InputFactory.Model("TestModel", modelNamespace,
            access: access, properties: properties, discriminatedKind: discriminatorValue))!;
    }

    // Exercise only extensibility, comparing property bodies/visibility and constructor modifiers before and after.
    private static void AssertUnchanged(ModelProvider model)
    {
        var properties = model.Properties.Select(p => (p.Modifiers, p.Body)).ToArray();
        var constructors = model.Constructors.Select(c => c.Signature.Modifiers).ToArray();
        new TestExtensibleModelVisitor().Apply(model);
        Assert.That(model.Properties.Select(p => (p.Modifiers, p.Body)), Is.EqualTo(properties));
        Assert.That(model.Constructors.Select(c => c.Signature.Modifiers), Is.EqualTo(constructors));
    }

    // Expose the protected hook without traversing the rest of the generator pipeline.
    private sealed class TestExtensibleModelVisitor : ExtensibleModelVisitor
    {
        public TypeProvider? Apply(TypeProvider type) => PostVisitType(type);
    }

    // Stand-in for the generator's handwritten partial-class view used by customization precedence tests.
    private sealed class CustomizedDiscriminatorType(PropertyProvider discriminator) : TypeProvider
    {
        protected override string BuildName() => "TestModel";
        protected override string BuildRelativeFilePath() => "TestModel.cs";
        protected override PropertyProvider[] BuildProperties() => [discriminator];
    }
}
