using Microsoft.TypeSpec.Generator.Providers;
using NUnit.Framework;
using OpenAILibraryPlugin.Tests.TestHelpers;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin.Tests.Visitors
{
    [Category("Visitor")]
    public class OmittedTypesVisitorTests
    {
        [SetUp]
        public void Setup()
        {
            MockHelpers.LoadMockGenerator(configurationJson: "{ \"package-name\": \"OpenAI\" }");
        }

        [Test]
        public void VisitType_OmitsOpenAIModelFactory()
        {
            var type = new TestTypeProvider("OpenAIModelFactory");

            Assert.That(new TestOmittedTypesVisitor().Apply(type), Is.Null);
        }

        [Test]
        public void VisitType_PreservesOtherTypes()
        {
            var type = new TestTypeProvider("OtherType");

            Assert.That(new TestOmittedTypesVisitor().Apply(type), Is.SameAs(type));
        }

        private sealed class TestOmittedTypesVisitor : OmittedTypesVisitor
        {
            public TypeProvider? Apply(TypeProvider type) => base.VisitType(type);
        }

        private sealed class TestTypeProvider : TypeProvider
        {
            private readonly string _name;

            public TestTypeProvider(string name)
            {
                _name = name;
            }

            protected override string BuildNamespace() => "OpenAI";

            protected override string BuildRelativeFilePath() => $"{_name}.cs";

            protected override string BuildName() => _name;
        }
    }
}
