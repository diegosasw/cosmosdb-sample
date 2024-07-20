using CosmosDbManager.Tests.Fixtures;
using Microsoft.Azure.Cosmos;
using Xunit.Abstractions;

namespace CosmosDbManager.Tests;

[Trait("Category", "TestContainerEmulator")]
public class CosmosServiceTestContainerTests(TestContainerFixture testContainerFixture, ITestOutputHelper testOutputHelper)
    : IClassFixture<TestContainerFixture>
{
    [Fact]
    public async Task Secure_Client_Should_Fail()
    {
        // Given
        var cosmosClientOptions =
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            };

        var cosmosClient = new CosmosClient(testContainerFixture.CosmosDbConnectionString, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"with connection string {testContainerFixture.CosmosDbConnectionString}");
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => sut.Create(databaseName));
        testOutputHelper.WriteLine("Attempt finished");

        // Then
        Assert.Contains("The SSL connection could not be established", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
    
    [Fact]
    public async Task Insecure_Client_Should_Succeed()
    {
        // Given
        var cosmosClientOptions =
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () => testContainerFixture.CosmosDbHttpClient
            };

        var cosmosClient = new CosmosClient(testContainerFixture.CosmosDbConnectionString, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"with connection string {testContainerFixture.CosmosDbConnectionString}");
        var result = await sut.Create(databaseName);
        testOutputHelper.WriteLine("Attempt finished");

        // Then
        Assert.True(result.IsSuccessful);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task Create_Container_Should_Succeed(int testNumber)
    {
        testOutputHelper.WriteLine($"Test {testNumber}");
        // Given
        var cosmosClientOptions =
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () => testContainerFixture.CosmosDbHttpClient
            };

        var cosmosClient = new CosmosClient(testContainerFixture.CosmosDbConnectionString, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var containerName = $"container-{testNumber}";
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"with connection string {testContainerFixture.CosmosDbConnectionString} " +
            $"and container {containerName}");
        var result = await sut.CreateContainer(databaseName, containerName);

        // Then
        Assert.True(result.IsSuccessful);
    }
    
    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    [InlineData(3, 5)]
    [InlineData(4, 5)]
    [InlineData(5, 5)]
    [InlineData(6, 5)]
    [InlineData(7, 5)]
    [InlineData(8, 5)]
    [InlineData(9, 5)]
    [InlineData(10, 5)]
    public async Task Create_Containers_Should_Succeed(int testNumber, int numberOfContainers)
    {
        testOutputHelper.WriteLine($"Test {testNumber}");
        // Given
        var cosmosClientOptions =
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () => testContainerFixture.CosmosDbHttpClient
            };

        var cosmosClient = new CosmosClient(testContainerFixture.CosmosDbConnectionString, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var containerName = $"container-{testNumber}";
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"with connection string {testContainerFixture.CosmosDbConnectionString} " +
            $"and container {containerName}");
        var result = await sut.CreateContainers(databaseName, containerName, numberOfContainers);

        // Then
        Assert.True(result.IsSuccessful);
    }
}