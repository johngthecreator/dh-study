namespace Backend.Services.AiServices;

public interface IAiService
{
    Task<List<string>> Execute(string? memoryCollectionName, string userQuestion, string studySessionId);
}