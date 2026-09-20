using Microsoft.SemanticKernel.Embeddings;

namespace PdfChatbot.backend.Services;

#pragma warning disable SKEXP0001

public class EmbeddingService
{
    private readonly ITextEmbeddingGenerationService _embeddingGenerator;

    public EmbeddingService(ITextEmbeddingGenerationService embeddingGenerator)
    {
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<List<float>> GenerateEmbeddingAsync(string text)
    {
        var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(text);
        return embedding.ToArray().ToList();
    }
}

#pragma warning restore SKEXP0001