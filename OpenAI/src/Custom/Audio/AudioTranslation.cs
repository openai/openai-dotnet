using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

[CodeGenType("CreateTranslationResponseVerboseJson")]
public partial class AudioTranslation
{
    // CUSTOM: Made nullable because this is an optional property.
    /// <summary> The duration of the input audio. </summary>
    public TimeSpan? Duration { get; }
}