using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenAI.Videos;
using System;
using System.ClientModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class VectorStoreExamples
{
    [Test]
    public async Task Example01_SimpleVideoCreationAsync()
    {
        // 1) Create the client
        VideoClient client = new(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));

        // 2) Build the multipart/form-data payload with an explicit boundary
        var boundary = Guid.NewGuid().ToString();
        var contentType = $"multipart/form-data; boundary=\"{boundary}\"";
        using var multipart = new MultipartFormDataContent(boundary);

        multipart.Add(new StringContent("sora-2", Encoding.UTF8, "text/plain"), "model");
        multipart.Add(new StringContent("A calico cat playing a piano on stage", Encoding.UTF8, "text/plain"), "prompt");

        // 3) Get a stream for the multipart body
        using var bodyStream = await multipart.ReadAsStreamAsync();

        // 4) Send the request
        var createResult = await client.CreateVideoAsync(BinaryContent.Create(bodyStream), contentType);
        var createRaw = createResult.GetRawResponse().Content;

        // 5) Parse the JSON response
        using var createdDoc = JsonDocument.Parse(createRaw);
        var id = createdDoc.RootElement.GetProperty("id").GetString();
        var status = createdDoc.RootElement.GetProperty("status").GetString();

        Console.WriteLine($"CreateVideo => id: {id}, status: {status}");
    }
}

#pragma warning restore OPENAI001
