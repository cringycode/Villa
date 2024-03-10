using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Infrastructure.Data;

namespace VillaWeb.Controllers;

[Authorize(Roles = SD.RoleAdmin)]
public class VillaController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
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
            if (obj.Image is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageUrl = @"\images\VillaImage\" + fileName;
            }
            else
            {
                obj.ImageUrl = "https://placehold.co/600x400";
            }

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
            if (obj.Image is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                if (!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageUrl = @"\images\VillaImage\" + fileName;
            }
            
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
            if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            
            _unitOfWork.Villa.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The villa has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View();
    }
}