using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Application.Services.Interface;
using Villa.Domain.Entities;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

[Authorize(Roles = SD.RoleAdmin)]
public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;

    public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
    }

    public IActionResult Index()
    {
        var villaNumbers = _villaNumberService.GetAllVillaNumbers();
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM VillaNumberVM = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };

        return View(VillaNumberVM);
    }

    [HttpPost]
    public IActionResult Create(VillaNumberVM obj)
    {
        bool roomNumberExists = _villaNumberService.CheckVillaNumberExists(obj.VillaNumber.Villa_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _villaNumberService.CreateVillaNumber(obj.VillaNumber);
            TempData["success"] = "The villa Number has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The Villa Number Already Exists.";
        }

        obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });


        return View(obj);
    }

    public IActionResult Update(int villaNumberId)
    {
        VillaNumberVM villaNumberVM = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
        };
        if (villaNumberVM.VillaNumber is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(villaNumberVM);
    }


    [HttpPost]
    public IActionResult Update(VillaNumberVM villaNumberVm)
    {
        if (ModelState.IsValid)
        {
            _villaNumberService.UpdateVillaNumber(villaNumberVm.VillaNumber);
            TempData["success"] = "The villa Number has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        return View(villaNumberVm);
    }


    public IActionResult Delete(int villaNumberId)
    {
        VillaNumberVM villaNumberVM = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
        };

        if (villaNumberVM.VillaNumber is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(villaNumberVM);
    }


    [HttpPost]
    public IActionResult Delete(VillaNumberVM villaNumberVm)
    {
        VillaNumber? objFromDb = _villaNumberService.GetVillaNumberById(villaNumberVm.VillaNumber.Villa_Number);
        if (objFromDb is not null)
        {
            _villaNumberService.DeleteVillaNumber(objFromDb.Villa_Number);
            TempData["success"] = "The villa Number has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa Number could not be deleted.";
        return View();
    }
}