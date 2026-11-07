using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallOutputItemOutput")]
public partial class ComputerCallOutput
{
    public static ComputerCallComputerScreenshotOutput CreateComputerScreenshotOutput(Uri screenshotImageUri)
    {
        Argument.AssertNotNull(screenshotImageUri, nameof(screenshotImageUri));

        return new ComputerCallComputerScreenshotOutput()
        {
            ImageUri = screenshotImageUri.AbsoluteUri,
        };
    }

    public static ComputerCallComputerScreenshotOutput CreateComputerScreenshotOutput(string screenshotImageFileId)
    {
        Argument.AssertNotNull(screenshotImageFileId, nameof(screenshotImageFileId));

        return new ComputerCallComputerScreenshotOutput()
        {
            FileId = screenshotImageFileId,
        };
    }

    public static ComputerCallComputerScreenshotOutput CreateComputerScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        Argument.AssertNotNull(screenshotImageBytes, nameof(screenshotImageBytes));
        Argument.AssertNotNull(screenshotImageBytesMediaType, nameof(screenshotImageBytesMediaType));

        return new ComputerCallComputerScreenshotOutput()
        {
            ImageUri = DataEncodingHelpers.CreateDataUri(screenshotImageBytes, screenshotImageBytesMediaType),
        };
    }
}
