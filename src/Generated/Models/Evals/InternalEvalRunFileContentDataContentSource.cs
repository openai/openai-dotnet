// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using OpenAI;

namespace OpenAI.Evals
{
    internal partial class InternalEvalRunFileContentDataContentSource : InternalEvalRunDataContentSource
    {
        internal InternalEvalRunFileContentDataContentSource(IEnumerable<InternalEvalRunFileContentDataContentSourceContent> content) : base(InternalEvalRunDataContentSourceType.FileContent)
        {
            Argument.AssertNotNull(content, nameof(content));

            Content = content.ToList();
        }

        internal InternalEvalRunFileContentDataContentSource(InternalEvalRunDataContentSourceType kind, IDictionary<string, BinaryData> additionalBinaryDataProperties, IList<InternalEvalRunFileContentDataContentSourceContent> content) : base(kind, additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Content = content ?? new ChangeTrackingList<InternalEvalRunFileContentDataContentSourceContent>();
        }

        internal IList<InternalEvalRunFileContentDataContentSourceContent> Content { get; }
    }
}
