using System.Net;
using Microsoft.Azure.Cosmos;
// ReSharper disable ConvertToPrimaryConstructor

namespace CosmosDbManager;

public class CosmosService
{
    private readonly CosmosClient _cosmosClient;

    public CosmosService(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<OperationResult> Create(string databaseName)
    {
        var result = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        var creatorResult = new OperationResult(result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);
        return creatorResult;
    }
    
    public async Task<OperationResult> CreateContainer(string databaseName, string containerName)
    {
        var databaseResult = await Create(databaseName);
        if (!databaseResult.IsSuccessful)
        {
            return new OperationResult(false);
        }
        var result = await _cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id");
        var creatorResult = new OperationResult(result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);
        return creatorResult;
    }
    
    public async Task<OperationResult> CreateContainers(string databaseName, string containerName, int numberOfContainers)
    {
        var databaseResult = await Create(databaseName);
        if (!databaseResult.IsSuccessful)
        {
            return new OperationResult(false);
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
            ? new OperationResult(false) 
            : new OperationResult(true);
    }
    
    public async Task<OperationResult> DeleteContainer(string databaseName, string containerName)
    {
        var result = await _cosmosClient.GetDatabase(databaseName).GetContainer(containerName).DeleteContainerAsync();

        var isSuccess = result.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent;
        return !isSuccess 
            ? new OperationResult(false) 
            : new OperationResult(true);
    }
}

public record OperationResult(bool IsSuccessful);