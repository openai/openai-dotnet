using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

public class RealtimeProtocolTests : RealtimeTestFixtureBase
{
    public RealtimeProtocolTests(bool isAsync) : base(isAsync)
    { }

    [LiveOnly]
    [Test]
    public async Task ProtocolCanConfigureSession()
    {
        RealtimeClient client = GetTestClient();
        using RealtimeSession session = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

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

    [Test]
    public async Task CreateEphemeralToken()
    {
        RealtimeClient client = GetTestClient(excludeDumpPolicy: true);

        BinaryData input = BinaryData.FromBytes("""
            {
               "model": "gpt-4o-realtime-preview",
               "instructions": "You are a friendly assistant."
            }
            """u8.ToArray());

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = await client.CreateEphemeralTokenAsync(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        string objectKind = outputAsJson.RootElement
            .GetProperty("object"u8)
            .GetString();

        Assert.That(objectKind, Is.EqualTo("realtime.session"));
    }

    [Test]
    public async Task CreateEphemeralTranscriptionToken()
    {
        RealtimeClient client = GetTestClient(excludeDumpPolicy: true);

        BinaryData input = BinaryData.FromBytes("""
            {
            }
            """u8.ToArray());

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = await client.CreateEphemeralTranscriptionTokenAsync(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        string objectKind = outputAsJson.RootElement
            .GetProperty("object"u8)
            .GetString();

        Assert.That(objectKind, Is.EqualTo("realtime.transcription_session"));
    }
}
