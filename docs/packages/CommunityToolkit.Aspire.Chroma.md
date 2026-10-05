[![ChromaDotNet](https://raw.githubusercontent.com/ChromaDotNet/.github/main/assets/logo-64.png)](https://chromadotnet.org)

# ChromaDotNet.Aspire.Client

Registers a `ChromaClient` of [ChromaDotNet.Client](https://www.nuget.org/packages/ChromaDotNet.Client) in an Aspire service, with a health check, traces and metrics.

Website: [chromadotnet.org](https://chromadotnet.org)

```bash
dotnet add package ChromaDotNet.Aspire.Client
```

```csharp
using ChromaDB.Client;

builder.AddChromaClient("chroma");

app.MapGet("/search", async (ChromaClient chroma) =>
{
    var collection = await chroma.GetOrCreateCollectionAsync("movies");
    return await chroma.GetCollectionClient(collection)
        .QueryAsync(new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f]), nResults: 1);
});
```

The connection string is the address of the server, like `http://localhost:8000` or `Endpoint=http://localhost:8000`. For Chroma Cloud it also takes the token, the tenant and the database: `Endpoint=https://api.trychroma.com;Token=<API key>;Tenant=<tenant>;Database=<database>`, with the token sent in the `X-Chroma-Token` header; in the AppHost, `builder.AddConnectionString("chroma")` passes it to the services that reference it. The settings are read from `Aspire:Chroma:Client` (`Endpoint`, `Token`, `Tenant`, `Database`, `DisableHealthChecks`, `HealthCheckTimeout` in milliseconds, `DisableTracing`, `DisableMetrics`): the values in the connection string win over them, and the settings given in code, with the `configureSettings` argument, win over both. The health check is named `Chroma`. `AddKeyedChromaClient` registers a keyed client, with its settings in `Aspire:Chroma:Client:{name}` and the health check `Chroma_{name}`.

This is the Chroma client integration proposed to the Community Toolkit for Aspire ([CommunityToolkit/Aspire#2219](https://github.com/CommunityToolkit/Aspire/pull/2219)), published by ChromaDotNet until the toolkit ships it as `CommunityToolkit.Aspire.Chroma`. The API and namespaces are the same, so moving to that package only changes the package reference. Community project, not affiliated with the .NET Foundation, the Community Toolkit or Chroma.
