namespace Villa.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IVillaRepo Villa { get; }
    IVillaNumberRepo VillaNumber { get; }
    IAmenityRepo Amenity { get; }
    IBookingRepo Booking { get; }
    IAppUserRepo AppUser { get; }
    void Save();
}