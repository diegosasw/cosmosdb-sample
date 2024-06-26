using CosmosDbSample;
using Microsoft.Azure.Cosmos;

const string accountEndpoint = "https://localhost:8081";
const string authKeyOrResourceToken = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

var cosmosClient =
    new CosmosClient(
        accountEndpoint,
        authKeyOrResourceToken);


var sample = new Sample(cosmosClient);

try
{
    Console.WriteLine("It reaches this");
    await sample.CreateDatabase("foo");
    Console.WriteLine("It never reaches this");
    await sample.CreateContainer("foo", "bar");
}
catch (Exception exception)
{
    Console.WriteLine("It never reaches this");
}