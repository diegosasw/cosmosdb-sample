using CosmosDbManager.Tests.Fixtures;
using Microsoft.Azure.Cosmos;
using Xunit.Abstractions;

namespace CosmosDbManager.Tests;

public class DatabaseCreatorFluentDockerTests(ITestOutputHelper testOutputHelper)
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
        
        var sut = new DatabaseCreatorTestContainer(cosmosClient);
        
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
        var sut = new DatabaseCreatorTestContainer(cosmosClient);
        
        // When
        testOutputHelper.WriteLine($"Attempting to create {databaseName}");
        var result = await sut.Create(databaseName);
        testOutputHelper.WriteLine("Attempt finished");

        // Then
        Assert.True(result.IsSuccessful);
    }
}