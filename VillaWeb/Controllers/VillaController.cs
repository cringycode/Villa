using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

[Authorize(Roles = SD.RoleAdmin)]
public class VillaController : Controller
{
    #region DI

    private readonly IVillaService _villaService;

    public VillaController(IVillaService villaService)
    {
        _villaService = villaService;
    }

    #endregion

    public IActionResult Index()
    {
        var villas = _villaService.GetAllVillas();
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
            TempData["success"] = "The villa has been updated successfully.";
            _villaService.CreateVilla(obj);
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Update(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _villaService.GetVillaById(villaId);
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
            _villaService.UpdateVilla(obj);
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Delete(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _villaService.GetVillaById(villaId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }


    [HttpPost]
    public IActionResult Delete(Villa.Domain.Entities.Villa obj)
    {
        bool deleted = _villaService.DeleteVilla(obj.Id);
        if (deleted)
        {
            TempData["success"] = "The villa has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = "Failed to delete the villa..";
        }

        return View();
    }
}