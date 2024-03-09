using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

public class AmenityController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AmenityController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public IActionResult Index()
    {
        var Amenitys = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
        return View(Amenitys);
    }

    public IActionResult Create()
    {
        AmenityVM AmenityVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };

        return View(AmenityVM);
    }

    [HttpPost]
    public IActionResult Create(AmenityVM obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Add(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been created successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });


        return View(obj);
    }

    public IActionResult Update(int AmenityId)
    {
        AmenityVM AmenityVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _unitOfWork.Amenity.Get(u => u.Id == AmenityId)
        };

        if (AmenityVM.Amenity is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(AmenityVM);
    }


    [HttpPost]
    public IActionResult Update(AmenityVM AmenityVm)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Update(AmenityVm.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        AmenityVm.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        return View(AmenityVm);
    }


    public IActionResult Delete(int AmenityId)
    {
        AmenityVM AmenityVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _unitOfWork.Amenity.Get(u => u.Id == AmenityId)
        };

        if (AmenityVM.Amenity is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(AmenityVM);
    }


    [HttpPost]
    public IActionResult Delete(AmenityVM AmenityVm)
    {
        Amenity? objFromDb = _unitOfWork.Amenity.Get
            (u => u.Id == AmenityVm.Amenity.Id);
        if (objFromDb is not null)
        {
            _unitOfWork.Amenity.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The Amenity could not be deleted.";
        return View();
    }
}