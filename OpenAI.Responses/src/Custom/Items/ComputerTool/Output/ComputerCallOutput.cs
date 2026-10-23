using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallOutputItemOutput")]
public partial class ComputerCallOutput
{
    public static ComputerCallScreenshotOutput CreateScreenshotOutput(Uri screenshotImageUri)
    {
        Argument.AssertNotNull(screenshotImageUri, nameof(screenshotImageUri));

        return new ComputerCallScreenshotOutput()
        {
            ImageUri = screenshotImageUri.AbsoluteUri,
        };
    }

    public static ComputerCallScreenshotOutput CreateScreenshotOutput(string screenshotImageFileId)
    {
        Argument.AssertNotNull(screenshotImageFileId, nameof(screenshotImageFileId));

        return new ComputerCallScreenshotOutput()
        {
            FileId = screenshotImageFileId,
        };
    }

    public static ComputerCallScreenshotOutput CreateScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        Argument.AssertNotNull(screenshotImageBytes, nameof(screenshotImageBytes));
        Argument.AssertNotNull(screenshotImageBytesMediaType, nameof(screenshotImageBytesMediaType));

        return new ComputerCallScreenshotOutput()
        {
            ImageUri = DataEncodingHelpers.CreateDataUri(screenshotImageBytes, screenshotImageBytesMediaType),
        };
    }
}
