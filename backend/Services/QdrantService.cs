using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace PdfChatbot.backend.Services;

public class QdrantService
{
    private readonly QdrantClient _client;
    private const string CollectionName = "pdf_chunks";

    public QdrantService()
    {
        // Qdrant's client library talks over gRPC on port 6334 (not the 6333 REST/dashboard port)
        _client = new QdrantClient("localhost", 6334);
    }

    public async Task EnsureCollectionExistsAsync(ulong vectorSize)
    {
        var collections = await _client.ListCollectionsAsync();

        if (collections.Contains(CollectionName))
        {
            return;
        }

        await _client.CreateCollectionAsync(
            collectionName: CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = vectorSize,
                Distance = Distance.Cosine
            }
        );
    }

    public async Task UpsertChunksAsync(List<string> chunkTexts, List<List<float>> embeddings, string fileName)
    {
        var points = new List<PointStruct>();

        for (int i = 0; i < chunkTexts.Count; i++)
        {
            var point = new PointStruct
            {
                Id = new PointId { Num = (ulong)i },
                Vectors = embeddings[i].ToArray()
            };

            point.Payload.Add("text", chunkTexts[i]);
            point.Payload.Add("fileName", fileName);
            point.Payload.Add("chunkIndex", i);

            points.Add(point);
        }

        await _client.UpsertAsync(CollectionName, points);
    }

    public async Task<List<string>> SearchSimilarChunksAsync(List<float> queryEmbedding, ulong topK = 5)
    {
        var results = await _client.SearchAsync(
            collectionName: CollectionName,
            vector: queryEmbedding.ToArray(),
            limit: topK
        );

        var chunkTexts = new List<string>();

        foreach (var result in results)
        {
            var text = result.Payload["text"].StringValue;
            chunkTexts.Add(text);
        }

        return chunkTexts;
    }

    public async Task RecreateCollectionAsync(ulong vectorSize)
    {
        var collections = await _client.ListCollectionsAsync();

        if (collections.Contains(CollectionName))
        {
            await _client.DeleteCollectionAsync(CollectionName);
        }

        await _client.CreateCollectionAsync(
            collectionName: CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = vectorSize,
                Distance = Distance.Cosine
            }
        );
    }
}