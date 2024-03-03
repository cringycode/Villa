using Microsoft.AspNetCore.Mvc;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

public class VillaController : Controller
{
    private readonly AppDbContext _db;

    public VillaController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var villas = _db.Villas.ToList();
        return View(villas);
    }
}