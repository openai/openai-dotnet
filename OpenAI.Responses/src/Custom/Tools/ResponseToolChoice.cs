using System;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetResponseToolChoice")]
[CodeGenVisibility(nameof(ResponseToolChoice), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class ResponseToolChoice
{
    /// <summary>Creates a tool choice from a string-backed selection mode.</summary>
    /// <param name="defaultToolChoice">The selection mode, such as <see cref="ResponseToolChoiceEnum.Auto"/>.</param>
    /// <exception cref="ArgumentException"><paramref name="defaultToolChoice"/> is uninitialized.</exception>
    public ResponseToolChoice(ResponseToolChoiceEnum defaultToolChoice)
    {
        if (defaultToolChoice == default)
        {
            throw new ArgumentException("The tool choice must be initialized.", nameof(defaultToolChoice));
        }

        DefaultToolChoice = defaultToolChoice;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        // CUSTOM: Initialize patch forwarding for publicly constructed union wrappers.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    public ResponseToolChoice(ResponseToolChoiceObject customToolChoice)
    {
        Argument.AssertNotNull(customToolChoice, nameof(customToolChoice));

        CustomToolChoice = customToolChoice;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        // CUSTOM: Initialize patch forwarding for publicly constructed union wrappers.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    [CodeGenMember("DefaultToolChoice")]
    public ResponseToolChoiceEnum? DefaultToolChoice { get; }

    [CodeGenMember("CustomToolChoice")]
    public ResponseToolChoiceObject CustomToolChoice { get; }

    public static implicit operator ResponseToolChoice(ResponseToolChoiceEnum defaultToolChoice) => new(defaultToolChoice);

    public static implicit operator ResponseToolChoice(ResponseToolChoiceObject customToolChoice) => customToolChoice is null ? null : new(customToolChoice);
}
