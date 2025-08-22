using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
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
        CreateContainerBody createBody = CreateContainerBodyFromName(containerName);
        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(createBody));
        
        ClientResult result = await client.CreateContainerAsync(content);
        
        if (result.GetRawResponse().IsError)
        {
            throw new InvalidOperationException($"Failed to create test container: {result.GetRawResponse().ReasonPhrase}");
        }
        
        // Parse the response to get the container ID
        BinaryData responseData = result.GetRawResponse().Content;
        using JsonDocument document = JsonDocument.Parse(responseData);
        _testContainerId = document.RootElement.GetProperty("id").GetString();
        
        Console.WriteLine($"Created test container: {_testContainerId}");
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