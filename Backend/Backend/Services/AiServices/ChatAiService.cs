using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Backend.Services.AiServices;

public class ChatAiService
{
    private readonly EmbeddingCacheService _embeddingCacheService;
    private static TextEmbeddingService? _textEmbeddingService;
    private readonly ISKFunction _chatFunction;

    private readonly IKernel _kernel;
    private readonly IUserAuthService _userAuthService;

    public ChatAiService(IConfiguration configuration, EmbeddingCacheService embeddingCacheService, 
        KernelService kernelService, TextEmbeddingService? textEmbeddingService, IUserAuthService userAuthService)
    {
        _embeddingCacheService = embeddingCacheService;
        _textEmbeddingService = textEmbeddingService;
        _userAuthService = userAuthService;
        _kernel = kernelService.KernelBuilder;
        _chatFunction = RegisterChatFunction(_kernel);
    }

    public async Task<string> Execute(string userQuestion, string studySessionId)
    {
        string? userId = _userAuthService.GetUserUuid();
        string memoryCollectionName = $"{userId}/{studySessionId}";
        await RefreshMemory(_kernel, userId, studySessionId, memoryCollectionName);
        string responses = await GetChatResponse(_kernel, _chatFunction, userQuestion, memoryCollectionName);

        return responses;
    }

    private async Task RefreshMemory(IKernel kernel, string? userId, string studySessionId,
        string memoryCollectionName)
    {
        if (_embeddingCacheService.TryGetEmbeddings(userId, out ISemanticTextMemory cachedEmbeddings ))
        {
            kernel.RegisterMemory(cachedEmbeddings);
            return;
        }
        IEnumerable<Chunk> chunks = await _textEmbeddingService.GetChunks(userId, studySessionId);
        foreach (Chunk chunk in chunks)
            await kernel.Memory.SaveInformationAsync(memoryCollectionName, chunk.Text, chunk.GetHashCode().ToString(),
                chunk.SourceFile);
        _embeddingCacheService.SetEmbeddings(studySessionId, kernel.Memory);
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
        and remain impartial to any bias use the context rather than making up knowledge. If the context does not include 
        information that seems at least somewhat relevant, reply with 'I am sorry but there is not enough information about this in your 
        document'.

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
        string memoryCollectionName)
    {
        SKContext kernelContext = kernel.CreateNewContext();

        string? fileContext = null;
        // IEnumerable<Chunk> chunks =
        //     await _textEmbeddingService.GetChunks("matthew_dev", "622e1e17-e1e1-4a15-8b37-a57073e12052");
        // foreach (Chunk chunk in chunks)
        //     await kernel.Memory.SaveInformationAsync(memoryCollectionName, chunk.Text, chunk.GetHashCode().ToString(),
        //         chunk.SourceFile);
        await foreach (MemoryQueryResult memory in kernel.Memory.SearchAsync(memoryCollectionName, userQuestion, 5, 0.5))
            fileContext = fileContext + Environment.NewLine + memory.Metadata.Text;

        string history = string.Empty;
        kernelContext.Variables["history"] = history;
        kernelContext.Variables["CONTEXT"] = fileContext ?? "[no context found]";
        kernelContext.Variables["QUESTION"] = userQuestion;

        return (await chatFunction.InvokeAsync(kernelContext)).Result;
    }
}