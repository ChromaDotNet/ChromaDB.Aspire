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

This is the Chroma hosting integration proposed to the Community Toolkit for Aspire ([CommunityToolkit/Aspire#2219](https://github.com/CommunityToolkit/Aspire/pull/2219)), published by ChromaDotNet until the toolkit ships it as `CommunityToolkit.Aspire.Hosting.Chroma`. The API and namespaces are the same, so moving to that package only changes the package reference. Not affiliated with the .NET Foundation or Chroma.
