using CosmosDbManager;
using Microsoft.Azure.Cosmos;

const string accountEndpoint = "https://localhost:8081";
const string authKeyOrResourceToken = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

var cosmosClientOptions =
    new CosmosClientOptions
    {
        // Only needed if SSL not installed
        // HttpClientFactory = () =>
        // {
        //     var httpMessageHandler =
        //         new HttpClientHandler
        //         {
        //             ServerCertificateCustomValidationCallback =
        //                 HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //         };
        //
        //     return new HttpClient(httpMessageHandler);
        // },
        ConnectionMode = ConnectionMode.Gateway
    };

var cosmosClient =
    new CosmosClient(
        accountEndpoint,
        authKeyOrResourceToken,
        cosmosClientOptions);

try
{
    var databaseCreator = new DatabaseCreatorTestContainer(cosmosClient);
    var result = await databaseCreator.Create("foo");
    Console.WriteLine(
        result.IsSuccessful 
            ? "Database successfully created" 
            : "Failed to create database");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Unexpected error creating database. {exception.Message}");
    throw;
}