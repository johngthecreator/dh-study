using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel;

namespace Backend.Services.AiServices;

public abstract class BaseAiService : IAiService
{
    private const string _gptModel = "";

    private static readonly string _apiKey = "";
    private readonly IConfiguration _configuration;
    private readonly EmbeddingCacheService _embeddingCacheService;
    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly IUserAuthService _userAuthService;
    private string _getApiKey = _apiKey;
    private readonly TextEmbeddingService _textEmbeddingService;

    protected BaseAiService(IConfiguration configuration,
        EmbeddingCacheService embeddingCacheService, KernelService kernelService,
        TextEmbeddingService textEmbeddingService,
        IUserAuthService userAuthService)
    {
        _configuration = configuration;
        _embeddingCacheService = embeddingCacheService;
        _kernelService = kernelService;
        _textEmbeddingService = textEmbeddingService;
        _userAuthService = userAuthService;

        _kernel = kernelService.KernelBuilder;
    }

    public abstract Task<List<string>> Execute(string fileName, string userQuestion, string studySessionId);

    /// <summary>
    ///     This is only for chataiservice right now
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="userId"></param>
    /// <param name="studySessionId"></param>
    protected async Task RefreshMemory(IKernel kernel, string userId, string studySessionId)
    {
        IEnumerable<Chunk> chunks = await _textEmbeddingService.GetChunks(userId, studySessionId);
        const string memoryCollectionName = "matthew_dev/f581f3ea-ea78-4dd0-8128-08b98bd7b0d1";
        foreach (Chunk chunk in chunks)
            await kernel.Memory.SaveInformationAsync(memoryCollectionName, chunk.Text, chunk.GetHashCode().ToString(),
                chunk.SourceFile);
    }
}