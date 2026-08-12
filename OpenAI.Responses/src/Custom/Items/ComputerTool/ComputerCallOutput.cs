using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallOutputItemOutput")]
public partial class ComputerCallOutput
{
    public static ComputerCallOutput CreateScreenshotOutput(Uri screenshotImageUri)
    {
        Argument.AssertNotNull(screenshotImageUri, nameof(screenshotImageUri));

        return new InternalComputerToolCallOutputItemOutputComputerScreenshot()
        {
            ImageUrl = screenshotImageUri.AbsoluteUri,
        };
    }

    public static ComputerCallOutput CreateScreenshotOutput(string screenshotImageFileId)
    {
        Argument.AssertNotNull(screenshotImageFileId, nameof(screenshotImageFileId));

        return new InternalComputerToolCallOutputItemOutputComputerScreenshot()
        {
            FileId = screenshotImageFileId,
        };
    }

    /// <summary> Creates a computer screenshot output from binary image data. </summary>
    /// <param name="screenshotImageBytes">
    /// The screenshot image bytes.
    ///
    /// No copy of the memory is made. If a mutable buffer is used,
    /// it must remain unaltered until the operation is complete. The
    /// caller retains ownership of the memory backing this value.
    /// </param>
    /// <param name="screenshotImageBytesMediaType"> The MIME type of the screenshot image. </param>
    public static ComputerCallOutput CreateScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        Argument.AssertNotNull(screenshotImageBytes, nameof(screenshotImageBytes));
        Argument.AssertNotNull(screenshotImageBytesMediaType, nameof(screenshotImageBytesMediaType));

        InternalComputerToolCallOutputItemOutputComputerScreenshot output = new();
        output.SetImageBytes(screenshotImageBytes, screenshotImageBytesMediaType);
        return output;
    }
}
