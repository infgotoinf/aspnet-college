using Microsoft.AspNetCore.Mvc;

namespace asp3.Controllers;

public class CatalogController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
