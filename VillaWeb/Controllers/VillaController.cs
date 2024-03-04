using System.Runtime.InteropServices.JavaScript;
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
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Update(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
        if (obj == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }

    [HttpPost]
    public IActionResult Update(Villa.Domain.Entities.Villa obj)
    {
        if (ModelState.IsValid)
        {
            _db.Villas.Update(obj);
            _db.SaveChanges();
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Delete(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
        if (obj == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }


    [HttpPost]
    public IActionResult Delete(Villa.Domain.Entities.Villa obj)
    {
        Villa.Domain.Entities.Villa? objFromDb = _db.Villas.FirstOrDefault(u => u.Id == obj.Id);
        if (objFromDb is not null)
        {
            _db.Villas.Remove(objFromDb);
            _db.SaveChanges();
            TempData["success"] = "The villa has been deleted successfully.";

            return RedirectToAction("Index");
        }

        return View();
    }
}