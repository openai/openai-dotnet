using System;
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
        private static OpenAILibraryGenerator? s_instance;
        internal static OpenAILibraryGenerator Instance => s_instance ?? throw new InvalidOperationException("OpenAILibraryGenerator was not initialized.");

        [ImportingConstructor]
        public OpenAILibraryGenerator(GeneratorContext context) : base(context)
        {
            s_instance = this;
        }

        protected override void Configure()
        {
            base.Configure();
            AddVisitor(new ConstructorFixupVisitor());
            AddVisitor(new KindRenameVisitor());
            AddVisitor(new VisibilityVisitor());
            AddVisitor(new ContentInnerCollectionDefinedVisitor());
            AddVisitor(new NonAbstractPublicTypesVisitor());
            AddVisitor(new PageOrderRemovalVisitor(this));
            AddVisitor(new OmittedTypesVisitor());
            AddVisitor(new InvariantFormatAdditionalPropertiesVisitor());
            AddVisitor(new OpenAILibraryVisitor());
            AddVisitor(new VirtualMessageCreationVisitor());
            AddVisitor(new ProhibitedNamespaceVisitor());
            AddVisitor(new ExplicitConversionFromClientResultVisitor());
            AddVisitor(new ImplicitConversionToBinaryContentVisitor());
            AddVisitor(new ModelSerializationVisitor());
            AddVisitor(new ExperimentalAttributeVisitor());
            AddVisitor(new ModelDirectoryVisitor());
        }
    }
}