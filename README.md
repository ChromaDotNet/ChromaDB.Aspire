[![ChromaDotNet](https://raw.githubusercontent.com/ChromaDotNet/.github/main/assets/logo-64.png)](https://chromadotnet.org)

# ChromaDB.Aspire

[Aspire](https://aspire.dev) integrations for [Chroma](https://www.trychroma.com/):

| Package | Use |
|---|---|
| `ChromaDotNet.Aspire.Hosting` | AppHost: `AddChroma`, `WithDataVolume`, `WithDataBindMount`. Image `chromadb/chroma:1.5.9`, health check on `/api/v2/heartbeat`. |
| `ChromaDotNet.Aspire.Client` | Service: `AddChromaClient`, `AddKeyedChromaClient`, with [ChromaDB.Client](https://github.com/ChromaDotNet/ChromaDB.Client), a health check, traces and metrics. Connects to a Chroma server or to Chroma Cloud (`Endpoint=https://api.trychroma.com;Token=...;Tenant=...;Database=...`). |

```csharp
// AppHost
var chroma = builder.AddChroma("chroma").WithDataVolume();
builder.AddProject<Projects.ApiService>("api").WithReference(chroma).WaitFor(chroma);

// Service
builder.AddChromaClient("chroma");
```

Website: [chromadotnet.org](https://chromadotnet.org)

## Why this repository

The code is the Chroma integration proposed to the Aspire Community Toolkit in [CommunityToolkit/Aspire#2219](https://github.com/CommunityToolkit/Aspire/pull/2219), which continues [#1132](https://github.com/CommunityToolkit/Aspire/pull/1132) by [ali-Hamza817](https://github.com/ali-Hamza817). This repository publishes it until the toolkit ships `CommunityToolkit.Aspire.Hosting.Chroma` and `CommunityToolkit.Aspire.Chroma`. Then this repository will be archived, and the packages deprecated in favor of those. The assemblies, namespaces and API are the same as in the toolkit, so moving changes only the package references.

Every change to the integrations goes first into that pull request. `eng/sync-from-toolkit.sh` then copies the Chroma folders (`src`, `tests`, `examples/chromadb`) here. So the README.md in each `src` folder is the one proposed to the toolkit, with its package ids, and the READMEs of the ChromaDotNet.Aspire packages are in `docs/packages`.

The build and test files come from CommunityToolkit/Aspire, adapted to publish the ChromaDotNet.Aspire packages. `docs/packages`, the CI and `eng` belong to this repository.

This is a community project. It is not affiliated with or endorsed by the .NET Foundation, the Aspire Community Toolkit or Chroma.

## Building and testing

```bash
dotnet build ChromaDB.Aspire.slnx
dotnet test --project tests/CommunityToolkit.Aspire.Chroma.Tests
dotnet test --project tests/CommunityToolkit.Aspire.Hosting.Chroma.Tests
```

The hosting tests start Chroma in Docker. The TypeScript AppHost test also needs the Aspire CLI, Node.js and PowerShell.

## License

MIT, as CommunityToolkit/Aspire: see [LICENSE](LICENSE).
