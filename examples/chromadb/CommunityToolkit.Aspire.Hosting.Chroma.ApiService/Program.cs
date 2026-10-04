using ChromaDB.Client;
using ChromaDB.Client.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddChromaClient("chroma");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/create", async (ChromaClient chroma) =>
{
    var collectionName = $"movies_{Guid.NewGuid():N}";
    var collection = await chroma.CreateCollection(collectionName);
    var collectionClient = chroma.GetCollectionClient(collection);

    await collectionClient.Add(
        ids: ["1", "2"],
        embeddings: [new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f]), new ReadOnlyMemory<float>([0.4f, 0.5f, 0.6f])],
        metadatas: [
            new Dictionary<string, object> { { "title", "Inception" } },
            new Dictionary<string, object> { { "title", "Interstellar" } }
        ],
        documents: ["A thief who enters the dreams of others.", "A group of explorers travel through a wormhole."]
    );

    return Results.Ok(new { Collection = collectionName, Count = await collectionClient.Count() });
});

app.MapGet("/query", async (ChromaClient chroma, string collectionName) =>
{
    var collectionClient = chroma.GetCollectionClient(await chroma.GetCollection(collectionName));

    var results = await collectionClient.Query(
        queryEmbeddings: new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f]),
        nResults: 1,
        include: ChromaQueryInclude.Metadatas | ChromaQueryInclude.Documents | ChromaQueryInclude.Distances
    );

    return Results.Ok(results.Select(r => new { r.Id, r.Document, Title = r.Metadata?["title"], r.Distance }));
});

app.Run();
