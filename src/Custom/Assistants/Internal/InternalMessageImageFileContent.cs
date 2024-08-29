using System;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of image file content within an Assistants API message.
/// </summary>
/// <remarks>
/// Use the <see cref="MessageContent.FromImageFileId(string,MessageImageDetail?)"/> method to
/// create an instance of this type.
/// </remarks>
[CodeGenModel("MessageContentImageFileObject")]
[CodeGenSuppress("InternalMessageImageFileContent", typeof(InternalMessageContentItemFileObjectImageFile))]
internal partial class InternalMessageImageFileContent
{
    [CodeGenMember("Type")]
    private string _type = "image_file";

    [CodeGenMember("ImageFile")]
    internal InternalMessageContentItemFileObjectImageFile _imageFile;

    /// <inheritdoc cref="InternalMessageContentItemFileObjectImageFile.FileId"/>
    public string InternalFileId => _imageFile.FileId;

    /// <inheritdoc cref="InternalMessageContentItemFileObjectImageFile.Detail"/>
    public MessageImageDetail? InternalDetail => _imageFile.Detail?.ToMessageImageDetail();

    /// <summary> Initializes a new instance of <see cref="InternalMessageImageFileContent"/>. </summary>
    internal InternalMessageImageFileContent(string imageFileId, MessageImageDetail? detail = null)
        : this(new InternalMessageContentItemFileObjectImageFile(imageFileId, detail?.ToSerialString(), null))
    { }

    /// <summary> Initializes a new instance of <see cref="InternalMessageImageFileContent"/>. </summary>
    /// <param name="imageFile"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFile"/> is null. </exception>
    internal InternalMessageImageFileContent(InternalMessageContentItemFileObjectImageFile imageFile)
    {
        Argument.AssertNotNull(imageFile, nameof(imageFile));
        _imageFile = imageFile;
    }
}
