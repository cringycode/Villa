using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Application.Services.Interface;
using Villa.Domain.Entities;

namespace VillaWeb.Controllers;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IVillaService _villaService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IVillaNumberService _villaNumberService;

    public BookingController(IBookingService bookingService,
        IVillaService villaService, IVillaNumberService villaNumberService,
        IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
    {
        _userManager = userManager;
        _villaService = villaService;
        _villaNumberService = villaNumberService;
        _bookingService = bookingService;
        _webHostEnvironment = webHostEnvironment;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        AppUser user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

        Booking booking = new()
        {
            VillaId = villaId,
            Villa = _villaService.GetVillaById(villaId),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights),
            UserId = userId,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Name = user.Name
        };
        booking.TotalCost = booking.Villa.Price * nights;
        return View(booking);
    }

    [Authorize]
    [HttpPost]
    public IActionResult FinalizeBooking(Booking booking)
    {
        var villa = _villaService.GetVillaById(booking.VillaId);
        booking.TotalCost = villa.Price * booking.Nights;

        booking.Status = SD.StatusPending;
        booking.BookingDate = DateTime.Now;


        if (!_villaService.IsVillaAvailableByDate(villa.Id, booking.Nights, booking.CheckInDate))
        {
            TempData["error"] = "Room has been sold out!";
            //no rooms available
            return RedirectToAction(nameof(FinalizeBooking), new
            {
                villaId = booking.VillaId,
                checkInDate = booking.CheckInDate,
                nights = booking.Nights
            });
        }


        _bookingService.CreateBooking(booking);

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = domain +
                        $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
        };


        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = villa.Name
                    //Images = new List<string> { domain + villa.ImageUrl },
                },
            },
            Quantity = 1,
        });


        var service = new SessionService();
        Session session = service.Create(options);

        _bookingService.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    [Authorize]
    public IActionResult BookingConfirmation(int bookingId)
    {
        Booking bookingFromDb = _bookingService.GetBookingById(bookingId);

        if (bookingFromDb.Status == SD.StatusPending)
        {
            //this is a pending order, we need to confirm if payment was successful

            var service = new SessionService();
            Session session = service.Get(bookingFromDb.StripeSessionId);

            if (session.PaymentStatus == "paid")
            {
                _bookingService.UpdateStatus(bookingFromDb.Id, SD.StatusApproved, 0);
                _bookingService.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);
            }
        }

        return View(bookingId);
    }

    [Authorize]
    public IActionResult BookingDetails(int bookingId)
    {
        Booking bookingFromDb = _bookingService.GetBookingById(bookingId);

        if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
        {
            var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);

            bookingFromDb.VillaNumbers = _villaNumberService.GetAllVillaNumbers().Where(u =>
                u.VillaId == bookingFromDb.VillaId
                && availableVillaNumber.Any(x => x == u.Villa_Number)).ToList();
        }

        return View(bookingFromDb);
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin)]
    public IActionResult CheckIn(Booking booking)
    {
        _bookingService.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
        TempData["Success"] = "Booking Updated Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin)]
    public IActionResult CheckOut(Booking booking)
    {
        _bookingService.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
        TempData["Success"] = "Booking Completed Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin)]
    public IActionResult CancelBooking(Booking booking)
    {
        _bookingService.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
        TempData["Success"] = "Booking Cancelled Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }


    private List<int> AssignAvailableVillaNumberByVilla(int villaId)
    {
        List<int> availableVillaNumbers = new();

        var villaNumbers = _villaNumberService.GetAllVillaNumbers().Where(u => u.VillaId == villaId);

        var checkedInVilla = _bookingService.GetCheckedInVillaNumbers(villaId);

        foreach (var villaNumber in villaNumbers)
        {
            if (!checkedInVilla.Contains(villaNumber.Villa_Number))
            {
                availableVillaNumbers.Add(villaNumber.Villa_Number);
            }
        }

        return availableVillaNumbers;
    }


    #region API Calls

    [HttpGet]
    [Authorize]
    public IActionResult GetAll(string status)
    {
        IEnumerable<Booking> objBookings;
        string userId = "";
        if (string.IsNullOrEmpty(status))
        {
            status = "";
        }

        if (!User.IsInRole(SD.RoleAdmin))
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        objBookings = _bookingService.GetAllBookings(userId, status);

        return Json(new { data = objBookings });
    }

    #endregion
}