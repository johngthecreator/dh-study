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
        //TODO: ADD 
        List<string> responses = await _chatAiService.Execute("compsci", questionModel.Question, "622e1e17-e1e1-4a15-8b37-a57073e12052");
        string response = responses[0];
        return Ok(new { response });
    }
    
    [HttpPost]
    [Route("AiTools/createFlashcards")]
    public async Task<IActionResult> CreateFlashcards([FromForm] string sessionToScrape)
    {
        List<string> responses = await _flashcardService.Execute(sessionToScrape, "", "622e1e17-e1e1-4a15-8b37-a57073e12052");
        string response = responses[0];
        return Ok(new { response });
    }
}