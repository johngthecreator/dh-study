using Backend.Services.DataService;

namespace Backend.Services;

public class TextEmbeddingService
{
    private readonly IDataService _dataService;

    public TextEmbeddingService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IEnumerable<Chunck>> GetChuncks(string userId, string studySessionId)
    {
        var chuncks = new List<Chunck>();

        var files = await _dataService.GetSessionDocuments(userId, studySessionId);

        foreach (var file in files)
        {
            Stream stream = null;
            try
            {
                (stream, string ext) = await _dataService.GetFile(userId, studySessionId, file.id);

                var es = new EmbeddingService(stream, ext);

                foreach (var chunck in es.Paragraphs)
                {
                    chuncks.Add(new Chunck
                    {
                        Text = chunck,
                        SourceFile = file.FileName
                    });
                }

            }
            catch (Exception ex)
            {
                stream?.Close();
                stream?.Dispose();
                throw;
            }
        }

        return chuncks;
    }
}

public class Chunck
{
    public string Text { get; set; }
    public string SourceFile { get; set; }
}
