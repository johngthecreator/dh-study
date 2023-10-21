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
        if (string.IsNullOrEmpty(questionModel?.Question)) 
            return BadRequest("Question cannot be empty");
        string response = await _chatAiService.Execute(questionModel.Question, "622e1e17-e1e1-4a15-8b37-a57073e12052");
        return Ok(new { response });
    }
    
    [HttpPost]
    [Route("AiTools/createFlashcards")]
    public async Task<IActionResult> CreateFlashcards([FromForm] string? studySessionId)
    {
        List<string> responses = await _flashcardService.Execute("622e1e17-e1e1-4a15-8b37-a57073e12052");
        string response = responses[0];
        return Ok(new { response });
    }
    
    [HttpPost]
    [Route("AiTools/createMultipleChioce")]
    public async Task<IActionResult> CreateMultipleChoice([FromForm] string? studySessionId)
    {
        List<string> responses = await _multipleChoiceService.Execute("622e1e17-e1e1-4a15-8b37-a57073e12052");
        string response = responses[0];
        return Ok(new { response });
    }
}