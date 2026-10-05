# ChromaDotNet.Aspire.Hosting

Adds a [Chroma](https://www.trychroma.com/) container to an Aspire AppHost: the `chromadb/chroma:1.5.9` image, with a health check on `/api/v2/heartbeat`.

```bash
dotnet add package ChromaDotNet.Aspire.Hosting
```

```csharp
var chroma = builder.AddChroma("chroma")
    .WithDataVolume();

builder.AddProject<Projects.ApiService>("api")
    .WithReference(chroma)
    .WaitFor(chroma);
```

`WithDataVolume` and `WithDataBindMount` mount `/data`, where Chroma keeps its database.

`AddChroma` takes an optional `port`, the host port of the container; without it the port is assigned at run time. The resource gives the connection string `Endpoint=http://{host}:{port}` and the connection properties `Host`, `Port` and `Uri`. The methods are exported to TypeScript AppHosts too (`addChroma`, `withDataVolume`, `withDataBindMount`), as in [the example](https://github.com/ChromaDotNet/ChromaDB.Aspire/blob/main/examples/chromadb/CommunityToolkit.Aspire.Hosting.Chroma.AppHost.TypeScript/apphost.mts).

This is the Chroma hosting integration proposed to the Community Toolkit for Aspire ([CommunityToolkit/Aspire#2219](https://github.com/CommunityToolkit/Aspire/pull/2219)), published by ChromaDotNet until the toolkit ships it as `CommunityToolkit.Aspire.Hosting.Chroma`. The API and namespaces are the same, so moving to that package only changes the package reference. Community project, not affiliated with the .NET Foundation or Chroma.
