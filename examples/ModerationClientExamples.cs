using NUnit.Framework;
using OpenAI.Moderations;
using System;
using System.Collections.Generic;

namespace OpenAI.Examples;

public partial class ModerationClientExamples
{
    [Test]
    public void ClassifyMultimodalExample()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        ModerationClient client = new("omni-moderation-latest", apiKey);

        var inputs = new List<ModerationInput>
        {
            ModerationInput.FromText("text to classify goes here"),
            //ModerationInput.FromImageUrl("https://example.com/image.png"),
            //ModerationInput.FromImageUrl($"data:image/jpeg;base64,iVBORw0KGgoA...")
        };

        var result = client.Classify(inputs);
        Assert.NotNull(result.Value);
    }
}