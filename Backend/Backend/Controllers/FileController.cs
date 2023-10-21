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
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        string tempFilePath = Guid.NewGuid() + Path.GetExtension(file.FileName); 
        await using (Stream stream = new FileStream(tempFilePath, FileMode.Create))
        {
            EmbeddingService embeddingService = new EmbeddingService(stream, file.FileName);
            string json = JsonSerializer.Serialize(embeddingService.Paragraphs);
            using MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            _uploadAzure.UploadStream(jsonStream);
        }
        await _uploadAzure.UploadToCloud("sebDunn", "compsci", tempFilePath, file.FileName);
        System.IO.File.Delete(tempFilePath);
        
        return Ok("uploaded");
    }

    [HttpPost("upload2")]
    public async Task<IActionResult> Upload2(IFormFile file, string sessionId, string userId)
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