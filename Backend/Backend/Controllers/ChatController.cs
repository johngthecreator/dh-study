using Backend.Services.AiServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class QuestionModel
{
    public string Question { get; set; }
}

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ChatAiService _chatAiService;

    public ChatController(ChatAiService chatAiService)
    {
        _chatAiService = chatAiService;
    }

    [HttpPost]
    public async Task<IActionResult> PostQuestion([FromBody] QuestionModel questionModel)
    {
        if (string.IsNullOrEmpty(questionModel?.Question)) return BadRequest("Question cannot be empty");
        const string fileName = "testingContainer.json";
        List<string> responses = await _chatAiService.Execute(fileName, questionModel.Question);
        string response = responses[0];
        return Ok(new { response });
    }
}