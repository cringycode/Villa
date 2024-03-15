using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

public class DashboardController : Controller
{
    #region DI

    private readonly IUnitOfWork _unitOfWork;
    static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
    readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
    readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #endregion

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetTotalBookingChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll
            (u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

        var countByCurrentMonth = totalBookings.Count
            (u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);

        var countByPreviousMonth = totalBookings.Count
            (u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= currentMonthStartDate);

        RadialBarChartVM radialBarChartVM = new();

        int increaseDecreaseRatio = 100;

        if (countByPreviousMonth != 0)
        {
            increaseDecreaseRatio =
                Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
        }

        radialBarChartVM.TotalCount = totalBookings.Count();
        radialBarChartVM.CountInCurrentMonth = countByCurrentMonth;
        radialBarChartVM.HasRatioIncreased = currentMonthStartDate > previousMonthStartDate;
        radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

        return Json(radialBarChartVM);
    }
}