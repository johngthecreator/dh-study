using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : Controller
{
    [Authorize]
    [HttpPost]
    public IActionResult AuthTest()
    {
        return Ok(new
        {
            Id = string.Join(", ", User.Claims.Select(c => $"{c.Issuer}:{c.Value}"))
        }) ;
    }
}