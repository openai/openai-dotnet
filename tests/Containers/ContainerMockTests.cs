using NUnit.Framework;
using OpenAI.Containers;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Tests.Containers;

#pragma warning disable OPENAI001

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

    [Test]
    public void ContainerCreationOptionsPublicConstructorDoesNotPropagatePatch()
    {
        ContainerCreationOptions options = new("test-container")
        {
            ExpirationPolicy = new ContainerExpirationPolicy(),
        };

        options.Patch.Set("$.expires_after.custom_property"u8, "custom_value");

        Assert.That(options.ExpirationPolicy.Patch.Contains("$.custom_property"u8), Is.False);
    }

    [Test]
    public void ContainerCreationOptionsModelReaderWriterReadPropagatesPatch()
    {
        ContainerCreationOptions options = ModelReaderWriter.Read<ContainerCreationOptions>(
            BinaryData.FromString("""
                {
                  "name": "test-container",
                  "expires_after": {}
                }
                """));

        options.Patch.Set("$.expires_after.custom_property"u8, "custom_value");

        Assert.That(options.ExpirationPolicy.Patch.Contains("$.custom_property"u8), Is.True);
    }
}