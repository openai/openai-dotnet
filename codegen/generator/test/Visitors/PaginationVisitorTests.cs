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

    [TestCase("AssistantClient", "GetAssistants")]
    [TestCase("BatchClient", "GetBatches")]
    [TestCase("ChatClient", "GetChatCompletions")]
    [TestCase("ChatClient", "GetChatCompletionMessages")]
    [TestCase("FineTuningClient", "GetFineTuningCheckpointPermissions")]
    [TestCase("VectorStoreClient", "GetVectorStores")]
    [TestCase("VectorStoreClient", "GetVectorStoreFiles")]
    [TestCase("VectorStoreClient", "GetVectorStoreFilesInBatch")]
    [TestCase("VideoClient", "GetVideos")]
    public void VisitMethod_RestoresLegacyNamesOnPublicProtocolOverloads(string clientName, string methodName)
    {
        foreach (string suffix in new[] { "", "Async" })
        {
            var type = new TestTypeProvider(clientName);
            var method = new MethodProvider(
                new MethodSignature(methodName + suffix, $"", MethodSignatureModifiers.Public,
                    typeof(CollectionResult), $"",
                    [
                        new ParameterProvider("pageSizeLimit", $"", typeof(int?)),
                        new ParameterProvider("order", $"", typeof(string)),
                        new ParameterProvider("afterId", $"", typeof(string)),
                        new ParameterProvider("beforeId", $"", typeof(string)),
                        new ParameterProvider("options", $"", typeof(RequestOptions)),
                    ]), MethodBodyStatement.Empty, type);

            new TestPaginationVisitor().Apply(method);

            Assert.That(method.Signature.Parameters.Select(parameter => parameter.Name),
                Is.EqualTo(new[] { "limit", "order", "after", "before", "options" }));
        }
    }

    [Test]
    public void VisitMethod_DoesNotRenameParametersOnOtherClients()
    {
        var type = new TestTypeProvider("InternalAssistantClient");
        var method = new MethodProvider(
            new MethodSignature("GetAssistants", $"", MethodSignatureModifiers.Public,
                typeof(CollectionResult), $"",
                [
                    new ParameterProvider("pageSizeLimit", $"", typeof(int?)),
                    new ParameterProvider("afterId", $"", typeof(string)),
                    new ParameterProvider("beforeId", $"", typeof(string)),
                    new ParameterProvider("options", $"", typeof(RequestOptions)),
                ]), MethodBodyStatement.Empty, type);

        new TestPaginationVisitor().Apply(method);

        Assert.That(method.Signature.Parameters.Select(parameter => parameter.Name),
            Is.EqualTo(new[] { "pageSizeLimit", "afterId", "beforeId", "options" }));
    }

    private sealed class TestPaginationVisitor : PaginationVisitor
    {
        public MethodProvider? Apply(MethodProvider method) => base.VisitMethod(method);
    }

    private sealed class TestTypeProvider : TypeProvider
    {
        private readonly string _name;

        public TestTypeProvider(string name = nameof(TestTypeProvider))
        {
            _name = name;
        }

        protected override string BuildName() => _name;
        protected override string BuildRelativeFilePath() => $"{Name}.cs";
    }

    private sealed class RunStep;
}
