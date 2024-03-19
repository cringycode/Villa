using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using VillaWeb.Models;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers
{
    public class HomeController : Controller
    {
        #region DI

        private readonly IVillaService _villaService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IVillaService villaService, IWebHostEnvironment webHostEnvironment)
        {
            _villaService = villaService;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region INDEX

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _villaService.GetAllVillas(),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVM);
        }

        #endregion

        [HttpPost]
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = _villaService.GetAvailabilityByDate(nights, checkInDate),
                Nights = nights
            };

            return PartialView("_VillaList", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}