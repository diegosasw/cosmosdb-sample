using CosmosDbManager.Tests.Fixtures;
using Microsoft.Azure.Cosmos;
using xRetry;
using Xunit.Abstractions;

namespace CosmosDbManager.Tests;

[Trait("Category", "FluentDockerEmulator")]
public class CosmosServiceFluentDockerTests(ITestOutputHelper testOutputHelper)
    : IClassFixture<FluentDockerFixture>
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
        
        var cosmosClient = new CosmosClient(FluentDockerFixture.AccountEndpoint, FluentDockerFixture.AuthKeyOrResourceToken, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine($"Attempting to create {databaseName}");
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
                HttpClientFactory = () =>
                {
                    var httpMessageHandler =
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                
                    return new HttpClient(httpMessageHandler);
                },
            };

        var cosmosClient = new CosmosClient(FluentDockerFixture.AccountEndpoint, FluentDockerFixture.AuthKeyOrResourceToken, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine($"Attempting to create {databaseName}");
        var result = await sut.Create(databaseName);
        testOutputHelper.WriteLine("Attempt finished");

        // Then
        Assert.True(result.IsSuccessful);
    }
    
    [RetryTheory]
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
                HttpClientFactory = () =>
                {
                    var httpMessageHandler =
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                
                    return new HttpClient(httpMessageHandler);
                }
            };

        var cosmosClient = new CosmosClient(FluentDockerFixture.AccountEndpoint, FluentDockerFixture.AuthKeyOrResourceToken, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var containerName = $"container-{testNumber}";
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"and container {containerName}");
        var creationResult = await sut.CreateContainer(databaseName, containerName);
        var deletionResult = await sut.DeleteContainer(databaseName, containerName);
        testOutputHelper.WriteLine("Attempt finished");

        // Then
        Assert.True(creationResult.IsSuccessful);
        Assert.True(deletionResult.IsSuccessful);
    }
    
    [RetryTheory]
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
                HttpClientFactory = () =>
                {
                    var httpMessageHandler =
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                
                    return new HttpClient(httpMessageHandler);
                },
            };

        var cosmosClient = new CosmosClient(FluentDockerFixture.AccountEndpoint, FluentDockerFixture.AuthKeyOrResourceToken, cosmosClientOptions);
        var databaseName = Guid.NewGuid().ToString();
        var containerName = $"container-{testNumber}";
        var sut = new CosmosService(cosmosClient);
        
        // When
        testOutputHelper.WriteLine(
            $"Attempting to create {databaseName} " +
            $"and container {containerName}");
        var creationResult = await sut.CreateContainers(databaseName, containerName, numberOfContainers);
        var deletionResult = await sut.DeleteContainer(databaseName, containerName);

        // Then
        Assert.True(creationResult.IsSuccessful);
        Assert.True(deletionResult.IsSuccessful);
    }
}