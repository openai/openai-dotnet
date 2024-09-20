using NUnit.Framework;
using System;

namespace OpenAI.Tests.Miscellaneous;

public class GitHubTests
{
    [Test(Description = "Test that we can use a GitHub secret")]
    [Category("Online")]
    [Ignore("Placeholder")]
    public void CanUseGitHubSecret()
    {
        string gitHubSecretString = Environment.GetEnvironmentVariable("SECRET_VALUE");
        Assert.That(gitHubSecretString, Is.Not.Null.And.Not.Empty);
    }

    [Test(Description = "That that we can run some tests without secrets")]
    [Category("Offline")]
    [Ignore("Placeholder")]
    public void CanTestWithoutSecretAccess()
    {
        int result = 2 + 1;
        Assert.That(result, Is.EqualTo(3));
    }
}
