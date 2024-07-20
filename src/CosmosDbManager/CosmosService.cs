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
    
    public async Task<CreatorResult> CreateContainers(string databaseName, string containerName, int numberOfContainers)
    {
        var databaseResult = await Create(databaseName);
        if (!databaseResult.IsSuccessful)
        {
            return new CreatorResult(false);
        }

        var isAnyFailure = false;
        for (var i = 1; i <= numberOfContainers; i++)
        {
            var result = await _cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id");
            var isSuccess = result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;
            if (!isSuccess)
            {
                isAnyFailure = true;
            }
        }

        return isAnyFailure 
            ? new CreatorResult(false) 
            : new CreatorResult(true);
    }
}

public record CreatorResult(bool IsSuccessful);