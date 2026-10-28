using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using Microsoft.TypeSpec.Generator.Expressions;
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
// Pipeline tests combine the relevant passes; isolated visitor tests ensure unrelated behavior stays unchanged.
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
    public void Pipeline_EmitsExtensibilityContract(string modelNamespace)
    {
        var model = CreateModel(modelNamespace, enumAccess: "internal", additionalRequiredProperty: true);
        ApplyPropertyVisitors(model);
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

        ApplyPipeline(model);

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
        new TestVisibilityVisitor().Apply(enumProvider!);
        var enumSource = new TypeProviderWriter(enumProvider!).Write().Content;
        Assert.That(enumSource, Does.Contain("public readonly partial struct TestKind"));
        Assert.That(enumSource, Does.Contain("public TestKind(string value)"));
        Assert.That(enumSource, Does.Contain("public static"));
        Assert.That(enumSource, Does.Contain("ExperimentalAttribute"));
    }

    // A later annotation pass must see the promoted enum and add exactly one experimental attribute.
    [Test]
    public void ExperimentalPassAnnotatesPromotedEnumOnce()
    {
        var model = CreateModel(enumAccess: "internal");
        var discriminator = model.Properties.Single(p => p.IsDiscriminator);
        var kind = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[discriminator.Type]!;
        var experimentalVisitor = new TestExperimentalAttributeVisitor();
        experimentalVisitor.Apply(kind);
        Assert.That(kind.Attributes, Is.Empty);

        new TestExtensibleModelVisitor().Apply(model);
        Assert.That(kind.Type.IsPublic, Is.True);
        Assert.That(kind.Attributes, Is.Empty);

        experimentalVisitor.Apply(kind);
        experimentalVisitor.Apply(kind);

        Assert.That(kind.Attributes.Count(attribute =>
            attribute.Type.Equals(typeof(System.Diagnostics.CodeAnalysis.ExperimentalAttribute))), Is.EqualTo(1));
        Assert.That(new TypeProviderWriter(kind).Write().Content, Does.Contain("ExperimentalAttribute"));
    }

    // The ResponseItem-shaped base must keep exactly the discriminator-only initialization signature.
    [Test]
    public void Pipeline_PreservesSingleKindConstructor()
    {
        var model = CreateModel();
        ApplyPipeline(model);
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

        ApplyPipeline(model);
        var firstSource = new TypeProviderWriter(model).Write().Content;
        ApplyPipeline(model);

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
        ApplyPropertyVisitors(model);
        var constructor = model.Constructors.Single(c => c.Signature.Parameters.Count == 2);
        model.Update(properties: []);

        new TestExtensibleModelVisitor().Apply(model);
        new TestVisibilityVisitor().Apply(model);

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

        ApplyPipeline(intermediate.BaseModelProvider!);
        new TestExtensibleModelVisitor().Apply(intermediate);
        new TestExtensibleModelVisitor().Apply(leaf);

        Assert.That(intermediate.BaseModelProvider!.Properties.Single(p => p.IsDiscriminator).Modifiers,
            Is.EqualTo(MethodSignatureModifiers.Public));
        Assert.That(intermediate.Constructors.Any(c =>
            c.Signature.Modifiers == (MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal)), Is.True,
            new TypeProviderWriter(intermediate).Write().Content);
        Assert.That(leaf.Constructors.Select(c => c.Signature.Modifiers), Is.EqualTo(leafModifiers));
    }

    // Visiting a child after its base must not undo the base's explicit internal visibility override.
    [Test]
    public void InheritedDiscriminatorPreservesBaseVisibilityOverride()
    {
        var rootInput = InputFactory.Model("Root", "OpenAI.Responses",
            properties: [InputFactory.Property("type", InputFactory.StringEnum("InheritedKind", [], isExtensible: true),
                isRequired: true, isDiscriminator: true)]);
        var input = InputFactory.Model("Child", "OpenAI.Responses", properties: [], baseModel: rootInput);
        var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;
        var root = model.BaseModelProvider!;
        root.Update(attributes: [VisibilityAttribute("Kind", CodeGenVisibility.Internal)]);
        ApplyPipeline(root);

        new TestExtensibleModelVisitor().Apply(model);

        Assert.That(root.Properties.Single(p => p.IsDiscriminator).Modifiers, Is.EqualTo(MethodSignatureModifiers.Internal));
    }

    // Every supported override wins over the extensibility visitor for both property and constructor.
    [TestCase(CodeGenVisibility.Internal)]
    [TestCase(CodeGenVisibility.Public)]
    [TestCase(CodeGenVisibility.ProtectedInternal)]
    public void ExplicitVisibilityOverridesTakePrecedence(CodeGenVisibility visibility)
    {
        var model = CreateModel();
        var kindType = model.Properties.Single(p => p.IsDiscriminator).Type;
        model.Update(attributes:
        [
            VisibilityAttribute("TestModel", visibility, new TypeOfExpression(kindType)),
            VisibilityAttribute("Kind", visibility),
        ]);

        ApplyPipeline(model);

        var expected = visibility switch
        {
            CodeGenVisibility.Internal => MethodSignatureModifiers.Internal,
            CodeGenVisibility.Public => MethodSignatureModifiers.Public,
            CodeGenVisibility.ProtectedInternal => MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal,
            _ => throw new ArgumentOutOfRangeException(nameof(visibility)),
        };
        Assert.That(model.Constructors.Single(c => c.Signature.Parameters.Count == 1).Signature.Modifiers,
            Is.EqualTo(expected));
        Assert.That(model.Properties.Single(p => p.IsDiscriminator).Modifiers, Is.EqualTo(expected));
    }

    // Even an override that changes no modifiers is authoritative: do not fall through to the
    // "public property with non-public type becomes internal" default rule.
    [Test]
    public void MatchingOverrideDoesNotFallThroughToDefaultVisibility()
    {
        var model = CreateModel(enumAccess: "internal");
        var discriminator = model.Properties.Single(p => p.IsDiscriminator);
        discriminator.Update(modifiers: MethodSignatureModifiers.Public);
        model.Update(attributes: [VisibilityAttribute(discriminator.Name, CodeGenVisibility.Public)]);

        new TestVisibilityVisitor().Apply(model);

        Assert.That(discriminator.Modifiers, Is.EqualTo(MethodSignatureModifiers.Public));
        Assert.That(((AutoPropertyBody)discriminator.Body).HasSetter, Is.False);
    }

    // Build the generator's attribute representation; enum arguments appear as underlying integer literals.
    private static AttributeStatement VisibilityAttribute(string name, CodeGenVisibility visibility, params ValueExpression[] parameters) =>
        new(new CSharpType(typeof(CodeGenVisibilityAttribute)),
            [new LiteralExpression(name), new LiteralExpression((int)visibility), .. parameters]);

    // Defaults describe an eligible ResponseItem-shaped model. Each argument varies one eligibility condition;
    // the optional sequenceNumber models bases whose initialization requires more than the kind.
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
            ? [InputFactory.Property("type", inputEnum, isRequired: true, isDiscriminator: isDiscriminator),
                InputFactory.Property("sequenceNumber", InputPrimitiveType.Int32, isRequired: true)]
            : [InputFactory.Property("type", inputEnum, isRequired: true, isDiscriminator: isDiscriminator)];
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

    // Mirror relevant production ordering: rename/protocol setters, extensibility, explicit visibility, then
    // experimental annotations. Visit the enum explicitly because this helper does not traverse a whole library.
    private static void ApplyPipeline(ModelProvider model)
    {
        ApplyPropertyVisitors(model);
        new TestExtensibleModelVisitor().Apply(model);
        new TestVisibilityVisitor().Apply(model);
        var experimentalVisitor = new TestExperimentalAttributeVisitor();
        experimentalVisitor.Apply(model);
        var discriminator = model.Properties.SingleOrDefault(property => property.IsDiscriminator);
        if (discriminator is not null
            && ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap.TryGetValue(discriminator.Type, out var kind))
        {
            experimentalVisitor.Apply(kind!);
        }
    }

    // Establish the CLR Kind name and protocol setter before testing how extensibility finalizes the property.
    private static void ApplyPropertyVisitors(ModelProvider model)
    {
        foreach (var property in model.Properties)
        {
            var input = InputFactory.Property(property.Name, InputPrimitiveType.String);
            new TestKindRenameVisitor().Apply(input, property);
            new TestProtocolModelVisitor().Apply(input, property);
        }
    }

    // Adapters expose protected hooks so tests can run individual passes without a full generator traversal.
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

    // Stand-in for the generator's handwritten partial-class view used by customization precedence tests.
    private sealed class CustomizedDiscriminatorType(PropertyProvider discriminator) : TypeProvider
    {
        protected override string BuildName() => "TestModel";
        protected override string BuildRelativeFilePath() => "TestModel.cs";
        protected override PropertyProvider[] BuildProperties() => [discriminator];
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
