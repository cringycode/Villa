using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.DTO;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Application.Services.Interface;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

public class DashboardController : Controller
{
    #region DI

    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    #endregion

    #region INDEX

    public IActionResult Index()
    {
        return View();
    }

    #endregion
    
    public async Task<IActionResult> GetTotalBookingChartData()
    {
        return Json(await _dashboardService.GetTotalBookingChartData());
    }


    public async Task<IActionResult> GetRegisteredUserChartData()
    {
        return Json(await _dashboardService.GetRegisteredUserChartData());
    }


    public async Task<IActionResult> GetRevenueChartData()
    {
        return Json(await _dashboardService.GetRevenueChartData());
    }


    public async Task<IActionResult> GetBookingPieChartData()
    {
        return Json(await _dashboardService.GetBookingPieChartData());
    }

    public async Task<IActionResult> GetMemberAndBookingLineChartData()
    {
        return Json(await _dashboardService.GetMemberAndBookingLineChartData());

    }
}