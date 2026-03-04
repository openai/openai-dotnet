using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

public partial class ResponsesToolTests
{
    [RecordedTest]
    public async Task FileSearch()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        OpenAIFile testFile = await fileClient.UploadFileAsync(
            BinaryData.FromString("""
                    Travis's favorite food is pizza.
                    """),
            "test_favorite_foods.txt",
            FileUploadPurpose.UserData);
        Validate(testFile);

        VectorStoreClient vscClient = GetProxiedOpenAIClient<VectorStoreClient>();
        VectorStore vectorStore = await vscClient.CreateVectorStoreAsync(
            new VectorStoreCreationOptions()
            {
                FileIds = { testFile.Id },
            });
        Validate(vectorStore);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions(TestModel.Responses, [ResponseItem.CreateUserMessageItem("Using the file search tool, what's Travis's favorite food?")])
            {
                Tools =
                {
                    ResponseTool.CreateFileSearchTool(vectorStoreIds: [vectorStore.Id]),
                }
            });
        Assert.That(response.OutputItems?.Count, Is.EqualTo(2));
        FileSearchCallResponseItem fileSearchCall = response.OutputItems[0] as FileSearchCallResponseItem;
        Assert.That(fileSearchCall, Is.Not.Null);
        Assert.That(fileSearchCall?.Status, Is.EqualTo(FileSearchCallStatus.Completed));
        Assert.That(fileSearchCall?.Queries, Has.Count.GreaterThan(0));
        MessageResponseItem message = response.OutputItems[1] as MessageResponseItem;
        Assert.That(message, Is.Not.Null);
        ResponseContentPart messageContentPart = message.Content?.FirstOrDefault();
        Assert.That(messageContentPart, Is.Not.Null);
        Assert.That(messageContentPart.Text, Does.Contain("pizza"));
        Assert.That(messageContentPart.OutputTextAnnotations, Is.Not.Null.And.Not.Empty);
        FileCitationMessageAnnotation annotation = messageContentPart.OutputTextAnnotations[0] as FileCitationMessageAnnotation;
        Assert.That(annotation.FileId, Is.EqualTo(testFile.Id));
        Assert.That(annotation.Filename, Is.EqualTo(testFile.Filename));
        Assert.That(annotation.Index, Is.GreaterThan(0));

        await foreach (ResponseItem inputItem in client.GetResponseInputItemsAsync(new ResponseItemCollectionOptions(response.Id)))
        {
            Console.WriteLine(ModelReaderWriter.Write(inputItem).ToString());
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithContainerIdFromContainerApi()
    {
        ContainerClient containerClient = GetProxiedOpenAIClient<ContainerClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        // Create a container first using the Containers API
        CreateContainerBody containerBody = new("test-container-for-code-interpreter");
        var containerResult = await containerClient.CreateContainerAsync(containerBody);
        Assert.That(containerResult.Value, Is.Not.Null);
        Assert.That(containerResult.Value.Id, Is.Not.Null.And.Not.Empty);

        string containerId = containerResult.Value.Id;

        try
        {
            // Create CodeInterpreter tool with the container ID
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(containerId));
            CreateResponseOptions responseOptions = new(TestModel.Responses, [ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code.")])
            {
                Tools = { codeInterpreterTool },
            };

            ResponseResult response = await client.CreateResponseAsync(
                responseOptions);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.OutputItems, Has.Count.EqualTo(2));
            Assert.That(response.OutputItems[0], Is.InstanceOf<CodeInterpreterCallResponseItem>());
            Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

            MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
            Assert.That(message.Content, Has.Count.GreaterThan(0));
            Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
            Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);

            // Basic validation that the response was created successfully
            Assert.That(response.Id, Is.Not.Null.And.Not.Empty);

            Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
        }
        finally
        {
            // Clean up the container
            try
            {
                await containerClient.DeleteContainerAsync(containerId);
            }
            catch
            {
                // Best effort cleanup - don't fail test if cleanup fails
            }
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithUploadedFileIds()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        // Create some test files to upload
        string csvContent = "name,age,city\nAlice,30,New York\nBob,25,Los Angeles\nCharlie,35,Chicago";
        string pythonContent = "# This is a simple Python file\ndef hello():\n    print('Hello from uploaded file!')\n\nif __name__ == '__main__':\n    hello()";

        List<string> fileIds = new();

        try
        {
            // Upload CSV file
            using Stream csvStream = BinaryData.FromString(csvContent).ToStream();
            OpenAIFile csvFile = await fileClient.UploadFileAsync(csvStream, "test_data.csv", FileUploadPurpose.Assistants);
            Validate(csvFile);
            fileIds.Add(csvFile.Id);

            // Upload Python file
            using Stream pythonStream = BinaryData.FromString(pythonContent).ToStream();
            OpenAIFile pythonFile = await fileClient.UploadFileAsync(pythonStream, "test_script.py", FileUploadPurpose.Assistants);
            Validate(pythonFile);
            fileIds.Add(pythonFile.Id);

            // Create CodeInterpreter tool with uploaded file IDs
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration(fileIds)));
            CreateResponseOptions responseOptions = new(TestModel.Responses, [ResponseItem.CreateUserMessageItem("Analyze the CSV data in the uploaded file and create a simple visualization. Also run the Python script that was uploaded.")])
            {
                Tools = { codeInterpreterTool },
            };

            ResponseResult response = await client.CreateResponseAsync(
                responseOptions);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.OutputItems, Is.Not.Null.And.Not.Empty);

            // Basic validation that the response was created successfully
            Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
        }
        catch
        {
            // If the test fails, still try to clean up the files immediately
            // (They'll also be cleaned up in OneTimeTearDown, but this is more immediate)
            foreach (string fileId in fileIds)
            {
                try
                {
                    await fileClient.DeleteFileAsync(fileId);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
            throw;
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolStreamingWithFiles()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        // Create test CSV data
        string csvContent = "x,y\n1,2\n2,4\n3,6\n4,8\n5,10";
        List<string> fileIds = new();

        try
        {
            // Upload CSV file
            using Stream csvStream = BinaryData.FromString(csvContent).ToStream();
            OpenAIFile csvFile = await fileClient.UploadFileAsync(csvStream, "test_data.csv", FileUploadPurpose.Assistants);
            Validate(csvFile);
            fileIds.Add(csvFile.Id);

            // Create CodeInterpreter tool with uploaded file IDs
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration(fileIds)));
            CreateResponseOptions responseOptions = new(TestModel.Responses, [ResponseItem.CreateUserMessageItem("Load the CSV file and create a simple plot visualization showing the relationship between x and y values.")])
            {
                Tools = { codeInterpreterTool },
                StreamingEnabled = true,
            };

            int inProgressCount = 0;
            int interpretingCount = 0;
            int codeDeltaCount = 0;
            int codeDoneCount = 0;
            int completedCount = 0;
            bool gotFinishedCodeInterpreterItem = false;
            StringBuilder codeBuilder = new StringBuilder();

            await foreach (StreamingResponseUpdate update
                in client.CreateResponseStreamingAsync(responseOptions))
            {
                ValidateCodeInterpreterEvent(ref inProgressCount, ref interpretingCount, ref codeDeltaCount, ref codeDoneCount, ref completedCount, ref gotFinishedCodeInterpreterItem, codeBuilder, update);
            }

            Assert.That(gotFinishedCodeInterpreterItem, Is.True);
            Assert.That(inProgressCount, Is.GreaterThan(0));
            Assert.That(interpretingCount, Is.GreaterThan(0));
            Assert.That(codeDeltaCount, Is.GreaterThan(0));
            Assert.That(codeDoneCount, Is.GreaterThanOrEqualTo(1)); // Should be at least one "done" event
            Assert.That(completedCount, Is.GreaterThan(0));
        }
        catch
        {
            // If the test fails, still try to clean up the files immediately
            foreach (string fileId in fileIds)
            {
                try
                {
                    await fileClient.DeleteFileAsync(fileId);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
            throw;
        }
    }

    [RecordedTest]
    [Category("MPFD")]
    public async Task ImageGenToolInputMaskWithFileId()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();

        string imageFilename = "images_empty_room.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        BinaryData imageBytes = BinaryData.FromBytes(File.ReadAllBytes(imagePath));

        string maskFilename = "images_empty_room_with_mask.png";
        string maskPath = Path.Combine("Assets", maskFilename);
        BinaryData maskBytes = BinaryData.FromBytes(File.ReadAllBytes(maskPath));

        OpenAIFile imageFile;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            imageFile = await fileClient.UploadFileAsync(imageBytes, imageFilename, FileUploadPurpose.UserData);
        }
        Validate(imageFile);

        OpenAIFile maskFile;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            maskFile = await fileClient.UploadFileAsync(maskBytes, maskFilename, FileUploadPurpose.UserData);
        }
        Validate(imageFile);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageFileId: imageFile.Id)])
        ];

        CreateResponseOptions options = new(TestModel.Responses, inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1-mini",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    size: ImageGenerationToolSize.W1024xH1024,
                    quality: ImageGenerationToolQuality.Low,
                    inputImageMask: new(fileId: maskFile.Id))
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<ImageGenerationCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<ImageGenerationTool>());

        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[0];
        Assert.That(imageGenResponse.Status, Is.EqualTo(ImageGenerationCallStatus.Completed));
        Assert.That(imageGenResponse.ImageResultBytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    [RecordedTest]
    public async Task WebSearchCall()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions(TestModel.Responses, [ResponseItem.CreateUserMessageItem("Searching the internet, what's the weather like in Seattle?")])
            {
                Tools =
                {
                    ResponseTool.CreateWebSearchTool()
                },
                ToolChoice = ResponseToolChoice.CreateWebSearchChoice()
            });

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<WebSearchCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);
        Assert.That(message.Content[0].OutputTextAnnotations, Has.Count.GreaterThan(0));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<WebSearchTool>());
    }

    [RecordedTest]
    public async Task WebSearchCallPreview()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions(TestModel.Responses, [ResponseItem.CreateUserMessageItem("What was a positive news story from today?")])
            {
                Tools =
                {
                    ResponseTool.CreateWebSearchPreviewTool()
                },
                ToolChoice = ResponseToolChoice.CreateWebSearchChoice()
            });

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<WebSearchCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);
        Assert.That(message.Content[0].OutputTextAnnotations, Has.Count.GreaterThan(0));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<WebSearchPreviewTool>());
    }

    [RecordedTest]
    public async Task WebSearchCallStreaming()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        const string message = "Searching the internet, what's the weather like in San Francisco?";

        CreateResponseOptions responseOptions = new(TestModel.Responses, [ResponseItem.CreateUserMessageItem(message)])
        {
            Tools =
            {
                ResponseTool.CreateWebSearchTool(
                    userLocation: WebSearchToolLocation.CreateApproximateLocation(city: "San Francisco"),
                    searchContextSize: WebSearchToolContextSize.Low)
            },
            StreamingEnabled = true,
        };

        string searchItemId = null;
        int inProgressCount = 0;
        int searchingCount = 0;
        int completedCount = 0;
        bool gotFinishedSearchItem = false;

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(responseOptions))
        {
            if (update is StreamingResponseWebSearchCallInProgressUpdate searchCallInProgressUpdate)
            {
                Assert.That(searchCallInProgressUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallInProgressUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallInProgressUpdate.ItemId));
                Assert.That(searchCallInProgressUpdate.OutputIndex, Is.EqualTo(0));
                inProgressCount++;
            }
            else if (update is StreamingResponseWebSearchCallSearchingUpdate searchCallSearchingUpdate)
            {
                Assert.That(searchCallSearchingUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallSearchingUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallSearchingUpdate.ItemId));
                Assert.That(searchCallSearchingUpdate.OutputIndex, Is.EqualTo(0));
                searchingCount++;
            }
            else if (update is StreamingResponseWebSearchCallCompletedUpdate searchCallCompletedUpdate)
            {
                Assert.That(searchCallCompletedUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallCompletedUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallCompletedUpdate.ItemId));
                Assert.That(searchCallCompletedUpdate.OutputIndex, Is.EqualTo(0));
                completedCount++;
            }
            else if (update is StreamingResponseOutputItemDoneUpdate outputItemDoneUpdate)
            {
                if (outputItemDoneUpdate.Item is WebSearchCallResponseItem webSearchCallItem)
                {
                    Assert.That(webSearchCallItem.Status, Is.EqualTo(WebSearchCallStatus.Completed));
                    Assert.That(webSearchCallItem.Id, Is.EqualTo(searchItemId));
                    gotFinishedSearchItem = true;
                }
            }
        }

        Assert.That(gotFinishedSearchItem, Is.True);
        Assert.That(searchingCount, Is.EqualTo(1));
        Assert.That(inProgressCount, Is.EqualTo(1));
        Assert.That(completedCount, Is.EqualTo(1));
        Assert.That(searchItemId, Is.Not.Null.And.Not.Empty);
    }

    private List<string> FileIdsToDelete = [];
    private List<string> VectorStoreIdsToDelete = [];

    private void Validate<T>(T input) where T : class
    {
        if (input is OpenAIFile file)
        {
            FileIdsToDelete.Add(file.Id);
        }
        if (input is VectorStore vectorStore)
        {
            VectorStoreIdsToDelete.Add(vectorStore.Id);
        }
    }
}
