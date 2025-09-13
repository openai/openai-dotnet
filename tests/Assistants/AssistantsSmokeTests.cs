using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[Category("Assistants")]
[Category("Smoke")]
public class AssistantsSmokeTests
{
    [Test]
    public void RunStepDeserialization()
    {
        BinaryData runStepData = BinaryData.FromString(
            """
            {
              "id": "step_Ksdfr5ooy26sayKbIQu2d2Vb",
              "object": "thread.run.step",
              "created_at": 1718906747,
              "run_id": "run_vvuLqtPTte9qCnRb7a5MQPgB",
              "assistant_id": "asst_UyBYTjqlwhSOdHOEzwwGZM6d",
              "thread_id": "thread_lIk2yQzSGHzXrzA4K6N8uPae",
              "type": "tool_calls",
              "status": "completed",
              "cancelled_at": null,
              "completed_at": 1718906749,
              "expires_at": null,
              "failed_at": null,
              "last_error": null,
              "step_details": {
                "type": "tool_calls",
                "tool_calls": [
                  {
                    "id": "call_DUP8WOybwaxKcMoxtr6cJDw1",
                    "type": "code_interpreter",
                    "code_interpreter": {
                      "input": "# Let's read the content of the uploaded file to understand its content.\r\nfile_path = '/mnt/data/assistant-SvXXKd0VKpGbVq9rBDlvZTn0'\r\nwith open(file_path, 'r') as file:\r\n    content = file.read()\r\n\r\n# Output the first few lines of the file to understand its structure and content\r\ncontent[:2000]",
                      "outputs": [
                        {
                          "type": "logs",
                          "logs": "'Index,Value\\nIndex #1,1\\nIndex #2,4\\nIndex #3,9\\nIndex #4,16\\nIndex #5,25\\nIndex #6,36\\nIndex #7,49\\nIndex #8,64\\nIndex #9,81\\nIndex #10,100\\nIndex #11,121\\nIndex #12,144\\nIndex #13,169\\nIndex #14,196\\nIndex #15,225\\nIndex #16,256\\nIndex #17,289\\nIndex #18,324\\nIndex #19,361\\nIndex #20,400\\nIndex #21,441\\nIndex #22,484\\nIndex #23,529\\nIndex #24,576\\nIndex #25,625\\nIndex #26,676\\nIndex #27,729\\nIndex #28,784\\nIndex #29,841\\nIndex #30,900\\nIndex #31,961\\nIndex #32,1024\\nIndex #33,1089\\nIndex #34,1156\\nIndex #35,1225\\nIndex #36,1296\\nIndex #37,1369\\nIndex #38,1444\\nIndex #39,1521\\nIndex #40,1600\\nIndex #41,1681\\nIndex #42,1764\\nIndex #43,1849\\nIndex #44,1936\\nIndex #45,2025\\nIndex #46,2116\\nIndex #47,2209\\nIndex #48,2304\\nIndex #49,2401\\nIndex #50,2500\\nIndex #51,2601\\nIndex #52,2704\\nIndex #53,2809\\nIndex #54,2916\\nIndex #55,3025\\nIndex #56,3136\\nIndex #57,3249\\nIndex #58,3364\\nIndex #59,3481\\nIndex #60,3600\\nIndex #61,3721\\nIndex #62,3844\\nIndex #63,3969\\nIndex #64,4096\\nIndex #65,4225\\nIndex #66,4356\\nIndex #67,4489\\nIndex #68,4624\\nIndex #69,4761\\nIndex #70,4900\\nIndex #71,5041\\nIndex #72,5184\\nIndex #73,5329\\nIndex #74,5476\\nIndex #75,5625\\nIndex #76,5776\\nIndex #77,5929\\nIndex #78,6084\\nIndex #79,6241\\nIndex #80,6400\\nIndex #81,6561\\nIndex #82,6724\\nIndex #83,6889\\nIndex #84,7056\\nIndex #85,7225\\nIndex #86,7396\\nIndex #87,7569\\nIndex #88,7744\\nIndex #89,7921\\nIndex #90,8100\\nIndex #91,8281\\nIndex #92,8464\\nIndex #93,8649\\nIndex #94,8836\\nIndex #95,9025\\nIndex #96,9216\\nIndex #97,9409\\nIndex #98,9604\\nIndex #99,9801\\nIndex #100,10000\\nIndex #101,10201\\nIndex #102,10404\\nIndex #103,10609\\nIndex #104,10816\\nIndex #105,11025\\nIndex #106,11236\\nIndex #107,11449\\nIndex #108,11664\\nIndex #109,11881\\nIndex #110,12100\\nIndex #111,12321\\nIndex #112,12544\\nIndex #113,12769\\nIndex #114,12996\\nIndex #115,13225\\nIndex #116,13456\\nIndex #117,13689\\nIndex #118,13924\\nIndex #119,14161\\nIndex #120,14400\\nIndex #121,14641\\nIndex #122,14884\\nIndex #123,15129\\nIndex #124,15376\\nIndex #125,15625\\nIndex #126,15876\\nIndex #127,16129\\nIndex #128,16384\\nIndex #129,16641\\nIndex #130,16900\\nIndex #131,17161\\nIndex #132,'"
                        }
                      ]
                    }
                  }
                ]
              },
              "usage": {
                "prompt_tokens": 201,
                "completion_tokens": 84,
                "total_tokens": 285
              }
            }
            """);
        RunStep deserializedRunStep = ModelReaderWriter.Read<RunStep>(runStepData);
        Assert.That(deserializedRunStep.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(deserializedRunStep.AssistantId, Is.Not.Null.And.Not.Empty);
        Assert.That(deserializedRunStep.Details, Is.Not.Null);
        Assert.That(deserializedRunStep.Details.ToolCalls, Has.Count.EqualTo(1));
        Assert.That(deserializedRunStep.Details.ToolCalls[0].CodeInterpreterOutputs, Has.Count.EqualTo(1));
        Assert.That(deserializedRunStep.Details.ToolCalls[0].CodeInterpreterOutputs[0].Logs, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void ResponseFormatEquality()
    {
        Assert.That(AssistantResponseFormat.CreateAutoFormat() == "auto");
        Assert.That(AssistantResponseFormat.CreateAutoFormat(), Is.EqualTo("auto"));
        Assert.That(AssistantResponseFormat.CreateAutoFormat(), Is.Not.EqualTo("automatic"));
        Assert.That(AssistantResponseFormat.CreateAutoFormat() == AssistantResponseFormat.CreateAutoFormat());
        Assert.That(AssistantResponseFormat.CreateTextFormat() == AssistantResponseFormat.CreateTextFormat());
        Assert.That(AssistantResponseFormat.CreateTextFormat(), Is.EqualTo(AssistantResponseFormat.CreateTextFormat()));
        Assert.That(AssistantResponseFormat.CreateAutoFormat() != AssistantResponseFormat.CreateTextFormat());
        Assert.That(AssistantResponseFormat.CreateAutoFormat(), Is.Not.EqualTo(AssistantResponseFormat.CreateTextFormat()));
        Assert.That((AssistantResponseFormat)null == (AssistantResponseFormat)null);
        Assert.That((AssistantResponseFormat)null != AssistantResponseFormat.CreateTextFormat());
        Assert.That(AssistantResponseFormat.CreateTextFormat() != null);
        Assert.That(AssistantResponseFormat.CreateTextFormat(), Is.Not.EqualTo(null));
        Assert.That(AssistantResponseFormat.CreateTextFormat(), Is.Not.Null);

        AssistantResponseFormat jsonSchemaFormat = AssistantResponseFormat.CreateJsonSchemaFormat(
            name: "test_schema",
            description: "A description of the schema",
            jsonSchema: BinaryData.FromString("""
                {
                  "type": "object",
                  "properties": {
                    "foo": { "type": "string" }
                  },
                  "additionalProperties": false
                }
                """),
            strictSchemaEnabled: true);

        Assert.That(jsonSchemaFormat == AssistantResponseFormat.CreateJsonSchemaFormat("test_schema", BinaryData.FromObjectAsJson(new { })));
        Assert.That(jsonSchemaFormat != AssistantResponseFormat.CreateJsonSchemaFormat("not_test_schema", BinaryData.FromObjectAsJson(new { })));
    }
}

#pragma warning restore OPENAI001
