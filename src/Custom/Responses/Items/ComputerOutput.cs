using System;

namespace OpenAI.Responses;

[CodeGenType("ResponsesComputerCallOutputItemOutput")]
public partial class ComputerOutput
{
    public static ComputerOutput CreateScreenshotOutput(Uri screenshotImageUri)
    {
        Argument.AssertNotNull(screenshotImageUri, nameof(screenshotImageUri));

        return new InternalResponsesComputerCallOutputItemScreenshot()
        {
            ImageUrl = screenshotImageUri.AbsoluteUri,
        };
    }

    public static ComputerOutput CreateScreenshotOutput(string screenshotImageFileId)
    {
        Argument.AssertNotNull(screenshotImageFileId, nameof(screenshotImageFileId));

        return new InternalResponsesComputerCallOutputItemScreenshot()
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

        return new InternalResponsesComputerCallOutputItemScreenshot()
        {
            ImageUrl = dataUri,
        };
    }
}
