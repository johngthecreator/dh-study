using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class FlashcardService : BaseAiService
{
    private readonly IConfiguration _configuration;
    private readonly EmbeddingCacheService _embeddingCacheService;
    private readonly ISKFunction _flashcardFunction;

    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly ISKFunction _multipleChoiceFunction;
    private readonly IUserAuthService _userAuthService;

    public FlashcardService(IConfiguration configuration,
        EmbeddingCacheService embeddingCacheService,
        KernelService kernelService, TextEmbeddingService textEmbeddingService, IUserAuthService userAuthService) :
        base(configuration,
            embeddingCacheService, kernelService, textEmbeddingService, userAuthService)
    {
        _userAuthService = userAuthService;
        IKernel kernelServiceKernelBuilder = _kernel;
        _kernel = kernelService.KernelBuilder;
        _flashcardFunction = RegisterFlashcardFunction(_kernel);
    }

    private static ISKFunction RegisterFlashcardFunction(IKernel kernel)
    {
        const string skPrompt = """
Please make comprehensive and detailed multiple-choice test questions based on the most important parts of the following information. Each question should have four options, with only one being the correct answer. Format the questions in JSON, where each question includes four nested answers. Send only the JSON response; NO OTHER TEXT
{
  \"questions\": [
        {
            \"question\": \"...\",
                \"options\": [
                \"...\",
                \"...\",
                \"...\",
                \"...\"
                ],
                \"answer\": \"...\"
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


    public override Task<List<string>> Execute(string fileName, string userQuestion, string studySessionId)
    {
        throw new NotImplementedException();
    }
}