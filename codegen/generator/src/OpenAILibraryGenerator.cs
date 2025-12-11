using System.ComponentModel.Composition;
using Microsoft.TypeSpec.Generator;
using Microsoft.TypeSpec.Generator.ClientModel;
using OpenAILibraryPlugin.Visitors;

namespace OpenAILibraryPlugin
{
    [Export(typeof(CodeModelGenerator))]
    [ExportMetadata("GeneratorName", nameof(OpenAILibraryGenerator))]
    public class OpenAILibraryGenerator : ScmCodeModelGenerator
    {
        [ImportingConstructor]
        public OpenAILibraryGenerator(GeneratorContext context) : base(context) { }

        protected override void Configure()
        {
            base.Configure();
            // This should be first, as it recomputes the type
            AddVisitor(new NonAbstractPublicTypesVisitor());
            AddVisitor(new ConstructorFixupVisitor());
            AddVisitor(new KindRenameVisitor());
            AddVisitor(new VisibilityVisitor());
            AddVisitor(new ContentInnerCollectionDefinedVisitor());
            AddVisitor(new PageOrderRemovalVisitor(this));
            AddVisitor(new OmittedTypesVisitor());
            AddVisitor(new InvariantFormatAdditionalPropertiesVisitor());
            AddVisitor(new OpenAILibraryVisitor());
            AddVisitor(new VirtualMessageCreationVisitor());
            AddVisitor(new ProhibitedNamespaceVisitor());
            AddVisitor(new ModelSerializationVisitor());
            AddVisitor(new ExperimentalAttributeVisitor());
            AddVisitor(new ModelDirectoryVisitor());
            AddVisitor(new PaginationVisitor());
            AddVisitor(new MetadataQueryParamVisitor());
            AddVisitor(new ProtocolModelVisitor());
        }
    }
}