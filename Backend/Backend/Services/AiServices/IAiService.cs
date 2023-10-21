namespace Backend.Services.AiServices;

public interface IAiService
{
    Task<List<string>> Execute(string fileName, string userQuestion, string studySessionId);
}