using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

public class VillaNumberController : Controller
{
    private readonly AppDbContext _db;

    public VillaNumberController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var villaNumbers = _db.VillaNumbers.ToList();
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View();
    }

    [HttpPost]
    public IActionResult Create(VillaNumber obj)
    {
        if (ModelState.IsValid)
        {
            _db.VillaNumbers.Add(obj);
            _db.SaveChanges();
            TempData["success"] = "The villa Number has been created successfully.";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Update(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
        //Villa? obj = _db.Villas.Find(villaId);
        //var VillaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
        if (obj == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }


    [HttpPost]
    public IActionResult Update(Villa.Domain.Entities.Villa obj)
    {
        if (ModelState.IsValid && obj.Id > 0)
        {
            _db.Villas.Update(obj);
            _db.SaveChanges();
            TempData["success"] = "The villa has been updated successfully.";
            return RedirectToAction("Index");
        }

        return View();
    }


    public IActionResult Delete(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
        if (obj is null)
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

        TempData["error"] = "The villa could not be deleted.";
        return View();
    }
}