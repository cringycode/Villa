using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;

namespace VillaWeb.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
    {
        Booking booking = new()
        {
            VillaId = villaId,
            Villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights),
        };
        booking.TotalCost = booking.Villa.Price * nights;

        return View(booking);
    }
}