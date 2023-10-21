using Backend.Services;
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
    private readonly MaterialCacheService _materialCacheService;


    public AiToolsController(ChatAiService chatAiService,
        MaterialCacheService materialCacheService)
    {
        _chatAiService = chatAiService;
        _materialCacheService = materialCacheService;
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
        return Ok(await _materialCacheService.GetFlashCards(studySessionId));
    }

    [HttpPost]
    [Route("AiTools/createMultipleChioce")]
    public async Task<IActionResult> CreateMultipleChoice(string? studySessionId)
    {
        return Ok(await _materialCacheService.GetQuiz(studySessionId));
    }
}