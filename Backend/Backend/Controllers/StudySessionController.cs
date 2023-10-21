using Backend.AzureBlobStorage;
using Backend.Services;
using Backend.Services.DataService;

using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class StudySessionController : Controller
{
    private readonly IDataService _dataService;
    private readonly IUserAuthService _userAuthService;

    public StudySessionController(
        IDataService dataService,
        IUserAuthService userAuthService)
    {
        _dataService = dataService;
        _userAuthService = userAuthService;
    }

    [HttpPost("makesession")]
    public async Task<IActionResult> CreateSession([FromForm] List<IFormFile> files, [FromForm] string sessionName)
    {

        var studySessionId = await _dataService.CreateStudySession(sessionName, _userAuthService.GetUserUuid());

        foreach (var file in files)
        {
            using var stream = file.OpenReadStream();

            await _dataService.UploadFile(file.FileName, studySessionId, _userAuthService.GetUserUuid(), stream);
        }

        return Ok(studySessionId);
    }

    [HttpPost("getsessions")]
    public async Task<ActionResult<IEnumerable<StudySession>>> GetSessions()
    {
        return Ok(await _dataService.GetStudySessions(_userAuthService.GetUserUuid()));
    }

    [HttpPost("addfile")]
    public async Task<IActionResult> AddFile([FromForm] IFormFile formFile, string sessionId)
    {
        using var stream = formFile.OpenReadStream();

        await _dataService.UploadFile(formFile.FileName, sessionId, _userAuthService.GetUserUuid(), stream);

        return Ok();
    }

    [HttpPost("getfiles")]
    public async Task<ActionResult<IEnumerable<UserDocument>>> GetFiles(string sessionId){
        var files = await _dataService.GetSessionDocuments(_userAuthService.GetUserUuid(), sessionId);

        return Ok(files);
    }
}
