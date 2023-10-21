using Backend.AzureBlobStorage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Backend.Services;

public abstract class BaseAiService : IAiService
{
    private const string _gptModel = "";

    private static readonly string _apiKey = "";
    private readonly IConfiguration _configuration;
    private readonly EmbeddingCacheService _embeddingCacheService;
    private readonly IKernel _kernel;
    private readonly KernelService _kernelService;
    private readonly UploadAzure _uploadAzure;
    private readonly UserAuthService _userAuthService;
    private string _getApiKey = _apiKey;

    protected BaseAiService(IConfiguration configuration, UploadAzure uploadAzure,
        EmbeddingCacheService embeddingCacheService,
        UserAuthService userAuthService, KernelService kernelService)
    {
        _configuration = configuration;
        _uploadAzure = uploadAzure;
        _embeddingCacheService = embeddingCacheService;
        _userAuthService = userAuthService;

        _kernel = kernelService.KernelBuilder;
    }
    
    protected async Task RefreshMemory(IKernel kernel, string fileName)
    {
        string? userUuid = _userAuthService.GetUserUuid();

        if (_embeddingCacheService.TryGetEmbeddings(userUuid, out ISemanticTextMemory cachedEmbeddings))
        {
            // Use cachedEmbeddings if available
            kernel.RegisterMemory(cachedEmbeddings);
            return;
        }
        IAsyncEnumerable<List<string>> paragraphs = _uploadAzure.DownloadParagraphEmbeddings("testingContainer.json");
        await foreach (List<string> paragraphList in paragraphs)
        {
            foreach (string paragraph in paragraphList)
            {
                await kernel.Memory.SaveInformationAsync(fileName, paragraph, paragraph.GetHashCode().ToString());
            }
        }

        _embeddingCacheService.SetEmbeddings(userUuid, kernel.Memory);
    }

    public abstract Task<List<string>> Execute(string fileName, string userQuestion);
}