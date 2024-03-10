using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Domain.Entities;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

[Authorize(Roles = SD.RoleAdmin)]
public class VillaNumberController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public VillaNumberController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public IActionResult Index()
    {
        var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM VillaNumberVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
        bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _unitOfWork.VillaNumber.Add(obj.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa Number has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The Villa Number Already Exists.";
        }

        obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
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
            _unitOfWork.VillaNumber.Update(villaNumberVm.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa Number has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
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
        VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get
            (u => u.Villa_Number == villaNumberVm.VillaNumber.Villa_Number);
        if (objFromDb is not null)
        {
            _unitOfWork.VillaNumber.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The villa Number has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa Number could not be deleted.";
        return View();
    }
}