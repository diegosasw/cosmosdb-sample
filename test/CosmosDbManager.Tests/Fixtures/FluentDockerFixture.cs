using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace CosmosDbManager.Tests.Fixtures;

public class FluentDockerFixture
    : IAsyncLifetime
{
    private IContainerService? _cosmosDbService;

    private const int Port = 8081;
    
    public static string AccountEndpoint => $"https://localhost:{Port}";
    public static string AuthKeyOrResourceToken => "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    
    public Task InitializeAsync()
    {
        _cosmosDbService =
            new Builder()
                .UseContainer()
                .UseImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
                .ExposePort(Port, Port)
                .ExposePortRange(10250, 10255)
                .WithEnvironment(
                    "AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1",
                    "AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=false",
                    "AZURE_COSMOS_EMULATOR_PARTITION_COUNT=12")
                .DeleteIfExists()
                .RemoveVolumesOnDispose()
                .WaitForHttps($"https://localhost:{Port}/_explorer/emulator.pem", ignoreSslErrors: true)
                .Build();

        _ = _cosmosDbService.Start();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _cosmosDbService?.Stop();
        _cosmosDbService?.Remove();
        return Task.CompletedTask;
    }
}

internal static class ContainerBuilderExtensions
{
    public static ContainerBuilder ExposePortRange(this ContainerBuilder containerBuilder, int start, int end)
    {
        for (var port = start; port <= end; port++)
        {
            containerBuilder.ExposePort(port);
        }

        return containerBuilder;
    }
	
    public static ContainerBuilder WaitForHttps(
        this ContainerBuilder builder, 
        string url, 
        bool ignoreSslErrors = false,
        int retries = 40, 
        int delayMilliseconds = 5000)
    {
        return builder.Wait("Wait for Https", (_, count) =>
        {
            if (count > retries)
            {
                var secondsWaited = count * (delayMilliseconds / 1000);
                throw new Exception($"Failed to wait for {url} after {secondsWaited} seconds");
            }

            var httpClientHandler =
                ignoreSslErrors
                    ? new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true }
                    : new HttpClientHandler();

            using var client = new HttpClient(httpClientHandler);
            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            Thread.Sleep(delayMilliseconds);
            return 1;
        });
    }
}