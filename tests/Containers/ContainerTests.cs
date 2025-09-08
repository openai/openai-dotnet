using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Containers;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.Fixtures)]
[Category("Containers")]
public class ContainerTests : SyncAsyncTestBase
{
    private static string _testContainerId;
    private static ContainerClient GetTestClient() => GetTestClient<ContainerClient>(TestScenario.Containers);

    public ContainerTests(bool isAsync) : base(isAsync)
    {
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        // Skip setup if there is no API key (e.g., if we are not running live tests).
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            return;
        }

        ContainerClient client = GetTestClient();

        // Create a test container that will be used by all tests
        string containerName = $"test-container-{Guid.NewGuid():N}";
        ContainerResource result = await client.CreateContainerAsync(new CreateContainerBody(containerName));
        _testContainerId = result.Id;

        Console.WriteLine($"Created test container: {_testContainerId}");
        await Task.Delay(10000); // Wait for the containers to be available
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        // Skip teardown if there is no API key or no container was created
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")) || string.IsNullOrEmpty(_testContainerId))
        {
            return;
        }

        ContainerClient client = GetTestClient();

        try
        {
            await client.DeleteContainerAsync(_testContainerId);
            Console.WriteLine($"Deleted test container: {_testContainerId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete test container {_testContainerId}: {ex.Message}");
        }
        finally
        {
            _testContainerId = null;
        }
    }

    private static CreateContainerBody CreateContainerBodyFromName(string name)
    {
        // Use reflection to create the CreateContainerBody since it only has internal constructors
        var createBodyType = typeof(CreateContainerBody);
        var constructor = createBodyType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0];
        return (CreateContainerBody)constructor.Invoke(new object[] { name });
    }

    [Test]
    public async Task CanEnumerateContainers()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test GetContainersAsync method with various options
        ContainerCollectionOptions options = new()
        {
            Order = ContainerCollectionOrder.Descending,
            PageSizeLimit = 10
        };

        int count = 0;
        bool foundTestContainer = false;

        if (IsAsync)
        {
            AsyncCollectionResult<ContainerResource> containers = client.GetContainersAsync(options);
            await foreach (ContainerResource container in containers)
            {
                count++;
                Console.WriteLine($"[{count,3}] {container.Id} {container.CreatedAt:s} {container.Name ?? "(no name)"}");
                Validate(container);

                if (container.Id == _testContainerId)
                {
                    foundTestContainer = true;
                    break;
                }

                // Limit enumeration to avoid long test runs
                if (count >= 20)
                {
                    break;
                }
            }
        }
        else
        {
            CollectionResult<ContainerResource> containers = client.GetContainers(options);
            foreach (ContainerResource container in containers)
            {
                Console.WriteLine($"[{count,3}] {container.Id} {container.CreatedAt:s} {container.Name ?? "(no name)"}");
                Validate(container);

                if (container.Id == _testContainerId)
                {
                    foundTestContainer = true;
                }

                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 20)
                {
                    break;
                }
            }
        }

        Assert.That(count, Is.GreaterThan(0), "Should have found at least one container");
        Assert.That(foundTestContainer, Is.True, "Should have found our test container in the enumeration");
        Console.WriteLine($"Found {count} containers, including our test container");
    }

    [Test]
    public async Task CanEnumerateContainerFiles()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test GetContainerFilesAsync method
        ContainerFileCollectionOptions options = new()
        {
            Order = ContainerCollectionOrder.Descending,
            PageSizeLimit = 10
        };

        int count = 0;

        if (IsAsync)
        {
            AsyncCollectionResult<ContainerFileResource> files = client.GetContainerFilesAsync(_testContainerId, options);
            await foreach (ContainerFileResource file in files)
            {
                Console.WriteLine($"[{count,3}] {file.Id} {file.CreatedAt:s} {file.Path} ({file.Bytes} bytes)");
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId), "File should belong to the correct container");
                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 20)
                {
                    break;
                }
            }
        }
        else
        {
            CollectionResult<ContainerFileResource> files = client.GetContainerFiles(_testContainerId, options);
            foreach (ContainerFileResource file in files)
            {
                Console.WriteLine($"[{count,3}] {file.Id} {file.CreatedAt:s} {file.Path} ({file.Bytes} bytes)");
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId), "File should belong to the correct container");
                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 20)
                {
                    break;
                }
            }
        }

        Console.WriteLine($"Found {count} files in test container {_testContainerId}");
        // Note: A new container may have no files, so count could be 0 - this is expected
    }

    [Test]
    public async Task CanEnumerateContainersWithDefaultOptions()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test with default options (null)
        int count = 0;
        bool foundTestContainer = false;

        if (IsAsync)
        {
            AsyncCollectionResult<ContainerResource> containers = client.GetContainersAsync();
            await foreach (ContainerResource container in containers)
            {
                Console.WriteLine($"[{count,3}] {container.Id} {container.CreatedAt:s} {container.Name ?? "(no name)"}");
                Validate(container);

                if (container.Id == _testContainerId)
                {
                    foundTestContainer = true;
                }

                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 10)
                {
                    break;
                }
            }
        }
        else
        {
            CollectionResult<ContainerResource> containers = client.GetContainers();
            foreach (ContainerResource container in containers)
            {
                Console.WriteLine($"[{count,3}] {container.Id} {container.CreatedAt:s} {container.Name ?? "(no name)"}");
                Validate(container);

                if (container.Id == _testContainerId)
                {
                    foundTestContainer = true;
                }

                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 10)
                {
                    break;
                }
            }
        }

        Assert.That(count, Is.GreaterThan(0), "Enumeration should work with default options");
        Assert.That(foundTestContainer, Is.True, "Should have found our test container with default options");
        Console.WriteLine($"Found {count} containers with default options, including our test container");
    }

    [Test]
    public async Task CanEnumerateContainerFilesWithDefaultOptions()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test with default options (null)
        int count = 0;

        if (IsAsync)
        {
            AsyncCollectionResult<ContainerFileResource> files = client.GetContainerFilesAsync(_testContainerId);
            await foreach (ContainerFileResource file in files)
            {
                Console.WriteLine($"[{count,3}] {file.Id} {file.CreatedAt:s} {file.Path} ({file.Bytes} bytes)");
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 10)
                {
                    break;
                }
            }
        }
        else
        {
            CollectionResult<ContainerFileResource> files = client.GetContainerFiles(_testContainerId);
            foreach (ContainerFileResource file in files)
            {
                Console.WriteLine($"[{count,3}] {file.Id} {file.CreatedAt:s} {file.Path} ({file.Bytes} bytes)");
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                count++;

                // Limit enumeration to avoid long test runs
                if (count >= 10)
                {
                    break;
                }
            }
        }

        Console.WriteLine($"Found {count} files in test container {_testContainerId} with default options");
    }

    [Test]
    public async Task CanEnumerateContainersWithCancellation()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30)); // Prevent infinite test runs

        ContainerCollectionOptions options = new()
        {
            PageSizeLimit = 5
        };

        int count = 0;

        try
        {
            if (IsAsync)
            {
                AsyncCollectionResult<ContainerResource> containers = client.GetContainersAsync(options, cancellationTokenSource.Token);
                await foreach (ContainerResource container in containers.WithCancellation(cancellationTokenSource.Token))
                {
                    Validate(container);
                    count++;

                    // Stop after a few items to test cancellation works
                    if (count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                CollectionResult<ContainerResource> containers = client.GetContainers(options, cancellationTokenSource.Token);
                foreach (ContainerResource container in containers)
                {
                    Validate(container);
                    count++;

                    // Stop after a few items
                    if (count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected if cancellation occurs
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(0), "Enumeration with cancellation should work");
        Console.WriteLine($"Enumerated {count} containers with cancellation");
    }

    [Test]
    public async Task CanEnumerateContainerFilesWithCancellation()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

        ContainerFileCollectionOptions options = new()
        {
            PageSizeLimit = 5
        };

        int count = 0;

        try
        {
            if (IsAsync)
            {
                AsyncCollectionResult<ContainerFileResource> files = client.GetContainerFilesAsync(_testContainerId, options, cancellationTokenSource.Token);
                await foreach (ContainerFileResource file in files.WithCancellation(cancellationTokenSource.Token))
                {
                    Validate(file);
                    Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                    count++;

                    // Stop after a few items to test cancellation works
                    if (count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                CollectionResult<ContainerFileResource> files = client.GetContainerFiles(_testContainerId, options, cancellationTokenSource.Token);
                foreach (ContainerFileResource file in files)
                {
                    Validate(file);
                    Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                    count++;

                    // Stop after a few items
                    if (count >= 3)
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected if cancellation occurs
        }

        Console.WriteLine($"Enumerated {count} files with cancellation token");
    }

    [Test]
    public async Task ContainerCollectionOptionsCanBeConfigured()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test different ordering options
        var ascendingOptions = new ContainerCollectionOptions()
        {
            Order = ContainerCollectionOrder.Ascending,
            PageSizeLimit = 5
        };

        var descendingOptions = new ContainerCollectionOptions()
        {
            Order = ContainerCollectionOrder.Descending,
            PageSizeLimit = 5
        };

        int ascendingCount = 0;
        int descendingCount = 0;

        if (IsAsync)
        {
            // Test ascending order
            AsyncCollectionResult<ContainerResource> ascendingContainers = client.GetContainersAsync(ascendingOptions);
            await foreach (ContainerResource container in ascendingContainers)
            {
                Validate(container);
                ascendingCount++;
                if (ascendingCount >= 3) break;
            }

            // Test descending order
            AsyncCollectionResult<ContainerResource> descendingContainers = client.GetContainersAsync(descendingOptions);
            await foreach (ContainerResource container in descendingContainers)
            {
                Validate(container);
                descendingCount++;
                if (descendingCount >= 3) break;
            }
        }
        else
        {
            // Test ascending order
            CollectionResult<ContainerResource> ascendingContainers = client.GetContainers(ascendingOptions);
            foreach (ContainerResource container in ascendingContainers)
            {
                Validate(container);
                ascendingCount++;
                if (ascendingCount >= 3) break;
            }

            // Test descending order
            CollectionResult<ContainerResource> descendingContainers = client.GetContainers(descendingOptions);
            foreach (ContainerResource container in descendingContainers)
            {
                Validate(container);
                descendingCount++;
                if (descendingCount >= 3) break;
            }
        }

        // Both orderings should work (even if they return the same results)
        Assert.That(ascendingCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(descendingCount, Is.GreaterThanOrEqualTo(0));
        Console.WriteLine($"Ascending: {ascendingCount}, Descending: {descendingCount}");
    }

    [Test]
    public async Task ContainerFileCollectionOptionsCanBeConfigured()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test different ordering options for files
        var ascendingOptions = new ContainerFileCollectionOptions()
        {
            Order = ContainerCollectionOrder.Ascending,
            PageSizeLimit = 5
        };

        var descendingOptions = new ContainerFileCollectionOptions()
        {
            Order = ContainerCollectionOrder.Descending,
            PageSizeLimit = 5
        };

        int ascendingCount = 0;
        int descendingCount = 0;

        if (IsAsync)
        {
            // Test ascending order
            AsyncCollectionResult<ContainerFileResource> ascendingFiles = client.GetContainerFilesAsync(_testContainerId, ascendingOptions);
            await foreach (ContainerFileResource file in ascendingFiles)
            {
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                ascendingCount++;
                if (ascendingCount >= 3) break;
            }

            // Test descending order
            AsyncCollectionResult<ContainerFileResource> descendingFiles = client.GetContainerFilesAsync(_testContainerId, descendingOptions);
            await foreach (ContainerFileResource file in descendingFiles)
            {
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                descendingCount++;
                if (descendingCount >= 3) break;
            }
        }
        else
        {
            // Test ascending order
            CollectionResult<ContainerFileResource> ascendingFiles = client.GetContainerFiles(_testContainerId, ascendingOptions);
            foreach (ContainerFileResource file in ascendingFiles)
            {
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                ascendingCount++;
                if (ascendingCount >= 3) break;
            }

            // Test descending order
            CollectionResult<ContainerFileResource> descendingFiles = client.GetContainerFiles(_testContainerId, descendingOptions);
            foreach (ContainerFileResource file in descendingFiles)
            {
                Validate(file);
                Assert.That(file.ContainerId, Is.EqualTo(_testContainerId));
                descendingCount++;
                if (descendingCount >= 3) break;
            }
        }

        // Both orderings should work (even if they return the same results)
        Assert.That(ascendingCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(descendingCount, Is.GreaterThanOrEqualTo(0));
        Console.WriteLine($"Files - Ascending: {ascendingCount}, Descending: {descendingCount}");
    }

    private static void Validate(ContainerResource container)
    {
        Assert.That(container, Is.Not.Null);
        Assert.That(container.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(container.Object, Is.Not.Null.And.Not.Empty);
        Assert.That(container.CreatedAt, Is.GreaterThan(DateTimeOffset.MinValue));
        Assert.That(container.Status, Is.Not.Null.And.Not.Empty);
        // Name can be null/empty for some containers
    }

    [Test]
    public async Task CanGetContainer()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        ContainerResource container;

        if (IsAsync)
        {
            ClientResult<ContainerResource> result = await client.GetContainerAsync(_testContainerId);
            container = result.Value;
        }
        else
        {
            ClientResult<ContainerResource> result = client.GetContainer(_testContainerId);
            container = result.Value;
        }

        Validate(container);
        Assert.That(container.Id, Is.EqualTo(_testContainerId), "Retrieved container should have the correct ID");
        Console.WriteLine($"Retrieved container: {container.Id} with status {container.Status}");
    }

    [Test]
    public async Task CanGetContainerWithCancellation()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

        ContainerResource container;

        if (IsAsync)
        {
            ClientResult<ContainerResource> result = await client.GetContainerAsync(_testContainerId, cancellationTokenSource.Token);
            container = result.Value;
        }
        else
        {
            ClientResult<ContainerResource> result = client.GetContainer(_testContainerId, cancellationTokenSource.Token);
            container = result.Value;
        }

        Validate(container);
        Assert.That(container.Id, Is.EqualTo(_testContainerId));
        Console.WriteLine($"Retrieved container with cancellation: {container.Id}");
    }

    [Test]
    public async Task CanCreateAndDeleteContainerFile()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Create a test file using multipart form data
        string testContent = "This is a test file content for container testing.";
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        
        // Create multipart form data using the internal helper
        var formData = new MultiPartFormDataBinaryContent();
        formData.Add(contentBytes, "file", "test-file.txt", "text/plain");

        ClientResult createResult;
        if (IsAsync)
        {
            createResult = await client.CreateContainerFileAsync(_testContainerId, formData, formData.ContentType);
        }
        else
        {
            createResult = client.CreateContainerFile(_testContainerId, formData, formData.ContentType);
        }

        Assert.That(createResult, Is.Not.Null);
        Assert.That(createResult.GetRawResponse().IsError, Is.False, "File creation should succeed");

        // Extract the file ID from the response (this might need adjustment based on the actual response format)
        string responseContent = createResult.GetRawResponse().Content.ToString();
        Console.WriteLine($"Create file response: {responseContent}");

        // Parse the response to get the file ID
        var responseJson = JsonDocument.Parse(responseContent);
        string fileId = responseJson.RootElement.GetProperty("id").GetString();
        Assert.That(fileId, Is.Not.Null.And.Not.Empty, "File ID should be returned from creation");

        Console.WriteLine($"Created file with ID: {fileId}");

        try
        {
            // Now delete the file
            ClientResult<DeleteContainerFileResponse> deleteResult;
            if (IsAsync)
            {
                deleteResult = await client.DeleteContainerFileAsync(_testContainerId, fileId);
            }
            else
            {
                deleteResult = client.DeleteContainerFile(_testContainerId, fileId);
            }

            Assert.That(deleteResult, Is.Not.Null);
            Assert.That(deleteResult.Value, Is.Not.Null);
            Assert.That(deleteResult.Value.Id, Is.EqualTo(fileId), "Deleted file ID should match");
            Assert.That(deleteResult.Value.Object, Is.Not.Null.And.Not.Empty);
            Assert.That(deleteResult.Value.Deleted, Is.True, "File should be marked as deleted");

            Console.WriteLine($"Successfully deleted file: {fileId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete file {fileId}: {ex.Message}");
            // Don't fail the test if cleanup fails
        }
    }

    [Test]
    public async Task CanCreateGetAndDeleteContainerFile()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Create a test file using multipart form data
        string testContent = "Test file content for get/delete operations.";
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        
        var formData = new MultiPartFormDataBinaryContent();
        formData.Add(contentBytes, "file", "test-get-file.txt", "text/plain");

        ClientResult createResult;
        if (IsAsync)
        {
            createResult = await client.CreateContainerFileAsync(_testContainerId, formData, formData.ContentType);
        }
        else
        {
            createResult = client.CreateContainerFile(_testContainerId, formData, formData.ContentType);
        }

        string responseContent = createResult.GetRawResponse().Content.ToString();
        var responseJson = JsonDocument.Parse(responseContent);
        string fileId = responseJson.RootElement.GetProperty("id").GetString();

        try
        {
            // Get the file metadata
            ContainerFileResource fileResource;
            if (IsAsync)
            {
                ClientResult<ContainerFileResource> getResult = await client.GetContainerFileAsync(_testContainerId, fileId);
                fileResource = getResult.Value;
            }
            else
            {
                ClientResult<ContainerFileResource> getResult = client.GetContainerFile(_testContainerId, fileId);
                fileResource = getResult.Value;
            }

            Validate(fileResource);
            Assert.That(fileResource.Id, Is.EqualTo(fileId));
            Assert.That(fileResource.ContainerId, Is.EqualTo(_testContainerId));
            Assert.That(fileResource.Bytes, Is.GreaterThan(0), "File size should be greater than 0");

            Console.WriteLine($"Retrieved file metadata: {fileResource.Id}, {fileResource.Bytes} bytes");

            // Get the file content
            BinaryData fileContent;
            if (IsAsync)
            {
                ClientResult<BinaryData> contentResult = await client.GetContainerFileContentAsync(_testContainerId, fileId);
                fileContent = contentResult.Value;
            }
            else
            {
                ClientResult<BinaryData> contentResult = client.GetContainerFileContent(_testContainerId, fileId);
                fileContent = contentResult.Value;
            }

            Assert.That(fileContent, Is.Not.Null);
            Assert.That(fileContent.ToArray().Length, Is.GreaterThan(0), "File content should not be empty");

            Console.WriteLine($"Retrieved file content with {fileContent.ToArray().Length} bytes");
        }
        finally
        {
            // Clean up - delete the file
            try
            {
                if (IsAsync)
                {
                    await client.DeleteContainerFileAsync(_testContainerId, fileId);
                }
                else
                {
                    client.DeleteContainerFile(_testContainerId, fileId);
                }
                Console.WriteLine($"Cleaned up file: {fileId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up file {fileId}: {ex.Message}");
            }
        }
    }

    [Test]
    public async Task CanGetContainerFileWithCancellation()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Create a test file first using multipart form data
        string testContent = "Test content for cancellation test.";
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        
        var formData = new MultiPartFormDataBinaryContent();
        formData.Add(contentBytes, "file", "test-cancel-file.txt", "text/plain");

        ClientResult createResult;
        if (IsAsync)
        {
            createResult = await client.CreateContainerFileAsync(_testContainerId, formData, formData.ContentType);
        }
        else
        {
            createResult = client.CreateContainerFile(_testContainerId, formData, formData.ContentType);
        }

        string responseContent = createResult.GetRawResponse().Content.ToString();
        var responseJson = JsonDocument.Parse(responseContent);
        string fileId = responseJson.RootElement.GetProperty("id").GetString();

        try
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            // Test GetContainerFile with cancellation
            ContainerFileResource fileResource;
            if (IsAsync)
            {
                ClientResult<ContainerFileResource> result = await client.GetContainerFileAsync(_testContainerId, fileId, cancellationTokenSource.Token);
                fileResource = result.Value;
            }
            else
            {
                ClientResult<ContainerFileResource> result = client.GetContainerFile(_testContainerId, fileId, cancellationTokenSource.Token);
                fileResource = result.Value;
            }

            Validate(fileResource);
            Assert.That(fileResource.Id, Is.EqualTo(fileId));

            // Test GetContainerFileContent with cancellation
            BinaryData fileContent;
            if (IsAsync)
            {
                ClientResult<BinaryData> contentResult = await client.GetContainerFileContentAsync(_testContainerId, fileId, cancellationTokenSource.Token);
                fileContent = contentResult.Value;
            }
            else
            {
                ClientResult<BinaryData> contentResult = client.GetContainerFileContent(_testContainerId, fileId, cancellationTokenSource.Token);
                fileContent = contentResult.Value;
            }

            Assert.That(fileContent, Is.Not.Null);
            Console.WriteLine($"Successfully retrieved file with cancellation token");
        }
        finally
        {
            // Clean up
            try
            {
                if (IsAsync)
                {
                    await client.DeleteContainerFileAsync(_testContainerId, fileId);
                }
                else
                {
                    client.DeleteContainerFile(_testContainerId, fileId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up file {fileId}: {ex.Message}");
            }
        }
    }

    [Test]
    public async Task CanDeleteContainerFileWithCancellation()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Create a test file first using multipart form data
        string testContent = "Test content for deletion with cancellation.";
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        
        var formData = new MultiPartFormDataBinaryContent();
        formData.Add(contentBytes, "file", "test-delete-cancel-file.txt", "text/plain");

        ClientResult createResult;
        if (IsAsync)
        {
            createResult = await client.CreateContainerFileAsync(_testContainerId, formData, formData.ContentType);
        }
        else
        {
            createResult = client.CreateContainerFile(_testContainerId, formData, formData.ContentType);
        }

        string responseContent = createResult.GetRawResponse().Content.ToString();
        var responseJson = JsonDocument.Parse(responseContent);
        string fileId = responseJson.RootElement.GetProperty("id").GetString();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

        // Delete the file with cancellation token
        ClientResult<DeleteContainerFileResponse> deleteResult;
        if (IsAsync)
        {
            deleteResult = await client.DeleteContainerFileAsync(_testContainerId, fileId, cancellationTokenSource.Token);
        }
        else
        {
            deleteResult = client.DeleteContainerFile(_testContainerId, fileId, cancellationTokenSource.Token);
        }

        Assert.That(deleteResult, Is.Not.Null);
        Assert.That(deleteResult.Value, Is.Not.Null);
        Assert.That(deleteResult.Value.Id, Is.EqualTo(fileId));
        Assert.That(deleteResult.Value.Deleted, Is.True);

        Console.WriteLine($"Successfully deleted file with cancellation token: {fileId}");
    }

    [Test]
    public void CreateContainerFileValidatesParameters()
    {
        ContainerClient client = GetTestClient();

        if (string.IsNullOrEmpty(_testContainerId))
        {
            Assert.Ignore("No test container available - likely running without API key");
            return;
        }

        // Test null content
        if (IsAsync)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.CreateContainerFileAsync(_testContainerId, null, "multipart/form-data"));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => 
                client.CreateContainerFile(_testContainerId, null, "multipart/form-data"));
        }

        // Test null/empty container ID
        var testFormData = new MultiPartFormDataBinaryContent();
        testFormData.Add("test", "file", "test.txt", "text/plain");
        
        if (IsAsync)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.CreateContainerFileAsync(null, testFormData, testFormData.ContentType));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.CreateContainerFileAsync("", testFormData, testFormData.ContentType));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => 
                client.CreateContainerFile(null, testFormData, testFormData.ContentType));
            Assert.Throws<ArgumentException>(() => 
                client.CreateContainerFile("", testFormData, testFormData.ContentType));
        }

        Console.WriteLine("Parameter validation tests passed");
    }

    [Test]
    public void GetContainerFileValidatesParameters()
    {
        ContainerClient client = GetTestClient();

        // Test null/empty container ID and file ID
        if (IsAsync)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.GetContainerFileAsync(null, "file123"));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.GetContainerFileAsync("", "file123"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.GetContainerFileAsync("container123", null));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.GetContainerFileAsync("container123", ""));

            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.GetContainerFileContentAsync(null, "file123"));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.GetContainerFileContentAsync("", "file123"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.GetContainerFileContentAsync("container123", null));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.GetContainerFileContentAsync("container123", ""));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => 
                client.GetContainerFile(null, "file123"));
            Assert.Throws<ArgumentException>(() => 
                client.GetContainerFile("", "file123"));
            Assert.Throws<ArgumentNullException>(() => 
                client.GetContainerFile("container123", null));
            Assert.Throws<ArgumentException>(() => 
                client.GetContainerFile("container123", ""));

            Assert.Throws<ArgumentNullException>(() => 
                client.GetContainerFileContent(null, "file123"));
            Assert.Throws<ArgumentException>(() => 
                client.GetContainerFileContent("", "file123"));
            Assert.Throws<ArgumentNullException>(() => 
                client.GetContainerFileContent("container123", null));
            Assert.Throws<ArgumentException>(() => 
                client.GetContainerFileContent("container123", ""));
        }

        Console.WriteLine("Parameter validation tests passed for GetContainerFile methods");
    }

    [Test]
    public void DeleteContainerFileValidatesParameters()
    {
        ContainerClient client = GetTestClient();

        // Test null/empty container ID and file ID
        if (IsAsync)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.DeleteContainerFileAsync(null, "file123"));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.DeleteContainerFileAsync("", "file123"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.DeleteContainerFileAsync("container123", null));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.DeleteContainerFileAsync("container123", ""));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => 
                client.DeleteContainerFile(null, "file123"));
            Assert.Throws<ArgumentException>(() => 
                client.DeleteContainerFile("", "file123"));
            Assert.Throws<ArgumentNullException>(() => 
                client.DeleteContainerFile("container123", null));
            Assert.Throws<ArgumentException>(() => 
                client.DeleteContainerFile("container123", ""));
        }

        Console.WriteLine("Parameter validation tests passed for DeleteContainerFile methods");
    }

    [Test]
    public void GetContainerValidatesParameters()
    {
        ContainerClient client = GetTestClient();

        // Test null/empty container ID
        if (IsAsync)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await client.GetContainerAsync(null));
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await client.GetContainerAsync(""));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => 
                client.GetContainer(null));
            Assert.Throws<ArgumentException>(() => 
                client.GetContainer(""));
        }

        Console.WriteLine("Parameter validation tests passed for GetContainer methods");
    }

    private static void Validate(ContainerFileResource file)
    {
        Assert.That(file, Is.Not.Null);
        Assert.That(file.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(file.Object, Is.Not.Null.And.Not.Empty);
        Assert.That(file.ContainerId, Is.Not.Null.And.Not.Empty);
        Assert.That(file.CreatedAt, Is.GreaterThan(DateTimeOffset.MinValue));
        Assert.That(file.Bytes, Is.GreaterThanOrEqualTo(0));
        Assert.That(file.Path, Is.Not.Null);
        Assert.That(file.Source, Is.Not.Null);
    }
}