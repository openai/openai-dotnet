// Keeps endpoint security-policy tests separate from the runnable mTLS example.

using NUnit.Framework;
using System;

namespace OpenAI.Examples;

public partial class MutualTlsExamples
{
    [TestCase(GlobalEndpoint)]
    [TestCase(EuropeanUnionEndpoint)]
    [TestCase("https://mtls.api.openai.com:443/v1")]
    [TestCase("https://mtls.api.openai.com/v1/")]
    public void GetEndpointAcceptsSupportedMtlsBaseUrls(string value)
    {
        Assert.That(GetEndpoint(value), Is.Not.Null);
    }

    [TestCase("http://mtls.api.openai.com/v1")]
    [TestCase("https://mtls.api.openai.com:444/v1")]
    [TestCase("https://mtls.api.openai.com@attacker.example/v1")]
    [TestCase("https://mtls.api.openai.com.attacker.example/v1")]
    [TestCase("https://mtls.api.openai.com/v1/other")]
    [TestCase("https://mtls.api.openai.com/v1?query=value")]
    [TestCase("https://mtls.api.openai.com/v1#fragment")]
    public void GetEndpointRejectsUntrustedDestinations(string value)
    {
        Assert.Throws<InvalidOperationException>(() => GetEndpoint(value));
    }
}
