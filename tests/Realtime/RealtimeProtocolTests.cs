using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Realtime;
using OpenAI.Tests.Telemetry;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.Telemetry.TestMeterListener;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[TestFixture(true)]
[TestFixture(false)]
public class RealtimeProtocolTests : RealtimeTestFixtureBase
{
    public RealtimeProtocolTests(bool isAsync) : base(isAsync)
    { }

    [Test]
    public async Task ProtocolCanConfigureSession()
    {
        RealtimeClient client = GetTestClient();
        using RealtimeSession session = await client.StartConversationSessionAsync(GetTestModel(), CancellationToken);

        BinaryData configureSessionCommand = BinaryData.FromString("""
            {
              "type": "session.update",
              "session": {
                "turn_detection": null
              }
            }
            """);
        await session.SendCommandAsync(configureSessionCommand, CancellationOptions);

        List<JsonNode> receivedCommands = [];

        await foreach (RealtimeUpdate update in session.ReceiveUpdatesAsync(CancellationToken))
        {
            BinaryData rawContentBytes = update.GetRawContent();
            JsonNode jsonNode = JsonNode.Parse(rawContentBytes);
            string updateType = jsonNode["type"]?.GetValue<string>();
            Assert.That(updateType, Is.Not.Null.And.Not.Empty);

            receivedCommands.Add(jsonNode);

            if (updateType == "error")
            {
                Assert.Fail($"Error encountered: {rawContentBytes.ToString()}");
            }
            else if (updateType == "session.created")
            {
                BinaryData createResponseCommand = BinaryData.FromString("""
                    {
                      "type": "response.create",
                      "response": {
                        "max_output_tokens": null
                      }
                    }
                    """);
                await session.SendCommandAsync(createResponseCommand, CancellationOptions);
            }
            else if (updateType == "response.done")
            {
                break;
            }
        }

        List<JsonNode> NodesOfType(string type) => receivedCommands.Where(command => command["type"].GetValue<string>() == type).ToList();

        Assert.That(NodesOfType("session.created"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("session.updated"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.created"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.done"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.output_item.added"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("conversation.item.created"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.content_part.added"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.audio_transcript.delta"), Has.Count.GreaterThan(0));
        Assert.That(NodesOfType("response.audio.delta"), Has.Count.GreaterThan(0));
        Assert.That(NodesOfType("response.audio_transcript.done"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.content_part.done"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.output_item.done"), Has.Count.EqualTo(1));
    }
}
