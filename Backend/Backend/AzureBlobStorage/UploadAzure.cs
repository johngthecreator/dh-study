using Azure.Storage;
using Azure.Storage.Blobs;

namespace Backend.AzureBlobStorage;

public class UploadAzure
{
    private readonly IConfiguration _configuration;
    private readonly BlobContainerClient _blobUserContainer;

    public UploadAzure(IConfiguration configuration)
    {
        _configuration = configuration;
        BlobServiceClient blobServiceClient = new(GetSensitiveString("AzureBlobConnectionString1"));
        _blobUserContainer = blobServiceClient.GetBlobContainerClient("user-files");
    }

    private string GetSensitiveString(string sensitiveSettingsKey)
    {
        return _configuration.GetConnectionString(sensitiveSettingsKey);
    }

    public async void UploadToBlob(string userId, string sessionId, string filePath)
    {
        await _blobUserContainer.CreateIfNotExistsAsync();
        // The following also creates a blob container if it doesn't already exist
        string blobFilePath = $"{userId}/{sessionId}/{filePath}";
        BlobClient blobClient = _blobUserContainer.GetBlobClient(blobFilePath);
        await using FileStream fs = File.OpenRead(blobFilePath);
        await blobClient.UploadAsync(fs, true);
    }
}