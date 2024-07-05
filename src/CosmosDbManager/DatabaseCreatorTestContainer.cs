using System.Net;
using Microsoft.Azure.Cosmos;
// ReSharper disable ConvertToPrimaryConstructor

namespace CosmosDbManager;

public class DatabaseCreatorTestContainer
{
    private readonly CosmosClient _cosmosClient;

    public DatabaseCreatorTestContainer(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<DatabaseCreatorResult> Create(string databaseName)
    {
        var result = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        var databaseCreatorResult = new DatabaseCreatorResult(result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);
        return databaseCreatorResult;
    }
}

public record DatabaseCreatorResult(bool IsSuccessful);