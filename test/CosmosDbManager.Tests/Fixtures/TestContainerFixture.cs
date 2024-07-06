using Testcontainers.CosmosDb;

// ReSharper disable ClassNeverInstantiated.Global

namespace CosmosDbManager.Tests.Fixtures;

public class TestContainerFixture
    : IAsyncLifetime
{
    private CosmosDbContainer _cosmosDbContainer = null!;
    public string CosmosDbConnectionString { get; private set; } = null!;
    public HttpClient CosmosDbHttpClient { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        _cosmosDbContainer = 
            new CosmosDbBuilder()
                //.WithExposedPort(8081)
                .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE","127.0.0.1")
                .Build();
        await _cosmosDbContainer.StartAsync();
        CosmosDbConnectionString = _cosmosDbContainer.GetConnectionString();
        CosmosDbHttpClient = _cosmosDbContainer.HttpClient;
    }

    public Task DisposeAsync() => _cosmosDbContainer.StopAsync();
}