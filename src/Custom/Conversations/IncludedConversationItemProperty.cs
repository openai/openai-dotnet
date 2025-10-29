using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.Conversations
{
    // CUSTOM:
    // - Renamed.
    // - Implemented in custom code due to a bug in the code generator that skips generation for this type when only
    //   protocol methods are generated.
    [Experimental("OPENAI001")]
    [CodeGenType("IncludeEnum")]
    public readonly partial struct IncludedConversationItemProperty : IEquatable<IncludedConversationItemProperty>
    {
        private readonly string _value;
        private const string FileSearchCallResultsValue = "file_search_call.results";
        private const string WebSearchCallResultsValue = "web_search_call.results";
        private const string WebSearchCallActionSourcesValue = "web_search_call.action.sources";
        private const string MessageInputImageImageUrlValue = "message.input_image.image_url";
        private const string ComputerCallOutputOutputImageUrlValue = "computer_call_output.output.image_url";
        private const string CodeInterpreterCallOutputsValue = "code_interpreter_call.outputs";
        private const string ReasoningEncryptedContentValue = "reasoning.encrypted_content";
        private const string MessageOutputTextLogprobsValue = "message.output_text.logprobs";

        public IncludedConversationItemProperty(string value)
        {
            Argument.AssertNotNull(value, nameof(value));

            _value = value;
        }

        public static IncludedConversationItemProperty FileSearchCallResults { get; } = new IncludedConversationItemProperty(FileSearchCallResultsValue);

        public static IncludedConversationItemProperty WebSearchCallResults { get; } = new IncludedConversationItemProperty(WebSearchCallResultsValue);

        public static IncludedConversationItemProperty WebSearchCallActionSources { get; } = new IncludedConversationItemProperty(WebSearchCallActionSourcesValue);

        // CUSTOM: Renamed.
        [CodeGenMember("MessageInputImageImageUrl")]
        public static IncludedConversationItemProperty MessageInputImageUri { get; } = new IncludedConversationItemProperty(MessageInputImageImageUrlValue);

        // CUSTOM: Renamed.
        [CodeGenMember("ComputerCallOutputOutputImageUrl")]
        public static IncludedConversationItemProperty ComputerCallOutputImageUri { get; } = new IncludedConversationItemProperty(ComputerCallOutputOutputImageUrlValue);

        public static IncludedConversationItemProperty CodeInterpreterCallOutputs { get; } = new IncludedConversationItemProperty(CodeInterpreterCallOutputsValue);

        public static IncludedConversationItemProperty ReasoningEncryptedContent { get; } = new IncludedConversationItemProperty(ReasoningEncryptedContentValue);

        public static IncludedConversationItemProperty MessageOutputTextLogprobs { get; } = new IncludedConversationItemProperty(MessageOutputTextLogprobsValue);

        public static bool operator ==(IncludedConversationItemProperty left, IncludedConversationItemProperty right) => left.Equals(right);

        public static bool operator !=(IncludedConversationItemProperty left, IncludedConversationItemProperty right) => !left.Equals(right);

        public static implicit operator IncludedConversationItemProperty(string value) => new IncludedConversationItemProperty(value);

        public static implicit operator IncludedConversationItemProperty?(string value) => value == null ? null : new IncludedConversationItemProperty(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is IncludedConversationItemProperty other && Equals(other);

        public bool Equals(IncludedConversationItemProperty other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

        public override string ToString() => _value;
    }
}
