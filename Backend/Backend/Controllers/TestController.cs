using Backend.Services;
using Backend.Services.DataService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : Controller
{
    private readonly IUserAuthService _userAuthService;
    private readonly TextEmbeddingService _textEmbeddingService;
    private readonly IDataService _dataService;

    public TestController(
        IUserAuthService userAuthService,
        TextEmbeddingService textEmbeddingService,
        IDataService dataService)
    {
        _userAuthService = userAuthService;
        _textEmbeddingService = textEmbeddingService;
        _dataService = dataService;
    }


    [Authorize]
    [HttpPost]
    public IActionResult AuthTest()
    {
        return Ok(new
        {
            Id = string.Join(", ", User.Claims.Select(c => $"{c.Issuer}:{c.Value}"))
        }) ;
    }

    [HttpPost("emb")]
    public async Task<IActionResult> EmbTest(string sessionId)
    {

        return Ok(
            await _textEmbeddingService.GetChuncks(_userAuthService.GetUserUuid(),sessionId )
            ) ;
    }
}