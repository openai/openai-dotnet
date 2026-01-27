using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Conversations
{
    // CUSTOM:
    // - Renamed.
    // - Implemented in custom code due to a bug in the code generator that skips generation for this type when only
    //   protocol methods are generated.
    [CodeGenType("IncludeEnum")]
    public readonly partial struct IncludedConversationItemProperty : IEquatable<IncludedConversationItemProperty>
    {
        private const string MessageInputImageImageUrlValue = "message.input_image.image_url";
        private const string ComputerCallOutputOutputImageUrlValue = "computer_call_output.output.image_url";
        
        // CUSTOM: Renamed.
        [CodeGenMember("MessageInputImageImageUrl")]
        public static IncludedConversationItemProperty MessageInputImageUri { get; } = new IncludedConversationItemProperty(MessageInputImageImageUrlValue);

        // CUSTOM: Renamed.
        [CodeGenMember("ComputerCallOutputOutputImageUrl")]
        public static IncludedConversationItemProperty ComputerCallOutputImageUri { get; } = new IncludedConversationItemProperty(ComputerCallOutputOutputImageUrlValue);
    }
}
