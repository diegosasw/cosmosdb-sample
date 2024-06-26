using Microsoft.Azure.Cosmos;

namespace CosmosDbSample;

public class Sample
{
    private readonly CosmosClient _cosmosClient;

    public Sample(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public Task CreateDatabase(string databaseId)
    {
        return _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
    }
    
    public Task CreateContainer(string databaseId, string containerId)
    {
        return _cosmosClient.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(containerId, "/mypartitionkey");
    }
}