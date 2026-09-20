using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;
using PdfChatbot.backend.Services;

namespace PdfChatbot.backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{

    private readonly TextChunker _textChunker;
    private readonly EmbeddingService _embeddingService;
    private readonly QdrantService _qdrantService;

    public PdfController(TextChunker textChunker, EmbeddingService embeddingService, QdrantService qdrantService)
    {
        _textChunker = textChunker;
        _embeddingService = embeddingService;
        _qdrantService = qdrantService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadPdf(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var stream = file.OpenReadStream();
        using var pdf = PdfDocument.Open(stream);

        var extractedText = "";

        foreach (var page in pdf.GetPages())
        {
            extractedText += page.Text + "\n";
        }

        var chunks = _textChunker.ChunkText(extractedText);
        // var firstChunkEmbedding = await _embeddingService.GenerateEmbeddingAsync(chunks[0]);

         var embeddings = new List<List<float>>();

        for (int i = 0; i < chunks.Count; i++)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(chunks[i]);
            embeddings.Add(embedding);

            // Log progress every 50 chunks so we can see it's working, not stuck
            if (i % 50 == 0)
            {
                Console.WriteLine($"Embedded chunk {i} of {chunks.Count}");
            }
        }

        // await _qdrantService.EnsureCollectionExistsAsync(vectorSize: (ulong)embeddings[0].Count);
        await _qdrantService.RecreateCollectionAsync(vectorSize: (ulong)embeddings[0].Count);
        await _qdrantService.UpsertChunksAsync(chunks, embeddings, file.FileName);

        return Ok(new
        {
            fileName = file.FileName,
            pageCount = pdf.NumberOfPages,
            // textPreview = extractedText.Length > 500
            //     ? extractedText.Substring(0, 500) + "..."
            //     : extractedText
            totalChunks = chunks.Count,
            // firstChunkPreview = chunks.Count > 0 ? chunks[0] : "",
            // secondChunkPreview = chunks.Count > 1 ? chunks[1] : ""
            // embeddingLength = firstChunkEmbedding.Count,
            // embeddingPreview = firstChunkEmbedding.Take(5).ToList()
            message = "PDF processed and stored in Qdrant successfully."
        });
    }
}