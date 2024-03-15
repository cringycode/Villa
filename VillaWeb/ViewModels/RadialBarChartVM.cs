namespace VillaWeb.ViewModels;

public class RadialBarChartVM
{
    public decimal TotalCount { get; set; }
    public decimal IncreaseDecreasedAmount { get; set; }
    public bool HasRatioIncreased { get; set; }
    public  int[] Series { get; set; }
}