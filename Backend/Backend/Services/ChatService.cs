using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace Backend.Services;

public class ChatService : BaseAiService
{

    public ChatService(IConfiguration configuration, UploadAzure uploadAzure, EmbeddingCacheService embeddingCacheService, 
        UserAuthService userAuthService, KernelService kernelService) : base(configuration, uploadAzure,
        embeddingCacheService, userAuthService, kernelService)
    {
        _kernel = kernelService.KernelBuilder;
        _chatFunction = RegisterChatFunction(_kernel);
    }

    public override async Task<List<string>> Execute(string fileName, string userQuestion)
    {
        await RefreshMemory(_kernel, fileName);
        return new List<string>() { await GetChatResponse(_kernel, _chatFunction, userQuestion, fileName) };
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
        
        PromptTemplateConfig promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0.4,
                TopP = 0.65,
            }
        };

        PromptTemplate promptTemplate = new PromptTemplate(skPrompt, promptConfig, kernel);
        SemanticFunctionConfig functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

        // Register the semantic function itself, params: (plugin name, function name, function config)
        return kernel.RegisterSemanticFunction("AskPdfQuestion", "AskAway", functionConfig);
    }

    private static async Task<string> GetChatResponse(IKernel kernel, ISKFunction chatFunction, string userQuestion, string fileName)
    {
        SKContext kernelContext = kernel.CreateNewContext();

        string? fileContext = null;
        await foreach(MemoryQueryResult memory in kernel.Memory.SearchAsync(fileName, userQuestion, limit: 5, minRelevanceScore: 0.5))
        {
            fileContext = fileContext + Environment.NewLine + memory.Metadata.Text;
        }

        string history = string.Empty;
        kernelContext.Variables["history"] = history;
        kernelContext.Variables["CONTEXT"] = fileContext ?? "[no context found]";
        kernelContext.Variables["QUESTION"] = userQuestion;

        return (await chatFunction.InvokeAsync(kernelContext)).Result;
    }
    
    private readonly IKernel _kernel;
    private readonly ISKFunction _chatFunction;
}
