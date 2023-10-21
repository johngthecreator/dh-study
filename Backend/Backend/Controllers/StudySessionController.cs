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
    private readonly UploadAzure _uploadAzure;

    public StudySessionController(
        IDataService dataService,
        IUserAuthService userAuthService,
        UploadAzure uploadAzure)
    {
        _dataService = dataService;
        _userAuthService = userAuthService;
        _uploadAzure = uploadAzure;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromForm] List<IFormFile> files, [FromForm] string sessionName)
    {

        var studySessionId  = await _dataService.CreateStudySession(sessionName, _userAuthService.GetUserUuid());

        foreach (var file in files)
        {
            using var stream = file.OpenReadStream();

            await _dataService.UploadFile(file.FileName, studySessionId, _userAuthService.GetUserUuid(), stream);
        }

        return Ok();
    }
}
