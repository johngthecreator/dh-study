using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Newtonsoft.Json;

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

    public async Task UploadToCloud(string userId, string sessionId, string localFilePath, string actualFileName)
    {
        // The following creates a blob item if it doesn't already exist
        string blobFilePath = $"{userId}/{sessionId}/Content/{actualFileName}";
        BlobClient blobClient = _blobUserContainer.GetBlobClient(blobFilePath);

        if (await UploadFileContent(blobClient, localFilePath, userId, sessionId))
        {
            await AddFileToMetadata(userId, sessionId, blobClient, actualFileName);
        }
    }

    private static Dictionary<string, string> SetTags(string userId, string sessionId)
    {
        return new Dictionary<string, string>
            {
                { "userId", userId },
                { "sessionId", sessionId },
                { "uploadDate", DateTime.Now.ToShortDateString() }
            };
    }

    private async Task AddFileToMetadata(string userId, string sessionId, BlobClient fileBlob, string fileName)
    {
        try
        {
            await UpdateSessionMetaData(sessionId, userId, fileBlob, fileName);
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
    }

    private static async Task<bool> UploadFileContent(BlobClient blobClient, string localFilePath, string userId, string sessionId)
    {
        try
        {
            await using FileStream fs = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(fs, true);
            await blobClient.SetTagsAsync(SetTags(userId, sessionId));
            return true;
        }
        catch
        {
            return false;
        }
        
    }

    private async Task UpdateSessionMetaData(string sessionId, string userId, BlobBaseClient fileBlob, string fileName)
    {
        BlobClient? metadataBlobClient = _blobUserContainer.GetBlobClient($"{userId}/{sessionId}/metadata.json");
        SessionMetadata sessionMetadata = new SessionMetadata();

        try
        {
            Response<BlobDownloadInfo>? response = await metadataBlobClient.DownloadAsync();
            using StreamReader reader = new StreamReader(response.Value.Content);
            string existingMetadataJson = await reader.ReadToEndAsync();
            sessionMetadata = JsonConvert.DeserializeObject<SessionMetadata>(existingMetadataJson);
        }
        catch (Exception e)
        {
            //TODO: Add throw can't find metadata file
            Console.WriteLine(e);
            metadataBlobClient = _blobUserContainer.GetBlobClient($"{userId}/{sessionId}/metadata.json");
        }
        

        // Get blob info and update metadata
        Response<GetBlobTagResult>? tags = await fileBlob.GetTagsAsync();
        sessionMetadata.Files[fileName] = tags.Value.Tags["uploadDate"];

            // Serialize updated metadata to JSON
        string updatedMetadataJson = JsonConvert.SerializeObject(sessionMetadata, Formatting.Indented);

        // Upload updated metadata to blob
        using MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(updatedMetadataJson));
        await metadataBlobClient.UploadAsync(stream, overwrite: true);
    }
}