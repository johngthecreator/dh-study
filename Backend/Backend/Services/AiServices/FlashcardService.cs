using Backend.AzureBlobStorage;
using Backend.Services.DataService;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Services.AiServices;

public class FlashcardService 
{
    private readonly IConfiguration _configuration;
    private readonly EmbeddingCacheService _embeddingCacheService;
    private ISKFunction _flashcardFunction;

    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly IUserAuthService _userAuthService;
    private readonly TextEmbeddingService _textEmbeddingService;

    public FlashcardService(IConfiguration configuration, KernelService kernelService,
        IUserAuthService userAuthService, TextEmbeddingService textEmbeddingService)
    {
        _configuration = configuration;
        _userAuthService = userAuthService;
        _textEmbeddingService = textEmbeddingService;
        _kernel = kernelService.KernelBuilder;
        _flashcardFunction = RegisterFlashcardFunction(_kernel);
    }

    private static ISKFunction RegisterFlashcardFunction(IKernel kernel)
    {
        const string skPrompt = @"
Please make 30 comprehensive and detailed flashcards based on the most important terms to be found in the following information. Each question should have four options, with only one being the correct answer. 
Format the flashcards in JSON, where each term is the key and the definition is the value. Make sure the return format is perfect JSON so I can parse the response directly into c# string -> json parsing; NO OTHER TEXT
{
  ""term"": ""definition"",
  ""term"": ""definition"",
  ...
}

```INFORMATION
{{$INFORMATION}}
```
";

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

        return kernel.RegisterSemanticFunction("CreateFlashcards", "ImportantInfoFlashcards", functionConfig);
    }


    public async Task<List<string>> Execute(string studySessionId)
    {
        List<string>? paragraphs = await GetFlashcardString(_userAuthService.GetUserUuid(), studySessionId);
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

    private async Task<List<string>> GetFlashcardString(string? userId, string studySessionId)
    {
        IEnumerable<Chunk> chunks = await _textEmbeddingService.GetChunks(userId, studySessionId);

        return chunks.Select(c => c.Text).ToList();;
    }
    
    private static async Task<List<string>> GetFlashcards(IKernel kernel, ISKFunction flashcardsFunction, List<string> contextFile)
    {
        SKContext kernelContext = kernel.CreateNewContext();


        List<string> response = new();
        foreach (string section in contextFile)
        {
            kernelContext.Variables["INFORMATION"] = contextFile.FirstOrDefault();
            response.Add((await flashcardsFunction.InvokeAsync(kernelContext)).Result);
        }

        return response;
    }

}