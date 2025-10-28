using System;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

public partial class ResponseFormatJsonSchemaJsonSchema
{
    [Experimental("SCME0001")]
    private JsonPatch _patch;

    public ResponseFormatJsonSchemaJsonSchema(string name)
    {
        Argument.AssertNotNull(name, nameof(name));

        Name = name;
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    public ResponseFormatJsonSchemaJsonSchema(string description, string name, BinaryData schema, bool? strict, in JsonPatch patch)
    {
        Description = description;
        Name = name;
        Schema = schema;
        Strict = strict;
        _patch = patch;
    }

#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

    public string Description { get; set; }

    public string Name { get; set; }

    public bool? Strict { get; set; }

    public BinaryData Schema { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Experimental("SCME0001")]
    public ref JsonPatch Patch => ref _patch;
}