using Backend.Services.AiServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class QuestionModel
{
    public string Question { get; set; }
}

[ApiController]
public class AiToolsController : ControllerBase
{
    private readonly ChatAiService _chatAiService;
    private readonly  MultipleChoiceService _multipleChoiceService;
    private readonly FlashcardService _flashcardService;


    public AiToolsController(ChatAiService chatAiService, MultipleChoiceService multipleChoiceService, FlashcardService flashcardService)
    {
        _chatAiService = chatAiService;
        _multipleChoiceService = multipleChoiceService;
        _flashcardService = flashcardService;
    }

    [HttpPost]
    [Route("AiTools/chat")]
    public async Task<IActionResult> PostQuestion([FromBody] QuestionModel questionModel)
    {
        if (string.IsNullOrEmpty(questionModel?.Question)) return BadRequest("Question cannot be empty");
        List<string> responses = await _chatAiService.Execute("compsci", questionModel.Question, "f581f3ea-ea78-4dd0-8128-08b98bd7b0d1");
        string response = responses[0];
        return Ok(new { response });
    }
    
    [HttpPost]
    [Route("AiTools/createFlashcards")]
    public async Task<IActionResult> CreateFlashcards([FromForm] string sessionToScrape)
    {
        List<string> responses = await _flashcardService.Execute(sessionToScrape, "", "f581f3ea-ea78-4dd0-8128-08b98bd7b0d1");
        string response = responses[0];
        return Ok(new { response });
    }
}