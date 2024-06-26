using Microsoft.Azure.Cosmos;

const string accountEndpoint = "https://localhost:8081";
const string authKeyOrResourceToken = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

// IRRELEVANT
// var cosmosClientOptions =
//     new CosmosClientOptions
//     {
//         HttpClientFactory = () =>
//         {
//             var httpMessageHandler =
//                 new HttpClientHandler
//                 {
//                     ServerCertificateCustomValidationCallback =
//                         HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//                 };
//
//             return new HttpClient(httpMessageHandler);
//         },
//         ConnectionMode = ConnectionMode.Gateway
//     };

var cosmosClient =
    new CosmosClient(
        accountEndpoint,
        authKeyOrResourceToken);
        //,cosmosClientOptions);

try
{
    Console.WriteLine("It reaches this");
    await cosmosClient.CreateDatabaseIfNotExistsAsync("foo");
    Console.WriteLine("It never reaches this");
    await cosmosClient.GetDatabase("foo").CreateContainerIfNotExistsAsync("bar", "/mypartitionkey");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"It does not reach this {exception.Message}");
}