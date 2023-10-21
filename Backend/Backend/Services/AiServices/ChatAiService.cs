using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class ChatAiService : BaseAiService
{
    private readonly ISKFunction _chatFunction;

    private readonly IKernel _kernel;
    private readonly IUserAuthService _userAuthService;

    public ChatAiService(IConfiguration configuration, UploadAzure uploadAzure,
        EmbeddingCacheService embeddingCacheService,
        KernelService kernelService, TextEmbeddingService textEmbeddingService, IUserAuthService userAuthService) :
        base(configuration, uploadAzure,
            embeddingCacheService, kernelService, textEmbeddingService, userAuthService)
    {
        _userAuthService = userAuthService;
        _kernel = kernelService.KernelBuilder;
        _chatFunction = RegisterChatFunction(_kernel);
    }

    public override async Task<List<string>> Execute(string fileName, string userQuestion, string studySessionId)
    {
        //TODO
        string userId = "";
        await RefreshMemory(_kernel, userId, studySessionId);
        return new List<string> { await GetChatResponse(_kernel, _chatFunction, userQuestion, fileName) };
    }

    private static ISKFunction RegisterChatFunction(IKernel kernel)
    {
        const string skPrompt = @"
        Here is your chat history, ONLY USE IF A USER ASKS YOU A QUESTION DIRECTLY ABOUT YOUR CHAT WITH THEM. IT IS SORTED
        CHRONOLOGICALLY WITH MOST RECENT ON TOP.
        ```History
         {{$HISTORY}}
        ```
        
        Use the following context to answer the question ```{{$QUESTION}}```. Answer as concise and brief as possible, 
        and remain impartial to any bias. Stick to facts and only use the context. If the context does not include 
        information that seems relevant, reply with 'I am sorry but there is not enough information about this in your 
        document'

        ``` CONTEXT
        {{$CONTEXT}}
        ```";

        PromptTemplateConfig promptConfig = new()
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0.4,
                TopP = 0.65
            }
        };

        PromptTemplate promptTemplate = new(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("AskPdfQuestion", "AskAway", functionConfig);
    }

    private static async Task<string> GetChatResponse(IKernel kernel, ISKFunction chatFunction, string userQuestion,
        string fileName)
    {
        SKContext kernelContext = kernel.CreateNewContext();

        string? fileContext = null;
        await foreach (MemoryQueryResult memory in kernel.Memory.SearchAsync(fileName, userQuestion, 5, 0.5))
            fileContext = fileContext + Environment.NewLine + memory.Metadata.Text;

        string history = string.Empty;
        kernelContext.Variables["history"] = history;
        kernelContext.Variables["CONTEXT"] = fileContext ?? "[no context found]";
        kernelContext.Variables["QUESTION"] = userQuestion;

        return (await chatFunction.InvokeAsync(kernelContext)).Result;
    }
}