using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

public class VillaController : Controller
{
    private readonly IVillaRepo _villaRepo;

    public VillaController(IVillaRepo villaRepo)
    {
        _villaRepo = villaRepo;
    }

    public IActionResult Index()
    {
        var villas = _villaRepo.GetAll();
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
            _villaRepo.Add(obj);
            _villaRepo.Save();
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Update(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _villaRepo.Get(u => u.Id == villaId);
        if (obj is null)
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
            _villaRepo.Update(obj);
            _villaRepo.Save();
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Delete(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _villaRepo.Get(u => u.Id == villaId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }


    [HttpPost]
    public IActionResult Delete(Villa.Domain.Entities.Villa obj)
    {
        Villa.Domain.Entities.Villa? objFromDb = _villaRepo.Get(u => u.Id == obj.Id);
        if (objFromDb is not null)
        {
            _villaRepo.Remove(objFromDb);
            _villaRepo.Save();
            TempData["success"] = "The villa has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }
}