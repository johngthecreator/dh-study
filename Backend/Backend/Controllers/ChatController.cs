using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Services.AiServices;

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
        const string fileName = "";
        List<string> responses = await _chatService.Execute(fileName, questionModel.Question);
        string response = responses[0];
        return Ok(new { response });
    }
}