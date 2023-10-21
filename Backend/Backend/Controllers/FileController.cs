using Backend.AzureBlobStorage;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class FileController : ControllerBase
{
    private readonly UploadAzure _uploadAzure;
    
    public FileController(UploadAzure uploadAzure)
    {
        _uploadAzure = uploadAzure;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        string tempFilePath = Path.GetTempFileName();
        await using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        _uploadAzure.UploadToBlob("sebDunn", "compsci", tempFilePath);
        System.IO.File.Delete(tempFilePath);
        
        return Ok("uploaded");
    }
}