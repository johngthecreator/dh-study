using Backend.AzureBlobStorage;
using Backend.Services.DataService;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        _flashcardFunction = RegisterFlashcardFunction(_kernel);
    }

    private static ISKFunction RegisterFlashcardFunction(IKernel kernel)
    {
        const string skPrompt = """
Please make 30 comprehensive and detailed flashcards based on the most important terms to be found in the following information. Each question should have four options, with only one being the correct answer. 
Format the flashcards in JSON, where each term is the key and the definition is the value. Make sure the return format is perfect JSON so I can parse the response directly into c# string -> json parsing; NO OTHER TEXT
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


    public override async Task<List<string>> Execute(string memoryCollectionName, string fileId, string studySessionId)
    {
        List<string>? paragraphs = await GetFlashcardString(_userAuthService.GetUserUuid(), studySessionId, fileId);
        List<string> results = await GetFlashcards(_kernel, _flashcardFunction, paragraphs);

        for (int i = 0; i < results.Count; i++)
        {
            string unescaped = System.Text.RegularExpressions.Regex.Unescape(results[i]);
            if (IsValidJson(unescaped))
            {
                results[i] = unescaped;
            }
            else
            {
                Console.WriteLine("The string is not a valid JSON.");
            }
        }


        return results;
    }
    
    public static bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For an object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) // For an array
        {
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                // Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) // Some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private async Task<List<string>?> GetFlashcardString(string userId, string studySessionId, string fileId)
    {
        studySessionId = "622e1e17-e1e1-4a15-8b37-a57073e12052";
        userId = "matthew_dev";
        fileId = "eab67f93-61b4-4590-96e8-de1eea919959";
        (Stream stream, string ext) = await _dataService.GetFile(userId, studySessionId, fileId);

        EmbeddingService es = new EmbeddingService(stream, ".txt");
        
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

}