using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
