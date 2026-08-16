using System.ClientModel.Primitives;
using System.Linq;
using System.Threading;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.Common;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors;

[Category("Visitor")]
public class PaginationVisitorTests
{
    [SetUp]
    public void Setup()
    {
        MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"TestLibrary\" }");
    }

    [Test]
    public void VisitMethod_ReplacesPascalCasePaginationParametersWithOptions()
    {
        var optionsModel = InputFactory.Model("RunStepCollectionOptions", "Samples");
        MockHelpers.LoadMockGenerator(
            inputModels: () => [optionsModel],
            configurationJson: "{ \"package-name\": \"TestLibrary\" }");

        var type = new TestTypeProvider();
        var method = new MethodProvider(
            new MethodSignature("GetRunSteps", $"", MethodSignatureModifiers.Public,
                typeof(System.ClientModel.CollectionResult<RunStep>), $"",
                [
                    new ParameterProvider("threadId", $"", typeof(string)),
                    new ParameterProvider("runId", $"", typeof(string)),
                    new ParameterProvider("AfterId", $"", typeof(string)),
                    new ParameterProvider("BeforeId", $"", typeof(string)),
                    new ParameterProvider("PageSizeLimit", $"", typeof(int?)),
                    new ParameterProvider("Order", $"", typeof(string)),
                    new ParameterProvider("include", $"", typeof(string)),
                    new ParameterProvider("cancellationToken", $"", typeof(CancellationToken)),
                ]), MethodBodyStatement.Empty, type);

        new TestPaginationVisitor().Apply(method);

        Assert.That(method.Signature.Parameters.Select(p => p.Name),
            Is.EqualTo(new[] { "threadId", "runId", "options", "include", "cancellationToken" }));
        Assert.That(method.Signature.Parameters[2].Type.Name,
            Is.EqualTo("RunStepCollectionOptions"));
    }

    private sealed class TestPaginationVisitor : PaginationVisitor
    {
        public MethodProvider? Apply(MethodProvider method) => base.VisitMethod(method);
    }

    private sealed class TestTypeProvider : TypeProvider
    {
        protected override string BuildName() => nameof(TestTypeProvider);
        protected override string BuildRelativeFilePath() => $"{Name}.cs";
    }

    private sealed class RunStep;
}
