using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

public class QuestionModel
{
    public string Question { get; set; }
}

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> PostQuestion([FromBody] QuestionModel questionModel)
    {
        if (string.IsNullOrEmpty(questionModel?.Question)) return BadRequest("Question cannot be empty");
        string fileName = "";
        string response = await _chatService.Chat(fileName, questionModel.Question);
        return Ok(new { response });
    }
}