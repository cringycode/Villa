using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
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

    #region INDEX

    public IActionResult Index()
    {
        return View();
    }

    #endregion

    #region TOTAL BOOKINGS

    public async Task<IActionResult> GetTotalBookingChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll
            (u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

        var countByCurrentMonth = totalBookings.Count
            (u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);

        var countByPreviousMonth = totalBookings.Count
            (u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= currentMonthStartDate);

        return Json(GetRadialChartDataModel
            (totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));
    }

    #endregion

    #region TOTAL USERS

    public async Task<IActionResult> GetRegisteredUserChartData()
    {
        var totalUsers = _unitOfWork.AppUser.GetAll();

        var countByCurrentMonth = totalUsers.Count
            (u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);

        var countByPreviousMonth = totalUsers.Count
            (u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= currentMonthStartDate);


        return Json(GetRadialChartDataModel
            (totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
    }

    #endregion

    #region REVENUE

    public async Task<IActionResult> GetRevenueChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
                                                            || u.Status == SD.StatusCancelled);

        var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));

        var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
                                                           u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);

        var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
                                                            u.BookingDate <= currentMonthStartDate)
            .Sum(u => u.TotalCost);

        return Json(GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth));
    }

    #endregion

    #region PIE CHART

    public async Task<IActionResult> GetBookingPieChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll
        (u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
              (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

        var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId)
            .Where(x => x.Count() == 1)
            .Select(x => x.Key).ToList();

        int bookingsByNewCustomer = customerWithOneBooking.Count();
        int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

        PieChartVM pieChartVM = new()
        {
            Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
            Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
        };
        return Json(pieChartVM);
    }

    #endregion

    #region LINE CHART

    public async Task<IActionResult> GetMemberAndBookingLineChartData()
    {
        var bookingData = _unitOfWork.Booking
            .GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && u.BookingDate.Date <= DateTime.Now)
            .GroupBy(b => b.BookingDate.Date)
            .Select(u => new
            {
                DateTime = u.Key,
                NewBookingCount = u.Count()
            });

        var customerData = _unitOfWork.AppUser
            .GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) && u.CreatedAt.Date <= DateTime.Now)
            .GroupBy(b => b.CreatedAt.Date)
            .Select(u => new
            {
                DateTime = u.Key,
                NewCustomerCount = u.Count()
            });

        var leftJoin = bookingData
            .GroupJoin(customerData, booking => booking.DateTime,
                customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });


        var rightJoin = customerData
            .GroupJoin(bookingData, customer => customer.DateTime,
                booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

        var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

        var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
        var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
        var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

        List<ChartData> chartDataList = new()
        {
            new ChartData
            {
                Name = "New Bookings",
                Data = newBookingData
            },
            new ChartData
            {
                Name = "New Members",
                Data = newCustomerData
            },
        };

        LineChartVM lineChartVM = new()
        {
            Categories = categories,
            Series = chartDataList
        };

        return Json(lineChartVM);
    }

    #endregion

    #region RADIAL CHART

    private static RadialBarChartVM GetRadialChartDataModel(int totalCount, double currentMonthCount,
        double prevMonthCount)
    {
        RadialBarChartVM radialBarChartVM = new();

        int increaseDecreaseRatio = 100;

        if (prevMonthCount != 0)
        {
            increaseDecreaseRatio =
                Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
        }

        radialBarChartVM.TotalCount = totalCount;
        radialBarChartVM.CountInCurrentMonth = Convert.ToUInt32(currentMonthCount);
        radialBarChartVM.HasRatioIncreased = currentMonthCount > prevMonthCount;
        radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

        return radialBarChartVM;
    }

    #endregion
}