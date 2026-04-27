using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Specifies the types of output content the model should generate for a chat completion request.
/// </summary>
/// <remarks>
/// Most models can generate text, which is the default <see cref="ChatResponseModalities.Text"/>. Some models like
/// <c>gpt-4o-audio-preview</c> can also generate audio, which can be requested by combining
/// <c>
/// <see cref="ChatResponseModalities.Text"/> | <see cref="ChatResponseModalities.Audio"/>.
/// </c>
/// </remarks>
[Experimental("OPENAI001")]
[Flags]
public enum ChatResponseModalities : int
{
    /// <summary>
    /// The value which specifies that the model should produce its default set of output content modalities.
    /// </summary>
    Default = 0,
    /// <summary>
    /// The flag that, if included, specifies that the model should generate text content in its response.
    /// </summary>
    Text = 1 << 0,
    /// <summary>
    /// The flag that, if included, specifies that the model should generate audio content in its response.
    /// </summary>
    Audio = 1 << 1,
}