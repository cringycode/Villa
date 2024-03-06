using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;
using VillaWeb.ViewModels;

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
        var villaNumbers = _db.VillaNumbers.Include(u => u.Villa).ToList();
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM VillaNumberVM = new()
        {
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
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
        bool roomNumberExists = _db.VillaNumbers.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _db.VillaNumbers.Add(obj.VillaNumber);
            _db.SaveChanges();
            TempData["success"] = "The villa Number has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The Villa Number Already Exists.";
            obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

        return View(obj);
    }

    public IActionResult Update(int villaNumberId)
    {
        VillaNumberVM villaNumberVM = new()
        {
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
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
            _db.VillaNumbers.Update(villaNumberVm.VillaNumber);
            _db.SaveChanges();
            TempData["success"] = "The villa Number has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
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
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
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
        VillaNumber? objFromDb = _db.VillaNumbers.FirstOrDefault
            (u => u.Villa_Number == villaNumberVm.VillaNumber.Villa_Number);
        if (objFromDb is not null)
        {
            _db.VillaNumbers.Remove(objFromDb);
            _db.SaveChanges();
            TempData["success"] = "The villa Number has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa Number could not be deleted.";
        return View();
    }
}