using Villa.Application.Common.DTO;

namespace Villa.Application.Services.Interface;

public interface IDashboardService
{
    Task<RadialBarChartDto> GetTotalBookingChartData();
    Task<RadialBarChartDto> GetRegisteredUserChartData();
    Task<RadialBarChartDto> GetRevenueChartData();
    Task<PieChartDto> GetBookingPieChartData();
    Task<LineChartDto> GetMemberAndBookingLineChartData();
}