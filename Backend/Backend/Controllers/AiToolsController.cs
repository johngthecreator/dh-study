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
    private readonly FlashcardService _flashcardService;
    private readonly MultipleChoiceService _multipleChoiceService;


    public AiToolsController(ChatAiService chatAiService, MultipleChoiceService multipleChoiceService,
        FlashcardService flashcardService)
    {
        _chatAiService = chatAiService;
        _multipleChoiceService = multipleChoiceService;
        _flashcardService = flashcardService;
    }

    [HttpPost]
    [Route("AiTools/chat")]
    public async Task<IActionResult> PostQuestion(QuestionModel questionModel, string studySessionId)
    {
        if (string.IsNullOrEmpty(questionModel?.Question))
            return BadRequest("Question cannot be empty");
        string response = await _chatAiService.Execute(questionModel.Question, studySessionId);
        return Ok(new { response });
    }

    [HttpPost]
    [Route("AiTools/createFlashcards")]
    public async Task<IActionResult> CreateFlashcards(string? studySessionId)
    {
        var response = await _flashcardService.Execute(studySessionId);
        return Ok(response);
    }

    [HttpPost]
    [Route("AiTools/createMultipleChioce")]
    public async Task<IActionResult> CreateMultipleChoice(string? studySessionId)
    {
        string responses = await _multipleChoiceService.Execute(studySessionId);
        return Ok(responses);
    }
}