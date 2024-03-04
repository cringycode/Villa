using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Villa.Domain.Entities.Villa obj)
    {
        if (ModelState.IsValid)
        {
            _db.Villas.Add(obj);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View();
    }
}