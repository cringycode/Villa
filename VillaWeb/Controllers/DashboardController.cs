using Microsoft.AspNetCore.Mvc;

namespace VillaWeb.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}