# ChromaDotNet.Aspire.Client

Registers a `ChromaClient` of [ChromaDotNet.Client](https://www.nuget.org/packages/ChromaDotNet.Client) in an Aspire service, with a health check.

```bash
dotnet add package ChromaDotNet.Aspire.Client
```

```csharp
builder.AddChromaClient("chroma");

app.MapGet("/search", async (ChromaClient chroma) =>
{
    var collection = await chroma.GetOrCreateCollection("movies");
    return await chroma.GetCollectionClient(collection)
        .Query(new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f]), nResults: 1);
});
```

The connection string is the address of the server, like `http://localhost:8000` or `Endpoint=http://localhost:8000`; the settings are read from `Aspire:Chroma:Client` (`DisableHealthChecks`, `HealthCheckTimeout`). `AddKeyedChromaClient` registers a keyed client, with its settings in `Aspire:Chroma:Client:{name}`.

This is the Chroma client integration proposed to the Community Toolkit for Aspire ([CommunityToolkit/Aspire#2219](https://github.com/CommunityToolkit/Aspire/pull/2219)), published by ChromaDotNet until the toolkit ships it as `CommunityToolkit.Aspire.Chroma`. The API and namespaces are the same, so moving to that package only changes the package reference. Not affiliated with the .NET Foundation or Chroma.
