using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;

namespace PdfChatbot.backend.Services;

#pragma warning disable SKEXP0001

public class ChatService
{
    private readonly ITextEmbeddingGenerationService _embeddingGenerator;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly QdrantService _qdrantService;

    public ChatService(
        ITextEmbeddingGenerationService embeddingGenerator,
        IChatCompletionService chatCompletionService,
        QdrantService qdrantService)
    {
        _embeddingGenerator = embeddingGenerator;
        _chatCompletionService = chatCompletionService;
        _qdrantService = qdrantService;
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        // Step 1: Embed the question using the same model we used for chunks
        var questionEmbedding = await _embeddingGenerator.GenerateEmbeddingAsync(question);

        // Step 2: Search Qdrant for the most relevant chunks
        var relevantChunks = await _qdrantService.SearchSimilarChunksAsync(questionEmbedding.ToArray().ToList());

        // Step 3: Build a prompt combining the retrieved context and the question
        var context = string.Join("\n\n", relevantChunks);

        var prompt = $"""
            Answer the question using ONLY the context below. If the answer is not in the context, say "I don't have enough information to answer that."

            Context:
            {context}

            Question:
            {question}
            """;

        // Step 4: Send the prompt to llama3.2 and get the answer back
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);

        return response.Content ?? "No response generated.";
    }
}

#pragma warning restore SKEXP0001