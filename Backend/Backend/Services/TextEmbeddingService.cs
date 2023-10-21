using Backend.Services.DataService;

namespace Backend.Services;

public class TextEmbeddingService
{
    private readonly IDataService _dataService;

    public TextEmbeddingService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IEnumerable<Chunk>> GetChunks(string? userId, string studySessionId)
    {
        List<Chunk> chunks = new List<Chunk>();

        IEnumerable<UserDocument> files = await _dataService.GetSessionDocuments(userId, studySessionId);

        foreach (UserDocument file in files)
        {
            Stream stream = null;
            try
            {
                (stream, string ext) = await _dataService.GetFile(userId, studySessionId, file.id);

                EmbeddingService es = new EmbeddingService(stream, ext);

                foreach (string chunk in es.Paragraphs)
                    chunks.Add(new Chunk
                    {
                        Text = chunk,
                        SourceFile = file.FileName
                    });
            }
            catch (Exception ex)
            {
                stream?.Close();
                stream?.Dispose();
                throw;
            }
        }

        return chunks;
    }
}

public class Chunk
{
    public string Text { get; set; }
    public string SourceFile { get; set; }
}