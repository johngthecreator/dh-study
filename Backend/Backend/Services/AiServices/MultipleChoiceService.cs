using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class MultipleChoiceService : BaseAiService
{
    private readonly IKernel _kernel;
    private readonly ISKFunction _multipleChoiceFunction;
    private readonly IUserAuthService _userAuthService;

    public MultipleChoiceService(IConfiguration configuration,
        EmbeddingCacheService embeddingCacheService,
        KernelService kernelService, TextEmbeddingService textEmbeddingService, IUserAuthService userAuthService) :
        base(configuration,
            embeddingCacheService, kernelService, textEmbeddingService, userAuthService)
    {
        _userAuthService = userAuthService;
        _kernel = kernelService.KernelBuilder;
        _multipleChoiceFunction = RegisterMultiplechoiceFunction(_kernel);
    }

    private static ISKFunction RegisterMultiplechoiceFunction(IKernel kernel)
    {
        const string skPrompt = """
Please make comprehensive and detailed multiple-choice test questions based on the most important parts of the following information. Each question should have four options, with only one being the correct answer. Format the questions in JSON, where each question includes four nested answers. Send only the JSON response; NO OTHER TEXT
{
  "questions": "answer"
        {
            "question": "...,"
                "options": [
                "...",
                "...",
                "...",
                "..."
                ],
                "answer": "..."
        },
        {...}
        ]
    }
```INFORMATION
{{@INFORMATION}}
```
""";

        PromptTemplateConfig promptConfig = new()
        {
            Completion =
            {
                MaxTokens = 20000,
                Temperature = 0.4,
                TopP = 0.65
            }
        };

        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("CreateMultipleChoices", "ImportantInfoMultipleChoices", functionConfig);
    }

    public override async Task<List<string>> Execute(string fileName, string fileForContext, string studySessionId)
    {
        await RefreshMemory(_kernel, fileName, "");
        return await GetMultipleChoiceResponse(_kernel, _multipleChoiceFunction, fileForContext);
    }

    private static async Task<List<string>> GetMultipleChoiceResponse(IKernel kernel, ISKFunction multiplechoiceFunction, string fileContext)
    {
        SKContext kernelContext = kernel.CreateNewContext();

        IEnumerable<string> paragraphs = SplitByLength(fileContext, 80000);

        List<string> response = new();
        foreach (string section in paragraphs)
        {
            kernelContext.Variables["INFORMATION"] = fileContext;
            response.Add((await multiplechoiceFunction.InvokeAsync(kernelContext)).Result);
        }

        return response;
    }

    private static IEnumerable<string> SplitByLength(string str, int maxLength)
    {
        for (int index = 0; index < str.Length; index += maxLength)
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
    }
}