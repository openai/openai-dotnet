using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
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
    [TestCase(CodeGenVisibility.Internal, false)]
    [TestCase(CodeGenVisibility.Public, false)]
    [TestCase(CodeGenVisibility.ProtectedInternal, false)]
    [TestCase(CodeGenVisibility.Internal, true)]
    [TestCase(CodeGenVisibility.Public, true)]
    [TestCase(CodeGenVisibility.ProtectedInternal, true)]
    public void ExplicitVisibilityOverridesTakePrecedence(CodeGenVisibility visibility, bool nested)
    {
        var model = nested ? CreateNestedHierarchy().Message : CreateModel();
        var discriminatorProperty = model.Properties.Single(property => property.IsDiscriminator);
        model.Update(attributes:
        [
            VisibilityAttribute(model.Name, visibility, new TypeOfExpression(discriminatorProperty.Type)),
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

    [Test]
    public void ReplacesDiscriminatorArgumentInDelegatingConstructor()
    {
        var rootInput = InputFactory.Model("Root", "OpenAI.Responses",
            properties: [InputFactory.Property("type", InputFactory.StringEnum("InheritedKind", [], isExtensible: true),
                isRequired: true, isDiscriminator: true)]);
        var input = InputFactory.Model("Intermediate", "OpenAI.Responses",
            properties: [InputFactory.Property("items", InputFactory.Array(InputPrimitiveType.String))],
            baseModel: rootInput);
        var model = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;
        var root = model.BaseModelProvider!;

        new TestConstructorFixupVisitor().Apply(model);
        new TestExtensibleModelVisitor().Apply(root);
        new TestExtensibleModelVisitor().Apply(model);

        var source = new TypeProviderWriter(model).Write().Content;
        Assert.That(source, Does.Contain("protected internal Intermediate(global::Sample.Models.InheritedKind @type) : this(@type, null, null)"));
    }

    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public void ExposesNestedDiscriminatorWhilePreservingFixedOuterKind(bool fixupConstructors, bool suppressProperty)
    {
        var (message, leaf, _) = CreateNestedHierarchy();
        var root = message.BaseModelProvider!;
        var role = message.Properties.Single(property => property.IsDiscriminator);
        var fullConstructor = message.FullConstructor;
        var originalFullParameters = fullConstructor.Signature.Parameters.Select(parameter => (parameter.Name, parameter.Type)).ToArray();
        var originalFullModifiers = fullConstructor.Signature.Modifiers;
        var originalFullBody = fullConstructor.BodyStatements;
        var originalFullInitializer = fullConstructor.Signature.Initializer;
        var originalLeafSource = new TypeProviderWriter(leaf).Write().Content;
        _ = message.Constructors;
        if (suppressProperty)
        {
            message.Update(properties: []);
        }
        if (fixupConstructors)
        {
            new TestConstructorFixupVisitor().Apply(message);
        }

        var visitor = new TestExtensibleModelVisitor();
        visitor.Apply(root);
        visitor.Apply(message);
        visitor.Apply(leaf);

        var source = new TypeProviderWriter(message).Write().Content;
        Assert.That(message.DiscriminatorValue, Is.EqualTo("message"));
        Assert.That(role.Modifiers, Is.EqualTo(MethodSignatureModifiers.Public));
        Assert.That(((AutoPropertyBody)role.Body).HasSetter, Is.False);
        Assert.That(role.Type.IsPublic, Is.True);
        if (!suppressProperty)
        {
            Assert.That(source, Does.Contain("public global::OpenAI.Responses.MessageRole Role { get; }"), source);
        }
        Assert.That(source, Does.Contain("protected internal Message(global::OpenAI.Responses.MessageRole role)"), source);
        Assert.That(source, Does.Contain(": base(global::OpenAI.Responses.ItemKind.Message)"), source);
        Assert.That(source, Does.Contain("Role = role;"), source);
        Assert.That(fullConstructor.Signature.Parameters.Select(parameter => (parameter.Name, parameter.Type)), Is.EqualTo(originalFullParameters));
        Assert.That(fullConstructor.Signature.Modifiers, Is.EqualTo(originalFullModifiers));
        Assert.That(fullConstructor.BodyStatements, Is.SameAs(originalFullBody));
        Assert.That(fullConstructor.Signature.Initializer, Is.SameAs(originalFullInitializer));
        Assert.That(new TypeProviderWriter(leaf).Write().Content, Is.EqualTo(originalLeafSource));
        visitor.Apply(message);
        Assert.That(new TypeProviderWriter(message).Write().Content, Is.EqualTo(source));
    }

    [Test]
    public void OpenIntermediateInheritsNestedDiscriminator()
    {
        var (message, _, messageInput) = CreateNestedHierarchy();
        var input = InputFactory.Model("OpenMessage", "OpenAI.Responses", properties: [], baseModel: messageInput);
        var intermediate = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(input)!;
        var visitor = new TestExtensibleModelVisitor();
        visitor.Apply(message.BaseModelProvider!);
        visitor.Apply(message);
        visitor.Apply(intermediate);

        var source = new TypeProviderWriter(intermediate).Write().Content;
        Assert.That(source, Does.Contain("protected internal OpenMessage(global::OpenAI.Responses.MessageRole role)"), source);
        Assert.That(source, Does.Contain(": base(role)"), source);
    }

    [Test]
    public void ClosedNestedDiscriminatorRemainsUnchanged()
    {
        var (message, _, _) = CreateNestedHierarchy(isExtensible: false);
        var source = new TypeProviderWriter(message).Write().Content;
        new TestExtensibleModelVisitor().Apply(message);
        Assert.That(new TypeProviderWriter(message).Write().Content, Is.EqualTo(source));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void AddsMissingNestedRoleWithoutReplacingOuterKind(bool delegatesToFullConstructor)
    {
        var (message, _, _) = CreateNestedHierarchy();
        var constructor = message.Constructors.Single(candidate => candidate.Signature.Parameters.Count == 1);
        constructor.Signature.Update(parameters: []);
        constructor.Update(bodyStatements: new MethodBodyStatements([]));
        if (delegatesToFullConstructor)
        {
            constructor.Signature.Update(initializer: new ConstructorInitializer(false,
                [message.DiscriminatorValueExpression!, Snippet.Null, Snippet.Default]));
        }

        var visitor = new TestExtensibleModelVisitor();
        visitor.Apply(message.BaseModelProvider!);
        visitor.Apply(message);

        var source = new TypeProviderWriter(message).Write().Content;
        Assert.That(source, Does.Match(@"protected internal Message\(global::OpenAI\.Responses\.MessageRole role\d*\)"), source);
        if (delegatesToFullConstructor)
        {
            Assert.That(source, Does.Match(@": this\(global::OpenAI\.Responses\.ItemKind\.Message, null, role\d*\)"), source);
        }
        else
        {
            Assert.That(source, Does.Contain(": base(global::OpenAI.Responses.ItemKind.Message)"), source);
            Assert.That(source, Does.Match(@"Role = role\d*;"), source);
        }
        visitor.Apply(message);
        Assert.That(new TypeProviderWriter(message).Write().Content, Is.EqualTo(source));
    }

    [Test]
    public void ConsumerCanDeriveFromNestedBaseInAnotherAssembly()
    {
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }", inputNamespace: "OpenAI");
        var (message, leaf, messageInput) = CreateNestedHierarchy();
        var intermediate = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(
            InputFactory.Model("OpenMessage", "OpenAI.Responses", properties: [], baseModel: messageInput))!;
        var root = message.BaseModelProvider!;
        var visitor = new TestExtensibleModelVisitor();
        visitor.Apply(root);
        visitor.Apply(message);
        visitor.Apply(leaf);
        visitor.Apply(intermediate);
        var kind = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[root.Properties.Single(property => property.IsDiscriminator).Type]!;
        var role = ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[message.Properties.Single(property => property.IsDiscriminator).Type]!;
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Append(typeof(BinaryData).Assembly.Location)
            .Distinct()
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToArray();
        var compilation = CSharpCompilation.Create("NestedModels",
            new[] { root, message, intermediate, leaf, kind, role }
                .Select(type => CSharpSyntaxTree.ParseText(new TypeProviderWriter(type).Write().Content))
                .Append(CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestData", "Argument.cs")))),
            references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var assembly = new MemoryStream();
        var result = compilation.Emit(assembly);
        Assert.That(result.Success, Is.True, string.Join(Environment.NewLine, result.Diagnostics));

        var consumer = CSharpCompilation.Create("NestedModelsConsumer",
            [CSharpSyntaxTree.ParseText("""
                public sealed class CustomMessage : OpenAI.Responses.Message
                {
                    public CustomMessage() : base(new OpenAI.Responses.MessageRole("custom")) { }
                    public OpenAI.Responses.MessageRole ReadRole() => Role;
                    public OpenAI.Responses.ItemKind ReadKind() => Kind;
                }
                """)],
            [.. references, MetadataReference.CreateFromImage(assembly.ToArray())],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var consumerAssembly = new MemoryStream();
        var consumerResult = consumer.Emit(consumerAssembly);
        Assert.That(consumerResult.Success, Is.True, string.Join(Environment.NewLine, consumerResult.Diagnostics));
    }

    private static (ModelProvider Message, ModelProvider Leaf, InputModelType MessageInput) CreateNestedHierarchy(bool isExtensible = true)
    {
        var itemSubtypes = new Dictionary<string, InputModelType>();
        var messageSubtypes = new Dictionary<string, InputModelType>();
        var rootInput = InputFactory.Model("Item", "OpenAI.Responses",
            properties: [InputFactory.Property("kind",
                InputFactory.StringEnum("ItemKind", [("Message", "message")], isExtensible: true,
                    clientNamespace: "OpenAI.Responses"), isRequired: true, isDiscriminator: true)],
            discriminatedModels: itemSubtypes);
        var messageInput = InputFactory.Model("Message", "OpenAI.Responses",
            properties: [InputFactory.Property("role",
                InputFactory.StringEnum("MessageRole", [("User", "user")], access: "internal", isExtensible: isExtensible,
                    clientNamespace: "OpenAI.Responses"), isRequired: true, isDiscriminator: true)],
            baseModel: rootInput, discriminatedKind: "message", discriminatedModels: messageSubtypes);
        var leafInput = InputFactory.Model("UserMessage", "OpenAI.Responses",
            properties: [InputFactory.Property("content", InputPrimitiveType.String, isRequired: true)],
            baseModel: messageInput, discriminatedKind: "user");
        itemSubtypes.Add("message", messageInput);
        messageSubtypes.Add("user", leafInput);
        var leaf = ScmCodeModelGenerator.Instance.TypeFactory.CreateModel(leafInput)!;
        var message = leaf.BaseModelProvider!;
        return (message, leaf, messageInput);
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

    private sealed class TestConstructorFixupVisitor : ConstructorFixupVisitor
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
