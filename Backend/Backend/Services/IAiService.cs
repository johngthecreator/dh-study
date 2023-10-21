using Microsoft.SemanticKernel;

namespace Backend.Services;

public interface IAiService
{
    Task<List<string>> Execute(string fileName, string userQuestion);
    
}