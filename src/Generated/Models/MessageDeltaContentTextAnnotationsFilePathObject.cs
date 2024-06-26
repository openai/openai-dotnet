// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    internal partial class MessageDeltaContentTextAnnotationsFilePathObject : MessageDeltaTextContentAnnotation
    {
        internal MessageDeltaContentTextAnnotationsFilePathObject(int index)
        {
            Type = "file_path";
            Index = index;
        }

        internal MessageDeltaContentTextAnnotationsFilePathObject(string type, IDictionary<string, BinaryData> serializedAdditionalRawData, int index, string text, MessageDeltaContentTextAnnotationsFilePathObjectFilePath filePath, int? startIndex, int? endIndex) : base(type, serializedAdditionalRawData)
        {
            Index = index;
            Text = text;
            FilePath = filePath;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        internal MessageDeltaContentTextAnnotationsFilePathObject()
        {
        }

        public int Index { get; }
        public string Text { get; }
        public MessageDeltaContentTextAnnotationsFilePathObjectFilePath FilePath { get; }
        public int? StartIndex { get; }
        public int? EndIndex { get; }
    }
}
