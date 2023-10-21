using Backend.AzureBlobStorage;
using Backend.Services.DataService;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class MultipleChoiceService 
{
    private readonly IKernel _kernel;
    private readonly ISKFunction _multipleChoiceFunction;
    private readonly IConfiguration _configuration;
    private readonly TextEmbeddingService _textEmbeddingService;
    private readonly IUserAuthService _userAuthService;

    public MultipleChoiceService(IConfiguration configuration,
        KernelService kernelService, TextEmbeddingService textEmbeddingService, IUserAuthService userAuthService)
    {
        _configuration = configuration;
        _textEmbeddingService = textEmbeddingService;
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

        return kernel.RegisterSemanticFunction("CreateMultipleChoices", "ImportantInfoMultipleChoices", functionConfig);
    }

    public async Task<List<string>> Execute(string studySessionId)
    {
        IEnumerable<Chunk> chunks = await _textEmbeddingService.GetChunks(_userAuthService.GetUserUuid(), studySessionId);

        return await GetMultipleChoiceResponse(_kernel, _multipleChoiceFunction, chunks.Select(c => c.Text).ToList());
    }

    private static async Task<List<string>> GetMultipleChoiceResponse(IKernel kernel, ISKFunction multiplechoiceFunction, List<string> fileContext)
    {
        SKContext kernelContext = kernel.CreateNewContext();


        List<string> response = new();
        foreach (string section in fileContext)
        {
            kernelContext.Variables["INFORMATION"] = section;
            response.Add((await multiplechoiceFunction.InvokeAsync(kernelContext)).Result);
        }

        return response;
    }
}