using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Chat;

[Category("Chat")]
public class ChatCompletionOptionTests
{
    [Test]
    public void Validate_Clone()
    {
        var original = new ChatCompletionOptions();
        CloneTestHelper.ValidateCloneMethod(original);
    }
}
