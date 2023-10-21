using System.Text;
using System.Text.Json;
using Backend.AzureBlobStorage;
using Backend.Services.DataService;

using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class FileController : ControllerBase
{
    private readonly UploadAzure _uploadAzure;
    private readonly IDataService _dataService;

    public FileController(UploadAzure uploadAzure, IDataService dataService)
    {
        _uploadAzure = uploadAzure;
        _dataService = dataService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string sessionId, string userId)
    {
        using var stream = file.OpenReadStream();
        
        await _dataService.UploadFile(file.FileName, sessionId, userId, stream);

        return Ok();
    }

    [HttpPost("makesession")]
    public async Task<IActionResult> MakeSession([FromForm] string sessionName, [FromForm] string userId)
    {
        await _dataService.CreateStudySession(sessionName, userId);

        return Ok();
    }
}