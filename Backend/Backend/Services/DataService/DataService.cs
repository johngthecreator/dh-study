namespace Backend.Services.DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using Microsoft.Azure.Cosmos;

public class DataService : IDataService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _filesContainer;
    private readonly Container _sessionsContainer;

    public DataService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobConnectionString1"));
        _cosmosClient = new CosmosClient(configuration.GetConnectionString("CosmosDb"));
        var database = _cosmosClient.GetDatabase("userDataMap");
        _filesContainer = database.GetContainer("sessionFiles");
        _sessionsContainer = database.GetContainer("studySessions");
    }

    public async Task UploadFile(string fileName, string studySessionId, string userId, Stream fileStream)
    {
        string containerName = "data"; // It could be a more general name or specified in the configuration
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        // Include userId in the blob's path
        var blobClient = blobContainerClient.GetBlobClient($"{userId}/{studySessionId}/content/{fileName}");

        await blobClient.UploadAsync(fileStream, true);
        fileStream.Close();

        var document = new UserDocument
        {
            id = Guid.NewGuid().ToString(),
            FileName = fileName,
            FileId = blobClient.Uri.AbsoluteUri,
            SessionId = studySessionId,
            UserId = userId
        };

        await _filesContainer.CreateItemAsync(document, new PartitionKey(studySessionId));
    }

    public async Task<string> CreateStudySession(string studySessionName, string userId)
    {
        var id = Guid.NewGuid().ToString();

        var studySession = new StudySession
        {
            Name = studySessionName,
            id = Guid.NewGuid().ToString(),
            UserId = userId
        };

        await _sessionsContainer.CreateItemAsync(studySession, new PartitionKey(userId));

        return id;
    }

    public async Task<IEnumerable<StudySession>> GetStudySessions(string userId)
    {
        var sqlQueryText = $"SELECT * FROM c WHERE c.userId = @userId";
        var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@userId", userId);
        var queryResultSetIterator = _sessionsContainer.GetItemQueryIterator<StudySession>(queryDefinition);

        List<StudySession> studySessions = new List<StudySession>();

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (var session in currentResultSet)
            {
                studySessions.Add(session);
            }
        }

        return studySessions;
    }

    public async Task<IEnumerable<UserDocument>> GetSessionDocuments(string userId, string studySessionId)
    {
        var sqlQueryText = $"SELECT * FROM c WHERE c.studySessionId = @sessionId";
        var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@sessionId", studySessionId);
        var queryResultSetIterator = _filesContainer.GetItemQueryIterator<UserDocument>(queryDefinition);

        List<UserDocument> documents = new List<UserDocument>();

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (var document in currentResultSet)
            {
                documents.Add(document);
            }
        }

        return documents;
    }

    public async Task<(Stream File, string FileType)> GetFile(string userId, string studySessionId, string fileId)
    {
        string containerName = "data"; // It could be a more general name or specified in the configuration
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Include userId in the blob's path
        var blobClient = blobContainerClient.GetBlobClient($"{userId}/{studySessionId}/content/{fileId}");

        var response = await blobClient.DownloadAsync();
        var metadata = blobClient.GetPropertiesAsync().Result.Value.Metadata;

        string fileType = metadata["fileType"]; // assuming you have stored file type in metadata

        var memoryStream = new MemoryStream();
        await response.Value.Content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return (memoryStream, fileType);
    }
}
