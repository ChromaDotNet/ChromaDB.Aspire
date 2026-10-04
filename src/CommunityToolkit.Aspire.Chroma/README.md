# CommunityToolkit.Aspire.Chroma

Provides extension methods for registering a `ChromaClient` of [ChromaDotNet.Client](https://www.nuget.org/packages/ChromaDotNet.Client), which uses the v2 API of Chroma, in a .NET application.

## Installation

```bash
dotnet add package CommunityToolkit.Aspire.Chroma
```

## Usage

In your application project:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddChromaClient("chroma");

// Or using keyed service
builder.AddKeyedChromaClient("chroma");
```

Then resolve the client in your services, and get a client for the records of a collection from it:

```csharp
public class MyService(ChromaClient chromaClient)
{
    public async Task QueryAsync()
    {
        var collection = await chromaClient.GetOrCreateCollection("movies");
        var collectionClient = chromaClient.GetCollectionClient(collection);
        var results = await collectionClient.Query(new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f]), nResults: 1);
        // ...
    }
}
```

## Configuration

The client can be configured using connection strings or settings. The connection string is the address of the server, like `http://localhost:8000` or `Endpoint=http://localhost:8000`, and the client adds the path of the v2 API:

```json
{
  "ConnectionStrings": {
    "chroma": "http://localhost:8000"
  },
  "Aspire": {
    "Chroma": {
      "DisableHealthChecks": false,
      "HealthCheckTimeout": 5000
    }
  }
}
```
