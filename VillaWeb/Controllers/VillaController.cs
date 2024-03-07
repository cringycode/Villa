using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

public class VillaController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public VillaController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var villas = _unitOfWork.Villa.GetAll();
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
            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Update(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
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
            _unitOfWork.Villa.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "The villa has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Delete(int villaId)
    {
        Villa.Domain.Entities.Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }


    [HttpPost]
    public IActionResult Delete(Villa.Domain.Entities.Villa obj)
    {
        Villa.Domain.Entities.Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == obj.Id);
        if (objFromDb is not null)
        {
            _unitOfWork.Villa.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The villa has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }
}