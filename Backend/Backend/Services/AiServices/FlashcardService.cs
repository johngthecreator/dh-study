using System.Collections.Concurrent;

using Backend.AzureBlobStorage;
using Backend.Services.DataService;
using System.Text.RegularExpressions;


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
    private ISKFunction _destupidFunction;
    private ISKFunction _deDupeFunction;

    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly TextEmbeddingService _textEmbeddingService;
    private readonly IUserAuthService _userAuthService;

    public FlashcardService(IConfiguration configuration, KernelService kernelService,
        IUserAuthService userAuthService, TextEmbeddingService textEmbeddingService)
    {
        _configuration = configuration;
        _userAuthService = userAuthService;
        _textEmbeddingService = textEmbeddingService;
        _kernel = kernelService.KernelBuilder;
        _flashcardFunction = RegisterFlashcardFunction(_kernel);
        _deDupeFunction = DeDupeFunction(_kernel);
    }

    private static ISKFunction RegisterFlashcardFunction(IKernel kernel)
    {
        const string skPrompt = @"
Come up with study flashcards for important terms to be found in the following information. Redundant or filler information should not be included (things like name, date, assignment info, ect.). 
Return a json object, where each term is the key and the definition is the value. Make sure the return format is perfect JSON; NO OTHER TEXT
If nothing seems like it might be useful in a study scenario, just return a blank object.
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
                TopP = 0.1
            }
        };

        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        return kernel.RegisterSemanticFunction("CreateFlashcards", "ImportantInfoFlashcards", functionConfig);
    }


    private static ISKFunction CreateDestupidFunction(IKernel kernel)
    {
        const string skPrompt = @"
The following json contains term and definition flash cards. DON'T INCLUDE THINGS THAT SEEM TO BE SPECIFIC TO A PARTICULAR ASSIGNMENT OR PROJECT! Remove cards that are too specific to be useful for test study, cards that are common sense, and cards that are too broad. The result should only contain cards that are useful for a human studying for a test. ONLY RETURN JSON.

```FLASHCARDS
{{$FLASHCARDS}}
```
";

        PromptTemplateConfig promptConfig = new()
        {
            Completion =
            {
                MaxTokens = 5000,
                Temperature = 0.4,
                TopP = 0.1
            }
        };



        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("DestupidFlashCards", "CleanFlashCards", functionConfig);
    }

        private static ISKFunction DeDupeFunction(IKernel kernel)
    {
        const string skPrompt = @"
The following json contains term and definition flash cards. Some flash cards may be similar. Remove the similar ones. With an overview of the scope of the cards, remove ones that are too specific and not useful to the general subject matter. Things that are specific to a particular project or assignment should be removed. REUTRN ONLY JSON
```
{{$JOINED}}
```
";

        PromptTemplateConfig promptConfig = new()
        {
            Completion =
            {
                MaxTokens = 5000,
                Temperature = 0.2,
                TopP = 0.1
            }
        };

        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("DedupeFlashCards", "DedupeFlashCards", functionConfig);
    }

    public async Task<string> Execute(string studySessionId)
    {
        List<string>? paragraphs = await GetFlashcardString(_userAuthService.GetUserUuid(), studySessionId);
        return await GetFlashcards(_kernel, _flashcardFunction, _destupidFunction, paragraphs);
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
    
    private async Task<string> GetFlashcards(IKernel kernel, ISKFunction flashcardsFunction, ISKFunction deStupidFunction, List<string> paragraphs)
    {
        ConcurrentBag<string> response = new();

        var tasks = paragraphs.Select(async paragraph =>
    {
        SKContext kernelContext = kernel.CreateNewContext();

            kernelContext.Variables["INFORMATION"] = paragraph;  // Changed to use `paragraph` instead of `paragraphs.FirstOrDefault()`
            var cards = (await flashcardsFunction.InvokeAsync(kernelContext)).Result;

            //kernelContext.Variables["FLASHCARDS"] = cards;

            //var dresponse = (await deStupidFunction.InvokeAsync(kernelContext)).Result;
            response.Add(cards);
        });

        await Task.WhenAll(tasks);
        var merged = MergeJsonObjects(response);

        var kernelContext = kernel.CreateNewContext();
        kernelContext.Variables["JOINED"] = merged;

        return (await _deDupeFunction.InvokeAsync(kernelContext)).Result;
    }

    static string MergeJsonObjects(IEnumerable<string> jsonStrings)
    {
        JObject mergedObject = new JObject();

        foreach (string jsonString in jsonStrings)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            mergedObject.Merge(jsonObject, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
        }

        return mergedObject.ToString();
    }

}