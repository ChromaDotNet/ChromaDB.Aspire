using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ChromaDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace CommunityToolkit.Aspire.Chroma.Tests;

public class ChromaClientExtensionsTests
{
    private const string DefaultConnectionName = "chroma";
    private const string DefaultConnectionString = "http://localhost:8000";

    [Fact]
    public void AddChromaClient_ShouldRegisterClient()
    {
        var builder = CreateBuilder();

        builder.AddChromaClient(DefaultConnectionName);

        using var host = builder.Build();

        var client = host.Services.GetService<ChromaClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public void AddKeyedChromaClient_ShouldRegisterKeyedClient()
    {
        var builder = CreateBuilder();

        builder.AddKeyedChromaClient(DefaultConnectionName);

        using var host = builder.Build();

        var client = host.Services.GetKeyedService<ChromaClient>(DefaultConnectionName);
        Assert.NotNull(client);
    }

    [Fact]
    public async Task AddChromaClient_HealthCheckShouldBeRegistered()
    {
        var builder = CreateBuilder();

        builder.AddChromaClient(DefaultConnectionName);

        using var host = builder.Build();

        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var healthCheckReport = await healthCheckService.CheckHealthAsync();
        Assert.Contains(healthCheckReport.Entries, x => x.Key == DefaultConnectionName);
    }

    [Fact]
    public async Task AddKeyedChromaClient_HealthCheckShouldBeRegisteredWithSuffix()
    {
        var builder = CreateBuilder();

        builder.AddKeyedChromaClient(DefaultConnectionName);

        using var host = builder.Build();

        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var healthCheckReport = await healthCheckService.CheckHealthAsync();
        Assert.Contains(healthCheckReport.Entries, x => x.Key == $"{DefaultConnectionName}_check");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AddChromaClient_WorksWithoutAnHttpClientFactoryRegisteredByTheApp(bool useKeyed)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"ConnectionStrings:{DefaultConnectionName}", DefaultConnectionString)
        ]);

        if (useKeyed)
        {
            builder.AddKeyedChromaClient(DefaultConnectionName);
        }
        else
        {
            builder.AddChromaClient(DefaultConnectionName);
        }

        using var host = builder.Build();

        var client = useKeyed
            ? host.Services.GetKeyedService<ChromaClient>(DefaultConnectionName)
            : host.Services.GetService<ChromaClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public async Task AddChromaClient_HealthCheckHonorsTheTimeout()
    {
        // A server that accepts the connection and never answers.
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var accept = listener.AcceptTcpClientAsync();
        var endpoint = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}";

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"ConnectionStrings:{DefaultConnectionName}", endpoint)
        ]);
        builder.AddChromaClient(DefaultConnectionName, settings => settings.HealthCheckTimeout = 500);

        using var host = builder.Build();

        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var stopwatch = Stopwatch.StartNew();
        var report = await healthCheckService.CheckHealthAsync(TestContext.Current.CancellationToken);
        stopwatch.Stop();

        Assert.Equal(HealthStatus.Unhealthy, report.Entries[DefaultConnectionName].Status);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"The health check took {stopwatch.Elapsed}.");
    }

    private static HostApplicationBuilder CreateBuilder()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"ConnectionStrings:{DefaultConnectionName}", DefaultConnectionString)
        ]);
        builder.Services.AddHttpClient();
        return builder;
    }
}
