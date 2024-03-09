﻿namespace VillaWeb.ViewModels;

public class HomeVM
{
    public IEnumerable<Villa.Domain.Entities.Villa>? VillaList { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public int Nights { get; set; }
}