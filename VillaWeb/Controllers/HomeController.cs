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
        #region DE

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region INDEX

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVM);
        }

        #endregion

        [HttpPost]
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
            var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll
                    (u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn)
                .ToList();

            foreach (var villa in villaList)
            {
                int roomAvailable = SD.VillaRoomsAvailable_Count
                    (villa.Id, villaNumberList, checkInDate, nights, bookedVillas);

                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
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