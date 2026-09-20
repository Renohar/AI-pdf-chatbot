using Microsoft.AspNetCore.Mvc;
using PdfChatbot.backend.Services;

namespace PdfChatbot.backend.Controllers;

public class AskRequest
{
    public string Question { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Question cannot be empty.");
        }

        var answer = await _chatService.AskQuestionAsync(request.Question);

        return Ok(new { question = request.Question, answer });
    }
}