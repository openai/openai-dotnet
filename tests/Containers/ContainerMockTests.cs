using NUnit.Framework;
using OpenAI.Containers;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Tests.Containers;

#pragma warning disable OPENAICUA001

[Category("Containers")]
[Category("Smoke")]
public partial class ContainerMockTests
{
    [Test]
    public void ContainerExpirationPolicyModelReaderWriterRoundTripsDuration()
    {
        ContainerExpirationPolicy policy = new()
        {
            Anchor = ContainerExpirationPolicyAnchor.LastActiveAt,
            Duration = TimeSpan.FromMinutes(20),
        };

        BinaryData serializedPolicy = ModelReaderWriter.Write(policy);
        string serializedPolicyJson = serializedPolicy.ToString();

        Assert.That(serializedPolicyJson, Does.Contain("\"anchor\":\"last_active_at\""));
        Assert.That(serializedPolicyJson, Does.Contain("\"minutes\":20"));

        ContainerExpirationPolicy deserializedPolicy = ModelReaderWriter.Read<ContainerExpirationPolicy>(serializedPolicy);

        Assert.That(deserializedPolicy, Is.Not.Null);
        Assert.That(deserializedPolicy.Anchor, Is.EqualTo(ContainerExpirationPolicyAnchor.LastActiveAt));
        Assert.That(deserializedPolicy.Duration, Is.EqualTo(TimeSpan.FromMinutes(20)));
    }
}