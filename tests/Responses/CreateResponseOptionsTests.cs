using NUnit.Framework;
using OpenAI.Responses;

namespace OpenAI.Tests.Responses;

[Category("Responses")]
public class CreateResponseOptionsTests
{
    [Test]
    public void Validate_Clone()
    {
        var original = new CreateResponseOptions();
        CloneTestHelper.ValidateCloneMethod(original);
    }
}
