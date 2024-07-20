using System.Net;
using Microsoft.Azure.Cosmos;
// ReSharper disable ConvertToPrimaryConstructor

namespace CosmosDbManager;

public class CosmosService
{
    private readonly CosmosClient _cosmosClient;

    public CosmosService(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<CreatorResult> Create(string databaseName)
    {
        var result = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        var creatorResult = new CreatorResult(result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);
        return creatorResult;
    }
    
    public async Task<CreatorResult> CreateContainer(string databaseName, string containerName)
    {
        var databaseResult = await Create(databaseName);
        if (!databaseResult.IsSuccessful)
        {
            return new CreatorResult(false);
        }
        var result = await _cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id");
        var creatorResult = new CreatorResult(result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);
        return creatorResult;
    }
}

public record CreatorResult(bool IsSuccessful);