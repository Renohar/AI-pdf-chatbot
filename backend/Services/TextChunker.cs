namespace PdfChatbot.backend.Services;

public class TextChunker
{
    public List<string> ChunkText(string text, int chunkSizeInWords = 200, int overlapInWords = 40)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();

        int startIndex = 0;

        while (startIndex < words.Length)
        {
            var chunkWords = words.Skip(startIndex).Take(chunkSizeInWords);
            var chunkText = string.Join(' ', chunkWords);

            chunks.Add(chunkText);

            // Move forward, but overlap with the previous chunk
            startIndex += chunkSizeInWords - overlapInWords;
        }

        return chunks;
    }
}