using Backend.AzureBlobStorage;
using Backend.Services.DataService;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class FlashcardService : BaseAiService
{
    private readonly IConfiguration _configuration;
    private readonly EmbeddingCacheService _embeddingCacheService;
    private ISKFunction _flashcardFunction;

    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly IUserAuthService _userAuthService;
    private readonly IDataService _dataService;

    public FlashcardService(IConfiguration configuration, EmbeddingCacheService embeddingCacheService, KernelService kernelService, 
        TextEmbeddingService textEmbeddingService, IUserAuthService userAuthService, IDataService dataService) :
        base(configuration,
            embeddingCacheService, kernelService, textEmbeddingService, userAuthService)
    {
        _userAuthService = userAuthService;
        _dataService = dataService;
        _kernel = kernelService.KernelBuilder;
    }

    private static ISKFunction RegisterFlashcardFunction(IKernel kernel)
    {
        const string skPrompt = """
Please make 30 comprehensive and detailed flashcards based on the most important terms to be found in the following information. Each question should have four options, with only one being the correct answer. Format the flashcards in JSON, where each term is the key and the definition is the value. Send only the JSON response; NO OTHER TEXT
{
  "term": "definition",
  "term": "definition",
  ...
}

```INFORMATION
{{$INFORMATION}}
```
""";

        PromptTemplateConfig promptConfig = new()
        {
            Completion =
            {
                MaxTokens = 5000,
                Temperature = 0.4,
                TopP = 0.65
            }
        };

        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("CreateFlashcards", "ImportantInfoFlashcards", functionConfig);
    }


    public override async Task<List<string>> Execute(string fileName, string fileId, string studySessionId)
    {
        _flashcardFunction = RegisterFlashcardFunction(_kernel);
        List<string>? paragraphs = await GetFlashcardString(_userAuthService.GetUserUuid(), studySessionId, fileId);
        return await GetFlashcards(_kernel, _flashcardFunction, paragraphs);    
    }

    private async Task<List<string>?> GetFlashcardString(string userId, string studySessionId, string fileId)
    {
        studySessionId = "f581f3ea-ea78-4dd0-8128-08b98bd7b0d1";
        userId = "matthew_dev";
        fileId = "b5d949f6-e24c-487e-b5f8-591836472f56";
        (Stream stream, string ext) = await _dataService.GetFile(userId, studySessionId, fileId);

        EmbeddingService es = new EmbeddingService(stream, ".pdf");
        
        return es.Paragraphs;
    }
    
    private static async Task<List<string>> GetFlashcards(IKernel kernel, ISKFunction flashcardsFunction, List<string> paragraphs)
    {
        SKContext kernelContext = kernel.CreateNewContext();


        List<string> response = new();
        // foreach (string section in paragraphs)
        // {
            kernelContext.Variables["INFORMATION"] = paragraphs.FirstOrDefault();
            response.Add((await flashcardsFunction.InvokeAsync(kernelContext)).Result);
        // }

        return response;
    }

    private static IEnumerable<string> SplitByLength(string str, int maxLength)
    {
        for (int index = 0; index < str.Length; index += maxLength)
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
    }

}