using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallOutputItemOutput")]
public partial class ComputerOutput
{
    public static ComputerOutput CreateScreenshotOutput(Uri screenshotImageUri)
    {
        Argument.AssertNotNull(screenshotImageUri, nameof(screenshotImageUri));

        return new InternalComputerToolCallOutputItemOutputComputerScreenshot()
        {
            ImageUrl = screenshotImageUri.AbsoluteUri,
        };
    }

    public static ComputerOutput CreateScreenshotOutput(string screenshotImageFileId)
    {
        Argument.AssertNotNull(screenshotImageFileId, nameof(screenshotImageFileId));

        return new InternalComputerToolCallOutputItemOutputComputerScreenshot()
        {
            FileId = screenshotImageFileId,
        };
    }

    public static ComputerOutput CreateScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        Argument.AssertNotNull(screenshotImageBytes, nameof(screenshotImageBytes));
        Argument.AssertNotNull(screenshotImageBytesMediaType, nameof(screenshotImageBytesMediaType));

        string base64EncodedData = Convert.ToBase64String(screenshotImageBytes.ToArray());
        string dataUri = $"data:{screenshotImageBytesMediaType};base64,{base64EncodedData}";

        return new InternalComputerToolCallOutputItemOutputComputerScreenshot()
        {
            ImageUrl = dataUri,
        };
    }
}
