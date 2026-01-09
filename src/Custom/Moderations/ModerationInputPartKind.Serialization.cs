using System;

namespace OpenAI.Moderations
{
    internal static partial class ModerationInputPartKindExtensions
    {
        public static string ToSerialString(this ModerationInputPartKind value) => value switch
        {
            ModerationInputPartKind.Image => "image_url",
            ModerationInputPartKind.Text => "text",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ModerationInputPartKind value.")
        };

        public static ModerationInputPartKind ToModerationInputPartKind(this string value)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "image_url"))
            {
                return ModerationInputPartKind.Image;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(value, "text"))
            {
                return ModerationInputPartKind.Text;
            }
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ModerationInputPartKind value.");
        }
    }
}
